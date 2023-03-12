using ForestLib.State;

namespace ForestLib.AgeSettings;

public class ScienceEffect
{
    private readonly string name;
    private readonly double multiplier;

    public ScienceEffect(string name, double multiplier)
    {
        this.name = name;
        this.multiplier = multiplier;
    }

    public override string ToString()
    {
        return $"{nameof(ScienceEffect)}: {name}";
    }

    public double GetBonus(int books, ProvinceState state, IAgeSettings age)
    {
        return (Math.Pow(books, (1 / 2.125)) * multiplier * GetTotalMod(state, age)) / 100.0;
    }

    public int CalculateBooksNeededFor(double bonus, ProvinceState state, IAgeSettings age)
    {
        double mod = GetTotalMod(state, age);
        return (int)Math.Pow((bonus * 100 / (multiplier * mod)), (2.125 / 1.0));
    }

    private double GetTotalMod(ProvinceState state, IAgeSettings age)
    {
        double libEffect = (1 + age.GetBuildingEffects().Library.EffectiveEffect(state.BuildingEffectiveness, state.Buildings.Libraries, state.Acres));
        // TODO: account for sage has scientific insights
        return state.Race.Science * libEffect;
    }
}

public interface IScienceEffects
{
    ScienceEffect Alchemy { get; }
    ScienceEffect Tools { get; }
    ScienceEffect Housing { get; }
    ScienceEffect Production { get; }
    ScienceEffect Bookkeeping { get; }
    ScienceEffect Artisan { get; }
    ScienceEffect Strategy { get; }
    ScienceEffect Siege { get; }
    ScienceEffect Tactics { get; }
    ScienceEffect Valor { get; }
    ScienceEffect Heroism { get; }
    ScienceEffect Resilience { get; }
    ScienceEffect Crime { get; }
    ScienceEffect Channeling { get; }
    ScienceEffect Shielding { get; }
    ScienceEffect Sorcery { get; }
    ScienceEffect Cunning { get; }
    ScienceEffect Finesse { get; }
}