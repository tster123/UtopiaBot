using ForestLib.AgeSettings;
using ForestLib.AgeSettings.Ages;
using ForestLib.State;

namespace ForestLib.Tick
{

    public class TickResult
    {
        public int FoodEaten, FoodProduced, FoodDecayed;
        public int? StarvationDeficit;
        public int RunesProduced, RunesDecayed;
        public int GoldIncome, GoldWagesOwed, GoldWagesPaid, GoldDraftSpent;
        public int SoldiersDrafted;
        public bool AllWagesPaid;
        public int PeasantGrowth;
    }
    public class Ticker
    {
        public IAgeSettings Age { get; }
        public ProvinceState State { get; }
        public UtopiaDate Date { get; private set; }

        public int? TicksSinceCeasefireStart { get; set; }

        public bool PatriatismOn { get; set; }
        // TODO: starvation

        public Ticker(Age100Settings age, ProvinceState provinceState, UtopiaDate date)
        {
            Age = age;
            State = provinceState;
            Date = date;
        }

        public TickResult Tick()
        {
            TickResult result = new();
            Date = Date.AddTicks(1);
            Construction();
            Income(result);
            Food(result);
            Runes(result);
            PeasantGrowth(result);
            WizardGrowth();
            Draft(result);
            Training();
            ScientistGrowth();
            UpdateBuildingEfficiency();
            UpdateMilitaryEfficiency();
            if (TicksSinceCeasefireStart != null) TicksSinceCeasefireStart++;
            return result;
        }

        private void UpdateMilitaryEfficiency()
        {
            //Base Military Efficiency =  .33 + .67 * ( Effective Wage Rate ) ^ 0.25
            double effectiveWageRate = Math.Pow((State.MilitaryEfficiency - 0.33) / 0.67, 4);

            //Change in Effective Wage Rate =  0.05 * (Wage Rate Paid - Effective Wage Rate)
            double change = 0.05 * (State.WageRate - effectiveWageRate);
            effectiveWageRate        += change;
            
            State.MilitaryEfficiency =  .33 + .67 * Math.Pow(effectiveWageRate, 0.25);
        }

        private void UpdateBuildingEfficiency()
        {
            // NOTE: this is from discord, can't find the change formula on wiki
            // Predicted BE = current BE + (final BE - current BE) * (1 - 1/(2^(hours/12)))

            // from wiki
            /*
                Available Workers         =  Peasants + ROUNDDOWN ( Prisoners / 2 )
                Optimal Workers           =  ROUNDDOWN ( Total Jobs * 0.67 )
                % Jobs Performed          =  MIN ( Available Workers / Optimal Workers , 1 )
                Building Efficiency       =  (0.5 * (1 + % Jobs Performed)) * Race * Personality * Tools Science * Dragon * Blizzard
             */
            double availableWorkers = State.Peasants + State.Prisoners;
            double optimalWorkers = .67 * 25 * (State.Buildings.Built - State.Buildings.Homes);
            double performed = Math.Min(1, availableWorkers / optimalWorkers);

            double toolsScienceBonus = Age.GetScienceEffects().Tools.GetBonus(State.Science.Tools, State, Age);
            double targetBe = (0.5 * (1 + performed)) * State.Race.BuildingEfficiency * (1 + toolsScienceBonus);

            double movementPercentage = (1 - 1 / Math.Pow(2, 1 / 12.0));
            double beDiff = targetBe - State.BuildingEffectiveness;
            double predictedBe = State.BuildingEffectiveness + beDiff * movementPercentage;
            State.BuildingEffectiveness = predictedBe;
        }
        private void Food(TickResult result)
        {
            double farmFood = Age.GetBuildingEffects().Farm.EffectiveFlatRate(State.BuildingEffectiveness, State.Buildings.Farms);
            double barrenFood = Age.GetBuildingEffects().BarrenFood.EffectiveFlatRate(State.BuildingEffectiveness, State.Buildings.Barren);
            double prodScience = Age.GetScienceEffects().Production.GetBonus(State.Science.Production, State, Age);
            
            double decay = State.Food * 0.01;

            double fertileLands = 1.25;

            double baseFoodProd = farmFood + barrenFood;
            double modFoodProd = baseFoodProd * (1 + prodScience) * fertileLands;

            double eaten = State.Race.FoodConsumption * State.GetTotalPopulation() * 0.25;

            State.Food = (int) (State.Food - decay + modFoodProd - eaten);
            if (State.Food < 0)
            {
                result.StarvationDeficit = -1 * State.Food;
                State.Food = 0;
            }

            result.FoodDecayed = (int)decay;
            result.FoodEaten = (int)eaten;
            result.FoodProduced = (int)modFoodProd;
        }
        private void Runes(TickResult result)
        {
            double baseRunes = Age.GetBuildingEffects().Tower.EffectiveFlatRate(State.BuildingEffectiveness, State.Buildings.Towers);
            double prodScience = Age.GetScienceEffects().Production.GetBonus(State.Science.Production, State, Age);
            result.RunesProduced = (int)(baseRunes * (1 + prodScience));
            State.Runes += result.RunesProduced;
            result.RunesDecayed = (int)(State.Runes * 0.012);
            State.Runes -= result.RunesDecayed;
        }

