using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using QTools.Schema;

namespace QTools
{
    public class SchemaBuilder
    {
        public static string DeclareEmptyTable(Type t)
        {
            var qTableName = GetQTableName(t);
            var properties = t.GetProperties(BindingFlags.Public|BindingFlags.Instance);

            string keyPropertyNames = null;
            var keyProperties = properties.Where(p => !p.HasAttribute<IgnoreAttribute>() && p.HasAttribute<KeyAttribute>());
            if (keyProperties.Count() > 0)
            {
                var keyPropertyTokens = new List<string>();
                foreach(var keyProperty in keyProperties)
                {
                    var schemaAttribute = keyProperty.GetCustomAttribute<SchemaAttribute>();
                    var schemaType = GetQTypeSchema(keyProperty);
                    if (schemaType != null)
                    {
                        string qField = schemaType.AsTableColumn(LeadingLowercase(keyProperty.Name), schemaAttribute?.InQ());
                        keyPropertyTokens.Add(qField);
                    }
                }

                keyPropertyNames = string.Join(";", keyPropertyTokens);
            }

            var bodyList = new List<string>();
            foreach (var bodyProperty in properties.Where(p => !p.HasAttribute<IgnoreAttribute>() && !p.HasAttribute<KeyAttribute>()))
            {
                var schemaType = GetQTypeSchema(bodyProperty);
                if (schemaType != null)
                {
                    var schemaAttribute = bodyProperty.GetCustomAttribute<SchemaAttribute>();
                    bodyList.Add(schemaType.AsTableColumn(LeadingLowercase(bodyProperty.Name), schemaAttribute?.InQ()));
                }
            }

            return string.Concat(qTableName, ":", "([", keyPropertyNames, "]",string.Join(";",bodyList),")");
        }

        public static QSchema GetQTypeSchema(MemberInfo info)
        {
            Type dataType = null;
            if (info.HasAttribute<DateAttribute>())
            {
                dataType = typeof(Date);
            }
            else if (info.HasAttribute<TimeAttribute>())
            {
                dataType = typeof(Time);
            }
            else if (info.HasAttribute<StringAttribute>())
            {
                dataType = typeof(String);
            }
            else
            {
                if (info is PropertyInfo propInfo)
                {
                    dataType = propInfo.PropertyType;
                }
                else if (info is FieldInfo fieldInfo)
                {
                    dataType = fieldInfo.FieldType;
                }
            }

            QSchema result;
            if (QTypeSchema.TryGetValue(dataType, out result))
            {
                return result;
            }
            return null;
        }

        private readonly static QBooleanSchema QBooleanSchema = new QBooleanSchema();
        private readonly static QCharSchema QCharSchema = new QCharSchema();
        private readonly static QByteSchema QByteSchema = new QByteSchema();
        private readonly static QShortSchema QShortSchema = new QShortSchema();
        private readonly static QIntSchema QIntegerSchema = new QIntSchema();
        private readonly static QLongSchema QLongSchema = new QLongSchema();
        private readonly static QRealSchema QRealSchema = new QRealSchema();
        private readonly static QFloatSchema QFloatSchema = new QFloatSchema();
        private readonly static QDecimalSchema QDecimalSchema = new QDecimalSchema();
        private readonly static QDateTimeSchema QDateTimeSchema = new QDateTimeSchema();
        private readonly static QDateSchema QDateSchema = new QDateSchema();
        private readonly static QTimeSchema QTimeSchema = new QTimeSchema();
        private readonly static QTimeSpanSchema QTimeSpanSchema = new QTimeSpanSchema();
        private readonly static QSymbolSchema QSymbolSchema = new QSymbolSchema();
        private readonly static QStringSchema QStringSchema = new QStringSchema();

        private class Date { }
        private class Time { }
        private class String { }
        
        readonly static Dictionary<Type, QSchema> QTypeSchema = new Dictionary<Type, QSchema>
        {
            {typeof(bool), QBooleanSchema },
            {typeof(bool?), QBooleanSchema },
            {typeof(char), QCharSchema },
            {typeof(char?), QCharSchema },
            {typeof(byte), QByteSchema },
            {typeof(byte?), QByteSchema },
            {typeof(short), QShortSchema },
            {typeof(short?), QShortSchema },
            {typeof(int), QIntegerSchema},
            {typeof(int?), QIntegerSchema },
            {typeof(long), QLongSchema},
            {typeof(long?), QLongSchema },
            {typeof(float), QRealSchema },
            {typeof(float?), QRealSchema },
            {typeof(double), QFloatSchema },
            {typeof(double?), QFloatSchema },
            {typeof(decimal), QDecimalSchema },
            {typeof(decimal?), QDecimalSchema },
            {typeof(DateTime), QDateTimeSchema },
            {typeof(DateTime?), QDateTimeSchema },
            {typeof(TimeSpan), QTimeSpanSchema },
            {typeof(TimeSpan?), QTimeSpanSchema },
            {typeof(Date), QDateSchema },
            {typeof(Time), QTimeSchema },
            {typeof(string), QSymbolSchema },
            {typeof(String), QStringSchema },
        };

        public static string GetQTableName(Type t)
        {
            var @namespace = t.Namespace.ToLower();
            var tableName = LeadingLowercase(t.Name);
            return string.Concat(".", @namespace, ".", tableName);
        }

        private static string LeadingLowercase(string s) => string.Concat(s[0].ToString().ToLower(), s.Substring(1));
    }
}
