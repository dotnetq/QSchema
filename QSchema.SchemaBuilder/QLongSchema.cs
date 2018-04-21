namespace DotnetQ.QSchema
{
    public class QLongSchema : QSchema
    {
        const string CharType = "j";

        const string NullValue = "0Nj";

        public override string QType => "long";

        public override string QNotation(object o) => ToQValue(o);

        public static string ToQValue(object o)
        {
            if (o != null && o is long value)
            {
                return $"{value}{CharType}";
            }
            return NullValue;
        }
    }
}
