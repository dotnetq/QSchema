namespace DotnetQ.QSchema
{
    public class QStringSchema : QSchema
    {
        const string NullValue = "\"\"";

        public override string QType => "string";

        protected override bool IsAtom => false;

        public override string QNotation(object o) => ToQValue(o);

        public static string ToQValue(object o)
        {
            if (o != null && o is string value)
            {
                return $"\"{value}\"";
            }
            return NullValue;
        }
    }
}
