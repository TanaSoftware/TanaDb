using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImFast.Dal;
using ImFast.BL;

namespace ImFast.CodeCreator
{
    /// <summary>
    /// creade update delete code string creation
    /// </summary>
    public class Crud : Base
    {

        public static string CreateInsertKeyValManager(Dictionary<string, EntityItem> dic, string key, string dbType, bool IsReadOnly)
        {

            string str = "public static HttpResponseMessage Post" + key + "(" + key + " item)" + NL();
            str += "{" + NL();
            str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
            str += "try" + NL();
            str += "{" + NL();

            str += "if (" + key + "List.ContainsKey(item." + dic[key].lstTables[0].Name + "))" + NL();
            str += "{" + NL();
            str += " var resp = new HttpResponseMessage(HttpStatusCode.Ambiguous)" + NL();
            str += " {" + NL();
            str += "    Content = new StringContent(\"Primary key violation\")" + NL();
            str += " };" + NL();
            str += "return resp;" + NL();
            str += "}" + NL();//end if


            str += key + "List.TryAdd(item." + dic[key].lstTables[0].Name + ", item);" + NL();
            str += "UpdateIsUpdate(true);" + NL();
            str += "}" + NL();
            str += "catch (Exception e)" + NL();
            str += "{" + NL();
            str += "    status = HttpStatusCode.BadRequest;" + NL();
            str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Post failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
            str += "}" + NL();
            str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
            str += "return response;" + NL();
            str += "}" + NL();

            return str;
        }

        public static string CreateUpdateKeyValManager(Dictionary<string, EntityItem> dic, string key, string dbType, bool IsReadOnly)
        {

            string str = "public static HttpResponseMessage Put" + key + "(" + key + " item)" + NL();
            str += "{" + NL();
            str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
            str += "try" + NL();
            str += "{" + NL();

            str += "if (!" + key + "List.ContainsKey(item." + dic[key].lstTables[0].Name + "))" + NL();
            str += "{" + NL();
            str += " var resp = new HttpResponseMessage(HttpStatusCode.Ambiguous)" + NL();
            str += " {" + NL();
            str += "    Content = new StringContent(\"The key does not exist\")" + NL();
            str += " };" + NL();
            str += "return resp;" + NL();
            str += "}" + NL();//end if

            str += key + "List[item." + dic[key].lstTables[0].Name + "] = item;" + NL();
            str += "UpdateIsUpdate(true);" + NL();
            //str += "object obj;" + NL();
            //str += "if ("+key+ "List.TryRemove(item." + dic[key].lstTables[0].Name +", out obj))" + NL();
            //str += "{" + NL();  
            //str +=   key + "List.TryAdd(item." + dic[key].lstTables[0].Name + ", item.value);" + NL();
            //str += "}" + NL();  


            str += "}" + NL();
            str += "catch (Exception e)" + NL();
            str += "{" + NL();
            str += "    status = HttpStatusCode.BadRequest;" + NL();
            str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Post failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
            str += "}" + NL();
            str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
            str += "return response;" + NL();
            str += "}" + NL();

            return str;
        }

        public static string CreateDeleteKeyValManager(Dictionary<string, EntityItem> dic, string key, string dbType, bool IsReadOnly)
        {

            string str = "public static HttpResponseMessage Delete" + key + "(" + key + " item)" + NL();
            str += "{" + NL();
            str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
            str += "try" + NL();
            str += "{" + NL();

            str += "if (!" + key + "List.ContainsKey(item." + dic[key].lstTables[0].Name + "))" + NL();
            str += "{" + NL();
            str += " var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
            str += " {" + NL();
            str += "    Content = new StringContent(\"The key does not exist\")" + NL();
            str += " };" + NL();
            str += "return resp;" + NL();
            str += "}" + NL();//end if

            str += key + " obj;" + NL();
            str += "if (!" + key + "List.TryRemove(item." + dic[key].lstTables[0].Name + ", out obj))" + NL();
            str += "{" + NL();
            str += " var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
            str += " {" + NL();
            str += "    Content = new StringContent(\"The key does not exist\")" + NL();
            str += " };" + NL();
            str += "}" + NL();
            str += "else" + NL();
            str += "   {" + NL();
            str += "    UpdateIsUpdate(true);" + NL();
            str += "   }" + NL();
            str += "}" + NL();
            str += "catch (Exception e)" + NL();
            str += "{" + NL();
            str += "    status = HttpStatusCode.BadRequest;" + NL();
            str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Post failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
            str += "}" + NL();
            str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
            str += "return response;" + NL();
            str += "}" + NL();

            return str;
        }

