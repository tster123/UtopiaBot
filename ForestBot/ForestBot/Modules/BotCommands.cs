﻿using System;
using System.Globalization;
using System.Net;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using ForestLib;
using ForestLib.AgeSettings.Ages;
using ForestLib.Database;
using ForestLib.Tools;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForestBot.Modules;

// Interation modules must be public and inherit from an IInterationModuleBase
public class BotCommands : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public InteractionService? Commands { get; set; }

    private InteractionHandler _handler;

    // Constructor injection is also a valid way to access the dependencies
    public BotCommands(InteractionHandler handler)
    {
        _handler = handler;
    }

    // You can use a number of parameter types in you Slash Command handlers (string, int, double, bool, IUser, IChannel, IMentionable, IRole, Enums) by default. Optionally,
    // you can implement your own TypeConverters to support a wider range of parameter types. For more information, refer to the library documentation.
    // Optional method parameters(parameters with a default value) also will be displayed as optional on Discord.

    // [Summary] lets you customize the name and the description of a parameter
    [SlashCommand("echo", "Repeat the input")]
    public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
        => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));

    private static MessageProcessor? _processor;
    
    [SlashCommand("process-messages", "Reads messages from another channel")]
    public async Task ProcessMessage(string channel, int lookback = 100, int numMessages = 0, string? startFromMessageId = null)
    {
        if (_processor != null && !_processor.IsDone())
        {
            await RespondAsync("Existing processor is still running: " + _processor);
            return;
        }

        SocketTextChannel? chan = Context.Guild.TextChannels.FirstOrDefault(c => c.Name.ToLower() == channel);
        if (chan == null)
        {
            await RespondAsync($"Unable to find channel [{channel}]");
            return;
        }

        ulong? startMessage = null;
        ulong? endMessage = null;
        if (startFromMessageId != null)
        {
            string[] parts = startFromMessageId.Split('-');
            if (parts.Length == 1)
            {
                startMessage = ulong.Parse(startFromMessageId);
            }
            else
            {
                startMessage = ulong.Parse(parts[0]);
                endMessage = ulong.Parse(parts[1]);
                if (startMessage < endMessage)
                {
                    (startMessage, endMessage) = (endMessage, startMessage);
                }
            }
        }

        if (_processor != null && _processor.MaxMessages == numMessages && _processor.StartFromMessageId == startMessage && _processor.EndMessageId == endMessage)
        {
            await RespondAsync((_processor.IsDone() ? "Completed: " : "Running:: ") + _processor);
            return;
        }

        _processor = new MessageProcessor(chan, startMessage, endMessage, numMessages);
        _processor.Start();

        await RespondAsync($"Started new processor: " + _processor);
    }

    [SlashCommand("read-messages", "Reads messages from another channel")]
    public async Task ReadMessage(string channel)
    {
        SocketTextChannel? chan = Context.Guild.TextChannels.FirstOrDefault(c => c.Name.ToLower() == channel);
        if (chan == null)
        {
            await RespondAsync($"Unable to find channel [{channel}]");
            return;
        }

        var messages = await chan.GetMessagesAsync(100).FlattenAsync();
        string message = "";
        foreach (IMessage m in messages)
        {
            message += "Message: " + m.Content + "\n";
        }

        await RespondAsync(message);
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task GreetUserAsync()
        => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

    public Dictionary<string, string> OpsShortToLong = new()
    {
        ["gp"] = "greater protection",
        ["sal"] = "salvation",
        ["mp"] = "minor protection",
        ["is"] = "illuminate shadows",
        ["ds"] = "divine shield",
        ["pat"] = "patriatism",
        ["lap"] = "love and peace",
        ["bb"] = "builders boon",
        ["ia"] = "inspire army",
        ["fl"] = "fertile lands",
        ["chas"] = "chastity",
        ["bg"] = "bribe generals",
        ["fb"] = "fireball",
        ["infiltrate"] = "infiltrate thieves guild",
        ["rob t"] = "rob the towers",
        ["rob v"] = "rob the vaults",
        ["rob g"] = "rob the granaries",
        ["sod"] = "spy on defense",
        ["sot"] = "spy on throne",
        ["som"] = "spy on military",
        ["ir"] = "incite riots",
        ["ms"] = "meteor showers",
        ["ll"] = "land lush",
        ["bt"] = "bribe thieves",
        ["cb"] = "crystal ball",
        ["pf"] = "pitfalls",
        ["nm"] = "nightmare",
        ["ns"] = "night strike",
        ["mv"] = "mystic vortex",
    };

    public Dictionary<string, string> OpsLongToShort => OpsShortToLong.ToDictionary(b => b.Value, b => b.Key);

    [SlashCommand("whoneeds", "Who in the kingdom needs a support spell")]
    public async Task WhoNeeds(string spell)
    {
        try
        {
            ForestContext db = new ForestContext();
            Kingdom? kingdom = db.Kingdoms.Where(k => k.GuildId == 897511707994361866L).OrderByDescending(k => k.Age).FirstOrDefault();
            if (kingdom == null)
            {
                await RespondAsync("Cannot find a kingdom with guildId: " + Context.Guild.Id);
                return;
            }

            spell = spell.ToLower();
            string longForm;
            if (OpsLongToShort.ContainsKey(spell))
            {
                longForm = spell;
            }
            else if (OpsShortToLong.TryGetValue(spell, out string? lf))
            {
                longForm = lf;
            }
            else
            {
                await RespondAsync("Unknown spell: " + spell);
                return;
            }

            Dictionary<string, DateTime> provEndTimes = new();
            Dictionary<string, Province> provLookup = new();
            foreach (Province prov in db.Provinces.Where(p => p.KingdomId == kingdom.KingdomId))
            {
                provEndTimes[prov.Name.ToLower()] = DateTime.MinValue;
                provLookup[prov.Name.ToLower()] = prov;
            }

            foreach (TmOperation op in db.Operations.Where(o => o.Timestamp > DateTime.UtcNow.AddHours(-40) && o.OpName == longForm && o.Success))
            {
                string targetProv = (op.TargetProvince ?? op.SourceProvince).Trim().ToLower();
                DateTime currentEnd = provEndTimes[targetProv];
                DateTime thisEnd = op.Timestamp.AddHours(op.Damage!.Value + 1);
                thisEnd = new DateTime(thisEnd.Year, thisEnd.Month, thisEnd.Day, thisEnd.Hour, 0, 0);
                if (thisEnd > currentEnd)
                {
                    currentEnd = thisEnd;
                }

                provEndTimes[targetProv] = currentEnd;
            }

            List<Province> ret = new();
            foreach (var provAndTime in provEndTimes)
            {
                if (provAndTime.Value < DateTime.UtcNow)
                {
                    ret.Add(provLookup[provAndTime.Key]);
                }
            }

            if (ret.Count == 0)
            {
                await RespondAsync("All provinces are covered with " + longForm);
                return;
            }

            await RespondAsync(string.Join("\n", ret.Select(p => p.Slot + " - " + p.Name)));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await RespondAsync("Error!\n" + e);
        }
    }

    [SlashCommand("timeline", "Generates a ceasefire timeline")]
    public async Task Timeline(
        string warEndDate,
        double avgPeonsPerAcre,
        double targetPeonsPerAcre = 5.5,
        double avgConstructionScience = 0.0,
        double avgDraftScience = 0.0,
        double avgToolsScience = 0.0,
        double avgTrainingTimeScience = 0.0,
        double avgProductionScience = 0.0,
        double expectedExpedientStrength = 1.15,
        double expectedHasteStrength = 1.15,
        double ritualCastSuccessRate = 0.80, 
        int ritualCastsDesired = 7,
        double ritualBuffer = 0.2,
        bool uglyString = false
    )
    {
        try
        {
            UtopiaDate warEnd = UtopiaDate.Parse(warEndDate);
            StrategySettings strat = new StrategySettings
            {
                AverageConstructionScience = avgConstructionScience,
                AverageDraftSciecne = avgDraftScience,
                ToolsScience = avgToolsScience,
                AveragePeonsPerAcre = avgPeonsPerAcre,
                AverageTrainingTimeScience = avgTrainingTimeScience,
                ExpectedExpedientStrength = expectedExpedientStrength,
                ExpectedHasteStrength = expectedHasteStrength,
                ProductionScience = avgProductionScience,
                RitualBuffer = ritualBuffer,
                RitualCastsDesired = ritualCastsDesired,
                RitualSuccessRate = ritualCastSuccessRate,
                TargetHorseFill = 0.9,
                TargetPeonsPerAcre = targetPeonsPerAcre,

            };
            CeasefireTimeline timeline = new CeasefireTimeline(new Age105Settings(), strat);
            List<TimelineEvent> ret = timeline.GetTimelineWithExpedientAndHaste(warEnd);
            string retStr = "";
            foreach (var e in ret)
            {
                if (retStr != "") retStr += "\n";
                retStr += uglyString ? e.UglyString() : e.ToString();
            }

            await RespondAsync(retStr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await RespondAsync("Error!\n" + e);
        }
    }

    [SlashCommand("calculate-be", "Calculates what your BE will be based on various inputs")]
    public async Task CalculateBe(
        int peasants = -1,
        int prisoners = 0,
        int availableJobs = -1,
        int acres = -1,
        double percentageHomes = -1,
        double toolsScienceBonus = 0,
        bool dwarf = false)
    {
        if (toolsScienceBonus > .5)
        {
            await RespondAsync("toolsScienceBonus most be between 0.0 and 0.5");
            return;
        }

        if (availableJobs <= 0)
        {
            if (acres <= 0 || percentageHomes < 0)
            {
                await RespondAsync("Must provide either availableJobs or acres & percentageHomes");
                return;
            }

            if (percentageHomes < 0 || percentageHomes > 1)
            {
                await RespondAsync("percentageHomes most be between 0.0 and 1.0");
                return;
            }

            availableJobs = (int)(acres * (1 - percentageHomes) * 25.0);
        }

        int availWorkers = peasants + prisoners / 2;
        int optimalWorkers = (int)(availableJobs * 0.67);
        double jobsPerformed = Math.Min(1, 1.0 * availWorkers / optimalWorkers);
        double race = dwarf ? 1.3 : 1.0;
        double be = (0.5 * (1.0 + jobsPerformed)) * race * (1.0 + toolsScienceBonus);
        await RespondAsync($"BE = {be:P} (workers = {availWorkers}, optimalWorkers={optimalWorkers})");
    }

    [SlashCommand("activity-times", "Shows the number of active hours (UTC) for a province over the time range (default last 30 days)")]
    public async Task ProvinceActivityTimes(string province, DateTime? start = null, DateTime? end = null, int utcOffset = 0)
    {
        try
        {
            ForestContext db = new ForestContext();
            start ??= DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            end ??= DateTime.UtcNow;
            end = end.Value.AddDays(1).Date;
            start = start.Value.Date;

            var union = (
                    from a in db.Attacks
                    where a.Timestamp > start.Value && a.Timestamp <= end.Value && a.SourceProvince == province
                    select new { a.Timestamp.Hour, a.Timestamp.Date })
                .Union(
                    from o in db.Operations
                    where o.Timestamp > start.Value && o.Timestamp <= end.Value && o.SourceProvince == province
                    select new { o.Timestamp.Hour, o.Timestamp.Date }
                );

            var res = from i in union
                group i by i.Hour into g
                select new { Hour = g.Key, Uniques = g.Select(ga => ga.Date).Distinct().Count() };
            int[] counts = new int[24];
            double max = 0;
            foreach (var r in res)
            {
                counts[r.Hour] = r.Uniques;
                max = Math.Max(max, r.Uniques);
            }

            StringBuilder m = new StringBuilder(500);
            m.AppendLine("```");
            int absOffset = Math.Abs(utcOffset);
            string localLabel;
            if (absOffset == 0)
                localLabel = "UTC";
            else if (utcOffset < 0)
                localLabel = "UTC" + utcOffset;
            else
                localLabel = "UTC+" + utcOffset;

            m.AppendLine(localLabel + " (uto date) : unique days with activity");
            for (int localHour = 0; localHour < 24; localHour++)
            {
                int utcHour = (24 + (localHour - utcOffset)) % 24;
                int utopiaDate = ((utcHour + 6) % 24) + 1;
                m.Append($"{localHour.ToString().PadLeft(2)} ({utopiaDate.ToString().PadLeft(2)}) : {counts[utcHour].ToString().PadLeft(3)} ");
                double percentage = counts[utcHour] / max;
                int size = (int)(25 * percentage);
                while (size > 0)
                {
                    size--;
                    m.Append("#");
                }
                m.AppendLine();
            }
            m.AppendLine("```");
            await RespondAsync(m.ToString());

        }
        catch (Exception e)
        {
            await RespondAsync("Error!\n" + e.Message);
            Console.WriteLine(e);
        }
    }

    [SlashCommand("damage-summary", "Shows damage summary (default last 24 hours).  Use start/end (try '3 days ago' or '2025-01-03')")]
    public async Task DamageSummary(string sourceProvince = null, string targetProvince = null, string start = null, string end = null, string targetKingdom = null, string opType = null)
    {
        DateTime startDt;
        DateTime? endDt;

        try
        {
            startDt = ParseFriendlyDatetime(start, nameof(start)) ?? DateTime.UtcNow.AddDays(-1);
            endDt = ParseFriendlyDatetime(end, nameof(end));
        }
        catch (Exception e)
        {
            await RespondAsync(e.Message);
            return;
        }

        await DamageSummary(startDt, endDt, sourceProvince, targetProvince, targetKingdom, opType);
    }

    private DateTime? ParseFriendlyDatetime(string str, string paramName)
    {
        if (str == null || str.Trim() == "") return null;
        string originalStr = str;
        if (str.ToLower().EndsWith("ago"))
        {
            str = str.Replace("ago", "").Trim();
            string[] parts = str.Split(' ');
            if (parts.Length != 2)
            {
                throw new InvalidOperationException($"Invalid number of tokens in {paramName} parameter.  Expected something like '3.5 days ago'");
            }

            if (!double.TryParse(parts[0], out double num))
            {
                throw new InvalidOperationException($"First part of {paramName} parameter expected to be a number.  Expected something like '3.5 days ago'");
            }

            string datepart = parts[1];
            if (datepart.EndsWith("s")) datepart = datepart.Substring(0, datepart.Length - 1);
            switch (datepart.ToLower())
            {
                case "day":
                    return DateTime.UtcNow.AddDays(-1 * num);
                case "hour":
                    return DateTime.UtcNow.AddHours(-1 * num);
                case "week":
                    return DateTime.UtcNow.AddDays(-7 * num);
                case "minute":
                    return DateTime.UtcNow.AddMinutes(-1 * num);
                default:
                    throw new InvalidOperationException($"In {paramName} Don't recognize {parts[1]} as a length of time.  Try one of: days, hours, weeks, or minutes");
            }
        }

        if (DateTime.TryParseExact(str, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime res))
            return res;
        if (DateTime.TryParseExact(str, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out res))
            return res;
        if (DateTime.TryParseExact(str, "yyyy-MM-dd hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out res))
            return res;
        if (DateTime.TryParseExact(str, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out res))
            return res;

        throw new InvalidOperationException($"cannot parse {paramName}: [{originalStr}]");
    }

    private async Task DamageSummary(DateTime start, DateTime? end, string sourceProvince, string targetProvince, string targetKingdom, string opType)
    {
        if (opType != null)
        {
            opType = opType.ToLower();
            if (opType == "attacks") opType = "attack";
            if (opType == "ops") opType = "op";
        }
        try
        {
            ForestContext db = new ForestContext();
            var union = (
                    from a in db.Attacks
                    where a.Timestamp > start &&
                          (end == null || a.Timestamp <= end.Value) &&
                          (sourceProvince == null || a.SourceProvince == sourceProvince) &&
                          (targetProvince == null || a.TargetProvince == targetProvince) &&
                          (targetKingdom == null || a.TargetKingdom == targetKingdom) &&
                          (opType == null || opType == "attack" || opType == a.AttackType)
                    group a by a.AttackType
                    into g
                    select new
                    {
                        OpType = "attack-" + g.Key,
                        Damage = g.Sum(a => a.Damage),
                        Kills = g.Sum(a => a.Kills),
                        Count = g.Count(),
                        Success = g.Count(a => a.Damage > 0)
                    })
                .Union(
                    from a in db.Operations
                    where a.Timestamp > start &&
                          (end == null || a.Timestamp <= end.Value) &&
                          (sourceProvince == null || a.SourceProvince == sourceProvince) &&
                          (targetProvince == null || a.TargetProvince == targetProvince) &&
                          a.TargetKingdom != null &&
                          (targetKingdom == null || a.TargetKingdom == targetKingdom) &&
                          (opType == null || opType == "op" || opType == a.OpName)
                    group a by a.OpName
                    into g
                    select new
                    {
                        OpType = g.Key,
                        Damage = g.Sum(o => o.Damage ?? 0),
                        Kills = 0,
                        Count = g.Count(),
                        Success = g.Count(o => o.Success)
                    }
                );
            StringBuilder m = new StringBuilder(500);
            m.AppendLine("```");
            m.AppendLine($"Op Type    | Count | Success |  Damage  | Kills (attack only)");
            foreach (var line in union)
            {
                if (!OpsLongToShort.TryGetValue(line.OpType, out string opName))
                    opName = line.OpType;
                opName = opName.Replace("propaganda", "prop");
                opName = opName.Replace("greater arson", "ga");
                opName = opName.Replace("soldiers", "sold");
                opName = opName.Replace("specialist", "spec");
                opName = opName.Replace("wizards", "wiz");
                opName = opName.Replace("watch towers", "wt");
                int damage = opName == "sod" ? 0 : line.Damage;
                m.AppendLine($"{opName,10} | {line.Count,5:N0} | {line.Success,5:N0} | {damage,6:N0}" + ((line.Kills == 0) ? "" : $" | {line.Kills,6:N0}"));
            }
            m.AppendLine("```");
            await RespondAsync(m.ToString());

        }
        catch (Exception e)
        {
            await RespondAsync("Error!\n" + e.Message);
            Console.WriteLine(e);
        }
        
    }


    /*
    [SlashCommand("bitrate", "Gets the bitrate of a specific voice channel.")]
    public async Task GetBitrateAsync([ChannelTypes(ChannelType.Voice, ChannelType.Stage)] IChannel channel)
        => await RespondAsync(text: $"This voice channel has a bitrate of {(channel as IVoiceChannel).Bitrate}");

    // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
    [Group("test_group", "This is a command group")]
    public class GroupExample : InteractionModuleBase<SocketInteractionContext>
    {
        // You can create command choices either by using the [Choice] attribute or by creating an enum. Every enum with 25 or less values will be registered as a multiple
        // choice option
        [SlashCommand("choice_example", "Enums create choices")]
        public async Task ChoiceExample(ExampleEnum input)
            => await RespondAsync(input.ToString());
    }

    // Use [ComponentInteraction] to handle message component interactions. Message component interaction with the matching customId will be executed.
    // Alternatively, you can create a wild card pattern using the '*' character. Interaction Service will perform a lazy regex search and capture the matching strings.
    // You can then access these capture groups from the method parameters, in the order they were captured. Using the wild card pattern, you can cherry pick component interactions.
    [ComponentInteraction("musicSelect:*,*")]
    public async Task ButtonPress(string id, string name)
    {
        // ...
        await RespondAsync($"Playing song: {name}/{id}");
    }

    // Select Menu interactions, contain ids of the menu options that were selected by the user. You can access the option ids from the method parameters.
    // You can also use the wild card pattern with Select Menus, in that case, the wild card captures will be passed on to the method first, followed by the option ids.
    [ComponentInteraction("roleSelect")]
    public async Task RoleSelect(string[] selections)
    {
        throw new NotImplementedException();
    }

    // With the Attribute DoUserCheck you can make sure that only the user this button targets can click it. This is defined by the first wildcard: *.
    // See Attributes/DoUserCheckAttribute.cs for elaboration.
    [DoUserCheck]
    [ComponentInteraction("myButton:*")]
    public async Task ClickButtonAsync(string userId)
        => await RespondAsync(text: ":thumbsup: Clicked!");

    // This command will greet target user in the channel this was executed in.
    [UserCommand("greet")]
    public async Task GreetUserAsync(IUser user)
        => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");

    // Pins a message in the channel it is in.
    [MessageCommand("pin")]
    public async Task PinMessageAsync(IMessage message)
    {
        // make a safety cast to check if the message is ISystem- or IUserMessage
        if (message is not IUserMessage userMessage)
            await RespondAsync(text: ":x: You cant pin system messages!");

        // if the pins in this channel are equal to or above 50, no more messages can be pinned.
        else if ((await Context.Channel.GetPinnedMessagesAsync()).Count >= 50)
            await RespondAsync(text: ":x: You cant pin any more messages, the max has already been reached in this channel!");

        else
        {
            await userMessage.PinAsync();
            await RespondAsync(":white_check_mark: Successfully pinned message!");
        }
    }
    */
}