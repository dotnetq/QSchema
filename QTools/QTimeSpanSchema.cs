using System;

namespace QTools
{
    public class QTimeSpanSchema : QSchema
    {
        const string CharType = "n";

        const string NullValue = "0Nn";

        public override string QType => "timespan";

        public override string QNotation(object o) => ToQValue(o);
        
        public static string ToQValue(object o)
        {
            if (o != null && o is TimeSpan value)
            {
                return value.ToString("hh:mm:ss.fffffffff");
            }
            return NullValue;
        }
    }
}
