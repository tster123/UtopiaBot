using ForestLib.State;

namespace ForestLib.AgeSettings
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
            BuildingEfficiency = 1.25,
            FoodConsumption = 1.5,
            ConstructionTime = 0.5,
            AllowAccelerateConstruction = false,
            BuildingCost = 0
            // TODO: account for miners mystique
        };
        // TODO: miners mystique, ToG
        public Race Elf = new("Elf") { Population = 0.9 };
        public Race Faery = new("Faery") { BirthRate = 0.8, HomeBonus = -5 };
        public Race Halfling = new("Halfling") { Population = 1.1, ThiefCost = 0.7 };
        public Race Human = new("Human") { BirthRate = 1.2, Science = 1.1 };
        public Race Orc = new("Orc") { DraftCosts = 0 };
        public Race Undead = new("Undead") { FoodConsumption = 0, Science = 0.8 };
        public Race[] Races { get; }

        public Personality Cleric = new("Cleric");
        public Personality Heretic = new("Heretic");
        public Personality Merchant = new("Merchant") { Income = 1.3 };
        public Personality Mystic = new("Mystic") { GuildEffectiveness = 2 };
        public Personality Rogue = new("Rogue");
        public Personality Sage = new("Sage");
        public Personality Shepherd = new("Shepherd");
        public Personality Tactician = new("Tactician");
        public Personality WarHero = new("WarHero") { HonorEffectiveness = 2 };
        public Personality Warrior = new("Warrior") { Wages = .8 };
        public Personality[] Personalities { get; }

        public BuildingEffectsAge100 BuildingEffects { get; }


        public Age100Settings()
        {
            Races = new[] { Avian, Dwarf, Elf, Faery, Halfling, Human, Orc, Undead };
            Personalities = new[] { Cleric, Heretic, Merchant, Mystic, Rogue, Sage, Shepherd, Tactician, WarHero, Warrior };
            BuildingEffects = new BuildingEffectsAge100();
        }
    }

    public class BuildingEffectsAge100 : IBuildingEffects
    {
        public BuildingEffectCapacity BarrenCapacity { get; } = new("Barren population", -1);
        public BuildingEffectFlat BarrenFood { get; } = new("Barren Food", 2, false);
        public BuildingEffectCapacity HomeCapacity { get; } = new("Home", 10);
        public BuildingEffectFlat HomeGrowth { get; } = new("Home peon growth", 0.5, false);
        public BuildingEffectFlat Farm { get; } = new("Farm", 60);
        public BuildingEffectPercentage MillConstruction { get; } = new("Mill Construction", 0.04);
        public BuildingEffectPercentage MillExploreGold { get; } = new("Mill Explore Gold", 0.03);
        public BuildingEffectPercentage MillExploreSold { get; } = new("Mill Explore SOld", 0.02);
        public BuildingEffectPercentage BankPercentage { get; } = new("Bank percentage", 0.015);
        public BuildingEffectFlat BankFlat { get; } = new("Farm flat", 25);
        public BuildingEffectPercentage TrainingGroundOme { get; } = new("Training Ground OME", 0.015);
        public BuildingEffectPercentage TrainingGroundMerc { get; } = new("Training Ground Merc", 0.02);
        public BuildingEffectPercentage ArmouryWage { get; } = new("Armoury wage", 0.02);
        public BuildingEffectPercentage ArmouryDraftCost { get; } = new("Armoury draft", 0.02);
        public BuildingEffectPercentage ArmouryTrainingCost { get; } = new("Armoury training", 0.015);
        public BuildingEffectPercentage Barracks { get; } = new("Barracks", 0.015);
        public BuildingEffectPercentage Fort { get; } = new("Fort", 0.015);
        public BuildingEffectPercentage GuardStation { get; } = new("Guard Station", 0.015);
        public BuildingEffectPercentage HospitalCasualties { get; } = new("Hospital casualties", 0.03);
        public BuildingEffectPercentage HospitalPlague { get; } = new("Hospital plague", 0.03);
        public BuildingEffectFlat Guild { get; } = new("Guild", 0.02, false);
        public BuildingEffectFlat Tower { get; } = new("Tower", 12);
        public BuildingEffectPercentage ThievesDenLosses { get; } = new("Thieves Den losses", 0.04, 0.95);
        public BuildingEffectPercentage ThievesDenTpa { get; } = new("Thieves Den TPA", 0.03);
        public BuildingEffectPercentage WatchTowerCatch { get; } = new("Watch Tower catch", 0.015);
        public BuildingEffectPercentage WatchTowerDamage { get; } = new("Watch Tower", 0.025);
        public BuildingEffectPercentage UniversityScientistsSpawn { get; } = new("University scientist spawn", 0.02, false);
        public BuildingEffectPercentage UniversityScience { get; } = new("University science", 0.01, false);
        public BuildingEffectPercentage Library { get; } = new("Library", 0.01, false);
        public BuildingEffectCapacity StableCapacity { get; } = new("Stable Capacity", 80);
        public BuildingEffectFlat StableProduction { get; } = new("Stable Production", 2);
        public BuildingEffectCapacity Dungeon { get; } = new("Dungeon", 30);
    }


}