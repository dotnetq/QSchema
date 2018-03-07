namespace QTools
{
    public class QIntegerSchema : QSchema
    {
        public override string QType => "integer";

        public override string QCharType => "i";

        public override string QNullValue => "0Ni";

        public override string QNotation(object o) => QNotationImpl<int>(o);
    }
}
