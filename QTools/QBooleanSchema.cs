namespace QTools
{
    public class QBooleanSchema : QSchema
    {
        public override string QType => "boolean";

        public override string QCharType => "b";

        public override string QNullValue => "0b";

        public override string QNotation(object o)
        {
            if (o != null && o is bool value)
            {
                return $"{(value?"1":"0")}{QCharType}";
            }
            return QNullValue;
        }
    }
}
