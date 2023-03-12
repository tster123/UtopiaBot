using ForestLib.State;

namespace ForestLib.AgeSettings
{
    public interface IAgeSettings
    {
        Race[] Races { get; }
        Personality[] Personalities { get; }
        IBuildingEffects GetBuildingEffects();
        IScienceEffects GetScienceEffects();
    }
}