namespace QTools
{
    public class QRealSchema : QSchema
    {
        public override string QType => "real";

        public override string QCharType => "e";

        public override string QNullValue => "0Ne";

        public override string QNotation(object o) => QNotationImpl<float>(o);
    }
}
