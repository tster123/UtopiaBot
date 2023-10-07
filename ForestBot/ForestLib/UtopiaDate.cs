using System.Text.RegularExpressions;

namespace ForestLib;

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

    /// <summary>
    /// Parses a utopia date from one of these formats:
    ///
    /// April 9 of YR2
    /// Apr 11, YR2
    /// Jun5YR1
    /// Feb  5 Y2
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static UtopiaDate Parse(string date)
    {
        try
        {
            Regex r = new Regex("(\\w{3})\\D*(\\d+)\\D+(\\d+)");
            var m = r.Match(date);
            if (!m.Success) throw new ApplicationException("Cannot parse utopia date [" + date + "]");
            int month = 1 + Array.IndexOf(new[] { "jan", "feb", "mar", "apr", "may", "jun", "jul" },
                m.Groups[1].Value.ToLower());
            return new UtopiaDate(int.Parse(m.Groups[3].Value), month, int.Parse(m.Groups[2].Value));
        }
        catch (Exception e)
        {
            throw new ApplicationException("Cannot parse UtopiaDate [" + date + "]", e);
        }
        
    }

    public override string ToString() => string.Format("{2}{0,3} Y{1}", Day, Year, MonthString());

    public string UglyString()
    {
        string str = string.Format("{2}{0}YR{1}", Day, Year, MonthString());
        if (Day < 10) str += " ";
        return str;
    }

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