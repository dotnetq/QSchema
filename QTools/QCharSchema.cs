namespace QTools
{
    public class QCharSchema : QSchema
    {
        public override string QType => "char";

        public override string QCharType => "c";

        public override string QNullValue => "\" \"";

        public override string QNotation(object o)
        {
            if (o != null && o is char c)
            {
                return $"\"{c}\"";
            }
            return QNullValue;
        }
    }
}
