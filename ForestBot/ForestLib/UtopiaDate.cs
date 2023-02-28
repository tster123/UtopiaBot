namespace ForestLib
{
    public class UtopiaDate : IComparable<UtopiaDate>
    {
        public readonly int Year, Month, Day;
        public readonly int Tick;

        public UtopiaDate(int year, int month, int day)
        {
            if (day < 1 || day > 24) throw new ArgumentOutOfRangeException(nameof(day));
            if (month < 1 || month > 7) throw new ArgumentOutOfRangeException(nameof(month));
            if (year < 0) throw new ArgumentOutOfRangeException(nameof(year));
            Year = year;
            Month = month;
            Day = day;
            Tick = (Day - 1) + (Month - 1) * 24 + Year * 24 * 7;
        }

        public UtopiaDate(int tick)
        {
            Year = tick / (24 * 7);
            int tickOfYear = tick % (24 * 7);
            Month = 1 + (tickOfYear / 24);
            Day = 1 + (tickOfYear % 24);
            Tick = tick;
        }
        
        public override string ToString() => string.Format("{2}{0,3} Y{1}", Day, Year, MonthString());

        public string MonthString()
        {
            return new[]
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul"
            }[Month - 1];
        }

        public UtopiaDate AddTicks(int ticks) => new UtopiaDate(Tick + ticks);

        public int CompareTo(UtopiaDate? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Tick.CompareTo(other.Tick);
        }

        protected bool Equals(UtopiaDate other)
        {
            return Tick == other.Tick;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UtopiaDate)obj);
        }

        public override int GetHashCode()
        {
            return Tick;
        }
    }
}
