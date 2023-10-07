using ForestLib.AgeSettings.Ages;
using ForestLib.State;

namespace ForestLib.Tools;

public class TimelineEvent
{
    public readonly UtopiaDate Date;
    public readonly string Event;

    public TimelineEvent(UtopiaDate date, string @event)
    {
        Date = date;
        Event = @event;
    }

    public override string ToString() => $"{Date} - {Event}";
}
public class CeasefireTimeline
{
    private readonly Age103Settings age;
    private readonly StrategySettings strat;

    public CeasefireTimeline(Age103Settings age, StrategySettings strat)
    {
        this.age = age;
        this.strat = strat;
    }

    public List<TimelineEvent> GetTimeline(UtopiaDate warEndDate)
    {
        var ret = new List<TimelineEvent>();
        ret.Add(new TimelineEvent(warEndDate, "War ended"));
        UtopiaDate cfEnd = warEndDate.AddTicks(age.CeasefireLength + 1);
        ret.Add(new TimelineEvent(cfEnd, "Ceasefire ends"));

        int buildTime = (int)(age.BaseBuildTime *
                              (1 - strat.ExpectedExpedientStrength * age.ExpedientBuildSpeedBonus) *
                              (1 - strat.AverageConstructionScience) *
                              (1 - age.BuildersBoonSpeedup));
        int buildTimeWithHaste = (int)(age.BaseBuildTime *
                              (1 - strat.ExpectedHasteStrength * age.HasteConstructionSpeedBonus) *
                              (1 - strat.AverageConstructionScience) *
                              (1 - age.BuildersBoonSpeedup));

        // stable build time
        double horsesNeeded = age.StableCapacity * strat.TargetHorseFill;
        double ticksToFillStables = horsesNeeded / (age.StableProduction * (1 + age.LoveAndPeaceHorseSpeedBonus));
        UtopiaDate buildStables = cfEnd.AddTicks(-1 * (buildTime + (int)Math.Round(ticksToFillStables)));
        ret.Add(new TimelineEvent(buildStables, "Build stables (BB + L&P)"));

        ret.Add(new TimelineEvent(cfEnd.AddTicks(-buildTimeWithHaste), "Switch to war build"));
        ret.Add(new TimelineEvent(cfEnd.AddTicks(-49), "Raise wages to 200%"));

        // training date
        double baseTrainingTime = age.BaseTrainingTime
                                  * (1 - age.HasteTrainingSpeedBonus * strat.ExpectedHasteStrength)
                                  * (1 - age.InspireArmyTrainingSpeedBonus)
                                  * (1 - strat.AverageTrainingTimeScience);
        int trainingTicks = (int)Math.Ceiling(baseTrainingTime);
        UtopiaDate trainingDate = cfEnd.AddTicks(-trainingTicks);
        ret.Add(new TimelineEvent(trainingDate, $"Last date to start training (w/ IA  & {strat.AverageTrainingTimeScience:P} valor science)"));
        ret.Add(new TimelineEvent(trainingDate.AddTicks(-5), $"Estimated start of haste ritual"));
        ret.Add(new TimelineEvent(trainingDate.AddTicks(-buildTime), "Latest build 25-30% arms (w/ BB)"));

        foreach (Race r in age.Races)
        {
            if (Math.Abs(r.TrainingTime - 1) > 0.01)
            {
                int raceTrainingTicks = (int)Math.Ceiling(baseTrainingTime * r.TrainingTime);
                UtopiaDate raceTrainingDate = cfEnd.AddTicks(-raceTrainingTicks);
                ret.Add(new TimelineEvent(raceTrainingDate, r.Name + $" Only - Last date to start training (w/ IA  & {strat.AverageTrainingTimeScience:P} valor science)"));
            }
        }

        double draftSpeed = age.DraftEmergency
                            * (1 + (age.ExpedientDraft * strat.ExpectedExpedientStrength))
                            * (1 + age.PatriatismBonus)
                            * (1 + strat.AverageDraftSciecne);
        int draftTime = 0;
        double ppa = strat.AveragePeonsPerAcre;
        while (ppa > strat.TargetPeonsPerAcre)
        {
            ppa *= (1 - draftSpeed);
            draftTime++;
        }

        ret.Add(new TimelineEvent(trainingDate.AddTicks(-draftTime - 1),
            $"Calculate your draft start time"));
        UtopiaDate ritualPopDate = trainingDate.AddTicks(-draftTime - 2);
        ret.Add(new TimelineEvent(ritualPopDate,
            "Last date to activate Expedient"));

        double runesNeededPerAcre = age.RitualCostPerAcre * (1 + strat.RitualBuffer) 
                                                          * strat.RitualCastsDesired / strat.RitualSuccessRate;
        double runesPerTower = age.TowerProduction * (1 + strat.ProductionScience) * (1 + strat.ToolsScience);
        int fastBuild = buildTime / 2;
        int towerTicks = ritualPopDate.Tick - warEndDate.Tick - fastBuild;
        runesPerTower *= towerTicks;
        double towerPercentage = runesNeededPerAcre / runesPerTower;
        ret.Add(new TimelineEvent(warEndDate,
            "Build towers (BB, double speed) " +
            $"(min {towerPercentage:P1} at {strat.ProductionScience:P1} prod, {strat.ToolsScience:P1} tools science)"));

        return ret.OrderBy(t => t.Date).ToList();
    }
}