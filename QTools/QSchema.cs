namespace QTools
{
    public abstract class QSchema
    {
        public const string EmptyList = "()";
        public const string Cast = "$";

        public abstract string QType { get; }
        public abstract string QNotation(object o);

        protected virtual bool IsAtom => true;
        protected virtual string ColumnType => IsAtom?string.Concat(QType, Cast):QType;

        public string AsTableColumn(string name, string qAttributes = "") => string.Concat(qAttributes,(IsAtom?"`":""), name, ":", ColumnType, EmptyList);
    }
}
