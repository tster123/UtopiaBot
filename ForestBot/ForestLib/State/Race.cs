namespace ForestLib.State;

public class Race
{
    public readonly string Name;
    public double Science = 1;
    public double BuildingEfficiency = 1;
    public double Population = 1;
    public double BirthRate = 1;
    public int HomeBonus = 0;
    public double ThiefCost = 1;
    public double FoodConsumption = 1;
    public double DraftCosts = 1;
    public double BuildingCost = 1;
    public bool AllowAccelerateConstruction = true;
    public double ConstructionTime = 1;
    public double TrainingTime = 1;

    public Race(string name)
    {
        Name = name;
    }
}