using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImFast.Dal;
using ImFast.Utils;

namespace ImFast.CodeCreator
{
    public class FunctionCreator : Base
    {
        //private static bool IsRO = false;
        
        //using Entities.table;using System;using System.Collections.Concurrent;using System.Collections.Generic;
        static string classKeyManger = @"
                            
                            public class Key@key@Manager
                            {

                                private static ConcurrentDictionary<string, KeyObj@key@> All@key@Keys = new ConcurrentDictionary<string, KeyObj@key@>();

                                public static IEnumerable<@key@> Get(string key)
                                {
                                    KeyObj@key@ keyObj = null;
                                    if (!All@key@Keys.TryGetValue(key, out keyObj))
                                        return null;

                                    TimeSpan span = DateTime.Now - keyObj.start;
                                    if (span.Minutes > keyObj.time)
                                    {
                                        All@key@Keys.TryRemove(key, out keyObj);
                                        return null;
                                    }
                                   return keyObj.value;

                                }

                                public static bool Set(string key, IEnumerable<@key@> value, int time)
                                {
                                    KeyObj@key@ keyObj = new KeyObj@key@();
                                    keyObj.time = time;
                                    keyObj.value = value;
                                    keyObj.start = DateTime.Now;

                                    KeyObj@key@ BalData = null;
                                    All@key@Keys.TryGetValue(key, out BalData);
                                    if (All@key@Keys.ContainsKey(key))
                                        return All@key@Keys.TryUpdate(key, keyObj, BalData);
        
                                    return  All@key@Keys.TryAdd(key, keyObj);
                                }
                            }

                            public class KeyObj@key@
                            {
                                public DateTime start { get; set; }
                                public int time { get; set; }
                                public IEnumerable<@key@> value { get; set; }
                            }";

        public static string CreateCacheManger(string key)
        {
            string source = classKeyManger.Replace("@key@", key);
            return source;
        }

        public static string CreateSqlFetch(string key, string where, string whereKey, string timeInMinutsToCache,bool IsShowPrefix = true)
        {
            string source = "";
            string prefix = IsShowPrefix ? "IEnumerable<" + key + "> " : "";
            
            source += prefix + "items = GetDataFromDb(" + where + ");" + NL();
            
            return source;
        }

        public static string CreateSql(string key)
        {
            string source = "";
            source += "private static IEnumerable<" + key + "> GetDataFromDb(string where){" + NL();
            
            source += "Dictionary<string, EntityItem> dicEntities = MyEntitiesController.GetdicEntities();" + NL();
            source += "string sConn = dicEntities[\"" + key + "\"].DataBaseConnectionString;" + NL();

            source += "DataBaseRetriever dal = new DataBaseRetriever(sConn);" + NL();
            source += "IEnumerable<" + key + "> data = dal.GetData<" + key + ">(\"Select * from " + key + " where \" + where , dicEntities[\"" + key + "\"].DataBaseTypeName);" + NL();
            source += "return data;" + NL();
            source += "}" + NL();

            return source;
        }
        private static string CheckDate(string type)
        {
            string source = "";
            if (IsDateType(type))
            {
                source += "DateTime d = new DateTime();" + NL();
                source += "string format = DateFormatter.getDateFormat();" + NL();
                source += "if (id.Length <= 10)" + NL();
                source += "{" + NL();
                source += "    id += \" 00-00-00\";" + NL();
                source += "}" + NL();
                source += "DateTime.TryParseExact(id, format, new CultureInfo(\"en-US\"), DateTimeStyles.None, out d);" + NL();
            }
            return source;
        }



