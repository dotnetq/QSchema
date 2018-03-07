namespace QTools
{
    public class QByteSchema : QSchema
    {
        public override string QType => "byte";

        public override string QCharType => "x";

        public override string QNullValue => "0x00";

        public override string QNotation(object o)
        {
            if (o != null && o is byte value)
            {
                return $"0x{value.ToString("X2")}";
            }
            return QNullValue;
        }
    }
}
