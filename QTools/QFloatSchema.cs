namespace QTools
{
    public class QFloatSchema : QSchema
    {
        public override string QType => "float";

        public override string QCharType => "f";

        public override string QNullValue => "0n";

        public override string QNotation(object o)
        {
            if (o != null && o is double value)
            {
                return $"{value}";
            }
            return QNullValue;
        }
    }
}
