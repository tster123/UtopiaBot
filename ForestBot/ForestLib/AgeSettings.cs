using ForestLib.State;

namespace ForestLib
{


    public class Age100Settings
    {
        public double DraftEmergency = 0.015;
        public int CeasefireLength = 4 * 24;
        public int StableCapacity = 80;
        public double StableProduction = 2;
        public double LoveAndPeaceHorseSpeedBonus = 0.4;
        public double PatriatismBonus = 0.3;

        public double BaseBuildTime = 16,
            BaseTrainingTime = 24,
            BuildersBoonSpeedup = 0.25,
            InspireArmyTrainingSpeedBonus = 0.20;

        public double ExpedientBuildSpeedBonus = 0.2,
            ExpedientTrainSpeedBonus = 0.25,
            ExpedientDraft = 0.2;

        public double RitualCostPerAcre = 8.8;
        public double TowerProduction = 12;

        public Race Avian = new("Avian");
        public Race Dwarf = new("Dwarf")
        {
            BuildingEfficiency          = 1.25,
            FoodConsumption             = 1.5,
            ConstructionTime            = 0.5,
            AllowAccelerateConstruction = false,
            BuildingCost = 0
            // TODO: account for miners mystique
        };
        // TODO: miners mystique, ToG
        public Race Elf = new("Elf") { Population           = 0.9 };
        public Race Faery = new("Faery") {BirthRate         = 0.8, HomeBonus = -5};
        public Race Halfling = new("Halfling") {Population  = 1.1, ThiefCost = 0.7};
        public Race Human = new("Human") {BirthRate         = 1.2, Science   = 1.1};
        public Race Orc = new("Orc") {DraftCosts            = 0};
        public Race Undead = new("Undead") {FoodConsumption = 0, Science = 0.8};
        public Race[] Races { get; }

        public Personality Cleric = new("Cleric");
        public Personality Heretic = new("Heretic");
        public Personality Merchant = new("Merchant") { Income = 1.3 };
        public Personality Mystic = new("Mystic") { GuildEffectiveness = 2};
        public Personality Rogue = new("Rogue");
        public Personality Sage = new("Sage");
        public Personality Shepherd = new("Shepherd");
        public Personality Tactician = new("Tactician");
        public Personality WarHero = new("WarHero") { HonorEffectiveness = 2};
        public Personality Warrior = new("Warrior") { Wages = .8 };
        public Personality[] Personalities { get; }

        public BuildingEffects BuildingEffects { get; }


        public Age100Settings()
        {
            Races = new[] {Avian, Dwarf, Elf, Faery, Halfling, Human, Orc, Undead};
            Personalities = new[] { Cleric, Heretic, Merchant, Mystic, Rogue, Sage, Shepherd, Tactician, WarHero, Warrior };
            BuildingEffects = new BuildingEffects();
        }
    }

    public class BuildingEffectFlat
    {
        // just here for making debugging easier
        public string Name { get; }
        public double FlatRate { get; }
        public bool AffectedByBe { get; }
        public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(FlatRate)}: {FlatRate}";

        public BuildingEffectFlat(string name, double flatRate, bool affectedByBe = true)
        {
            Name = name;
            FlatRate = flatRate;
            AffectedByBe = affectedByBe;
        }

