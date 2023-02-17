using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaLib.Tools
{
    public class TimelineEvent
    {
        public readonly UtopiaDate Date;
        public readonly string Event;

        public TimelineEvent(UtopiaDate date, string @event)
        {
            Date = date;
            Event = @event;
        }

        public override string ToString() => $"{Date} - {Event}";
    }
    public class CeasefireTimeline
    {
        private int ceasefireLength = 4 * 24;
        private int stableCapacity = 80;
        private double stableProduction = 2;
        private double loveAndPeaceHorceSpeed = 1.4;
        private double targetHorseFill = 0.9;

        private double baseBuildTime = 16,
            buildersBoonSpeedup = 0.25,
            expedientSavings = 0.2,
            expectedExpedientStrength = 1.0,
            averageConstructionScience = 0.03;

        public List<TimelineEvent> GetTimeline(UtopiaDate warEndDate)
        {
            var ret = new List<TimelineEvent>();
            ret.Add(new TimelineEvent(warEndDate, "War Ended"));
            UtopiaDate cfEnd = warEndDate.AddTicks(ceasefireLength + 1);
            ret.Add(new TimelineEvent(cfEnd, "Ceasefire Ends"));

            int buildTime = (int)(baseBuildTime *
                                  (1 - expectedExpedientStrength * expedientSavings) *
                                  (1 - averageConstructionScience) *
                                  (1 - buildersBoonSpeedup));

            double horsesNeeded = stableCapacity * targetHorseFill;
            double ticksToFillStables = horsesNeeded / (stableProduction * loveAndPeaceHorceSpeed);
            UtopiaDate buildStables = cfEnd.AddTicks(-1 * (buildTime + (int)Math.Round(ticksToFillStables)));
            ret.Add(new TimelineEvent(buildStables, "Build Stables (BB + L&P)"));
            return ret.OrderBy(t => t.Date).ToList();
        }
    }
}
