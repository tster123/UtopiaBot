﻿using ForestLib.State;

namespace ForestLib.AgeSettings.Ages;

public class Age105Settings : IAgeSettings
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

    public double ExpedientBuildSpeedBonus = 0.0,
        ExpedientTrainSpeedBonus = 0.0,
        ExpedientDraft = 0.2;

    public double HasteTrainingSpeedBonus = 0.25,
        HasteConstructionSpeedBonus = 0.25;

    public double RitualCostPerAcre = 8.8;
    public double TowerProduction = 12;

    public Race Avian = new("Avian") { BirthRate = 1.2 };
    public Race Dwarf = new("Dwarf")
    {
        BuildingEfficiency = 1.3,
        FoodConsumption = 1.5,
        BuildingCost = 0
        // TODO: account for miners mystique
    };

    public Race DarkElf = new("Dark Elf") { BirthRate = 0.8 };
    // TODO: miners mystique, ToG
    public Race Elf = new("Elf") { BirthRate = 1.2 };
    public Race Faery = new("Faery") { LandEffectTowers = 1.25, Population = .9};
    public Race Halfling = new("Halfling") { Population = 1.1, MilitaryTrainingCost = .85 };
    public Race Human = new("Human") { Science = 1.1, Wages = 1.3 };
    public Race Orc = new("Orc") { };
    public Race Undead = new("Undead") { FoodConsumption = 0 };
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

    private BuildingEffectsAge105 buildingEffects;
    private ScienceEffectsAge105 scienceEffects;
    public Age105Settings()
    {
        Races = new[] { Avian, Dwarf, Elf, Faery, Halfling, Human, Orc, Undead, DarkElf };
        Personalities = new[] { Cleric, Heretic, Merchant, Mystic, Rogue, Sage, Shepherd, Tactician, WarHero, Warrior };
        buildingEffects = new BuildingEffectsAge105();
        scienceEffects = new ScienceEffectsAge105();
    }

    public IBuildingEffects GetBuildingEffects()
    {
        return buildingEffects;
    }

    public IScienceEffects GetScienceEffects()
    {
        return scienceEffects;
    }
}

public class BuildingEffectsAge105 : IBuildingEffects
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
public class ScienceEffectsAge105 : IScienceEffects
{
    public ScienceEffect Alchemy { get; } = new("Alchemy", 0.0724);
    public ScienceEffect Tools { get; } = new("Tools", 0.0524);
    public ScienceEffect Housing { get; } = new("Housing", 0.0262);
    public ScienceEffect Production { get; } = new("Production", 0.2172);
    public ScienceEffect Bookkeeping { get; } = new("Bookkeeping", 0.068);
    public ScienceEffect Artisan { get; } = new("Artisan", 0.0478);
    public ScienceEffect Strategy { get; } = new("Strategy", 0.0367);
    public ScienceEffect Siege { get; } = new("Siege", 0.0262);
    public ScienceEffect Tactics { get; } = new("Tactics", 0.0367);
    public ScienceEffect Valor { get; } = new("Valor", 0.0582);
    public ScienceEffect Heroism { get; } = new("Heroism", 0.0418);
    public ScienceEffect Resilience { get; } = new("Resilience", 0.0367);
    public ScienceEffect Crime { get; } = new("Crime", 0.1557);
    public ScienceEffect Channeling { get; } = new("Channeling", 0.1875);
    public ScienceEffect Shielding { get; } = new("Shielding", 0.0314);
    public ScienceEffect Sorcery { get; } = new("Sorcery", 0.0314);
    public ScienceEffect Cunning { get; } = new("Cunning", 0.0314);
    public ScienceEffect Finesse { get; } = new("Finesse", 0.0478);
}