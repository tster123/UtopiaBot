namespace UtopiaLib
{
    public class AgeSettings
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
    }
}