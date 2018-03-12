using System;

namespace QTools
{
    public class QDateTimeSchema : QSchema
    {
        const string CharType = "z";

        const string NullValue = "0Nz";

        public override string QType => "`datetime$";

        public override string QNotation(object o)
        {
            return ToQValue(o);
        }

        internal const string QDateTimeFormat = QDateSchema.QDateFormat + "'T'" + QTimeSchema.QTimeFormat;

        public static string ToQValue(object o)
        {
            if (o != null && o is DateTime value)
            {
                return value.ToString(QDateTimeFormat);
            }

            return NullValue;
        }
    }
}
