using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using DotnetQ.QSchema.Attributes;
using TopologicalSorting;

namespace DotnetQ.QSchema
{
    public class SchemaBuilder
    {
        public static string DeclareEmptyTable(Type t)
        {
            var qTableName = GetQTableName(t);
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string keyPropertyNames = null;
            var keyProperties = properties.Where(p => !p.HasAttribute<IgnoreAttribute>() && p.HasAttribute<KeyAttribute>());
            if (keyProperties.Count() > 0)
            {
                var keyPropertyTokens = new List<string>();
                foreach (var keyProperty in keyProperties)
                {
                    var foreignKey = keyProperty.GetCustomAttribute<ForeignKeyAttribute>();
                    var schemaAttribute = keyProperty.GetCustomAttribute<SchemaAttribute>();
                    if (foreignKey == null)
                    {
                        var schemaType = GetQTypeSchema(keyProperty);
                        if (schemaType != null)
                        {
                            var qField = schemaType.AsTableColumn(VerifyColumnName(keyProperty), schemaAttribute?.InQ());
                            keyPropertyTokens.Add(qField);
                        }
                    }
                    else
                    {
                        var tableName = GetQTableName(foreignKey.ForeignType);
                        var qField = string.Concat(schemaAttribute?.InQ(), tableName, ":`", tableName, QSchema.Cast, QSchema.EmptyList);
                        keyPropertyTokens.Add(qField);
                    }
                }

                keyPropertyNames = string.Join(";", keyPropertyTokens);
            }

            var bodyList = new List<string>();
            foreach (var bodyProperty in properties.Where(p => !p.HasAttribute<IgnoreAttribute>() && !p.HasAttribute<KeyAttribute>()))
            {
                var foreignKey = bodyProperty.GetCustomAttribute<ForeignKeyAttribute>();
                if (foreignKey == null)
                {
                    var schemaType = GetQTypeSchema(bodyProperty);
                    if (schemaType != null)
                    {
                        var schemaAttribute = bodyProperty.GetCustomAttribute<SchemaAttribute>();
                        bodyList.Add(schemaType.AsTableColumn(VerifyColumnName(bodyProperty), schemaAttribute?.InQ()));
                    }
                }
                else
                {
                    bodyList.Add(string.Concat(VerifyColumnName(bodyProperty), ":`",
                        GetQTableName(foreignKey.ForeignType), QSchema.Cast, QSchema.EmptyList));
                }
            }

            return string.Concat(qTableName, ":", "([", keyPropertyNames, "]", string.Join(";", bodyList), ")");
        }

        public class InvalidColumnNameException : Exception
        {
            public InvalidColumnNameException(MemberInfo info)
                : base($"{info.DeclaringType}, {info.Name} will create an invalid column name.")
            {
                Info = info;
            }

            public MemberInfo Info { get; }
        }

        private static string VerifyColumnName(MemberInfo info)
        {
            var overrideColName = info.GetCustomAttribute<ColNameAttribute>();
            var result = overrideColName != null ? overrideColName.Name : LeadingLowercase(info.Name);

            if (reservedWords.Contains(result)) throw new InvalidColumnNameException(info);

            return result;
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

            // Output tables in dependency order
            var sortResult = graph.CalculateSort();
            foreach(OrderedProcess<Type> process in sortResult)
            {
                result.AppendLine(DeclareEmptyTable(process.Value));
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

        private static readonly HashSet<string> reservedWords = new HashSet<string>
        {
            "abs", "acos", "aj", "aj0", "all", "and", "any", "asc", "asin", "asof", "atan", "attr", "avg", "avgs",
            "bin", "binr",
            "ceiling", "cols", "cor", "cos", "count", "cov", "cross", "csv", "cut",
            "delete", "deltas", "desc", "dev", "differ", "distinct", "div", "do", "dsave",
            "each", "ej", "ema", "enlist", "eval", "except", "exec", "exit", "exp",
            "fby", "fills", "first", "fkeys", "flip", "floor",
            "get", "getenv", "group", "gtime",
            "hclose", "hcount", "hdel", "hopen", "hsym",
            "iasc", "idesc", "if", "ij", "in", "insert", "inter", "inv",
            "key", "keys",
            "last", "like", "lj", "ljf", "load", "log", "lower", "lsq", "ltime", "ltrim",
            "mavg", "max", "maxs", "mcount", "md5", "mdev", "med", "meta", "min", "mins", "mmax", "mmin", "mmu", "mod", "msum",
            "neg", "next", "not", "null",
            "or", "over",
            "parse", "peach", "pj", "plist", "prd", "prds", "prev", "prior",
            "rand", "rank", "ratios", "raze", "read0", "read1", "reciprocal", "reval", "reverse", "rload", "rotate", "rsave", "rtrim",
            "save", "scan", "scov", "sdev", "select", "set", "setenv", "show", "signum", "sin", "sqrt", "ss", "ssr", "string", "sublist", "sum", "sums", "sv", "svar", "system",
            "tables", "tan", "til", "trim", "txf", "type",
            "uj", "ungroup", "union", "update", "upper", "upsert",
            "value", "var", "view", "views", "vs",
            "wavg", "where", "while", "within", "wj", "wj1", "wsum", "ww",
            "xasc", "xbar", "xcol", "xcols", "xdesc", "xexp", "xgroup", "xkey", "xlog", "xprev", "xrank",
        };
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
