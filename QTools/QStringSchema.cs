namespace QTools
{
    public class QStringSchema : QSchema
    {
        public override string QType => "string";

        protected override bool IsAtom => false;

        public override string QCharType => "";

        public override string QNullValue => "";

        public override string QNotation(object o)
        {
            if (o != null && o is string c)
            {
                return $"\"{c}\"";
            }
            return QNullValue;
        }
    }
}
