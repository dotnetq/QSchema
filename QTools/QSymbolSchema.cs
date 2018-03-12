namespace QTools
{
    public class QSymbolSchema : QSchema
    {
        const string NullValue = "`";

        const string CharType = "s";

        public override string QType => "symbol";

        public override string QNotation(object o) => ToQString(o);

        public static string ToQString(object o)
        {
            if (o != null && o is string value)
            {
                if (value.Contains(" "))
                {
                    return $"`$\"{value}\"";
                }
                return $"`{value}";
            }
            return NullValue;
        }
    }
}
