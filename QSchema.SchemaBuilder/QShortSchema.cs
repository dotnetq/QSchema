namespace DotnetQ.QSchema
{
    public class QShortSchema : QSchema
    {
        const string CharType = "h";

        const string NullValue = "0Nh";

        public override string QType => "short";

        public override string QNotation(object o) => ToQValue(o);

        public static string ToQValue(object o)
        {
            if (o != null && o is short value)
            {
                return $"{value}{CharType}";
            }
            return NullValue;
        }
    }
}