        public static string CreateInsertListManager(Dictionary<string, EntityItem> dic, string key, string dbType, bool IsReadOnly)
        {
            string str = "";
            bool IsFile = dic[key].DbType == DbTypes.File;
            bool IsPK = false;
            string primaryKeys = "";
            List<Table> lstPrimary = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);
            IsPK = lstPrimary.Count > 0;

            if (IsPK)
            {
                if (lstPrimary.Count > 1)//more than one pk
                {
                    foreach (Table t in lstPrimary)
                    {
                        primaryKeys += "item." + t.Name + ".ToString() + ";
                    }
                    primaryKeys = primaryKeys.Substring(0, primaryKeys.Length - 3);
                }
                else//one pk
                    primaryKeys += GetCovertType(lstPrimary[0].type) + "(item." + lstPrimary[0].Name + ")";
            }
            str = "public static List<string> PostList" + key + "(List<" + key + "> items)" + NL();
            str += "{" + NL();
            str += "List<string> lst = new List<string>();" + NL();
            str += "bool isSuccessUpdate = false;" + NL();
            str += "if (isInMemory)" + NL();
            str += "{" + NL();
            str += "try" + NL();
            str += "{" + NL();
            //if(!IsPK)
            //    str += "int iCounter = 1;" + NL();

            str += "    foreach (" + key + " item in items)" + NL();
            str += "    {" + NL();

            if (IsPK)
            {
                str += "    if (" + key + "List.ContainsKey(" + primaryKeys + "))" + NL();
                str += "    {" + NL();
                str += "       lst.Add(\"Primary key violation in item key \" + " + primaryKeys + ".ToString());" + NL();
                str += "        break;" + NL();
                str += "    }" + NL();


                str += "if (" + key + "List.TryAdd(" + primaryKeys + ", item))" + NL();
                str += "        isSuccessUpdate = true;" + NL();
                str += "    else" + NL();
                str += "    {" + NL();
                str += "        lst.Add(\"Error insert item id \" + " + primaryKeys + ".ToString());" + NL();
                str += "    }" + NL();
            }
            else
            {
                str += key + "List.Add(item);" + NL();
                str += "   isSuccessUpdate = true;" + NL();
                //str += "   iCounter++;" + NL();
            }

            str += "    }" + NL();//end foreach
            str += "}" + NL();
            str += "catch (Exception e)" + NL();
            str += "{" + NL();
            if (!IsPK)
                str += "lst.Add(\"item failed save\");" + NL();
            str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Post failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
            str += "}" + NL();
            str += "if (isSuccessUpdate)" + NL();
            str += "        UpdateIsUpdate(true);" + NL();
            str += "}" + NL();//end isInMemory
            str += "return lst; " + NL();
            str += "}" + NL();


            return str;
        }

        private static string CreateInsertSql(Dictionary<string, EntityItem> dic, string key)
        {
            string source = "private static void InsertSql(" + key + " item){ " + NL();

            source += "string cols = \"\";" + NL();
            source += "string values = \"\";" + NL();


            for (int i = 0; i < dic[key].lstTables.Count; i++)
            {
                source += "if (item." + dic[key].lstTables[i].Name + " != null){" + NL();
                source += "cols += \"" + dic[key].lstTables[i].Name + ",\";" + NL();
                //if (dic[key].lstTables[i].type == "string")
                //    source += "values += \"'\" + item." + dic[key].lstTables[i].Name + "+\"',\";" + NL();
                //else
                source += "values += \"@" + dic[key].lstTables[i].Name + ",\";" + NL();

                source += "}" + NL();

            }

            source += "cols = cols.Substring(0, cols.Length - 1);" + NL();
            source += "values = values.Substring(0, values.Length - 1);" + NL();


            source += "string str = \"Insert " + key + "(\" + cols + \") values (\" + values + \")\";" + NL();

            source += "Dictionary<string, EntityItem> dicEntities = MyEntitiesController.GetdicEntities();" + NL();
            source += "string sConn = dicEntities[\"" + key + "\"].DataBaseConnectionString;" + NL();
            source += "try" + NL();
            source += "{" + NL();
            source += "    DataBaseRetriever dal = new DataBaseRetriever(sConn);" + NL();
            source += "    dal.Execute(str, dicEntities[\"" + key + "\"].DataBaseTypeName,item);" + NL();
            source += "}" + NL();
            source += "catch (Exception ex)" + NL();
            source += "{" + NL();
            source += "    LogManager.WriteError(\"Insert to Db failed: object " + key + " fail: Error - \" + ex.Message);" + NL();
            source += "}" + NL();

            source += "}";
            return source;
        }

