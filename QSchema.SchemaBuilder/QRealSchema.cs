namespace DotnetQ.QSchema
{
    public class QRealSchema : QSchema
    {
        const string CharType = "e";

        const string NullValue = "0Ne";

        public override string QType => "real";

        public override string QNotation(object o) => ToQValue(o);

        public static string ToQValue(object o)
        {
            if (o != null && o is float value)
            {
                return $"{value}{CharType}";
            }
            return NullValue;
        }
    }
}
