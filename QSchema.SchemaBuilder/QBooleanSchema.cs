namespace DotnetQ.QSchema
{
    public class QBooleanSchema : QSchema
    {
        const string CharType = "b";

        const string NullValue = "0b";

        public override string QType => "boolean";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        public static string ToQValue(object o)
        {
            if (o != null && o is bool value)
            {
                return $"{(value ? "1" : "0")}{CharType}";
            }
            return NullValue;
        }
    }
}
