using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ForestLib.AgeSettings;
using ForestLib.AgeSettings.Ages;

namespace ForestLib.State
{
    public enum DraftRate
    {
        None, Reserve, Normal, Aggressive, Emergency
    }

    public class Honor
    {
        public string Name { get; }
        public int Amount { get; }
        private readonly int level;

        public Honor(int honor)
        {
            Amount = honor;
            level = Math.Max(8, Amount / 750);
            Name = new[]
            {
                "Peasant",
                "Knight",
                "Lord",
                "Baron",
                "Viscount",
                "Count",
                "Marquis",
                "Duke",
                "Prince"
            }[level];
        }

        public double PopulationBonus => 0.01 * level;
        public double MilitaryEfficiencyBonus => 0.01 * level;
        public double IncomeBonus => 0.02 * level;
        public double ProductionBonus => 0.02 * level;
        public double TPABonus => 0.03 * level;
        public double WPABonus => 0.03 * level;
    }

    public class ProvinceState
    {
        public Race Race;
        public Personality Personality;
        public int Acres;
        public Honor Honor;
        public int Peasants;
        public double MaxDraft;
        public DraftRate DraftRate;
        public double WageRate;
        public double BuildingEffectiveness;
        public double MilitaryEfficiency;
        public int Money;
        public int Food;
        public int Runes;
        public int TradeBalance;
        public int Networth;
        public int Wizards;
        public int Horses;
        public int Prisoners;
        public int ModifiedOffense;
        public int ModifiedDefense;
        public MilitaryPopulation Military;
        public List<Tuple<int, MilitaryPopulation>> MilitaryTraining;
        public Buildings Buildings;
        public List<Tuple<int, Buildings>> BuildinsInProgress;
        public ProvinceScience Science;

        public int TotalMilitaryPopulation => Military.Sum() + MilitaryTraining.Select(t => t.Item2.Sum()).Sum();

        public int GetMaxPopulation(IAgeSettings age)
        {
            double popScience = 1 + age.GetScienceEffects().Housing.GetBonus(Science.Housing, this, age);
            double inHomes =
                age.GetBuildingEffects().HomeCapacity.EffectiveCapacity(BuildingEffectiveness, Buildings.Homes,
                    Race.HomeBonus);
            return (int) ((25 * (Acres - Buildings.Barren) + 15 * Buildings.Barren + inHomes) * Race.Population * popScience);
        }

        public int GetTotalPopulation()
        {
            return Peasants + Wizards + Military.Sum() + MilitaryTraining.Sum(t => t.Item2.Sum());
        }
    }

    public class MilitaryPopulation
    {
        public int Soldiers;
        public int OffSpecs;
        public int DefSpecs;
        public int Elites;
        public int Theives;

        public void Add(MilitaryPopulation other)
        {
            Soldiers += other.Soldiers;
            OffSpecs += other.OffSpecs;
            DefSpecs += other.DefSpecs;
            Elites += other.Elites;
            Theives += other.Theives;
        }

        public int Sum() => Soldiers + OffSpecs + DefSpecs + Elites + Theives;
    }

    public class Buildings
    {
        public int Barren,
            Homes,
            Farms,
            Mills,
            Banks,
            TrainingGrounds,
            Armouries,
            Barracks,
            Forts,
            GuardStations,
            Hospitals,
            Guilds,
            Towers,
            ThievesDns,
            WatchTowers,
            Universities,
            Libraries,
            Stables,
            Dungeons;

        public void Add(Buildings other)
        {
            Barren += other.Barren;
            Homes += other.Homes;
            Farms += other.Farms;
            Mills += other.Mills;
            Banks += other.Banks;
            TrainingGrounds += other.TrainingGrounds;
            Armouries += other.Armouries;
            Barracks += other.Barracks;
            Forts += other.Forts;
            GuardStations += other.GuardStations;
            Hospitals += other.Hospitals;
            Guilds += other.Guilds;
            Towers += other.Towers;
            ThievesDns += other.ThievesDns;
            WatchTowers += other.WatchTowers;
            Universities += other.Universities;
            Libraries += other.Libraries;
            Stables += other.Stables;
            Dungeons += other.Dungeons;
        }

        public int Built =>
            Homes +
            Farms +
            Mills +
            Banks +
            TrainingGrounds +
            Armouries +
            Barracks +
            Forts +
            GuardStations +
            Hospitals +
            Guilds +
            Towers +
            ThievesDns +
            WatchTowers +
            Universities +
            Libraries +
            Stables +
            Dungeons;
    }


    public class ProvinceScience
    {
        private double scienceMod;

        public int UnallocatedEconomy,
            UnallocatedMilitary,
            UnallocatedArcane;

        public int NumScientists;
        public double ProgressToNextScientist;

        public int Alchemy,
            Tools,
            Housing,
            Production,
            Bookkeeping,
            Artisan,
            Strategy,
            Siege,
            Tactics,
            Valor,
            Heroism,
            Resilience,
            Crime,
            Channeling,
            Shielding,
            Cunning,
            Sorcery,
            Finesse;
    }
}
