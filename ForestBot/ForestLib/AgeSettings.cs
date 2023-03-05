using ForestLib.State;

namespace ForestLib
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
            BuildingEfficiency          = 1.25,
            FoodConsumption             = 1.5,
            ConstructionTime            = 0.5,
            AllowAccelerateConstruction = false,
            BuildingCost = 0
            // TODO: account for miners mystique
        };
        // TODO: miners mystique, ToG
        public Race Elf = new("Elf") { Population           = 0.9 };
        public Race Faery = new("Faery") {BirthRate         = 0.8, HomeBonus = -5};
        public Race Halfling = new("Halfling") {Population  = 1.1, ThiefCost = 0.7};
        public Race Human = new("Human") {BirthRate         = 1.2, Science   = 1.1};
        public Race Orc = new("Orc") {DraftCosts            = 0};
        public Race Undead = new("Undead") {FoodConsumption = 0, Science = 0.8};
        public Race[] Races;

        public Age100Settings()
        {
            Races = new[] {Avian, Dwarf, Elf, Faery, Halfling, Human, Orc, Undead};
        }
    }
}