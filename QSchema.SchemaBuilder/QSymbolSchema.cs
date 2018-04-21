namespace DotnetQ.QSchema
{
    public class QSymbolSchema : QSchema
    {
        const string NullValue = "`";

        const string CharType = "s";

        public override string QType => "symbol";

        public override string QNotation(object o) => ToQValue(o);

        public static string ToQValue(object o)
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
