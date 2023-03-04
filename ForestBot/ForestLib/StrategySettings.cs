namespace ForestLib
{
    public class StrategySettings
    {
        public double TargetHorseFill = 0.9;

        public double ExpectedExpedientStrength = 1.0,
            AverageConstructionScience = 0.03,
            AverageTrainingTimeScience = 0.04,
            ProductionScience = .30,
            ToolsScience = .05;

        public double RitualSuccessRate = 0.80;
        public double RitualCastsDesired = 5;
        public double RitualBuffer = .2;

        public double    AveragePeonsPerAcre = 15,
            TargetPeonsPerAcre = 6;

    }
}