        private static string CreateUpdatetSql(Dictionary<string, EntityItem> dic, string key)
        {
            string source = "private static void UpdateSql(" + key + " item){ " + NL();

            List<Table> foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);
            if (foundPk.Count > 0)
            {
                source += "string values = \"\";" + NL();
                source += "string where = \"\";" + NL();

                for (int i = 0; i < foundPk.Count; i++)
                {
                    source += " where+= \"" + foundPk[i].Name + " = @" + foundPk[i].Name + " and \";" + NL();
                }
                source += ";" + NL();

                for (int i = 0; i < dic[key].lstTables.Count; i++)
                {
                    source += "if (item." + dic[key].lstTables[i].Name + " != null){" + NL();
                    source += "values +=\"" + dic[key].lstTables[i].Name + " = @" + dic[key].lstTables[i].Name + ",\";" + NL();

                    source += "}" + NL();

                }


                source += "values = values.Substring(0, values.Length - 1);" + NL();
                source += "where = where.Substring(0, where.Length - 4);" + NL();

                source += "string str = \" UPDATE " + key + " Set \" + values + \" where \" + where;" + NL();

                source += "Dictionary<string, EntityItem> dicEntities = MyEntitiesController.GetdicEntities();" + NL();
                source += "string sConn = dicEntities[\"" + key + "\"].DataBaseConnectionString;" + NL();
                source += "try" + NL();
                source += "{" + NL();
                source += "    DataBaseRetriever dal = new DataBaseRetriever(sConn);" + NL();
                source += "    dal.Execute(str, dicEntities[\"" + key + "\"].DataBaseTypeName,item);" + NL();
                source += "}" + NL();
                source += "catch (Exception ex)" + NL();
                source += "{" + NL();
                source += "    LogManager.WriteError(\"Insert to Db failed: object " + key + " fail: Error - \" + ex.Message);" + NL();
                source += "}" + NL();
            }
            source += "}";
            return source;
        }

        private static string CreateDeletetSql(Dictionary<string, EntityItem> dic, string key)
        {
            string source = "private static void DeleteSql(" + key + " item){ " + NL();

            List<Table> foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);
            if (foundPk.Count > 0)
            {
                source += "string where = \"\";" + NL();

                for (int i = 0; i < foundPk.Count; i++)
                {
                    source += " where+= \"" + foundPk[i].Name + " = @" + foundPk[i].Name + ",\";";
                }

                source += "where = where.Substring(0, where.Length - 1);" + NL();

                source += "string str = \" Delete From " + key + " Where \" + where;" + NL();

                source += "Dictionary<string, EntityItem> dicEntities = MyEntitiesController.GetdicEntities();" + NL();
                source += "string sConn = dicEntities[\"" + key + "\"].DataBaseConnectionString;" + NL();
                source += "try" + NL();
                source += "{" + NL();
                source += "    DataBaseRetriever dal = new DataBaseRetriever(sConn);" + NL();
                source += "    dal.Execute(str, dicEntities[\"" + key + "\"].DataBaseTypeName,item);" + NL();
                source += "}" + NL();
                source += "catch (Exception ex)" + NL();
                source += "{" + NL();
                source += "    LogManager.WriteError(\"Insert to Db failed: object " + key + " fail: Error - \" + ex.Message);" + NL();
                source += "}" + NL();
            }
            source += "}";
            return source;
        }

        public static string CreateInsertManager(Dictionary<string, EntityItem> dic, string key, string dbType, CheckedItems checkedItems)
        {            
            string str = "";

            if (!checkedItems.IsReadOnly)
            {
                str += CreateInsertSql(dic, key);


                str += "public static HttpResponseMessage Post" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
                str += "try" + NL();
                str += "{" + NL();              
                
                str += "Task.Factory.StartNew(() => { InsertSql(item); });" + NL();
                        
                str += "}" + NL();
                str += "catch (Exception e)" + NL();
                str += "{" + NL();
                str += "    status = HttpStatusCode.BadRequest;" + NL();
                str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Post failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
                str += "}" + NL();
                str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
                str += "return response;" + NL();
                str += "}" + NL();
            }
            return str;
        }


        private static string getPkCode(List<Table> foundPk, string key, string type = "")
        {
            string pkName = foundPk.Count == 1 ? foundPk[0].Name : "key";
            string pkValue = "";
            if (foundPk.Count == 1)
                pkValue = "item." + foundPk[0].Name;
            else
            {
                foreach (Table t in foundPk)
                {
                    pkValue += "item." + t.Name + ".ToString()";
                }

            }
            string str = "";
            if (type == "Post")
            {
                str = "HttpResponseMessage jsonContent = BalancerManager.createSeveralGetRequests(\"" + key + "/" + pkName + "/" + "\"+" + pkValue + ");" + NL();
            }
            else
            {
                str = "string url = BalancerManager.createSeveralGetRequests2(\"" + key + "/" + pkName + "/" + "\"+" + pkValue + ");" + NL();
            }
            return str;
        }
        public static string CreateInsert(Dictionary<string, EntityItem> dic, string key, string dbType,
            bool IsReplicate, bool IsBalancer, bool IsReadOnly, bool IsReplicationBalancer = false,
            bool IsSharding = false, bool IsCluster = false)
        {

            string str = "";

            //todo: is replicationBalancer use MasterServer.PostData
            if (IsBalancer)
            {
                str += "public HttpResponseMessage Post" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();
                if (IsCluster)
                    str += "return ClusterManager.PostData(item,\"" + key + "/Post" + key + "\");" + NL();
                else
                    str += "return BalancerManager.PostData(item,\"" + key + "/Post" + key + "\");" + NL();

            }
            else if (IsSharding)
            {
                str += "public HttpResponseMessage Post" + key + "(" + key + " item)" + NL();
                str += "{" + NL();

                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();

                List<Table> foundPk = null;

                if (dic[key].lstTables != null)
                    foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);

                bool IsPkExist = foundPk != null && foundPk.Count > 0;
                if (IsPkExist)
                {
                    str += getPkCode(foundPk, key, "Post");
                    str += "if (jsonContent != null && jsonContent.Content.ReadAsStringAsync().Result.Length > 2)" + NL();
                    str += "{" + NL();
                    str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Ambiguous);" + NL();
                    str += "    return resp;" + NL();
                    str += "}" + NL();
                }
                str += "return BalancerManager.createSeveralPosts(item,\"" + key + "/Post" + key + "\", \"POST\");" + NL();

            }
            else
            {
                str += "[HttpPost]" + NL();
                str += "public HttpResponseMessage Post" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                if (!IsReadOnly)
                {
                    str += "if (item == null)" + NL();
                    str += "{" + NL();
                    str += "var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
                    str += "{" + NL();
                    str += "    Content = new StringContent(\"" + key + " parameter is null\")" + NL();
                    str += "};" + NL();
                    str += "return resp;" + NL();
                    str += "}" + NL();

                    str += "if (!IsUpdate)" + NL();
                    str += "{" + NL();
                    str += "   HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);" + NL();
                    str += "    return response;" + NL();
                    str += "}" + NL();

                    str += "if (MemoryObserver.IsexceededMemory())" + NL();
                    str += "{" + NL();
                    str += "LogManager.WriteError(\"Memory has exceeded\");" + NL();
                    str += "var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
                    str += "{" + NL();
                    str += "    Content = new StringContent(\"Memory has exceeded\")" + NL();
                    str += "};" + NL();
                    str += "return resp;" + NL();
                    str += "}" + NL();
                }
                //str += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //str += "{" + NL();
                str += "    var data = " + key + "Manager.Post" + key + "(item);" + NL();

                if (IsReplicate || IsCluster)
                {
                    str += "if (data.IsSuccessStatusCode)" + NL();
                    str += "    Task.Factory.StartNew(() => PostToSlave(item));" + NL();
                }

                str += "    return data;" + NL();

                //str += "});" + NL();
            }
            str += "}" + NL();


            return str;
        }

        public static string CreateInsertList(Dictionary<string, EntityItem> dic, string key, string dbType,
            bool IsReplicate, bool IsBalancer, bool IsReadOnly, bool IsReplicationBalancer = false,
            bool IsSharding = false, bool IsCluster = false)
        {

            string str = "";

            //todo: is replicationBalancer use MasterServer.PostData
            if (IsBalancer)
            {
                str += "public HttpResponseMessage PostList" + key + "(List<" + key + "> item)" + NL();
                str += "{" + NL();
                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();
                if (IsCluster)
                    str += "return ClusterManager.PostData(item,\"" + key + "/Post" + key + "\");" + NL();
                else
                    str += "return BalancerManager.PostData(item,\"" + key + "/Post" + key + "\");" + NL();

            }
            else if (IsSharding)
            {
                str += "public HttpResponseMessage PostList" + key + "(List<" + key + "> item)" + NL();
                str += "{" + NL();

                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();

                //List<Table> foundPk = null;

                //if (dic[key].lstTables != null)
                //    foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);

                //bool IsPkExist = foundPk != null && foundPk.Count > 0;
                //if (IsPkExist)
                //{
                //    str += getPkCode(foundPk, key, "Post");
                //    str += "if (jsonContent != null && jsonContent.Content.ReadAsStringAsync().Result.Length > 2)" + NL();
                //    str += "{" + NL();
                //    str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.Ambiguous);" + NL();
                //    str += "    return resp;" + NL();
                //    str += "}" + NL();
                //}
                str += "return BalancerManager.createSeveralPosts(item,\"" + key + "/Post" + key + "\", \"POST\");" + NL();

            }
            else
            {
                str += "[HttpPost]" + NL();
                str += "public HttpResponseMessage PostList" + key + "(List<" + key + "> item)" + NL();
                str += "{" + NL();
                if (!IsReadOnly)
                {
                    str += "if (item == null)" + NL();
                    str += "{" + NL();
                    str += "var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
                    str += "{" + NL();
                    str += "    Content = new StringContent(\"" + key + " parameter is null\")" + NL();
                    str += "};" + NL();
                    str += "return resp;" + NL();
                    str += "}" + NL();

                    str += "if (!IsUpdate)" + NL();
                    str += "{" + NL();
                    str += "   HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);" + NL();
                    str += "    return response;" + NL();
                    str += "}" + NL();

                    str += "if (MemoryObserver.IsexceededMemory())" + NL();
                    str += "{" + NL();
                    str += "LogManager.WriteError(\"Memory has exceeded\");" + NL();
                    str += "var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
                    str += "{" + NL();
                    str += "    Content = new StringContent(\"Memory has exceeded\")" + NL();
                    str += "};" + NL();
                    str += "return resp;" + NL();
                    str += "}" + NL();
                }
                //str += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //str += "{" + NL();
                str += "    var data = " + key + "Manager.PostList" + key + "(item);" + NL();

                if (IsReplicate || IsCluster)
                {

                    str += "    Task.Factory.StartNew(() => PostListToSlave(item));" + NL();
                }

                str += "    return Request.CreateResponse(data);" + NL();

                //str += "});" + NL();
            }
            str += "}" + NL();


            return str;
        }


        public static bool IsPrimaryKey(List<Table> lstTables)
        {
            List<Table> lstPrimary = lstTables.FindAll(x => x.IsPrimaryKey == 1);
            return lstPrimary != null && lstPrimary.Count > 0;
        }
        public static string CreateUpdateManager(Dictionary<string, EntityItem> dic, string key, string dbType, CheckedItems checkedItems)
        {
            bool IsFile = dic[key].DbType == DbTypes.File;
            string str = "";
            List<Table> lstPrimary = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);

            if (lstPrimary.Count > 0 && checkedItems.IsReadOnly)
            {
                str += CreateUpdatetSql(dic, key);
                str += "public static HttpResponseMessage Put" + key + "(" + key + " item)" + NL();
                str += "{" + NL();//start func
                str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
                str += "try" + NL();
                str += "{" + NL();//start try
           
                str += "Task.Factory.StartNew(() => { UpdateSql(item); });" + NL();
                        
                str += "}" + NL();//end try
                str += "catch (Exception e)" + NL();
                str += "{" + NL();
                str += "    status = HttpStatusCode.BadRequest;" + NL();
                str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Put failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
                str += "}" + NL();
                str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
                str += "return response;" + NL();
                str += "}" + NL(); //end func
            }

            return str;
        }

        public static string CreateUpdateManagerNoPrimaryKeyNoReadOnly(Dictionary<string, EntityItem> dic, string key, string dbType)
        {
            bool IsFile = dic[key].DbType == DbTypes.File;
            string str = "public static HttpResponseMessage Put" + key + "(string id," + key + " item)" + NL();
            str += "{" + NL();//start func
            str += "bool IsMatch = false;" + NL();
            str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
            str += "try" + NL();
            str += "{" + NL();//start try
            str += "if (isInMemory)" + NL();
            str += "{" + NL(); //start isInMemory               

            str += "switch (id)" + NL();
            str += "    {" + NL();
            foreach (Table t in dic[key].lstTables)
            {
                str += "        case \"" + t.Name + "\":" + NL();
                str += "            {" + NL();
                string nullStr = t.type == "string" ? "x." + t.Name + " != null && " : "";
                str += "                var match = " + key + "List.Where(x => " + nullStr + "x." + t.Name + ".Equals(item." + t.Name + "));" + NL();
                str += "                if (match != null)" + NL();
                str += "                {" + NL();
                str += "                    foreach (var m in match)" + NL();
                str += "                    {" + NL();
                foreach (Table innerTable in dic[key].lstTables)
                {
                    if (innerTable.Name != t.Name)
                    {
                        str += "if(item." + innerTable.Name + "!=null)" + NL();
                        str += "m." + innerTable.Name + " = item." + innerTable.Name + ";" + NL();
                    }
                }
                str += "IsMatch = true;" + NL();
                str += "UpdateIsUpdate(true);" + NL();
                str += "                    }" + NL();
                str += "                }" + NL();
                str += " break;" + NL();
                str += "}" + NL();

            }


            str += "}" + NL();


            str += "}" + NL();//end isInMemory

            str += "}" + NL();//end try
            str += "catch (Exception e)" + NL();
            str += "{" + NL();
            str += "    status = HttpStatusCode.BadRequest;" + NL();
            str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Put failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
            str += "}" + NL();
            str += "if (!IsMatch)" + NL();
            str += "    status = HttpStatusCode.NotFound;" + NL();
            str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
            str += "return response;" + NL();
            str += "}" + NL(); //end func


            return str;
        }

        public static string CreateUpdateNoPrimaryKeyNoReadOnly(Dictionary<string, EntityItem> dic, string key, string dbType,
            bool IsReplicate, bool IsBalancer, bool IsReadOnly, bool IsSharding = false, bool IsCluster = false)
        {

            string str = "";
            foreach (Table t in dic[key].lstTables)
            {
                if (IsBalancer)
                {
                    str += "public HttpResponseMessage Put" + key + "By" + t.Name + "(" + key + " item)" + NL();
                    str += "{" + NL();
                    if (IsCluster)
                        str += "return ClusterManager.PutData(item,\"" + key + "/Put" + key + "By" + t.Name + "\");" + NL();
                    else
                        str += "return BalancerManager.PutData(item,\"" + key + "/Put" + key + "By" + t.Name + "\");" + NL();

                }
                else if (IsSharding)
                {
                    str += "public HttpResponseMessage Put" + key + "By" + t.Name + "(" + key + " item)" + NL();
                    str += "{" + NL();
                    str += "return BalancerManager.createSeveralPosts(item,\"" + key + "/Put" + key + "By" + t.Name + "\", \"PUT\");" + NL();
                }
                else
                {
                    str += "[HttpPut]" + NL();
                    str += "public HttpResponseMessage Put" + key + "By" + t.Name + "(" + key + " item)" + NL();
                    str += "{" + NL();
                    if (!IsReadOnly)
                    {
                        str += "if (item == null)" + NL();
                        str += "{" + NL();
                        str += "var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
                        str += "{" + NL();
                        str += "    Content = new StringContent(\"" + key + " parameter is null\")" + NL();
                        str += "};" + NL();
                        str += "return resp;" + NL();
                        str += "}" + NL();

                        str += "if (!IsUpdate)" + NL();
                        str += "{" + NL();
                        str += "   HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);" + NL();
                        str += "    return response;" + NL();
                        str += "}" + NL();
                    }
                    //str += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                    //str += "{" + NL();
                    str += "    var data = " + key + "Manager.Put" + key + "(\"" + t.Name + "\",item);" + NL();

                    if (IsReplicate || IsCluster)
                    {
                        str += "if (data.IsSuccessStatusCode)" + NL();
                        str += "    Task.Factory.StartNew(() => PutToSlave(item));" + NL();
                    }

                    str += "    return data;" + NL();

                    //str += "});" + NL();
                }
                str += "}" + NL();
            }
            return str;
        }

        public static string CreateUpdate(Dictionary<string, EntityItem> dic, string key, string dbType, bool IsReplicate,
            bool IsBalancer, bool IsReadOnly, bool IsReplicationBalancer = false, bool IsSharding = false, bool IsCluster = false)
        {

            string str = "";

            if (IsBalancer)
            {
                str += "public HttpResponseMessage Put" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();
                if (IsCluster)
                    str += "return ClusterManager.PutData(item,\"" + key + "/Put" + key + "\");" + NL();
                else
                    str += "return BalancerManager.PutData(item,\"" + key + "/Put" + key + "\");" + NL();

            }
            else if (IsSharding)
            {
                str += "public HttpResponseMessage Put" + key + "(" + key + " item)" + NL();
                str += "{" + NL();

                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();

                List<Table> foundPk = null;

                if (dic[key].lstTables != null)
                    foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);

                bool IsPkExist = foundPk != null && foundPk.Count > 0;
                if (IsPkExist)
                {
                    str += getPkCode(foundPk, key);
                    str += "if (url != null && url.Length <= 0)" + NL();
                    str += "{" + NL();
                    str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);" + NL();
                    str += "    resp.Content = new StringContent(\"item id not found\");" + NL();
                    str += "    return resp;" + NL();
                    str += "}" + NL();
                }
                str += "return BalancerManager.PutData2(item,url + \"/" + key + "/Put" + key + "\");" + NL();

            }
            else
            {
                str += "public HttpResponseMessage Put" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                if (!IsReadOnly)
                {
                    str += "if (!IsUpdate)" + NL();
                    str += "if (item == null)" + NL();
                    str += "{" + NL();
                    str += "var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)" + NL();
                    str += "{" + NL();
                    str += "    Content = new StringContent(\"" + key + " parameter is null\")" + NL();
                    str += "};" + NL();
                    str += "return resp;" + NL();
                    str += "}" + NL();
                    //str += "{" + NL();
                    //str += "   HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);" + NL();
                    //str += "    return response;" + NL();
                    //str += "}" + NL();
                }
                //str += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //str += "{" + NL();
                str += "    var data = " + key + "Manager.Put" + key + "(item);" + NL();

                if (IsReplicate || IsCluster)
                {
                    str += "if (data.IsSuccessStatusCode)" + NL();
                    str += "    Task.Factory.StartNew(() => PutToSlave(item));" + NL();
                }

                str += "    return data;" + NL();

                //str += "});" + NL();
            }
            str += "}" + NL();

            return str;
        }

        public static string CreateDeleteManager(Dictionary<string, EntityItem> dic, string key, string dbType, CheckedItems checkedItems)
        {
            List<Table> lstPrimary = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);
            string str = "";

            if (lstPrimary.Count > 0 && !checkedItems.IsReadOnly)
            {
                str += CreateDeletetSql(dic, key);


                str += "public static HttpResponseMessage Delete" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
                str += "try" + NL();
                str += "{" + NL();

                str += "Task.Factory.StartNew(() => { DeleteSql(item); });" + NL();

                str += "}" + NL();
                str += "catch (Exception e)" + NL();
                str += "{" + NL();
                str += "    status = HttpStatusCode.BadRequest;" + NL();
                str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"delete failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
                str += "}" + NL();
                str += "HttpResponseMessage response2 = new HttpResponseMessage(status);" + NL();
                str += "return response2;" + NL();
                str += "}" + NL();
            }
            return str;
        }

        public static string CreateDeleteManagerNoPrimaryKeyNoReadOnly(Dictionary<string, EntityItem> dic, string key, string dbType)
        {
            bool IsFile = dic[key].DbType == DbTypes.File;
            string str = "public static HttpResponseMessage Delete" + key + "(string id," + key + " item)" + NL();
            str += "{" + NL();//start func
            str += "HttpStatusCode status = HttpStatusCode.Created;" + NL();
            str += "bool IsMatch = false;" + NL();
            str += "try" + NL();
            str += "{" + NL();//start try
            str += "if (isInMemory)" + NL();
            str += "{" + NL(); //start isInMemory               

            str += "switch (id)" + NL();
            str += "    {" + NL();
            foreach (Table t in dic[key].lstTables)
            {
                str += "        case \"" + t.Name + "\":" + NL();
                str += "            {" + NL();
                str += "                var match = " + key + "List.Where(x => x." + t.Name + ".Equals(item." + t.Name + "));" + NL();
                str += "                if (match != null)" + NL();
                str += "                {" + NL();
                str += "                    foreach (var m in match)" + NL();
                str += "                    {" + NL();
                foreach (Table innerTable in dic[key].lstTables)
                {
                    if (innerTable.type != "bool")
                        str += "m." + innerTable.Name + " = null;" + NL();
                }
                str += "                    }" + NL();
                str += "UpdateIsUpdate(true);" + NL();
                str += "IsMatch = true;" + NL();
                str += "                }" + NL();
                str += "                break;" + NL();
                str += "            }" + NL();
            }


            str += "}" + NL();


            str += "}" + NL();//end isInMemory

            str += "}" + NL();//end try
            str += "catch (Exception e)" + NL();
            str += "{" + NL();
            str += "    status = HttpStatusCode.BadRequest;" + NL();
            str += "Task.Factory.StartNew(() => { LogManager.WriteError(\"Put failed: object " + key + " fail: Error - \" + e.Message); });" + NL();
            str += "}" + NL();

            str += "if (!IsMatch)" + NL();
            str += "    status = HttpStatusCode.NotFound;" + NL();

            str += "HttpResponseMessage response = new HttpResponseMessage(status);" + NL();
            str += "return response;" + NL();
            str += "}" + NL(); //end func


            return str;
        }

        public static string CreateDelete(Dictionary<string, EntityItem> dic, string key, string dbType, bool IsReplicate,
            bool IsBalancer, bool IsReadOnly, bool IsReplicationBalancer = false, bool IsSharding = false, bool IsCluster = false)
        {
            string str = "";

            if (IsBalancer)
            {
                str += "public HttpResponseMessage Delete" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                //if(IsReplicationBalancer)
                //    str += "return MasterServer.DeleteData(item,\"" + key + "\");" + NL();
                //else
                if (IsCluster)
                    str += "return ClusterManager.DeleteData(item,\"" + key + "\");" + NL();
                else
                    str += "return BalancerManager.DeleteData(item,\"" + key + "\");" + NL();
            }
            else if (IsSharding)
            {
                str += "public HttpResponseMessage Delete" + key + "(" + key + " item)" + NL();
                str += "{" + NL();

                str += "if (item == null)" + NL();
                str += "{" + NL();
                str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.BadRequest);" + NL();
                str += "    resp.Content = new StringContent(\"item is null\");" + NL();
                str += "    return resp;" + NL();
                str += "}" + NL();

                List<Table> foundPk = null;

                if (dic[key].lstTables != null)
                    foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);

                bool IsPkExist = foundPk != null && foundPk.Count > 0;
                if (IsPkExist)
                {
                    str += getPkCode(foundPk, key);
                    str += "if (url != null && url.Length <= 0)" + NL();
                    str += "{" + NL();
                    str += "    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);" + NL();
                    str += "    resp.Content = new StringContent(\"item id not found\");" + NL();
                    str += "    return resp;" + NL();
                    str += "}" + NL();
                }
                str += "return BalancerManager.DeleteData2(item,\"/" + key + "/Delete" + key + "\");" + NL();

            }
            else
            {
                str += "public HttpResponseMessage Delete" + key + "(" + key + " item)" + NL();
                str += "{" + NL();
                if (!IsReadOnly)
                {
                    str += "if (!IsUpdate)" + NL();
                    str += "{" + NL();
                    str += "   HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);" + NL();
                    str += "    return response;" + NL();
                    str += "}" + NL();
                }
                //str += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                //str += "{" + NL();
                str += "    var data = " + key + "Manager.Delete" + key + "(item);" + NL();

                if (IsReplicate || IsCluster)
                {
                    str += "if (data.IsSuccessStatusCode)" + NL();
                    str += "    Task.Factory.StartNew(() => DeletFromSlave(item));" + NL();
                }

                str += "    return data;" + NL();

                //str += "});" + NL();

            }
            str += "}" + NL();
            return str;
        }

        public static string CreateDeleteNoPrimaryKeyNoReadOnly(Dictionary<string, EntityItem> dic, string key, string dbType,
            bool IsReplicate, bool IsBalancer, bool IsReadOnly, bool IsSharding = false, bool IsCluster = false)
        {
            string str = "";
            foreach (Table t in dic[key].lstTables)
            {
                if (IsBalancer)
                {
                    str += "public HttpResponseMessage Delete" + key + "By" + t.Name + "(" + key + " item)" + NL();
                    str += "{" + NL();
                    if (IsCluster)
                        str += "return ClusterManager.DeleteData(item,\"" + key + "By" + t.Name + "\");" + NL();
                    else
                        str += "return BalancerManager.DeleteData(item,\"" + key + "By" + t.Name + "\");" + NL();

                }
                else if (IsSharding)
                {
                    str += "public HttpResponseMessage Delete" + key + "By" + t.Name + "(" + key + " item)" + NL();
                    str += "{" + NL();
                    str += "return BalancerManager.createSeveralPosts(item,\"" + key + "/Delete" + key + "By" + t.Name + "\", \"DELETE\");" + NL();
                }
                else
                {
                    str += "public HttpResponseMessage Delete" + key + "By" + t.Name + "(" + key + " item)" + NL();
                    str += "{" + NL();
                    if (!IsReadOnly)
                    {
                        str += "if (!IsUpdate)" + NL();
                        str += "{" + NL();
                        str += "   HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Forbidden);" + NL();
                        str += "    return response;" + NL();
                        str += "}" + NL();
                    }
                    //str += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                    //str += "{" + NL();
                    str += "    var data = " + key + "Manager.Delete" + key + "(\"" + t.Name + "\",item);" + NL();

                    if (IsReplicate)
                    {
                        str += "if (data.IsSuccessStatusCode)" + NL();
                        str += "    Task.Factory.StartNew(() => DeletFromSlave(item));" + NL();
                    }

                    str += "    return data;" + NL();

                    //str += "});" + NL();

                }
                str += "}" + NL();
            }
            return str;
        }

        public static string CreateWebFarmInsert(string ServerId, string EntityName)
        {

            string source = "public async Task<HttpResponseMessage> Post" + EntityName + "(object" + " item)" + NL();

            source += "return await UrlHandler.PostData(" + ServerId + ",\"" + EntityName + "/Post" + EntityName + "\",item);" + NL();
            return source;
        }

        public static string CreateWebFarmUpdate(string ServerId, string EntityName)
        {

            string source = "public async Task<HttpResponseMessage> Put" + EntityName + "(object" + " item)" + NL();

            source += "return await UrlHandler.PostData(" + ServerId + ",\"" + EntityName + "/Put" + EntityName + "\",item);" + NL();
            return source;
        }

        public static string CreateWebFarmDelete(string ServerId, string EntityName)
        {

            string source = "public async Task<HttpResponseMessage> Delete" + EntityName + "(object" + " item)" + NL();

            source += "return await UrlHandler.PostData(" + ServerId + ",\"" + EntityName + "/Delete" + EntityName + "\",item);" + NL();
            return source;
        }
    }
}
