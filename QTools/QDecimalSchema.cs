namespace QTools
{
    public class QDecimalSchema : QSchema
    {
        public override string QType => "float";

        public override string QCharType => "f";

        public override string QNullValue => "0n";

        public override string QNotation(object o)
        {
            if (o != null && o is decimal value)
            {
                return $"{value}";
            }
            return QNullValue;
        }
    }
}
