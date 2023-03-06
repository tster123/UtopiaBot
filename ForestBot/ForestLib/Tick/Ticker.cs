using ForestLib.State;

namespace ForestLib.Tick
{
    public class Ticker
    {
        public Age100Settings Age { get; }
        public ProvinceState State { get; }
        public UtopiaDate Date { get; private set; }

        public bool PatriatismOn { get; set; }
        // TODO: starvation

        public Ticker(Age100Settings age, ProvinceState provinceState, UtopiaDate date)
        {
            Age = age;
            State = provinceState;
            Date = date;
        }

        public void Tick()
        {
            Date = Date.AddTicks(1);
            Construction();
            Income();
            Food();
            Runes();
            PeasantGrowth();
            WizardGrowth();
            Draft();
            Training();
            ScientistGrowth();
            UpdateBuildingEfficiency();
            UpdateMilitaryEfficiency();
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
            //double targetBe = (0.5 * (1 + performed)) * State.Race.BuildingEfficiency * State.Science.Tools.Effect
            //TODO: need to fix science to work more like buildings and then finish this accounting for tools science.
            throw new NotImplementedException();
        }

        private void Runes()
        {
            State.Runes +=
                (int) Age.BuildingEffects.Tower.EffectiveFlatRate(State.BuildingEffectiveness, State.Buildings.Towers);
            State.Runes = (int) (State.Runes * .988);
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
            double uni = Age.BuildingEffects.UniversityScientistsSpawn.EffectiveEffect(State.BuildingEffectiveness, State.Buildings.Universities, State.Acres);
            double growth = .02 * State.Buildings.Universities * uni;
            State.Science.ProgressToNextScientist += growth;
            if (State.Science.ProgressToNextScientist >= 1.0)
            {
                State.Science.ProgressToNextScientist -= 1;
                State.Science.NumScientists++;
            }
        }

        private void Draft()
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

            double heroismScience = 1; // TODO: fix science.

            // Cost of Soldier Drafting = Current Draft Level Factor * Draft Rate * Race Bonus * Personality Bonus * Armouries Mod * Heroism Science Effect * Sloth Effect
            // Current Draft Level Factor scales base soldier draft cost upwards once 50% of max population is reached
            // Draft Level Factor is MAX(1.0154 * ((Solds + Ospecs + Dspecs + Elites) / maxpop) ^ 2 + 1.1759 * ((Solds + Ospecs + Dspecs + Elites) / maxpop) + 0.3633, 1) * base rate for level
            double draftPerc = State.TotalMilitaryPopulation / (double)State.GetMaxPopulation(Age);
            double levelFactor = Math.Max(1.0154 * draftPerc * draftPerc + 1.1759 * draftPerc + 0.3633, 1);

            // TODO: complete cost and rate

            // Draft Rate = Base Draft Rate * Patriotism Bonus * Heroism Science Effect * Sloth * Ritual

            throw new NotImplementedException();
        }

        private void WizardGrowth()
        {
            State.Wizards +=
                (int)Age.BuildingEffects.Guild.EffectiveFlatRate(State.BuildingEffectiveness, State.Buildings.Guilds, State.Personality.GuildEffectiveness);
        }

        private void PeasantGrowth()
        {
            throw new NotImplementedException();
        }

        private void Food()
        {
            throw new NotImplementedException();
        }

        private void Income()
        {
            throw new NotImplementedException();
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
