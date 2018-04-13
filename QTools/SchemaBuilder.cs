using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using QTools.Schema;
using TopologicalSorting;

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
                    var foreignKeyAttribute = keyProperty.GetCustomAttribute<ForeignKeyAttribute>();
                    var schemaAttribute = keyProperty.GetCustomAttribute<SchemaAttribute>();
                    if (foreignKeyAttribute == null)
                    {
                        var schemaType = GetQTypeSchema(keyProperty);
                        if (schemaType != null)
                        {
                            var qField = schemaType.AsTableColumn(LeadingLowercase(keyProperty.Name), schemaAttribute?.InQ());
                            keyPropertyTokens.Add(qField);
                        }
                    }
                    else
                    {
                        var tableName = GetQTableName(foreignKeyAttribute.ForeignType);
                        var qField = string.Concat(schemaAttribute?.InQ(), tableName, ":", "`", QSchema.EmptyList);
                        keyPropertyTokens.Add(qField);
                    }
                }

                keyPropertyNames = string.Join(";", keyPropertyTokens);
            }

            var bodyList = new List<string>();
            foreach (var bodyProperty in properties.Where(p => !p.HasAttribute<IgnoreAttribute>() && !p.HasAttribute<KeyAttribute>()))
            {
                var foreignKeyAttribute = bodyProperty.GetCustomAttribute<ForeignKeyAttribute>();
                var schemaType = GetQTypeSchema(bodyProperty);
                if (schemaType != null)
                {
                    var schemaAttribute = bodyProperty.GetCustomAttribute<SchemaAttribute>();
                    bodyList.Add(schemaType.AsTableColumn(LeadingLowercase(bodyProperty.Name), schemaAttribute?.InQ()));
                }
            }

            return string.Concat(qTableName, ":", "([", keyPropertyNames, "]",string.Join(";",bodyList),")");
        }

        private static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance;

        public static string DeclareEmptySchema(Type[] schemaTypes)
        {
            var result = new StringBuilder();
            var dependentTables = new HashSet<Type>();
            var tables = schemaTypes.ToDictionary(k => k, v => GetQTableName(v));

            var graph = new DependencyGraph<Type>();
            var dependencyCache = new ObjectFactoryCache<Type, OrderedProcess<Type>>(t => new OrderedProcess<Type>(graph,t));

            foreach(var tableType in tables.Keys)
            {
                var currentProcess = dependencyCache[tableType];
                var properties = tableType.GetProperties(BindingFlags);
                foreach(var prop in properties)
                {
                    var foreignKey = prop.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKey != null)
                    {
                        dependentTables.Add(tableType);
                        var foreignProcess = dependencyCache[foreignKey.ForeignType];
                        currentProcess.After(foreignProcess);
                    }
                }
            }

            // Topological sort is overkill if we declare FK's after tables, 
            // but essential if they are part of the initial declaration
            var sortResult = graph.CalculateSort();
            foreach(OrderedProcess<Type> process in sortResult)
            {
                result.AppendLine(DeclareEmptyTable(process.Value));
            }

            foreach(var depTable in dependentTables)
            {
                var properties = depTable.GetProperties(BindingFlags);
                foreach (var prop in properties)
                {
                    var foreignKey = prop.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKey != null)
                    {
                        var updateLine = string.Concat(
                            "update ", LeadingLowercase(prop.Name), ":`",
                            GetQTableName(foreignKey.ForeignType),QSchema.Cast,QSchema.EmptyList, 
                            " from `", GetQTableName(depTable));
                        result.AppendLine(updateLine);
                    }
                }
            }

            return result.ToString();
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
        
        protected readonly static Dictionary<Type, QSchema> QTypeSchema = new Dictionary<Type, QSchema>
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

    internal class ObjectFactoryCache<KeyT, ValueT>
    {
        public Func<KeyT, ValueT> FactoryMethod { get; protected set; }

        private readonly Dictionary<KeyT, ValueT> cache = new Dictionary<KeyT, ValueT>();

        public ObjectFactoryCache(Func<KeyT, ValueT> factoryMethod)
        {
            FactoryMethod = factoryMethod;
        }

        public ObjectFactoryCache()
        { }

        public ValueT this[KeyT key]
        {
            get
            {
                ValueT value;
                if (cache.TryGetValue(key, out value))
                {
                    return value;
                }

                value = FactoryMethod(key);
                cache.Add(key, value);
                return value;
            }
        }
    }
}
