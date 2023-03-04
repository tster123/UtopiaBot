namespace ForestLib.KewlStuff
{
    public static class StringExtension
    {
        public static string? Nullify(this string v) => v.Trim() == "" ? null : v;
        public static int? Intify(this string? v) => v == null ? null : int.Parse(v);
        public static int ParseInt(this string? v) => v == null ? 0 : int.Parse(v);
    }
}