        public double EffectiveFlatRate(double be, int numBuildings, double race = 1) =>
            (AffectedByBe ? be : 1) * numBuildings * FlatRate * race;
    }

    public class BuildingEffectCapacity
    {
        // just here for making debugging easier
        public string Name { get; }
        public double Capacity { get; }
        public bool AffectedByBe { get; }
        public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Capacity)}: {Capacity}";

        public BuildingEffectCapacity(string name, double capacity, bool affectedByBe = false)
        {
            Name = name;
            Capacity = capacity;
            AffectedByBe = affectedByBe;
        }

        public double EffectiveCapacity(double be, int numBuildings, int race = 0) =>
            (AffectedByBe ? be : 1) * numBuildings * (Capacity + race);
    }

    public class BuildingEffectPercentage
    {
        // just here for making debugging easier
        public string Name { get; }
        public double BaseEffect { get; }
        public double MaxEffect { get; }
        public bool AffectedByBe { get; }
        public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(BaseEffect)}: {BaseEffect}";

        public BuildingEffectPercentage(string name, double baseEffect, double maxEffect, bool affectedByBe = true)
        {
            Name = name;
            BaseEffect = baseEffect;
            MaxEffect = maxEffect;
            AffectedByBe = affectedByBe;
        }

        public BuildingEffectPercentage(string name, double baseEffect, bool affectedByBe = true)
        : this(name, baseEffect, baseEffect * 25, affectedByBe)
        {
        }

        public double EffectiveEffect(double be, int numBuildings, int totalAcres, double race = 1)
        {
            double percentage = numBuildings / (double)totalAcres;
            be = AffectedByBe ? be : 1;
            // Base Effect * BE * MIN(50%, % of building * (1 + Race)) * (100% - MIN(50%, % of building * (1 + Race)))
            return BaseEffect * be * Math.Min(.5, percentage * race) * (1 - Math.Min(0.5, percentage * race));
        }
    }

    public class BuildingEffects
    {
        public BuildingEffectCapacity BarrenCapacity = new BuildingEffectCapacity("Barren population", -1);
        public BuildingEffectFlat BarrenFood = new BuildingEffectFlat("Barren Food", 2, false);
        public BuildingEffectCapacity HomeCapacity = new BuildingEffectCapacity("Home", 10);
        public BuildingEffectFlat HomeGrowth = new BuildingEffectFlat("Home peon growth", 0.5, false);
        public BuildingEffectFlat Farm = new BuildingEffectFlat("Farm", 60);
        public BuildingEffectPercentage MillConstruction = new BuildingEffectPercentage("Mill Construction", 0.04);
        public BuildingEffectPercentage MillExploreGold = new BuildingEffectPercentage("Mill Explore Gold", 0.03);
        public BuildingEffectPercentage MillExploreSold = new BuildingEffectPercentage("Mill Explore SOld", 0.02);
        public BuildingEffectPercentage BankPercentage = new BuildingEffectPercentage("Bank percentage", 0.015);
        public BuildingEffectFlat BankFlat = new BuildingEffectFlat("Farm flat", 25);
        public BuildingEffectPercentage TrainingGroundOme = new BuildingEffectPercentage("Training Ground OME", 0.015);
        public BuildingEffectPercentage TrainingGroundMerc = new BuildingEffectPercentage("Training Ground Merc", 0.02);
        public BuildingEffectPercentage ArmouryWage = new BuildingEffectPercentage("Armoury wage", 0.02);
        public BuildingEffectPercentage ArmouryDraftCost = new BuildingEffectPercentage("Armoury draft", 0.02);
        public BuildingEffectPercentage ArmouryTrainingCost = new BuildingEffectPercentage("Armoury training", 0.015);
        public BuildingEffectPercentage Barracks = new BuildingEffectPercentage("Barracks", 0.015);
        public BuildingEffectPercentage Fort = new BuildingEffectPercentage("Fort", 0.015);
        public BuildingEffectPercentage GuardStation = new BuildingEffectPercentage("Guard Station", 0.015);
        public BuildingEffectPercentage HospitalCasualties = new BuildingEffectPercentage("Hospital casualties", 0.03);
        public BuildingEffectPercentage HospitalPlague = new BuildingEffectPercentage("Hospital plague", 0.03);
        public BuildingEffectPercentage ThievesDenLosses = new BuildingEffectPercentage("Thieves Den losses", 0.04, 0.95);
        public BuildingEffectPercentage ThievesDenTpa = new BuildingEffectPercentage("Thieves Den TPA", 0.03);
        public BuildingEffectPercentage WatchTowerCatch = new BuildingEffectPercentage("Watch Tower catch", 0.015);
        public BuildingEffectPercentage WatchTowerDamage = new BuildingEffectPercentage("Watch Tower", 0.025);
        public BuildingEffectPercentage UniversityScientistsSpawn = new BuildingEffectPercentage("University scientist spawn", 0.02, false);
        public BuildingEffectPercentage UniversityScience = new BuildingEffectPercentage("University science", 0.01, false);
        public BuildingEffectPercentage Library = new BuildingEffectPercentage("Library", 0.01, false);
        public BuildingEffectCapacity StableCapacity = new BuildingEffectCapacity("Stable Capacity", 80);
        public BuildingEffectFlat StableProduction = new BuildingEffectFlat("Stable Production", 2);
        public BuildingEffectCapacity Dungeon = new BuildingEffectCapacity("Dungeon", 30);
    }
}