        /// <summary>
        /// join 2 tables
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetJoin2(string objName, string key, string type)
        {
            string source = "";
            source += "public static object GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2)" + NL();
            source += "{" + NL();
            source += "try" + NL();
            source += "{" + NL();
            source += "    var items = GetBy" + objName + "(id).AsQueryable();" + NL();
            source += "    return JoinManager.GetJoin2Tables(items, id, entity2, id2,\"" + objName + "\",\"" + key + "\");" + NL();
            source += "}" + NL();
            source += "catch" + NL();
            source += "{" + NL();
            source += "    return \"No valid query\";" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        private static string GetJoin2GT(string objName, string key, string type)
        {
            string source = "";
            source += "public static object GetByJoin" + objName + "(string GT," + GetFuncTypeByColumn(type) + " id, string entity2, string id2)" + NL();
            source += "{" + NL();
            source += "try" + NL();
            source += "{" + NL();
            source += "    var items = GetDataBy" + objName + "OP(GT,id).AsQueryable();" + NL();
            source += "    return JoinManager.GetJoin2Tables(items, id, entity2, id2,\"" + objName + "\",\"" + key + "\");" + NL();
            source += "}" + NL();
            source += "catch" + NL();
            source += "{" + NL();
            source += "    return \"No valid query\";" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        /// <summary>
        /// join 3 tables
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetJoin3(string objName, string key, string type)
        {
            string source = "";
            //join 3 tables
            source += "public static object GetByJoin" + objName + "2(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2, string entity3, string id3)" + NL();
            source += "{" + NL();
            source += "try" + NL();
            source += "{" + NL();
            source += "    var items = GetBy" + objName + "(id).AsQueryable();" + NL();
            source += "    return JoinManager.GetJoin3Tables(items, id, entity2, id2, entity3, id3,\"" + objName + "\",\"" + key + "\");" + NL();
            source += "}" + NL();
            source += "catch" + NL();
            source += "{" + NL();
            source += "    return \"No valid query\";" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        private static string GetPaging(string objName, string key, string type, string order, string OP, string orderParam, string OPName)
        {
            string source = "";
            //paging
            source += "public static IEnumerable<" + key + "> GetPagingBy" + objName + OP + GetFuncTypeByColumn(type) + " id" + orderParam + ", string paging, string size){" + NL();//start func get 

            source += "var items = GetBy" + objName + OPName + "id" + order + ");" + NL();
            source += "int amount = 0;" + NL();
            source += "int page = 0;" + NL();
            source += "if (Common.CheckPaging(paging, size, out page, out amount))" + NL();
            source += "{" + NL();
            source += "items = items.Skip(amount * (page - 1)).Take(amount);" + NL();
            source += "}" + NL();
            source += "return items;" + NL();
            source += "   }" + NL();
            return source;
        }

        public static string CreateWebApiFuncManager(string objName, string key, string type, string DbName, string DbType, bool IsGTLT, bool IsOrderBy)
        {
            string source = "";
            string OP = IsGTLT ? "OP(string GT," : "(";
            string orderParam = IsOrderBy ? ",string order,string asc" : "";
            string order = IsOrderBy ? ",order,asc" : "";

            //paging
            source += GetPaging(objName, key, type, order, OP, orderParam, "(");

            //if (!IsGTLT && !IsOrderBy)
            //{
            //    //join 2 tables
            //    source += GetJoin2(objName, key, type);

            //    //join 3 tables
            //    source += GetJoin3(objName, key, type);

            //}
            //if (IsGTLT && !IsOrderBy)
            //{
            //    //join 2 tables with gt lt
            //    source += GetJoin2GT(objName, key, type);
            //}

            source += GetRegularFunc(key, objName, OP, type, orderParam, DbType, DbName, order, IsGTLT, IsOrderBy, false);

            if (!IsNumeric(type))
                source += GetRegularFunc(key, objName, OP, type, orderParam, DbType, DbName, order, IsGTLT, IsOrderBy, true);

            return source;
        }

        /// <summary>
        /// create function that search data by eqval\contains orderby paging
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objName"></param>
        /// <param name="OP"></param>
        /// <param name="type"></param>
        /// <param name="orderParam"></param>
        /// <param name="DbType"></param>
        /// <param name="DbName"></param>
        /// <param name="order"></param>
        /// <param name="IsGTLT"></param>
        /// <param name="IsOrderBy"></param>
        /// <param name="IsContains"></param>
        /// <returns></returns>
        private static string GetRegularFunc(string key, string objName, string OP, string type, string orderParam, string DbType, string DbName, string order, bool IsGTLT, bool IsOrderBy, bool IsContains)
        {
            string source = "";
            string name = IsContains ? "Contains" : "";

            source += "public static IEnumerable<" + key + "> GetBy" + name + objName + OP + GetFuncTypeByColumn(type) + " id" + orderParam + "){" + NL();//start func get 
           
            source += "   try" + NL();
            source += "   {" + NL();

            if (!IsGTLT)
            {
                if (IsContains)
                    source += CreateLinqContains(key, type, objName, false);
                else
                    source += CreateLinq(key, type, objName, false, null, IsOrderBy);

                if (IsOrderBy)
                {
                    source += CreateOrderByAscDesc(objName);
                }
                source += "return items;" + NL();
            }
            else
            {
                source += GetAllLinqOperators(objName, key, type, IsOrderBy, false);
                if (IsOrderBy)
                {
                    source += CreateOrderByAscDesc(objName);
                    source += "return items;" + NL();
                }
            }

            source += "   }" + NL();
            source += "   catch" + NL();
            source += "   {" + NL();
            source += "   return QueryMe(\"" + objName + "\", id.ToString(),\"" + type + "\",\"" + DbType + "_" + DbName + "\"" + order + ");" + NL();

            source += "   }" + NL();
            //source += "   }" + NL();
            //source += "   else" + NL();
            //source += "   {" + NL();
            //source += "       return QueryMe(\"" + objName + "\", id.ToString(),\"" + type + "\",\"" + DbType + "_" + DbName + "\"" + order + ");" + NL();

            //source += "   }" + NL();

            if (IsGTLT && !IsOrderBy)
                source += "return null;" + NL();

            source += " }" + NL();//end func get
            return source;
        }

        public static string GetKeyValue(string key, string type)
        {
            string source = "";
            source += "public static " + key + " getValue(" + type + " id)" + NL();
            source += "{" + NL();
            source += key + " obj;" + NL();
            source += "if (" + key + "List.TryGetValue(id, out obj))" + NL();
            source += "    {    " + NL();
            source += "        return obj;" + NL();
            source += "    }" + NL();
            source += "    return null;" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateInitForKeVal(string key, EntityItem entity)
        {
            string source = "";
            source += "public static void Init()" + NL();
            source += "{" + NL();
            source += "lock (_sync)" + NL();
            source += "{" + NL();
            source += "    isInMemory = false;" + NL();
            source += "    " + key + "List.Clear();" + NL();

            //choose file from app_data or Shared Folder
            source += "string fileName = \"" + entity.DataSource + "\";" + NL();
            source += "string path = FileManager.GetPath() + \"App_Data\\\\\" + fileName;" + NL();
            source += "string Shared = FileManager.getSharedFolder();" + NL();

            source += "bool isSalve = ServerManager.IsSlave() || ServerManager.IsMaster();" + NL();
            source += "string sPath = isSalve ? Shared + fileName : path;" + NL();

            source += "if (isSalve && (Shared.Length == 0 || !File.Exists(Shared + fileName)))" + NL();
            source += "    sPath = path;" + NL();

            source += "    StreamReader file = new StreamReader(sPath);" + NL();
            source += "    try" + NL();
            source += "    {" + NL();
            source += "        string line = \"\";" + NL();
            string k = entity.lstTables[0].type == "string" ? "\"\"" : "0";
            source += GetFuncTypeByColumn(entity.lstTables[0].type) + " key = " + k + ";" + NL();
            source += "        while ((line = file.ReadLine()) != null)" + NL();
            source += "        {" + NL();
            source += "            " + key + " k = new " + key + "();" + NL();
            source += "            string[] arr = line.Split(FileManager.DelimeterChar);" + NL();
            source += "                key = " + GetCovertType(entity.lstTables[0].type) + "(arr[0]);" + NL();
            source += "                k.key = key;" + NL();
            source += "                k.value = arr[1];" + NL();
            source += "                " + key + "List.TryAdd(key, k);" + NL();
            source += "        }" + NL();
            source += "   }" + NL();
            source += "    catch (Exception e)" + NL();
            source += "    {" + NL();
            source += TAB() + "Task.Factory.StartNew(() => { LogManager.WriteError(\"init: object " + key + " fail: Error - \" + e.Message); });" + NL();
            source += "        isInMemory = false;" + NL();
            source += "" + key + "List.Clear();" + NL();
            source += "    }" + NL();
            source += "    finally" + NL();
            source += "    {" + NL();
            source += "        file.Close();" + NL();
            source += "        GC.Collect();" + NL();
            source += "    }" + NL();
            source += "    isInMemory = true;" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateIsUpdateFuncion(bool IsBackUp)
        {
            string source = "";

            source += "public static void UpdateIsUpdate(bool upd){" + NL();
            if (IsBackUp)
            {
                source += "if (!dbCheck.ContainsKey(\"IsBackUp\"))" + NL();
                source += "{" + NL();
                source += "    dbCheck.TryAdd(\"IsBackUp\", upd);" + NL();
                source += "}" + NL();
                source += "else" + NL();
                source += "    dbCheck[\"IsBackUp\"] = upd;" + NL();
            }
            source += "}" + NL();

            source += "private static bool CheckIsUpdate()" + NL();
            source += "{" + NL();
            source += "if (!dbCheck.ContainsKey(\"IsBackUp\"))" + NL();
            source += "{" + NL();
            source += "    dbCheck.TryAdd(\"IsBackUp\", false);" + NL();
            source += "}" + NL();
            source += "return dbCheck[\"IsBackUp\"];" + NL();
            source += "}" + NL();

            return source;
        }

        public static string CreateSaveToFile(string key, EntityItem entity, string targetFile, bool isDictionary,
            bool IsBackupHistory, bool isCluster=false)
        {
            string source = "";
            string ext = Common.PreojectFileExtension();
            string fName = entity.DataSource.Replace(ext, "");
            string values = isDictionary ? ".Values" : "";

            source += "public static bool SaveToFile()" + NL();
            source += "{" + NL();
            source += "if (!CheckIsUpdate())" + NL();
            source += "return false;" + NL();

            source += "bool IsSuccess = false;" + NL();
            source += "lock (_sync)" + NL();
            source += "    {" + NL();
            source += "string fileName = \"" + fName + "\";" + NL();
            source += "string p = FileManager.GetPath() + \"\\\\App_Data\\\\\";" + NL();
            source += "string path = p + fileName + \"" + ext + "\";" + NL();
            source += "string dest = p + fileName + \"_date_\" + DateTime.Now.ToShortDateString().Replace(\"/\", \"_\") + \"_time_\" + DateTime.Now.ToShortTimeString().Replace(\":\", \"_\") + \"" + ext + "\";" + NL();
            if (IsBackupHistory)
            {
                source += "FileManager.Copy(path, dest, true);" + NL();                
            }
            source += "try" + NL();
            source += "{" + NL();
            source += "Get" + key + "List().ToCsv(path);" + NL();
            source += "IsSuccess = true;" + NL();
            source += "UpdateIsUpdate(false);" + NL();
            source += "}" + NL();
            source += "catch (Exception e)" + NL();
            source += "{" + NL();
            source += "FileManager.Copy(dest,path , true);" + NL();
            source += "IsSuccess = false;" + NL();
            source += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Backup: object " + key + " fail: Error - \" + e.Message); });" + NL();
            source += "}" + NL();
            if (isCluster)            
                source += "if (ClusterMaster.IsMainMaster() && IsSuccess)" + NL();            
            else
                source += "if (ServerManager.IsMaster() && IsSuccess)" + NL();

            source += "{" + NL();
            source += "bool isCopy = FileManager.CopyToSharedFolder(path, fileName + \"" + ext + "\");" + NL();
            if (isCluster)
                source += "if (ClusterMaster.IsMainMaster() && !isCopy)" + NL();
            else
                source += "if (ServerManager.IsMaster() && !isCopy)" + NL();
            source += "    Task.Factory.StartNew(() => { LogManager.WriteError(\"Backup: CopyToSharedFolder: Error - didnt succeded copy backup file to shared folder\"); });" + NL();
            source += " }" + NL();
            source += "}" + NL();
            source += "return IsSuccess;" + NL();
            source += "}" + NL();

            return source;
        }
        public static string CreateSaveToFileOldNotInUse(string key, EntityItem entity, string targetFile)
        {
            string source = "";
            source += "public static bool SaveToFile()" + NL();
            source += "{" + NL();
            source += "bool IsSuccess = false;" + NL();

            source += "try" + NL();
            source += "{" + NL();
            source += "        FileManager.RenameFile(\"" + entity.DataSource + "\", \"" + targetFile + "\");" + NL();
            source += "        FileManager.CreateFile(\"" + entity.DataSource + "\");" + NL();
            source += "        for (int i = 0; i < " + key + "List.Count;i++)" + NL();
            source += "       {" + NL();
            source += "           " + GetFuncTypeByColumn(entity.lstTables[0].type) + " key = " + key + "List.ElementAt(i).Key;" + NL();
            source += "            object value = " + key + "List.ElementAt(i).Value;" + NL();
            source += "            if (value != null)" + NL();
            source += "            {" + NL();
            source += "                string str = \"\";" + NL();
            source += "                str += key.ToString();" + NL();
            source += "                str += (char)3;" + NL();
            source += "                str += JsonDal.Read(value);" + NL();
            source += "            FileManager.AddToFile(\"" + entity.DataSource + "\", str);" + NL();
            source += "            }" + NL();
            source += "        }" + NL();
            source += "IsSuccess = true;" + NL();
            source += "}" + NL();
            source += "catch (Exception e)" + NL();
            source += "{" + NL();
            source += "IsSuccess = false;" + NL();
            source += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Backup: object " + key + " fail: Error - \" + e.Message); });" + NL();
            source += "}" + NL();
            source += "return IsSuccess;" + NL();
            source += "}" + NL();

            return source;
        }
        private static string GetContainsController(string objName, string key, bool IsBalancer, bool IsSharding = false, bool IsCluster = false)
        {
            string source = "";
            //contains
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByContains" + objName + "(string GT,string id){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + GT + \"/\" + id);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetByContains" + objName + "(string GT,string id){" + NL();//start func get 
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + "/\" + GT + \"/\" + id);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByContains" + objName + "(string GT,string id){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "if(GT.ToUpper() == \"CONTAINS\" || GT.ToUpper() == \"LIKE\"){" + NL();
                source += "    var data = " + key + "Manager.GetByContains" + objName + "(id);" + NL();
                source += "    return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"SOUND\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataBySound" + objName+ "(id);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"SW\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataByStartWith" + objName + "(id);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"EW\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataByEndWith" + objName + "(id);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else" + NL();
                source += "return Request.CreateResponse(string.Empty);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get

            //contains order by
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByContains" + objName + "(string GT,string id, string order, string Asc){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetByContains" + objName + "(string GT,string id, string order, string Asc){" + NL();//start func get 
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByContains" + objName + "(string GT,string id, string order, string Asc){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "if(GT.ToUpper() == \"CONTAINS\" || GT.ToUpper() == \"LIKE\"){" + NL();
                source += "    var data = " + key + "Manager.GetByContains" + objName + "(id,order,Asc);" + NL();
                source += "    return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"SOUND\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataBySound" + objName + "(id,order,Asc);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"SW\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataByStartWith" + objName + "(id,order,Asc);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"EW\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataByEndWith" + objName + "(id,order,Asc);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else" + NL();
                source += "return Request.CreateResponse(string.Empty);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get

            //contains order by and paging
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByContains" + objName + "(string GT,string id, string order, string Asc, string paging, string size){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetByContains" + objName + "(string GT,string id, string order, string Asc, string paging, string size){" + NL();//start func get 
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByContains" + objName + "(string GT,string id, string order, string Asc, string paging, string size){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "if(GT.ToUpper() == \"CONTAINS\" || GT.ToUpper() == \"LIKE\"){" + NL();
                source += "    var data = " + key + "Manager.GetByContains" + objName + "(id,order,Asc, paging, size);" + NL();
                source += "    return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"SOUND\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataBySound" + objName + "(id,order,Asc, paging, size);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"SW\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataByStartWith" + objName + "(id,order,Asc, paging, size);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else if (GT.ToUpper() == \"EW\")" + NL();
                source += "{" + NL();
                source += "var data = " + key + "Manager.GetDataByEndWith" + objName + "(id,order,Asc, paging, size);" + NL();
                source += "return CheckCols(data);" + NL();
                source += "}" + NL();
                source += "else" + NL();
                source += "return Request.CreateResponse(string.Empty);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get

            return source;
        }

        private static string GetDateCheckString(string type)
        {
            string source = "";
            if (IsDateType(type))
            {
                source += "if (!Common.CheckDate(id))" + NL();
                source += "{" + NL();
                source += "    return Request.CreateResponse(HttpStatusCode.BadRequest, \"No Valid date\");" + NL();
                source += "}" + NL();
            }
            return source;
        }

        /// <summary>
        /// create controler function
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="DbName"></param>
        /// <param name="DbType"></param>
        /// <param name="IsGTLT"></param>
        /// <returns></returns>
        public static string CreateWebApiFunc(string objName, string key, string type, string DbName, string DbType, bool IsGTLT, bool IsCluster = false)
        {
            string source = "";
            string OP = IsGTLT ? "OP(string GT," : "(";
            string GT = IsGTLT ? "OP(GT,id)" : "(id)";
            string GTOrder = IsGTLT ? "OP(GT,id,order,Asc" : "(id,order,Asc";

            //controler

            //regular
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id){" + NL();//start func get 
            //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
            //source += "{" + NL();
            source += GetDateCheckString(type);
            source += "    var data = " + key + "Manager.GetBy" + objName + GT + ";" + NL();
            source += "    return CheckCols(data);" + NL();
            //source += "});" + NL();
            source += " }" + NL();//end func get

            if (!IsNumeric(type))
            {
                source += GetContainsController(objName, key, false, IsCluster);
            }

            //gt lt
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order,string Asc){" + NL();//start func get 
            //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
            //source += "{" + NL();
            source += GetDateCheckString(type);
            source += "    var data = " + key + "Manager.GetBy" + objName + GTOrder + ");" + NL();
            source += "    return CheckCols(data);" + NL();
            //source += "});" + NL();
            source += " }" + NL();//end func get

            //gt lt with sum/max/avg ...
            if (IsGTLT)
            {
                source += GetSeveralFunctionsSource(objName, key, type, DbName, DbType, IsGTLT);
            }

            //paging
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order,string Asc, string paging, string size){" + NL();//start func get 
            //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
            //source += "{" + NL();
            source += GetDateCheckString(type);
            source += "    var data = " + key + "Manager.GetPagingBy" + objName + GTOrder + ", paging, size);" + NL();
            source += "    return CheckCols(data);" + NL();
            //source += "});" + NL();
            source += " }" + NL();//end func get

            //if (!IsGTLT)
            //{
            //    source += GetJoinController(objName, key, type);
            //}
            return source;
        }

        /// <summary>
        /// create Function GetByFunc
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetFunction_GetByFunc(string key, EntityItem entity)
        {

            string source = "";
            for (int i = 0; i < entity.lstTables.Count; i++)
            {
                string name = entity.lstTables[i].Name;
                bool isDate = entity.lstTables[i].type == "DateTime";
                bool isString = entity.lstTables[i].type == "string";
                bool isUlong = entity.lstTables[i].type == "ulong";
                bool isBool = entity.lstTables[i].type == "bool";
                bool isByte = entity.lstTables[i].type == "byte[]";

                if (isBool || isByte)
                    continue;

                source += "private string GetByFunc" + name + "(string MAX, IEnumerable<" + key + "> data)" + NL();
                source += "{" + NL();
                source += "var response = \"\";" + NL();
                source += "string max = MAX.ToUpper();" + NL();
                source += "switch (max)" + NL();
                source += "{" + NL();
                source += "case \"MAX\":" + NL();
                source += "{" + NL();
                source += "response = data.Max(x => x." + name + ").ToString();" + NL();
                if (isDate)
                {
                    source += "response = Common.GetDate(response);" + NL();
                }
                source += "break;" + NL();
                source += "}" + NL();
                source += "case \"COUNT\":" + NL();
                source += "{" + NL();
                source += "response = data.Count().ToString();" + NL();
                source += "break;" + NL();
                source += "}" + NL();
                if (!isDate && !isString)
                {
                    source += "case \"SUM\":" + NL();
                    source += "{" + NL();
                    string sType = GetTypeByColumnForSumOperation(entity.lstTables[i].type);
                    source += sType + "? sum = 0;" + NL();

                    source += "foreach (var d in data)" + NL();
                    source += "{" + NL();
                    source += "    if (d." + name + " != null)" + NL();
                    source += "        sum += d." + name + ";" + NL();
                    source += "}" + NL();
                    source += "response = sum.ToString();" + NL();
                    //source += "response = data.Sum(x => x." + name + ").ToString();" + NL();
                    source += "break;" + NL();
                    source += "}" + NL();
                }
                source += "case \"MIN\":" + NL();
                source += "{" + NL();
                source += "response = data.Min(x => x." + name + ").ToString();" + NL();
                if (isDate)
                {
                    source += "response = Common.GetDate(response);" + NL();
                }
                source += "break;" + NL();
                source += "}" + NL();
                if (!isDate && !isString && !isUlong)
                {
                    source += "case \"AVG\":" + NL();
                    source += "{" + NL();
                    source += "response = data.Average(x => x." + name + ").ToString();" + NL();
                    source += "break;" + NL();
                    source += "}" + NL();
                }
                if (isDate)
                {
                    source += "case \"AVG\":" + NL();
                    source += "{" + NL();
                    source += "double averageTicks = data.Average(x => x."+name+".Value.Ticks);" + NL();
                    source += "long vOut = Convert.ToInt64(averageTicks);" + NL();
                    source += "var averageDate = new DateTime(vOut);" + NL();
                    source += "response = Common.GetDate(averageDate.ToShortDateString()) + \"T\" + averageDate.ToShortTimeString();" + NL();
                    source += "break;" + NL();
                    source += "}" + NL();
                }
                source += "}" + NL();
                source += "return response;" + NL();
                source += "}" + NL();
            }
            return source;
        }
        /// <summary>
        /// //gt lt with sum/max/avg ...
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="DbName"></param>
        /// <param name="DbType"></param>
        /// <param name="IsGTLT"></param>
        /// <returns></returns>
        private static string GetSeveralFunctionsSource(string objName, string key, string type, string DbName, string DbType, bool IsGTLT, bool IsBalancer = false, bool IsSharding = false, bool IsCluster = false)
        {
            string GT = IsGTLT ? "OP(GT,id)" : "(id)";
            string OPMax = IsGTLT ? "OP(string MAX,string GT," : "(";
            string source = "";
            bool isDate = type == "DateTime";


            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByidMax" + objName + OPMax + GetFuncTypeByColumn(type) + " id){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetByidMax" + objName + OPMax + GetFuncTypeByColumn(type) + " id){" + NL();//start func get 
                source += "HttpResponseMessage resp = BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + "/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();
                source += "if (resp.IsSuccessStatusCode)" + NL();
                source += "{" + NL();
                source += "    var myobject = resp.Content.ReadAsAsync<List<int>>();" + NL();
                source += "    HttpResponseMessage r = new HttpResponseMessage(HttpStatusCode.OK);" + NL();
                source += "    int? x = null;" + NL();
                source += "    if (MAX.ToUpper() == \"MAX\")" + NL();
                source += "    {" + NL();
                source += "        x = myobject.Result.Max();" + NL();
                source += "    }" + NL();
                source += "    else if (MAX.ToUpper() == \"MIN\")" + NL();
                source += "    {" + NL();
                source += "        x = myobject.Result.Min();" + NL();
                source += "    }" + NL();
                source += "    else if (MAX.ToUpper() == \"SUM\")" + NL();
                source += "    {" + NL();
                source += "        x = myobject.Result.Sum();" + NL();
                source += "    }" + NL();
                source += "    else if (MAX.ToUpper() == \"COUNT\")" + NL();
                source += "    {" + NL();
                source += "        x = myobject.Result.Sum();" + NL();
                source += "    }" + NL();
                source += "    else if (MAX.ToUpper() == \"AVG\")" + NL();
                source += "    {" + NL();
                source += "        double d = myobject.Result.Average();" + NL();
                source += "        r.Content = new StringContent(d.ToString());" + NL();
                source += "        return r;" + NL();
                source += "    }" + NL();
                source += "    r.Content = new StringContent(x.ToString());" + NL();
                source += "    return r;" + NL();
                source += "}" + NL();
                source += "return resp;" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByidMax" + objName + OPMax + GetFuncTypeByColumn(type) + " id){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "    IEnumerable<" + key + "> data = " + key + "Manager.GetBy" + objName + GT + ";" + NL();
                source += "    var response = GetByFunc" + objName + "(MAX, data);" + NL();
                source += "    return Request.CreateResponse(response);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get
            return source;
        }

        public static string CreateWebApiFuncWebFarm(string objName, string key, string type, string DbName, string DbType, bool IsGTLT)
        {
            string source = "";
            string OP = IsGTLT ? "OP(string GT," : "(";
            string GT = IsGTLT ? "OP(GT,id)" : "(id)";
            string GTOrder = IsGTLT ? "OP(GT,id,order,Asc" : "(id,order,Asc";

            //controler

            //regular
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id){" + NL();//start func get 

            if (!IsGTLT)
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id);" + NL();
            else
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id);" + NL();

            source += " }" + NL();//end func get

            //gt lt
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order,string Asc){" + NL();//start func get 

            if (!IsGTLT)
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
            else
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc);" + NL();

            source += " }" + NL();//end func get

            //paging
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order,string Asc, string paging, string size){" + NL();//start func get 
            if (!IsGTLT)
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
            else
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();

            source += " }" + NL();//end func get

            if (!IsGTLT)
            {
                source += GetJoinControllerWebFarm(objName, key, type);
            }


            if (!IsNumeric(type))
            {
                //contains
                source += "[ActionName(\"" + objName + "\")]" + NL();
                source += "public async Task<HttpResponseMessage> GetByContains" + objName + "(string GT,string id){" + NL();//start func get
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id);" + NL();
                source += " }" + NL();//end func get
            }

            return source;
        }

        private static string GetJoinGTController(string objName, string key, string type, bool IsBalancer = false, bool IsCluster = false)
        {
            string source = "";
            //join 2 tables
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByJoin" + objName + "(string GT," + GetFuncTypeByColumn(type) + " id, string entity2, string id2){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(GT,\"" + key + "/" + objName + "/\" + id + \"/\" + entity2 + \"/\" + id2);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(GT,\"" + key + "/" + objName + "/\" + id + \"/\" + entity2 + \"/\" + id2);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByJoin" + objName + "(string GT," + GetFuncTypeByColumn(type) + " id, string entity2, string id2){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += GetDateCheckString(type);
                source += "    var data = " + key + "Manager.GetByJoin" + objName + "(GT,id,entity2, id2);" + NL();
                source += "    return Request.CreateResponse(data);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get
            return source;
        }
        private static string GetJoinController(string objName, string key, string type, bool IsBalancer = false, bool IsCluster = false)
        {
            string source = "";
            //join 2 tables
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + entity2 + \"/\" + id2);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + entity2 + \"/\" + id2);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += GetDateCheckString(type);
                source += "    var data = " + key + "Manager.GetByJoin" + objName + "(id,entity2, id2);" + NL();
                source += "    return Request.CreateResponse(data);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get

            //join 3 tables
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2, string entity3, string id3){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + entity2 + \"/\" + id2+ \"/\" + entity3+ \"/\" + id3);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + id + \"/\" + entity2 + \"/\" + id2+ \"/\" + entity3+ \"/\" + id3);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2, string entity3, string id3){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += GetDateCheckString(type);
                source += "    var data = " + key + "Manager.GetByJoin" + objName + "2(id,entity2, id2, entity3, id3);" + NL();
                source += "    return Request.CreateResponse(data);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get

            return source;
        }

        private static string GetJoinControllerWebFarm(string objName, string key, string type)
        {
            string source = "";
            //join 2 tables
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2){" + NL();//start func get 

            source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id + \"/join/\" + entity2 + \"/\" + id2);" + NL();

            source += " }" + NL();//end func get

            //join 3 tables
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetByJoin" + objName + "(" + GetFuncTypeByColumn(type) + " id, string entity2, string id2, string entity3, string id3){" + NL();//start func get 

            source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id + \"/join/\" + entity2 + \"/\" + id2+ \"/\" + entity3+ \"/\" + id3);" + NL();

            source += " }" + NL();//end func get

            return source;
        }
        private static string CreateOrderByAscDesc(string column)
        {
            string source = "";
            source += "int amount = 0;" + NL();
            source += "int page = 0;" + NL();
            source += "if (Common.CheckPaging(order, asc, out page, out amount))" + NL();
            source += "{" + NL();
            source += "    items = items.Skip(amount * (page - 1)).Take(amount);" + NL();
            source += "}" + NL();
            source += "else" + NL();
            source += "{" + NL();
            source += "int orderType = Common.GetOrderType(asc);" + NL();
            source += "if (orderType == 1)" + NL();
            source += "    items = items.OrderBy(x => x." + column + ");" + NL();

            source += "if (orderType == 2)" + NL();
            source += "    items = items.OrderByDescending(x => x." + column + ");" + NL();
            source += "}" + NL();
            return source;
        }

        private static string CreateOrderByAscDescAndPaging(string column)
        {
            string source = "";
            source += "int amount = 0;" + NL();
            source += "int page = 0;" + NL();
            source += "if (Common.CheckPaging(paging, size, out page, out amount))" + NL();
            source += "{" + NL();
            source += "    items = items.Skip(amount * (page - 1)).Take(amount);" + NL();
            source += "}" + NL();

            source += "int orderType = Common.GetOrderType(asc);" + NL();
            source += "if (orderType == 1)" + NL();
            source += "    items = items.OrderBy(x => x." + column + ");" + NL();

            source += "if (orderType == 2)" + NL();
            source += "    items = items.OrderByDescending(x => x." + column + ");" + NL();

            return source;
        }

        private static string CreateDictionarySearch(string key, List<Table> foundPK)
        {
            string source = "";
            //string pks = "";
            //foreach (Table t in foundPK)
            //{
            //    if (t.type == "int" && foundPK.Count == 1)
            //    {
            //        pks = "";
            //    }
            //    else
            //        pks = ".ToString()";
            //}
            source += key + " it;" + NL();
            source += "bool found = " + key + "List.TryGetValue(id, out it);" + NL();
            source += "if (found)" + NL();
            source += "{" + NL();
            source += "    List<" + key + "> lstNew = new List<" + key + ">();" + NL();
            source += "    lstNew.Add(it);" + NL();
            source += "    return lstNew;" + NL();
            source += "}" + NL();
            source += "return null;" + NL();

            return source;
        }
        /// <summary>
        /// create function that search data by eqval\contains orderby paging
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objName"></param>
        /// <param name="OP"></param>
        /// <param name="type"></param>
        /// <param name="orderParam"></param>
        /// <param name="DbType"></param>
        /// <param name="DbName"></param>
        /// <param name="order"></param>
        /// <param name="IsGTLT"></param>
        /// <param name="IsOrderBy"></param>
        /// <param name="IsContains"></param>
        /// <returns></returns>
        private static string GetRegularFuncFile(string key, string objName, string OP, string type, string orderParam, string DbType, string DbName, string order, bool IsGTLT, bool IsOrderBy, bool IsContains, bool IsPK, List<Table> foundPK)
        {
            string source = "";
            string name = IsContains ? "Contains" : "";
            string parameterType = GetFuncTypeByColumn(type);
            string FinalType = foundPK != null && foundPK.Count > 1 && !IsContains && !IsGTLT && !IsOrderBy ? "string" : parameterType;

            source += "public static IEnumerable<" + key + "> GetDataBy" + name + objName + OP + parameterType + " id" + orderParam + "){" + NL();

            if (parameterType == "string")
            {
                source += "if (!QueryManager.CheckForbidenChars(id))" + NL();
                source += "{" + NL();
                source += "    return null;" + NL();
                source += "}" + NL();
            }
            if (!IsGTLT)
            {
                if (IsContains)
                {
                    source += CreateLinqContains(key, type, objName, IsPK);                    
                }
                else
                {
                    if (foundPK != null && foundPK.Count == 1)
                    {
                        source += CreateLinq(key, type, objName, true, foundPK, IsOrderBy);

                    }
                    else
                    {
                        if (foundPK != null && foundPK.Count > 0)
                            source += CreateLinq(key, type, objName, true, null, IsOrderBy);
                        else
                            source += CreateLinq(key, type, objName, false, null, IsOrderBy);
                    }
                }
                bool isPKExiststoObjName = GetPKCount(objName, foundPK) > 0;


                if (!IsOrderBy && source.IndexOf("found") < 0)
                    source += "return items;" + NL();

            }
            else
            {
                source += GetAllLinqOperators(objName, key, type, IsOrderBy, IsPK);
                if (!IsOrderBy)
                    source += "return null;" + NL();
            }

            if (IsOrderBy)
            {
                source += CreateOrderByAscDesc(objName);
                source += "return items;" + NL();
            }

            source += "}" + NL();

            return source;
        }

        private static string GetRegularFuncFileExtraForStrings(string key, string objName, string OP, string type, string orderParam, string DbType, string DbName, string order, bool IsOrderBy, string name, bool IsPK, List<Table> foundPK)
        {
            string source = "";
                        
            source += "public static IEnumerable<" + key + "> GetDataBy" + name + objName + OP + GetFuncTypeByColumn(type) + " id" + orderParam + "){" + NL();
            
            source += "if (!QueryManager.CheckForbidenChars(id))" + NL();
            source += "{" + NL();
            source += "    return null;" + NL();
            source += "}" + NL();
            
            if (name == "Sound")
                source += CreateLinqSound(key, type, objName, IsPK);
            else if (name == "StartWith")
                source += CreateLinqStartWith(key, type, objName, IsPK);
            else if (name == "EndWith")
                source += CreateLinqEndsWith(key, type, objName, IsPK);

            if(IsOrderBy)      
                source += CreateOrderByAscDesc(objName);
                            
            source += "return items;" + NL();
            source += "}" + NL();

            return source;
        }

        private static string GetRegularFuncFileContains(string key, string objName, string OP, string type, string orderParam, string DbType, string DbName, string order, bool IsGTLT, bool IsOrderBy, bool IsContains, bool IsPK)
        {
            string source = "";
            string name = "Contains";

            source += "public static IEnumerable<" + key + "> GetDataBy" + name + objName + OP + GetFuncTypeByColumn(type) + " id" + orderParam + "){" + NL();
            
            source += "if (!QueryManager.CheckForbidenChars(id))" + NL();
            source += "{" + NL();
            source += "    return null;" + NL();
            source += "}" + NL();
            
            source += CreateLinqContains(key, type, objName, IsPK);
            source += CreateOrderByAscDescAndPaging(objName);
            source += "return items;" + NL();

            source += "}" + NL();
            return source;
        }

        private static string GetRegularFuncFileExtraForString(string key, string objName, string OP, string type, string orderParam, string DbType, string DbName, string order, bool IsGTLT, bool IsOrderBy, bool IsContains, bool IsPK,string name)
        {
            string source = "";            

            source += "public static IEnumerable<" + key + "> GetDataBy" + name + objName + OP + GetFuncTypeByColumn(type) + " id" + orderParam + "){" + NL();
            
            source += "if (!QueryManager.CheckForbidenChars(id))" + NL();
            source += "{" + NL();
            source += "    return null;" + NL();
            source += "}" + NL();
            
            source += CreateLinqSound(key, type, objName, IsPK);            
            source += CreateOrderByAscDescAndPaging(objName);
            source += "return items;" + NL();

            source += "}" + NL();
            return source;
        }
        public static string CreateWebApiFuncForFilesManager(string objName, string key, string type, string DbName, string DbType, bool IsGTLT, bool IsOrderBy, bool IsPK, List<Table> foundPK)
        {
            string source = "";
            string OP = IsGTLT ? "OP(string GT," : "(";
            string OPName = IsGTLT ? "OP(GT," : "(";
            string orderParam = IsOrderBy ? ",string order,string asc" : "";
            string order = IsOrderBy ? ",order,asc" : "";
            string orderPagingParam = ",string order,string asc, string paging, string size";
            string orderPaging = ",order,asc,paging,size";

            //paging
            
            source += GetPaging(objName, key, type, order, OP, orderParam, OPName);
            //end paging 

            //if (!IsGTLT && !IsOrderBy)
            //{
            //    //join 2 tables                
            //    source += GetJoin2(objName, key, type);

            //    //join 3 tables
            //    source += GetJoin3(objName, key, type);
            //}
            //if (IsGTLT && !IsOrderBy)
            //{
            //    //join 2 tables with gt lt
            //    source += GetJoin2GT(objName, key, type);
            //}
            
            
            source += GetRegularFuncFile(key, objName, OP, type, orderParam, DbType, DbName, order, IsGTLT, IsOrderBy, false, IsPK, foundPK);
            source += CreateRegularFile2(key, objName, OP, type, orderParam, OPName, order, IsGTLT, false);

            if (!IsNumeric(type))
            {
                source += GetRegularFuncFile(key, objName, OP, type, orderParam, DbType, DbName, order, IsGTLT, IsOrderBy, true, IsPK, foundPK);
                source += CreateRegularFile2(key, objName, OP, type, orderParam, OPName, order, IsGTLT, true);

                source += GetRegularFuncFileExtraForStrings(key, objName, OP, type, orderParam, DbType, DbName, order, IsOrderBy, "Sound", IsPK, foundPK);
                source += GetRegularFuncFileExtraForStrings(key, objName, OP, type, orderParam, DbType, DbName, order, IsOrderBy, "StartWith", IsPK, foundPK);
                source += GetRegularFuncFileExtraForStrings(key, objName, OP, type, orderParam, DbType, DbName, order, IsOrderBy, "EndWith", IsPK, foundPK);

                //create order by and paging
                if (IsOrderBy)
                {
                    source += GetRegularFuncFileExtraForString(key, objName, OP, type, orderPagingParam, DbType, DbName, orderPaging, IsGTLT, IsOrderBy, true, IsPK, "Sound");
                    source += GetRegularFuncFileExtraForString(key, objName, OP, type, orderPagingParam, DbType, DbName, orderPaging, IsGTLT, IsOrderBy, true, IsPK, "StartWith");
                    source += GetRegularFuncFileExtraForString(key, objName, OP, type, orderPagingParam, DbType, DbName, orderPaging, IsGTLT, IsOrderBy, true, IsPK, "EndWith");
                    source += GetRegularFuncFileContains(key, objName, OP, type, orderPagingParam, DbType, DbName, orderPaging, IsGTLT, IsOrderBy, true, IsPK);
                    source += CreateRegularFile2(key, objName, OP, type, orderPagingParam, OPName, orderPaging, IsGTLT, true);
                }
            }

            return source;
        }


        private static string CreateRegularFile2(string key, string objName, string OP, string type, string orderParam, string OPName, string order, bool IsGTLT, bool IsContains)
        {
            string source = "";
            string name = IsContains ? "Contains" : "";

            string funcName = "<" + key + "> GetBy" + name + objName + OP + GetFuncTypeByColumn(type) + " id" + orderParam + ")";//start func get 
            source += "public static IEnumerable" + funcName + "{" + NL();

            
            source += "   try" + NL();
            source += "   {" + NL();

            source += "     return GetDataBy" + name + objName + OPName + "id" + order + ");";

            source += "   }" + NL();

            source += "catch (Exception e)" + NL();
            source += "    {" + NL();
            source += "        Task.Factory.StartNew(() => { LogManager.WriteError(\"funcName : " + funcName + ", Error - \" + e.Message); });" + NL();
            source += "        throw new Exception(\"error\");" + NL();
            source += "    }" + NL();
                       

            source += " }" + NL();//end func get

            return source;
        }


        public static string GetKeyValueRetrieve(string key, string type, bool IsBalancer, bool IsSharding = false, bool IsCluster = false)
        {
            string source = "";
            source += "[ActionName(\"getValue\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> getValue(" + type + " id){" + NL();
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/getvalue/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/getvalue/\" + id);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage getValue(" + type + " id){" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/getvalue/\" + id);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage getValue(" + type + " id){" + NL();
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "    var data = " + key + "Manager.getValue(id);" + NL();
                source += "    return Request.CreateResponse(data);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get
            return source;
        }

        public static string CreateWebApiFuncForFiles(string objName, string key, string type, string DbName, string DbType, 
            bool IsGTLT, List<Table> foundPK = null, int iterator = 1, bool IsBalancer = false, bool IsSharding = false,
            bool IsCluster = false)
        {
            string source = "";
            string OP = IsGTLT ? "OP(string GT," : "(";
            string OPName = IsGTLT ? "OP(GT," : "(";
            string OPBalancer = (IsBalancer || IsSharding) && IsGTLT ? "/\" + GT + \"" : "";

            if (!IsNumeric(type))
            {
                source += GetContainsController(objName, key, IsBalancer, IsSharding, IsCluster);
            }

            //gt lt with sum/max/avg ...
            if (IsGTLT)
            {
                source += GetSeveralFunctionsSource(objName, key, type, DbName, DbType, IsGTLT, IsBalancer, IsSharding,IsCluster);
            }

            //add key search for dictionary that has more than one key
            //if (iterator == 0 && foundPK != null && !IsGTLT && foundPK.Count > 1)
            //{
            //    source += "[ActionName(\"key\")]" + NL();

            //    if (IsBalancer)
            //    {
            //        source += "public async Task<HttpResponseMessage> GetByKey" + objName + "(string id){" + NL();
            //        if(IsCluster)
            //            source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + "/\" + id);" + NL();
            //        else
            //            source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + "/\" + id);" + NL();
            //    }
            //    else if (IsSharding)
            //    {
            //        source += "public HttpResponseMessage GetByKey" + objName + "(string id){" + NL();
            //        source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + "/\" + id);" + NL();
            //    }
            //    else
            //    {
            //        source += "public HttpResponseMessage GetByKey" + objName + "(string id){" + NL();
            //        //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
            //        //source += "{" + NL();
            //        source += "    var data = " + key + "Manager.GetDataByKey" + objName + "(id);" + NL();
            //        source += "    return CheckCols(data);" + NL();
            //        //source += "});" + NL();
            //    }
            //    source += " }" + NL();//end func get
            //}

            source += "[ActionName(\"" + objName + "\")]" + NL();

            //start func getby...

            bool isNeedClose = false;
            if (IsBalancer)
            {
                isNeedClose = true;
                if (IsGTLT)
                    source += "public async Task<HttpResponseMessage> GetBy" + objName + "(string GT,string id){" + NL();
                else
                    source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id){" + NL();
                if (IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + OPBalancer + "/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + OPBalancer + "/\" + id);" + NL();
            }
            else if (IsSharding)
            {
                isNeedClose = true;
                if (IsGTLT)
                    source += "public HttpResponseMessage GetBy" + objName + "(string GT,string id){" + NL();
                else
                    source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id){" + NL();

                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + OPBalancer + "/\" + id);" + NL();
            }
            else
            {
                isNeedClose = true;
                if (IsGTLT)
                {
                    source += "public HttpResponseMessage GetBy" + objName + "(string GT,string id){" + NL();//start func get 
                    source += "IEnumerable<" + key + "> data = null;" + NL();
                    source += "try" + NL();
                    source += "{" + NL();
                    source += "    if (id.IndexOf(\" \") > 0)" + NL();
                    source += "    {" + NL();
                    source += "        data = " + key + "Manager.GetQueryX(id);" + NL();
                    source += "        var response = GetByFunc" + objName + "(GT, data);" + NL();
                    source += "        return Request.CreateResponse(response);" + NL();
                    source += "    }" + NL();
                    source += "    else" + NL();
                    source += "    {" + NL();
                    if (IsDateType(type))
                    {
                        source += "        data = " + key + "Manager.GetBy" + objName + "OP(GT, id);" + NL();
                    }
                    else
                    {
                        source += "    " + GetFuncTypeByColumn(type) + " idX = " + GetCovertType(type) + "(id);" + NL();
                        source += "        data = " + key + "Manager.GetBy" + objName + "OP(GT, idX);" + NL();
                    }
                    source += "    }" + NL();
                    source += "}" + NL();
                    source += "catch (Exception ex)" + NL();
                    source += "{" + NL();
                    source += "   return Request.CreateResponse(ex.Message);" + NL();
                    source += "}" + NL();
                    source += "return CheckCols(data);" + NL();
                }
                else
                {

                    source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id){" + NL();
                    //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                    //source += "{" + NL();
                    source += "    var data = " + key + "Manager.GetBy" + objName + OPName + " id);" + NL();
                    source += "    return CheckCols(data);" + NL();                    
                }
                
            }
            if(isNeedClose)
                source += "}" +  NL();

            //start func getby... with orderby
            source += "[ActionName(\"" + objName + "\")]" + NL();


            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + OPBalancer + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + OPBalancer + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc){" + NL();//start func get 
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + OPBalancer + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "    var data = " + key + "Manager.GetBy" + objName + OPName + " id, order, Asc);" + NL();
                source += "    return CheckCols(data);" + NL();
                //source += "});" + NL();
            }

            source += " }" + NL();//end func get

            //start func getby... with orderby and paging
            source += "[ActionName(\"" + objName + "\")]" + NL();

            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc, string paging, string size){" + NL();//start func get 
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/" + objName + OPBalancer + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/" + objName + OPBalancer + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc, string paging, string size){" + NL();//start func get 
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/" + objName + OPBalancer + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc, string paging, string size){" + NL();//start func get 
                //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "{" + NL();
                source += "    var data = " + key + "Manager.GetPagingBy" + objName + OPName + " id, order, Asc, paging, size);" + NL();
                source += "    return CheckCols(data);" + NL();
                //source += "});" + NL();
            }
            source += " }" + NL();//end func get

            //if (!IsGTLT && !IsSharding)
            //{
            //    source += GetJoinController(objName, key, type, IsBalancer,IsCluster);
            //}

            //if (IsGTLT && !IsSharding)
            //{
            //    source += GetJoinGTController(objName, key, type, IsBalancer,IsCluster);
            //}

            return source;
        }


        public static string CreateWebApiFuncForFilesWebFarm(string objName, string key, string type, string DbName, string DbType, bool IsGTLT)
        {
            string source = "";
            string OP = IsGTLT ? "OP(string GT," : "(";
            string OPName = IsGTLT ? "OP(GT," : "(";

            source += "[ActionName(\"" + objName + "\")]" + NL();

            //start func getby...
            source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id){" + NL();
            if (!IsGTLT)
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id);" + NL();
            else
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id);" + NL();
            source += " }" + NL();//end func get

            //start func getby... with orderby
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc){" + NL();//start func get 

            if (!IsGTLT)
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
            else
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc);" + NL();

            source += " }" + NL();//end func get

            //start func getby... with orderby and paging
            source += "[ActionName(\"" + objName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetBy" + objName + OP + GetFuncTypeByColumn(type) + " id, string order, string Asc, string paging, string size){" + NL();//start func get 
            if (!IsGTLT)
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();
            else
                source += "return await UrlHandler.RetrieveData(GetServerId(), \"" + key + "/" + objName + "/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc+ \"/\" + paging+ \"/\" + size);" + NL();


            source += " }" + NL();//end func get

            if (!IsGTLT)
            {
                source += GetJoinControllerWebFarm(objName, key, type);
            }

            return source;
        }

        private static int GetPKCount(string objName, List<Table> foundPK)
        {
            var items = from p in foundPK
                        where p.Name.Equals(objName)
                        select p;

            return items.Count();
        }

        private static string CreateLinq(string key, string type, string objName, bool IsPK, List<Table> foundPK, bool IsOrderBy)
        {
            string source = "";
                        
            bool IsString = type == "string";
            
            string dateStr = CheckDate(type);
            source += dateStr;
            string param = dateStr.Length > 0 ? "d" : "id";

            string w="";

            if (IsString)
            {
                w = "\"" + objName + "='\"+"  + param + "+\"'\"";
            }
            else
                w = "\"" + objName + "=\"+" + param + ".ToString()";

            source += CreateSqlFetch(key, w, w, cacheTimeInMinutes);            
          
            return source;
        }

        private static string CreateLinqContains(string key, string type, string objName, bool IsPK)
        {
            string source = "";
            
            string w = "\"" + objName + " like '%\" + id + \"%'\"";
            source += CreateSqlFetch(key, w,w, cacheTimeInMinutes);
            
            return source;
        }

        private static string CreateLinqSound(string key, string type, string objName, bool IsPK)
        {
            //string objNotNull = !IsPK && !IsRO ? "p." + objName + " != null && " : "";
            string source = "";
            string w = "\"UPPER(" + objName + ") like '%\" + id.ToUpper() + \"%'\"";
            source += CreateSqlFetch(key, w, w, cacheTimeInMinutes);
            return source;
        }

        private static string CreateLinqStartWith(string key, string type, string objName, bool IsPK)
        {
            //string objNotNull = !IsPK && !IsRO ? "p." + objName + " != null && " : "";
            string source = "";
            string w = "\"" + objName + " like '\" + id + \"%'\"";
            source += CreateSqlFetch(key, w, w, cacheTimeInMinutes);
            return source;
        }
        private static string CreateLinqEndsWith(string key, string type, string objName, bool IsPK)
        {
            //string objNotNull = !IsPK && !IsRO ? "p." + objName + " != null && " : "";
            string source = "";
            string w = "\"" + objName + " like '%\" + id + \"'\"";
            source += CreateSqlFetch(key, w, w, cacheTimeInMinutes);
            return source;
        }
        private static string GetAllLinqOperators(string objName, string key, string type, bool IsOrderBy, bool IsPK)
        {
            string source = "";
            if (IsOperator(type))
            {
                string dateStr = CheckDate(type);
                source += dateStr;
                string param = dateStr.Length > 0 ? "d" : "id";
                
                source += "string EqType = GT.ToUpper();" + NL();
                source += "IEnumerable<"+key+ "> items = null;" + NL();
                source += "switch(EqType)" + NL();
                source += "{" + NL();

                source += GetSpecificLinq(objName, key, "=", "EQ", IsOrderBy, param, type);
                source += GetSpecificLinq(objName, key, ">", "GT", IsOrderBy, param, type);
                source += GetSpecificLinq(objName, key, "<", "LT", IsOrderBy, param, type);
                source += GetSpecificLinq(objName, key, ">=", "GTEQ", IsOrderBy, param, type);
                source += GetSpecificLinq(objName, key, "<=", "LTEQ", IsOrderBy, param, type);
                source += GetSpecificLinq(objName, key, "!=", "NOT", IsOrderBy, param, type);

                source += "}" + NL();//end switch
            }

            return source;
        }

        private static string GetSpecificLinq(string objName, string key, string sign, string signName, bool IsOrderBy, string param, string type)
        {
            //string pk = IsPK ? "Value" : "";
            string varObj = IsOrderBy ? "" : "var";
            string source = "";
            string s = type == "string" ? "" : ".ToString()";

            source += "case \"" + signName + "\":" + NL();
            source += "{" + NL();

            string w = "\"" + objName + sign + "\"+" + param + s;
            source += CreateSqlFetch(key, w, w, cacheTimeInMinutes,false);
            
            if (IsOrderBy)
                source += "break;" + NL();
            else
                source += "return items;" + NL();
            source += "}" + NL();
            return source;
        }

        //create getDataInMemory
        public static string CreategetDataInMemory(string key, bool IsBalancer, bool IsSharding = false, bool IsCluster = false)
        {
            string source = "";

            if (IsBalancer)
            {
                source += "private string getUrl(string url)" + NL();
                source += "{" + NL();
                source += "    url = url.Replace(\"_\", \"/\");" + NL();
                source += "    url = url.Replace(\"!\", \":\");" + NL();
                source += "    return url;" + NL();
                source += "}" + NL();

                source += "public async Task<HttpResponseMessage> getDataInMemory(string GT,int id)" + NL();
                source += "{" + NL();
                source += "string s = getUrl(GT);" + NL();
                if(IsCluster)
                    source += "return await ClusterManager.GetData(s + \"/" + key + "/getDataInMemory/\"+id);" + NL();
                else
                    source += "return await BalancerManager.GetData(s + \"/" + key + "/getDataInMemory/\"+id);" + NL();
                source += "}" + NL();
            }
            else if (IsSharding)
            {
                source += "private string getUrl(string url)" + NL();
                source += "{" + NL();
                source += "    url = url.Replace(\"_\", \"/\");" + NL();
                source += "    url = url.Replace(\"!\", \":\");" + NL();
                source += "    return url;" + NL();
                source += "}" + NL();

                source += "public HttpResponseMessage getDataInMemory(string GT,int id)" + NL();
                source += "{" + NL();
                source += "string s = getUrl(GT);" + NL();
                source += "return BalancerManager.createSeveralGetRequests(s + \"/" + key + "/getDataInMemory/\"+id);" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "public HttpResponseMessage getDataInMemory(int id){" + NL();//start getDataInMemory

                source += "HttpResponseMessage resp = new HttpResponseMessage();" + NL();
                source += "try" + NL();
                source += "{" + NL();
                source += "    " + key + "Manager.getDataInMemory(id);" + NL();
                source += "}" + NL();
                source += "catch (Exception ex)" + NL();
                source += "{" + NL();
                source += "    string msg = \"" + key + " table\";" + NL();
                source += "    msg += id == 1 ? \"Error init memory, \" : \"Error remove memory, \";" + NL();
                source += "    LogManager.WriteError(msg + ex.Message);" + NL();
                source += "    resp.StatusCode = HttpStatusCode.BadRequest;" + NL();
                source += "    resp.Content = new StringContent(\"failed\");" + NL();
                source += "}" + NL();
                source += "resp.StatusCode = HttpStatusCode.OK;" + NL();
                source += "    resp.Content = new StringContent(\"Success\");" + NL();
                source += "return resp;" + NL();
                source += "}" + NL();//end getDataInMemory func

                //create ClearMemory
                //source += "public static void ClearMemory(){" + NL();//start ClearMemory
                //source += key + "Manager.ClearMemory();" + NL();
                //source += "}" + NL();//end ClearMemory func

            }
            return source;
        }

        /// <summary>
        /// Create Init And IsDataInMemory in controller
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string CreateInitAndIsDataInMemory(string key)
        {
            string source = "";
            source += "public static void Init(){" + NL();//start init func
            source += key + "Manager.Init();" + NL();
            source += "}" + NL();//end init func

            //create function is in mem
            source += "public static bool IsDataInMemory(){" + NL();//start IsDataInMemory
            source += "return " + key + "Manager.IsDataInMemory();" + NL();
            source += "}" + NL();//end IsDataInMemory func
            return source;
        }

        /// <summary>
        /// Create Web Api In Memory
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        


        public static string CreategetDataInMemoryManager(string key, bool IsPK, bool IsReadOnly)
        {
            string source = "";
            source += "public static void getDataInMemory(int id)" + NL();
            source += "{" + NL();
            source += "isInMemory = id == 1;" + NL();
            source += "if (isInMemory)" + NL();
            source += "{" + NL();
            //source += "Statistics.RemoveFromHash(\"" + key + "\");" + NL();
            source += "Init();" + NL();
            source += "}" + NL();
            source += "else" + NL();
            source += "{" + NL();
            //source += "Statistics.AddToHash(\"" + key + "\");" + NL();
            if (!IsPK && !IsReadOnly)
            {
                source += TAB() + key + " val2;" + NL();
                source += TAB() + "while (!" + key + "List.IsEmpty)" + NL();
                source += TAB() + "{" + NL();
                source += TAB() + key + "List.TryTake(out val2);" + NL();
                source += TAB() + "}" + NL();
            }
            else
                source += key + "List.Clear();" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        /// <summary>
        /// Create Web Farm Class
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="dbName"></param>
        /// <param name="ServerId"></param>
        /// <param name="EntityName"></param>
        /// <param name="LstColumnName"></param>
        /// <param name="IsInsert"></param>
        /// <param name="IsUpdate"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public static string CreateWebFarmClass(string dbtype, string dbName, string ServerId, string EntityName, List<Table> LstColumnName, bool IsInsert, bool IsUpdate, bool IsDelete)
        {
            string source = "using System;" + NL();
            source += "using System.Collections.Generic;" + NL();
            source += "using System.Linq;" + NL();
            source += "using System.Net; " + NL();
            source += "using System.Net.Http; " + NL();
            source += "using System.Web.Http;" + NL();
            source += "using System.Web;" + NL();
            source += "using System.Data;" + NL();
            source += "using System.Threading.Tasks;" + NL();
            source += "namespace " + dbtype + "." + dbName + NL();
            source += "{" + NL();
            source += "public class " + EntityName + "Controller : ApiController" + NL();
            source += "  {" + NL();

            source += "private int GetServerId()" + NL();
            source += "{" + NL();
            source += "    return UrlHandler.GetServerIdByEntity(\"" + EntityName + "\");" + NL();
            source += "}" + NL();

            //create query
            source += CreateWebFarmBaseCallRetrieveQueryX(ServerId);

            //for(int i=0;i<LstColumnName.Count;i++)

            //    //create regular calls
            //    source += CreateWebApiFuncForFilesWebFarm(EntityName, LstColumnName[i].Name);

            for (int j = 0; j < LstColumnName.Count; j++)
            {
                string objName = LstColumnName[j].Name;
                if (dbtype == DbTypes.File)
                {
                    source += FunctionCreator.CreateWebApiFuncForFilesWebFarm(objName, EntityName, LstColumnName[j].type, dbName, dbtype, false);//file web api
                    if (IsOperator(LstColumnName[j].type))
                        source += FunctionCreator.CreateWebApiFuncForFilesWebFarm(objName, EntityName, LstColumnName[j].type, dbName, dbtype, true);
                }
                else
                {
                    source += FunctionCreator.CreateWebApiFuncWebFarm(objName, EntityName, LstColumnName[j].type, dbName, dbtype, false);//regular web api
                    source += NL();

                    //lt gt ... web api
                    if (IsOperator(LstColumnName[j].type))
                        source += FunctionCreator.CreateWebApiFuncWebFarm(objName, EntityName, LstColumnName[j].type, dbName, dbtype, true);
                }
            }

            source += CreateWebFarmBasegetDataInMemory(ServerId, EntityName);

            if (IsInsert)
                source += Crud.CreateWebFarmInsert(ServerId, EntityName);

            if (IsUpdate && Crud.IsPrimaryKey(LstColumnName))
                source += Crud.CreateWebFarmUpdate(ServerId, EntityName);

            if (IsDelete && Crud.IsPrimaryKey(LstColumnName))
                source += Crud.CreateWebFarmDelete(ServerId, EntityName);

            source += "  }" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateReplicateMasterCode(bool IsReplicate, string key, bool IsCluster=false)
        {
            string source = "";
            if (IsReplicate)
            {
                source += "private void PostToSlave(" + key + " item)" + NL();
                source += "{" + NL();
                source += "if (MasterServer.IsMainMaster())" + NL();
                source += "    MasterServer.PostData(item, \"" + key + "/Post" + key + "\");" + NL();
                source += "}" + NL();

                source += "private void PostListToSlave(List<" + key + "> item)" + NL();
                source += "{" + NL();
                source += "if (MasterServer.IsMainMaster())" + NL();
                source += "    MasterServer.PostData(item, \"" + key + "/PostList" + key + "\");" + NL();
                source += "}" + NL();

                source += "private void PutToSlave(" + key + " item)" + NL();
                source += "{" + NL();
                source += "if (MasterServer.IsMainMaster())" + NL();
                source += "    MasterServer.PutData(item, \"" + key + "/Put" + key + "\");" + NL();
                source += "}" + NL();

                source += "private void DeletFromSlave(" + key + " item)" + NL();
                source += "{" + NL();
                source += "if (MasterServer.IsMainMaster())" + NL();
                source += "    MasterServer.DeleteData(item,\"" + key + "\");" + NL();
                source += "}" + NL();
            }
            else if (IsCluster)
            {
                source += "private void PostToSlave(" + key + " item)" + NL();
                source += "{" + NL();
                source += "if (ClusterMaster.IsMainMaster())" + NL();
                source += "    ClusterMaster.PostData(item, \"" + key + "/Post" + key + "\");" + NL();
                source += "}" + NL();

                source += "private void PostListToSlave(List<" + key + "> item)" + NL();
                source += "{" + NL();
                source += "if (ClusterMaster.IsMainMaster())" + NL();
                source += "    ClusterMaster.PostData(item, \"" + key + "/PostList" + key + "\");" + NL();
                source += "}" + NL();

                source += "private void PutToSlave(" + key + " item)" + NL();
                source += "{" + NL();
                source += "if (ClusterMaster.IsMainMaster())" + NL();
                source += "    ClusterMaster.PutData(item, \"" + key + "/Put" + key + "\");" + NL();
                source += "}" + NL();

                source += "private void DeletFromSlave(" + key + " item)" + NL();
                source += "{" + NL();
                source += "if (ClusterMaster.IsMainMaster())" + NL();
                source += "    ClusterMaster.DeleteData(item,\"" + key + "\");" + NL();
                source += "}" + NL();
            }
            return source;
        }

        public static string CreateReplicateMasterCodeNoPrimaryKeyNoReadOnly(Dictionary<string, EntityItem> dic,
            bool IsReplicate,string key, bool IsCluster = false)
        {
            string source = "";
            if (IsReplicate)
            {
                foreach (Table t in dic[key].lstTables)
                {
                    source += "private void PutToSlaveBy" + t.Name + "(" + key + " item)" + NL();
                    source += "{" + NL();
                    source += "    MasterServer.PutData(item, \"" + key + "\\Put" + key + "By" + t.Name + "\");" + NL();
                    source += "}" + NL();

                    source += "private void DeletFromSlave" + t.Name + "(" + key + " item)" + NL();
                    source += "{" + NL();
                    source += "    MasterServer.DeleteData(item,\"" + key + "\\Delete" + key + "By" + t.Name + "\");" + NL();
                    source += "}" + NL();
                }
            }
            else if (IsCluster)
            {

            }
            return source;
        }

        public static string CreateGetEmpty(bool IsBalancer, string key, bool IsCluster=false)
        {
            string source = "";
            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetEmpty()" + NL();
                source += "{" + NL();
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GetEmpty\");" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GetEmpty\");" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "public " + key + "[] GetEmpty()" + NL();
                source += "{" + NL();
                source += key + " p = new " + key + "();" + NL();
                source += key + "[] p1 = new " + key + "[1];" + NL();
                source += "p1[0] = p;" + NL();
                source += "return p1;" + NL();
                source += "}" + NL();
            }
            return source;
        }

        public static string CreateGetAll(bool IsBalancer, string key, bool isSharding = false, bool isCluster = false)
        {
            string source = "";
            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetAll()" + NL();
                source += "{" + NL();
                if(isCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GetAll\");" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GetAll\");" + NL();
                source += "}" + NL();

                source += "public async Task<HttpResponseMessage> GetAll(string GT, string id)" + NL();
                source += "{" + NL();
                if (isCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GetAll/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GetAll/\" + GT + \"/\" + id);" + NL();

                source += "}" + NL();
            }
            else if (isSharding)
            {
                source += "public HttpResponseMessage GetAll()" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GetAll\");" + NL();
                source += "}" + NL();

                source += "public HttpResponseMessage GetAll(string GT, string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GetAll/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetAll()" + NL();
                source += "{" + NL();
                //source += "    return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "    {" + NL();
                source += "        var data = " + key + "Manager.Get" + key + "List();" + NL();
                source += "        return CheckCols(data);" + NL();
                //source += "    });" + NL();
                source += "}" + NL();

                source += "public HttpResponseMessage GetAll(string GT, string id)" + NL();
                source += "{" + NL();
                //source += "    return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //source += "    {" + NL();
                source += "        int amount = 0;" + NL();
                source += "        int page = 0;" + NL();
                source += "        var items = " + key + "Manager.Get" + key + "List();" + NL();
                source += "        if (Common.CheckPaging(GT, id, out page, out amount))" + NL();
                source += "        {" + NL();
                source += "            items = items.Skip(amount * (page - 1)).Take(amount);" + NL();
                source += "        }" + NL();
                source += "        return CheckCols(items);" + NL();
                //source += "    });" + NL();
                source += "}" + NL();
            }
            return source;
        }

        public static string CreateIsUpdate(bool IsBalancer, string key, bool IsSharding = false, bool IsCluster = false)
        {
            string source = "";
            if (IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetIsUpdate(int id)" + NL();
                source += "{" + NL();
                if(IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GetIsUpdate/\"+id+ \"\");" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GetIsUpdate/\"+id+ \"\");" + NL();
                source += "}" + NL();
            }
            else if (IsSharding)
            {
                source += "public HttpResponseMessage GetIsUpdate(int id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GetIsUpdate/\"+id+ \"\");" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "public static void GetIsUpdate(int id)" + NL();
                source += "{" + NL();
                source += " IsUpdate = id == 1;" + NL();
                source += "}" + NL();
            }
            return source;
        }

        /// <summary>
        /// CreateCheckCols
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string CreateCheckCols(string entityName)
        {
            string source = "";
            source += "private HttpResponseMessage CheckCols(IEnumerable<"+entityName+"> data)" + NL();
            source += "{" + NL();
            source += "string cols = Request.RequestUri.Query;" + NL();
            source += "int iCols = cols.ToUpper().IndexOf(\"COLS=\");" + NL();
            source += "if (iCols>0)" + NL();
            source += "{" + NL();
            source += "    try" + NL();
            source += "    {" + NL();
            source += "        string q = cols.Substring(iCols + 5, cols.Length - (iCols + 5));" + NL();
            source += "        var result = data" + NL();
            source += "       .Select(\"new (\" + q + \")\");" + NL();
            source += "        return Request.CreateResponse(result);" + NL();
            source += "    }" + NL();
            source += "    catch (Exception ex)" + NL();
            source += "    {" + NL();
            source += "        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);" + NL();
            source += "    }" + NL();
            source += "}" + NL();
            source += "else" + NL();
            source += "    return Request.CreateResponse(data);" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateJoin(string key)
        {
            string source = "";
            source += "[ActionName(\"Join\")]" + NL();
            source += "public object getJoin(string GT, string id, string order, string Asc)" + NL();
            source += "{" + NL();
            source += "    try" + NL();
            source += "    {" + NL();
            source += "        order = QueryManager.GetExpression(order);" + NL();
            source += "        return "+ key+"Manager.Join(GT, id, order, Asc);" + NL();
            source += "    }" + NL();
            source += "   catch (Exception ex)" + NL();
            source += "{" + NL();
            source += "return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);" + NL();
            source += "}" + NL();
            source += "}" + NL();

            source += "[ActionName(\"Join\")]" + NL();
            source += "public object getJoin(string MAX, string GT, string id)" + NL();
            source += "{" + NL();
            source += "try" + NL();
            source += "{" + NL();
            source += "id = QueryManager.GetExpression(id);" + NL();
            source += "return " + key + "Manager.Join(MAX, GT, id, \"\");" + NL();
            source += "}" + NL();
            source += "catch (Exception ex)" + NL();
            source += "{" + NL();
            source += "return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);" + NL();
            source += "}" + NL();
            source += "}" + NL();

            source += "[ActionName(\"Join\")]" + NL();
            source += "public object getJoin(string GT, string id)" + NL();
            source += "{" + NL();
            source += "try" + NL();
            source += "{           " + NL();
            source += "return " + key + "Manager.Join(GT, id, \"\", \"\");" + NL();
            source += "}" + NL();
            source += "catch (Exception ex)" + NL();
            source += "{" + NL();
            source += "return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateWebFarmBaseRetrieve(string BaseUrl)
        {
            string source = "private async Task<HttpResponseMessage> RetrieveData(string url)" + NL();
            source += "{" + NL();
            source += "HttpClient client = new HttpClient();" + NL();
            source += "client.BaseAddress = new Uri(\"" + BaseUrl + "\");" + NL();
            source += "var task = client.GetAsync(url);" + NL();
            source += "var result = await task;" + NL();
            source += "return result;" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateWebFarmBaseCallRetrieve(string ServerId, string EntityName, string ColumnName)
        {
            string source = "[ActionName(\"" + ColumnName + "\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetBy" + ColumnName + "(string id)" + NL();
            source += "{" + NL();
            source += "return await UrlHandler.RetrieveData(GetServerId(),\"" + EntityName + "/" + ColumnName + "/\" + id);" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateWebFarmBaseCallRetrieveQueryX(string ServerId)
        {
            string source = "[ActionName(\"Query\")]" + NL();
            source += "public async Task<HttpResponseMessage> GetQueryX(string id)" + NL();
            source += "{" + NL();
            source += "return await UrlHandler.RetrieveData(GetServerId(),\"Query/\" + id);" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateWebFarmBasegetDataInMemory(string ServerId, string EntityName)
        {
            string source = "[ActionName(\"getDataInMemory\")]" + NL();
            source += "public async Task<HttpResponseMessage> getDataInMemory(int id)" + NL();
            source += "{" + NL();
            source += "return await UrlHandler.RetrieveData(GetServerId(),\"" + EntityName + "/getDataInMemory/\" + id);" + NL();
            source += "}" + NL();
            return source;
        }

        public static string CreateCalcFunction(string key)
        {
            string source = "";
            source+="public static object Calc(string str, string where)" + NL();
            source+="{" + NL();
            source+="try{ IEnumerable<"+key+"> data = new List<"+key+">();" + NL();
            source+="if (where == null)" + NL();
            source+="{" + NL();
            source+="    data = Get"+key+"List();" + NL();
            source+="}" + NL();
            source+="else" + NL();
            source+="{" + NL();
            source+="    try" + NL();
            source+="    {" + NL();
            source+="        data = GetQueryX(where);" + NL();
            source+="    }" + NL();
            source+="    catch" + NL();
            source+="    {" + NL();
            source+="        data = Get"+key+"List();" + NL();
            source+="    }" + NL();
            source+="}" + NL();
            source+="string x = \"\" ;" + NL();
            source+="  string[] arr = str.Split(' ');" + NL();
            source+="    string select = \"\"; " + NL();
            source+="    for (int i = 0; i < arr.Length; i++)" + NL();
            source+="    {" + NL();
            source+="        if (arr[i].ToUpper() == \"MUL\" )" + NL();
            source+="            arr[i] = \"*\";" + NL();
            source+="        else if (arr[i].ToUpper() == \"DIV\")" + NL();
            source+="            arr[i] = \"/\"; " + NL();
            source+="        else if (arr[i].ToUpper() == \"MOD\")" + NL();
            source+="            arr[i] = \"%\";" + NL();
            source+="        else if (arr[i].ToUpper() == \"PLUS\")" + NL();
            source+="            arr[i] = \"+\"; " + NL();
            source+="        else if (arr[i].ToUpper() == \"MINUS\")" + NL();
            source+="            arr[i] = \"-\";" + NL();
            source+="        else" + NL();
            source+="            arr[i] = \" \" + arr[i] + \" \";" + NL();
            source+="    } " + NL();
            source+="    select = string.Join(\"\" , arr);" + NL();
            source+="    x = \"new(\" + select + \")\";" + NL();
            source+="var test = data.AsQueryable().AsParallel().Select(x);" + NL();
            source+="return test;" + NL();
            source+="  }" + NL();
            
            source+="catch (Exception ex)" + NL();
            source+="{" + NL();
            source+="    return ex.Message;" + NL();
            source += "}" + NL();
            source += "}" + NL();
            return source;
        }
        public static string CreateGetGroupByManager(string key)
        {
            string source = "";
            source += "public static object GetGroupBy(string groupById,string cols,string where,string havingStr=\"\")" + NL();
            source += "{" + NL();

            source += "string wh = \"\";" + NL();
            source += "string having = \"\";" + NL();

            source += "if (!string.IsNullOrEmpty(where))" + NL();
            source += "{" + NL();
            source += "    where = QueryManager.GetExpression(where);" + NL();
            source += "    if (!string.IsNullOrEmpty(havingStr))" + NL();
            source += "    {" + NL();
            source += "        having = \" \" + QueryManager.GetExpression(havingStr);" + NL();
            source += "    }" + NL();

            source += "    if (where.ToUpper().IndexOf(\"HAVING\") >= 0)" + NL();
            source += "    {" + NL();
            source += "        having = \" \" + where;" + NL();
            source += "    }" + NL();
            source += "    else" + NL();
            source += "    {" + NL();
            source += "       wh = \"where \" + where;" + NL();
            source += "    }" + NL();
            source += "}" + NL();
            source += "string w = \"SELECT \" + cols + \" FROM "+ key+" \" + wh + \" GROUP BY \" + groupById + having;" + NL();
            
            source += "object items = GroupByManager.GetDataFromDb(w, \"" + key + "\");" + NL();
            
            source += "return items;" + NL();

            source += "}" + NL();
            return source;
        }

        public static string CreateGetGroupBy2Manager(List<Table> lstTable, string key)
        {
            string source = "";
            source += "public static object GetGroupByData(string groupBy,string calc,string where){" + NL();
            source += "IEnumerable<"+key+"> data = new List<"+key+">();" + NL();
            source += "if (where == null)" + NL();
            source += "{" + NL();
            source += "    data = Get"+key+"List();" + NL();
            source += "}" + NL();
            source += "else" + NL();
            source += "{" + NL();
            source += "    try" + NL();
            source += "    {" + NL();
            source += "        data = GetQueryX(where);" + NL();
            source += "    }" + NL();
            source += "    catch" + NL();
            source += "    {" + NL();
            source += "        data = Get"+key+"List();" + NL();
            source += "    }" + NL();
            source += "}" + NL();
            source += "string cols = GetColFromCalc(calc);" + NL();
            source += "if (cols.Length == 0)" + NL();
            source += "    return null; " + NL();
            source += "var test = data.AsQueryable().AsParallel()" + NL();
            source += ".GroupBy(\"new(\" + groupBy + \")\", \"new(\" + cols + \")\")" + NL();
            source += ".Select(\"new(Key.\" + groupBy + \", \"+calc+\")\");" + NL();
            source += "return test;" + NL();

            source += "}" + NL();

            source += "private static string GetColFromCalc(string calc)" + NL();
            source += "{" + NL();
            source += "string ret = \"\";" + NL();
            foreach (Table table in lstTable)
            {
                source += "if (calc.Contains(\"("+ table.Name +")\"))" + NL();
                source += "   ret += \""+ table.Name+",\";" + NL();                
            }
            source += "if(ret.Length>0)" + NL();
            source += "    ret = ret.Substring(0, ret.Length - 1);" + NL();
            source += "return ret;" + NL();

            source += "}" + NL();
            return source;
        }
        
        public static string CreateGlobalQuery(string EntityName, string dbName)
        {            
            string source = "";
            source += "public static IEnumerable<" + EntityName + "> GetQueryX(string id)" + NL();
            source += "{" + NL();
            
            source += "string paging = \"\";" + NL();
            source += "string size = \"\";" + NL();
            source += "int amount = 0;" + NL();
            source += "int page = 0;" + NL();

            
            source += "try" + NL();
            source += "    {" + NL();
            source += "if (!QueryManager.CheckForbidenChars(id))" + NL();
            source += "{" + NL();
            source += "    throw new Exception(\"error\");" + NL();
            source += "}" + NL();
            source += "id = QueryManager.GetExpression(id);" + NL();
            source += "id = QueryManager.HandleComplicateExperessionsForDataBase(id,\"" + EntityName + "\");" + NL();
            source += "int ind = id.IndexOf(\"page=\");" + NL();
            source += "if (ind > 0)" + NL();
            source += "{" + NL();
            source += "    QueryManager.CheckPaging(id, out paging, out size);" + NL();
            source += "    id = id.Substring(0, ind);" + NL();
            source += "}" + NL();
            string w = "id";
            source += CreateSqlFetch(EntityName, w, w, cacheTimeInMinutes);
            source += "     if (Common.CheckPaging(paging, size, out page, out amount))" + NL();
            source += "     {" + NL();
            source += "         items = items.Skip(amount * (page - 1)).Take(amount);" + NL();
            source += "     }" + NL();
            source += "return items;" + NL();
            source += "      }" + NL();
            source += "catch (Exception e)" + NL();
            source += "    {" + NL();

            source += "        Task.Factory.StartNew(() => { LogManager.WriteError(\"funcName : GetQuery , Error - \" + e.Message); });" + NL();
            source += "        throw new Exception(\"error\");" + NL();
            source += "    }" + NL();
            //source += "    }" + NL();
            //source += "return null;" + NL();
            source += "}" + NL();
            return source;
        }
    }
}
