namespace ForestLib.AgeSettings;

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

public interface IBuildingEffects
{
    BuildingEffectCapacity BarrenCapacity { get; }
    BuildingEffectFlat BarrenFood { get; }
    BuildingEffectCapacity HomeCapacity { get; }
    BuildingEffectFlat HomeGrowth { get; }
    BuildingEffectFlat Farm { get; }
    BuildingEffectPercentage MillConstruction { get; }
    BuildingEffectPercentage MillExploreGold { get; }
    BuildingEffectPercentage MillExploreSold { get; }
    BuildingEffectPercentage BankPercentage { get; }
    BuildingEffectFlat BankFlat { get; }
    BuildingEffectPercentage TrainingGroundOme { get; }
    BuildingEffectPercentage TrainingGroundMerc { get; }
    BuildingEffectPercentage ArmouryWage { get; }
    BuildingEffectPercentage ArmouryDraftCost { get; }
    BuildingEffectPercentage ArmouryTrainingCost { get; }
    BuildingEffectPercentage Barracks { get; }
    BuildingEffectPercentage Fort { get; }
    BuildingEffectPercentage GuardStation { get; }
    BuildingEffectPercentage HospitalCasualties { get; }
    BuildingEffectPercentage HospitalPlague { get; }
    BuildingEffectFlat Guild { get; }
    BuildingEffectFlat Tower { get; }
    BuildingEffectPercentage ThievesDenLosses { get; }
    BuildingEffectPercentage ThievesDenTpa { get; }
    BuildingEffectPercentage WatchTowerCatch { get; }
    BuildingEffectPercentage WatchTowerDamage { get; }
    BuildingEffectPercentage UniversityScientistsSpawn { get; }
    BuildingEffectPercentage UniversityScience { get; }
    BuildingEffectPercentage Library { get; }
    BuildingEffectCapacity StableCapacity { get; }
    BuildingEffectFlat StableProduction { get; }
    BuildingEffectCapacity Dungeon { get; }
}