        private void Training()
        {
            List<Tuple<int, MilitaryPopulation>> post = new();
            foreach (var trainTick in State.MilitaryTraining)
            {
                if (trainTick.Item1 == 0)
                {
                    State.Military.Add(trainTick.Item2);
                }
                else
                {
                    post.Add(new Tuple<int, MilitaryPopulation>(trainTick.Item1 - 1, trainTick.Item2));
                }
            }

            State.MilitaryTraining = post;
        }

        private void ScientistGrowth()
        {
            double uni = Age.GetBuildingEffects().UniversityScientistsSpawn.EffectiveEffect(State.BuildingEffectiveness, State.Buildings.Universities, State.Acres);
            double growth = .02 * State.Buildings.Universities * uni;
            State.Science.ProgressToNextScientist += growth;
            if (State.Science.ProgressToNextScientist >= 1.0)
            {
                State.Science.ProgressToNextScientist -= 1;
                State.Science.NumScientists++;
            }
        }

        private void Draft(TickResult result)
        {
            //Reservist   0.3 % 30gc
            //    Normal  0.6 % 50gc
            //    Aggressive  1.0 % 75gc
            //    Emergency   1.5 % 110gc
            if (State.DraftRate == DraftRate.None) return;
            double rate, cost;
            switch (State.DraftRate)
            {
                case DraftRate.Reserve:
                    rate = 0.003;
                    cost = 30;
                    break;
                case DraftRate.Normal:
                    rate = 0.006;
                    cost = 50;
                    break;
                case DraftRate.Aggressive:
                    rate = 0.01;
                    cost = 75;
                    break;
                case DraftRate.Emergency:
                    rate = .015;
                    cost = 110;
                    break;
                default:
                    throw new ArgumentException("unknown draftRate: " + State.DraftRate);
            }

            double heroism = Age.GetScienceEffects().Heroism.GetBonus(State.Science.Heroism, State, Age);
            double draftRitual = 1; // TODO: factor ritual



            // Cost of Soldier Drafting = Current Draft Level Factor * Draft Rate * Race Bonus * Personality Bonus * Armouries Mod * Heroism Science Effect * Sloth Effect
            // Current Draft Level Factor scales base soldier draft cost upwards once 50% of max population is reached
            // Draft Level Factor is MAX(1.0154 * ((Solds + Ospecs + Dspecs + Elites) / maxpop) ^ 2 + 1.1759 * ((Solds + Ospecs + Dspecs + Elites) / maxpop) + 0.3633, 1) * base rate for level
            double draftPerc = State.TotalMilitaryPopulation / (double)State.GetMaxPopulation(Age);
            double levelFactor = Math.Max(1.0154 * draftPerc * draftPerc + 1.1759 * draftPerc + 0.3633, 1);
            double armouryCostSavings = Age.GetBuildingEffects().ArmouryDraftCost.EffectiveEffect(State.BuildingEffectiveness, State.Buildings.Armouries, State.Acres);
            int draftCost = (int)(levelFactor * cost * (1 - armouryCostSavings) * (1 - heroism));

            // Draft Rate = Base Draft Rate * Patriotism Bonus * Heroism Science Effect * Sloth * Ritual
            double draftRate = rate * 1.3 * (1 + heroism) * draftRitual;

            int numDraft = (int)(State.Peasants * draftRate);
            int totalCost = draftCost * numDraft;

            int overdraft = totalCost - State.Money;
            if (overdraft > 0)
            {
                int numReduce = 1 + (overdraft / draftCost);
                numDraft  -= numReduce;
                totalCost =  draftCost * numDraft;
            }

            result.GoldDraftSpent = totalCost;
            result.SoldiersDrafted = numDraft;
            State.Money             -= totalCost;
            State.Peasants          -= numDraft;
            State.Military.Soldiers += numDraft;
        }

