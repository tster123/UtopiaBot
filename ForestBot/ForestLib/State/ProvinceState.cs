using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ForestLib.State
{
    public enum DraftRate
    {
        None, Reserve, Normal, Aggressive, Emergency
    }

    public class ProvinceState
    {
        public Race Race;
        public Personality Personality;
        public int Acres;
        public int Honor;
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

        public int GetMaxPopulation(Age100Settings age)
        {
            double popScience = 1; // TODO: science
            double inHomes =
                age.BuildingEffects.HomeCapacity.EffectiveCapacity(BuildingEffectiveness, Buildings.Homes,
                    Race.HomeBonus);
            return (int) ((25 * (Acres - Buildings.Barren) + 15 * Buildings.Barren + inHomes) * Race.Population * popScience);
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


    public class Science
    {
        private readonly double totalMod;
        private readonly double multiplier;

        public Science(double totalMod, double multiplier)
        {
            this.totalMod   = totalMod;
            this.multiplier = multiplier;
        }

        public int Books
        {
            get;
            set;
        }

        public double Effect
        {
            get => Math.Pow(Books, (1/2.125)) * multiplier * totalMod / 100.0;
            set => Books = (int) Math.Pow((value * 100 / (multiplier * totalMod)), (2.125 / 1.0)) ;
        }
    }

    public class ProvinceScience
    {
        private double scienceMod;

        public int UnallocatedEconomy,
            UnallocatedMilitary,
            UnallocatedArcane;

        public int NumScientists;
        public double ProgressToNextScientist;

        public Science Alchemy,
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

        public ProvinceScience(double scienceMod)
        {
            this.scienceMod = scienceMod;
            Alchemy         = new Science(0.0724, scienceMod);
            Tools           = new Science(0.0524, scienceMod);
            Housing         = new Science(0.0262, scienceMod);
            Production      = new Science(0.2172, scienceMod);
            Bookkeeping     = new Science(0.068, scienceMod);
            Artisan         = new Science(0.0478, scienceMod);
            Strategy        = new Science(0.0367, scienceMod);
            Siege           = new Science(0.0262, scienceMod);
            Tactics         = new Science(0.0367, scienceMod);
            Valor           = new Science(0.0582, scienceMod);
            Heroism         = new Science(0.0418, scienceMod);
            Resilience      = new Science(0.0367, scienceMod);
            Crime           = new Science(0.1557, scienceMod);
            Channeling      = new Science(0.1875, scienceMod);
            Shielding       = new Science(0.0314, scienceMod);
            Sorcery         = new Science(0.0314, scienceMod);
            Cunning         = new Science(0.0314, scienceMod);
            Finesse         = new Science(0.0478, scienceMod);
        }
    }
}
