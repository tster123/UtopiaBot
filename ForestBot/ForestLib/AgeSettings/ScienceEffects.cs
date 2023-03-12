using ForestLib.State;

namespace ForestLib.AgeSettings;

public class ScienceEffect
{
    public readonly string name;
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
        double ret = Math.Pow(books, (1 / 2.125));
        ret *= state.Race.Science;
        // TODO: sage has scientific insights
        ret *= (1 + age.GetBuildingEffects().Library.EffectiveEffect(state.BuildingEffectiveness, state.Buildings.Libraries, state.Acres));
        return ret;
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