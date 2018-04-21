namespace DotnetQ.QSchema
{
    public class QByteSchema : QSchema
    {
        const string CharType = "x";

        const string NullValue = "0x00";

        public override string QType => "byte";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        public static string ToQValue(object o)
        {
            if (o != null && o is byte value)
            {
                return $"0x{value.ToString("X2")}";
            }
            return NullValue;
        }
    }
}
