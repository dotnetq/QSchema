namespace DotnetQ.QSchema
{
    public class QCharSchema : QSchema
    {
        const string CharType = "c";

        const string NullValue = "\" \"";

        public override string QType => "char";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        public static string ToQValue(object o)
        {
            if (o != null && o is char c)
            {
                return $"\"{c}\"";
            }

            return NullValue;
        }
    }
}
