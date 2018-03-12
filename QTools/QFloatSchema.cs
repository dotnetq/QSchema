namespace QTools
{
    public class QFloatSchema : QSchema
    {
        const string CharType = "f";

        const string NullValue = "0n";

        public override string QType => "float";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        public static string ToQValue(object o)
        {
            if (o != null && o is double value)
            {
                return $"{value}";
            }
            return NullValue;
        }
    }
}
