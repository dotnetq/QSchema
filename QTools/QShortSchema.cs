namespace QTools
{
    public class QShortSchema : QSchema
    {
        public override string QType => "short";

        public override string QCharType => "h";

        public override string QNullValue => "0Nh";

        public override string QNotation(object o) => QNotationImpl<short>(o);
    }
}
