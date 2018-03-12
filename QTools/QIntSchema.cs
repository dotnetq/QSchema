namespace QTools
{
    public class QIntSchema : QSchema
    {
        const string CharType = "i";

        const string NullValue = "0Ni";

        public override string QType => "int";

        public override string QNotation(object o) => ToQValue(o);

        public static string ToQValue(object o)
        {
            if (o != null && o is int value)
            {
                return $"{value}{CharType}";
            }

            return NullValue;
        }
    }
}
