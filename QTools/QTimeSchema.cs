using System;

namespace QTools
{
    public class QTimeSchema : QSchema
    {
        const string CharType = "t";

        const string NullValue = "0Nt";

        public override string QType => "time";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        internal const string TimeFormat = "hh':'mm':'ss'.'fff";

        public static string ToQValue(object o)
        {
            if (o != null && o is TimeSpan value)
            {
                return value.ToString(TimeFormat);
            }
            return NullValue;
        }
    }
}
