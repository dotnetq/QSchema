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
            var keyProperties = properties.Where(p => p.HasAttribute<KeyAttribute>());
            var bodyProperties = properties.Where(p => !p.HasAttribute<KeyAttribute>());
            if (keyProperties.Count() > 0)
            {
                var keyPropertyTokens = new List<string>();
                foreach(var keyProperty in keyProperties)
                {
                    var schemaType = GetQTypeSchema(keyProperty);
                    string qField = schemaType.AsTableColumn(keyProperty.Name);
                    keyPropertyTokens.Add(qField);
                }

                keyPropertyNames = string.Join(";", keyPropertyTokens);
            }
            else
            {
                var defaultKeyProperty = properties.First();
                var schemaType = GetQTypeSchema(defaultKeyProperty);
                string qField = schemaType.AsTableColumn(defaultKeyProperty.Name);
                keyPropertyNames = qField;
                bodyProperties = properties.Skip(1);
            }

            var bodyList = new List<string>();
            foreach (var bodyProperty in bodyProperties)
            {
                var schemaAttribute = bodyProperty.GetCustomAttribute<SchemaAttribute>();
                bodyList.Add(GetQTypeSchema(bodyProperty).AsTableColumn(bodyProperty.Name, schemaAttribute?.InQ()));
            }

            return string.Concat(qTableName, ":", "([", keyPropertyNames, "]",string.Join(";",bodyList),")");
        }

        static QSchema GetQTypeSchema(MemberInfo info)
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

            return QTypeSchema[dataType];
        }

        readonly static QBooleanSchema QBooleanSchema = new QBooleanSchema();
        readonly static QCharSchema QCharSchema = new QCharSchema();
        readonly static QByteSchema QByteSchema = new QByteSchema();
        readonly static QShortSchema QShortSchema = new QShortSchema();
        readonly static QIntegerSchema QIntegerSchema = new QIntegerSchema();
        readonly static QLongSchema QLongSchema = new QLongSchema();
        readonly static QRealSchema QRealSchema = new QRealSchema();
        readonly static QFloatSchema QFloatSchema = new QFloatSchema();
        readonly static QDecimalSchema QDecimalSchema = new QDecimalSchema();
        readonly static QDateTimeSchema QDateTimeSchema = new QDateTimeSchema();
        readonly static QDateSchema QDateSchema = new QDateSchema();
        readonly static QTimeSchema QTimeSchema = new QTimeSchema();
        readonly static QTimeSpanSchema QTimeSpanSchema = new QTimeSpanSchema();
        readonly static QSymbolSchema QSymbolSchema = new QSymbolSchema();
        readonly static QStringSchema QStringSchema = new QStringSchema();

        class Date { }
        class Time { }
        class String { }
        
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
