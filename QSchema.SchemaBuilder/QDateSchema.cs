using System;

namespace DotnetQ.QSchema
{
    public class QDateSchema : QSchema
    {
        const string CharValue = "d";

        const string NullValue = "0Nd";

        public override string QType => "date";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        internal const string DateFormat = "yyyy'.'MM'.'dd";

        public static string ToQValue(object o)
        {
            if (o != null && o is DateTime value)
            {
                return value.ToString(DateFormat);
            }

            return NullValue;
        }
    }
}
