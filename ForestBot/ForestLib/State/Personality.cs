namespace ForestLib.State;

public class Personality
{
    public readonly string Name;

    public Personality(string name)
    {
        Name = name;
    }

    public double GuildEffectiveness = 1;
    public double Income = 1;
    public double Wages = 1;
    public double HonorEffectiveness = 1;
}