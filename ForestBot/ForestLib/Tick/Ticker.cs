using ForestLib.State;

namespace ForestLib.Tick
{
    public class Ticker
    {
        public Age100Settings Age { get; }
        public ProvinceState State { get; }
        public UtopiaDate Date { get; private set; }

        // TODO: starvation
        // TODO: ME
        // TODO: BE

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
        }

        private void Runes()
        {
            throw new NotImplementedException();
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
