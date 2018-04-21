using System;

namespace DotnetQ.QSchema
{
    public class QDateTimeSchema : QSchema
    {
        const string CharType = "z";

        const string NullValue = "0Nz";

        public override string QType => "datetime";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        // TimeSpan & DateTime formatting not compatible
        internal const string DateTimeFormat = QDateSchema.DateFormat + "'T'" + "HH':'mm':'ss'.'fff"; 

        public static string ToQValue(object o)
        {
            if (o != null && o is DateTime value)
            {
                return value.ToString(DateTimeFormat);
            }

            return NullValue;
        }
    }
}
