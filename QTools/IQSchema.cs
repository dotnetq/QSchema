namespace QTools
{
    public interface IQSchema
    {
        string QType { get; }
        string QCharType { get; }
        string QNullValue { get; }
        string QNotation { get; }

        string AsTableColumn(string name, string qAttributes = "");
    }
}