        private void WizardGrowth()
        {
            int wizardsTrained = (int)Age.GetBuildingEffects().Guild.EffectiveFlatRate(State.BuildingEffectiveness,
                State.Buildings.Guilds, State.Personality.GuildEffectiveness);
            State.Wizards += wizardsTrained;
            State.Peasants -= wizardsTrained;
        }

        private void PeasantGrowth(TickResult result)
        {
            int maxPop = State.GetMaxPopulation(Age);
            int currentPop = State.GetTotalPopulation();
            if (maxPop == currentPop) return;
            if (maxPop < currentPop)
            {
                // overpop
                double leave = Math.Min(.1 * State.Peasants, currentPop - maxPop);
                State.Peasants -= (int)leave;
                result.PeasantGrowth = -1 * (int)leave;
                return;
            }
            //Peasants Hourly Change = (Current Peasants * ((Birth Rate + Love & Peace) * Race Bonus * EOWCF * Chastity - Storms)) + (Homes bonus * Chastity) - Drafted Soldiers - Wizards Trained
            // Base birth rate is 2.05% and ranges from 1.9457% up to 2.1525% (± 5% of 2.05%)
            // * +1000% Birthrate (minimum 500) for the first 36 hours
            double birthRate = 0.0205;
            if (TicksSinceCeasefireStart is < 36)
            {
                birthRate *= 10;
            }

            int born = Math.Min((int)(birthRate * State.Peasants), maxPop - currentPop);
            result.PeasantGrowth = born;
            State.Peasants += born;
        }

        private void Income(TickResult result)
        {
            // Raw Income = (3 * Employed Peasants) + (1 * Unemployed Peasants) + (0.75 * Prisoners) + Racial Gold Generation + (Banks * 25 * BE) + Miner's Mystique Gold Generation
            // Modified Income = Raw Income * Plague * Riots * Bank % Bonus * Income Sci * Honor Income Mod 
            //     *Race Mod* Personality Mod* Dragon *Ritual
            int jobs = 25 * (State.Buildings.Built - State.Buildings.Homes);
            int employedPeons = Math.Min(jobs, State.Peasants);
            int unemployedPeons = Math.Max(0, State.Peasants - jobs);
            double bankBase = (int) Age.GetBuildingEffects().BankFlat.EffectiveFlatRate(State.BuildingEffectiveness, State.Buildings.Banks);
            double rawIncome = bankBase + 3.0 * employedPeons + 1.0 * unemployedPeons + 0.75 * State.Prisoners;

            double bankMod = Age.GetBuildingEffects().BankPercentage.EffectiveEffect(State, State.Buildings.Banks);
            double incomeScience = Age.GetScienceEffects().Alchemy.GetBonus(State.Science.Alchemy, State, Age);
            double honorBonus = State.Honor.IncomeBonus;
            double income = rawIncome * (1 + bankMod) + (1 + incomeScience) * (1 + honorBonus);
            
            // Military Expenses = (((Def specs + Off specs )*0.5) + Elites * 0.75) * Wage Rate * Armouries Bonus * Race Mod * Personality Mod * max(Inspire Army , Hero's Inspiration) * Greed * Ritual * Dragon * Bookkeeping Science Effect 
            MilitaryPopulation mil = State.Military;
            double wages = (mil.DefSpecs + mil.OffSpecs) * 0.5 + 0.75 * mil.Elites;
            double armSavings = Age.GetBuildingEffects().ArmouryWage.EffectiveEffect(State, State.Buildings.Armouries);
            double wageScience = Age.GetScienceEffects().Bookkeeping.GetBonus(State.Science.Bookkeeping, State, Age);
            wages = wages * State.WageRate * (1 - armSavings) * State.Personality.Wages * (1 - wageScience);

            State.Money += (int)(income - wages);
            result.GoldWagesOwed = (int)wages;
            result.GoldWagesPaid = result.GoldWagesOwed;
            result.GoldIncome = (int)income;
            result.AllWagesPaid = State.Money >= 0;
            if (State.Money < 0)
            {
                result.GoldWagesPaid += State.Money;
                State.Money = 0;
            }
        }

        private void Construction()
        {
            List<Tuple<int, Buildings>> post = new();
            foreach (var constructionTick in State.BuildinsInProgress)
            {
                if (constructionTick.Item1 == 0)
                {
                    State.Buildings.Add(constructionTick.Item2);
                }
                else
                {
                    post.Add(new Tuple<int, Buildings>(constructionTick.Item1 - 1, constructionTick.Item2));
                }
            }

            State.BuildinsInProgress = post;
        }
    }
}
