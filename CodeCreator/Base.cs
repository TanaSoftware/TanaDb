using ImFast.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImFast.CodeCreator
{
    public class Base
    {
        public static string delimeter = " (char)3 ";
        public static string AllEntitiesKey = "AllEntitiesXXX";
        public static string cacheTimeInMinutes = "10";
        public static string NL()
        {
            return Environment.NewLine;
        }
        public static string TAB()
        {
            return "    ";
        }
        public static string doubleTAB()
        {
            return "        ";
        }

        public static bool IsInsertQuestionMark(string type)
        {
            return IsNumeric(type) && type != "bool" && type != "byte[]";
        }

        public static bool IsNumeric(string type)
        {
            switch (type)
            {
                case "bit":
                case "int":
                case "uint":
                case "ulong":
                case "uLong":
                case "int32":
                case "tinyint":
                case "smallint":
                case "bigint":
                case "float":
                case "decimal":
                case "numeric":
                case "money":
                case "date":
                case "DateTime":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                case "datetimeoffset":
                case "binary":
                case "varbinary":
                case "image":
                case "timestamp":
                case "real":
                case "long":
                case "Long":
                case "bool":
                case "double":
                case "byte[]":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOperator(string type)
        {

            switch (type)
            {
                case "int":
                case "uint":
                case "ulong":
                case "uLong":
                case "int32":
                case "tinyint":
                case "smallint":
                case "bigint":
                case "float":
                case "decimal":
                case "numeric":
                case "money":
                case "date":
                case "datetime":
                case "DateTime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                case "datetimeoffset":
                case "timestamp":
                case "real":
                case "long":
                case "Long":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDateType(string type)
        {
            switch (type)
            {

                case "date":
                case "datetime":
                case "DateTime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                case "datetimeoffset":
                case "timestamp":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetTypeByColumn(string colType)
        {


            switch (colType)
            {
                case "bool":
                case "Boolean":
                    {
                        return "bool";
                        break;
                    }
                case "ulong":
                case "uLong":
                    {
                        return "ulong";
                        break;
                    }
                case "uint":
                case "uInt":
                    {
                        return "uint";
                        break;
                    }
                case "int":
                case "integer":
                case "int32":
                case "smallint":
                case "tinyint":                
                    {
                        return "int";
                        break;
                    }
                case "bigint":
                case "long":
                case "Long":
                    {
                        return "long";
                        break;
                    }
                case "binary":
                case "varbinary":
                case "image":                
                case "byte[]":
                    {
                        return "byte[]";
                        break;
                    }
                case "numeric":
                case "money":
                case "smallmoney":
                case "decimal":
                case "NUMBER":
                    {
                        return "decimal";
                        break;
                    }
                case "char":
                case "nchar":
                case "nvarchar":
                case "varchar":
                case "ntext":
                case "text":
                case "xml":
                case "string":
                case "timestamp":
                    {
                        return "string";
                        break;
                    }
                case "float":
                    {
                        return "float";
                        break;
                    }
                case "real":
                case "double":
                    {
                        return "double";
                        break;
                    }
                case "bit":
                    {
                        return "bool";
                        break;
                    }
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "DateTime":
                    {
                        return "DateTime";
                        break;
                    }
                case "time":
                    {
                        return "TimeSpan";
                        break;
                    }
                case "datetimeoffset":
                    {
                        return "DateTimeOffset";
                        break;
                    }
                case "sql_variant":
                    {
                        return "object";
                        break;
                    }
                case "uniqueidentifier":
                    {
                        return "Guid";
                        break;
                    }
            }

            return "string";
        }

        
        /// <summary>
        /// use for sum operation
        /// </summary>
        /// <param name="colType"></param>
        /// <returns></returns>
        public static string GetTypeByColumnForSumOperation(string colType)
        {
            switch (colType)
            {
                case "float":
                case "Float":
                    {
                        return "float";
                    }
                case "int":
                case "uint":
                    {
                        return "Int64";
                    }
                case "decimal":
                case "Decimal":
                    {
                        return "decimal";
                    }
                case "long":
                case "Long":
                    {
                        return "long";
                    }
                case "ulong":
                case "uLong":
                    {
                        return "ulong";
                    }
                case "real":
                case "double":
                    {
                        return "double";
                    }

            }
            return "Int64";
        }

        public static string GetTypeByColumn(List<Table> foundPK)
        {
            string type = "";
            foreach (Table t in foundPK)
            {
                if (t.IsPrimaryKey > 0)
                    type = t.type;
            }
            return GetCovertType(type);

        }
        public static string GetCovertType(string convert)
        {
            switch (convert)
            {

                case "uint":
                    {
                        return "Convert.ToUInt32";
                    }
                case "ulong":
                case "uLong":
                    {
                        return "Convert.ToUInt64";
                    }
                case "decimal":
                    {
                        return "Convert.ToDecimal";
                    }
                case "byte[]":
                    {
                        return "System.Text.Encoding.ASCII.GetBytes";
                    }
                case "float":
                    {
                        return "Convert.ToSingle";
                    }
                case "double":
                    {
                        return "Convert.ToDouble";
                    }
                case "int":
                case "int32":
                    {
                        return "Convert.ToInt32";
                    }
                case "long":
                    {
                        return "Convert.ToInt64";
                    }
                case "bool":
                    {
                        return "Convert.ToBoolean";
                    }
                case "DateTime":
                    {
                        return "Convert.ToDateTime";
                    }
            }
            return "Convert.ToString";

        }
        public static string GetFuncTypeByColumn(string colType)
        {


            switch (colType)
            {
                case "uint":
                case "uInt":
                    {
                        return "uint";
                        break;
                    }
                case "ulong":
                case "uLong":
                    {
                        return "uint";
                        break;
                    }
                case "int":
                case "int32":
                case "smallint":
                case "tinyint":
                    {
                        return "int";
                        break;
                    }
                case "bigint":
                case "long":
                case "Long":
                    {
                        return "long";
                        break;
                    }
                case "binary":
                case "varbinary":
                case "image":
                case "timestamp":
                case "byte[]":
                    {
                        return "byte[]";
                        break;
                    }
                case "numeric":
                case "money":
                case "smallmoney":
                case "decimal":
                    {
                        return "decimal";
                        break;
                    }
                case "char":
                case "nchar":
                case "nvarchar":
                case "varchar":
                case "ntext":
                case "text":
                case "xml":
                case "string":
                    {
                        return "string";
                        break;
                    }
                case "float":
                    {
                        return "float";
                        break;
                    }
                case "real":
                    {
                        return "double";
                        break;
                    }
                case "bit":
                    {
                        return "bool";
                        break;
                    }
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "DateTime":
                    {
                        return "string";
                        break;
                    }
                case "time":
                    {
                        return "TimeSpan";
                        break;
                    }
                case "datetimeoffset":
                    {
                        return "DateTimeOffset";
                        break;
                    }
                case "sql_variant":
                    {
                        return "object";
                        break;
                    }
                case "uniqueidentifier":
                    {
                        return "Guid";
                        break;
                    }
            }

            return "string";
        }
    }

}
