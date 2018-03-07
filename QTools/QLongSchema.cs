namespace QTools
{
    public class QLongSchema : QSchema
    {
        public override string QType => "long";

        public override string QCharType => "j";

        public override string QNullValue => "0Nj";

        public override string QNotation(object o) => QNotationImpl<long>(o);
    }
}
