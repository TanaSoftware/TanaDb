using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImFast.Dal;
using System.IO;
using ImFast.Utils;
using System.Windows.Forms;
using ImFast.BL;
using ImFast.Servers;
using Newtonsoft.Json;


namespace ImFast.CodeCreator
{
    class CodeManager : Base
    {
        public static bool IsOpenSource;
        //public static int AppType; //1 = web app, 2 = self host , 3 = window service
        //public static int AppDateFormatType = 0; //0 = d-m-yyy, 1 = m-d-yyyy 2 = yyyy-m-d
        public static string portNum;


        //static Dictionary<string, DbTypeItem> dicDbTypes = new Dictionary<string, DbTypeItem>();

        private static Dictionary<string, string> dicConStr = new Dictionary<string, string>();
        private static Dictionary<string, StoredProcedure> dicStored = new Dictionary<string, StoredProcedure>();

        private static List<string> CodeList = new List<string>();//keep all code to compile to exe selef host
        private static string projectName;


        private static void CreateWebApiMainServer(string path, string key, string source)
        {
            WriteFileInAppCode(path, key + "Controller", source);
        }

        private static void CreareBalancerSourceCode(string path, Dictionary<string, EntityItem> dic, CheckedItems checkedItems, Dictionary<string, StoredProcedure> dicStoredProc = null)
        {
            CreateDateFormatterClass(path, checkedItems.AppDateFormatType);
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                CreateDataEntityFile(i, path, dic);//create entity class for table
                CreatewebApiFile(i, path, dic, checkedItems);

                CopyMyDbFile(path, dic[key].DataSource);
            }
            if (dicStoredProc != null && dicStoredProc.Count > 0)
            {
                CreateSpManager();
                CreateSpController(dic, dicStoredProc, checkedItems);
            }
            if (checkedItems.AppType == 1)
                CreateGlobalAsax(path, checkedItems.InitDataFromGlobalAsax, true, false, checkedItems);

            CreateWebConfig(path, checkedItems);
            CopyDlls(path);
            CopyCs(path, checkedItems);
            CopyProjectJsonFile(path);
            CopyPages(path, checkedItems);
        }
        private static void CreateDataEntityFile(int i, string path, Dictionary<string, EntityItem> dic)
        {
            string key = dic.ElementAt(i).Key;
            string strSource = CreateDataEntityFileString(key, dic);

            WriteFileInAppCode(path, key, strSource);
        }

        public static string CreateDataEntityFileString(string key, Dictionary<string, EntityItem> dic, bool isCheckDate = true, bool isUseValidationRegex = true)
        {
            bool IsDate = false;
            bool IsString = false;
            string source = "";
            string type = "";
            string usingValidations = "";
            string usingStr = "using System;" + NL();
            usingStr += "using System.ComponentModel.DataAnnotations;";
            if (dic[key].lstTables == null)
                return "";

            string Nullablae = "";
            source += "namespace " + dic[key].DbType + "." + dic[key].DbName + NL() + "{" + NL();
            source += "public class " + key + " {" + NL();//start data class

            if (dic[key].DbName == DbEntityTypes.keyVal)
            {
                type = GetTypeByColumn(dic[key].lstTables[0].type);
                source += "[Required]" + NL();
                source += " public " + type + " " + dic[key].lstTables[0].Name + " { get; set; } " + NL();
                source += " public string value { get; set; }" + NL();
            }
            else
            {
                for (int j = 0; j < dic[key].lstTables.Count; j++)
                {
                    type = GetTypeByColumn(dic[key].lstTables[j].type);
                    IsString = false;
                    if (isCheckDate && type == "DateTime")
                    {
                        IsDate = true;
                        source += "[JsonConverter(typeof(CustomDateTimeConverter2))]" + NL();
                    }
                    else if (type == "string")
                    {
                        //source += "string _" + dic[key].lstTables[j].Name + " = \"\";" + NL();
                        IsString = true;
                        char c = (char)3;
                        //if (isUseValidationRegex)
                        //  source += "[RegularExpression(\"^[^" + c + "]*$\", ErrorMessage=\"Invalid character\")] " + NL();
                    }


                    Nullablae = IsInsertQuestionMark(type) ? "? " : " ";

                    if (dic[key].lstTables[j].IsPrimaryKey > 0)
                        source += "[Required]" + NL();

                    source += " public " + type + Nullablae + dic[key].lstTables[j].Name + " { get; set; } " + NL();
                }
            }
            source += " }";//end class
            source += " }";//end namspace

            if (isCheckDate && IsDate)
                usingStr += "using Newtonsoft.Json; using Newtonsoft.Json.Converters;";

            if (IsString)
                usingStr += usingValidations;

            string strSource = usingStr + source;

            return strSource;
        }

        private static void CreateSpManager()
        {

            string source = "";
            source += "using Dapper;using ImFast.Dal;using System;using System.Collections.Generic;using System.Linq;" +
                "using System.Web;using System.Collections.Concurrent;using System.Reflection;using System.Threading.Tasks;" + NL();
            source += "namespace Entities.table{" + NL();
            source += "public class KeySPManager" + NL();
            source += "{" + NL();
            source += "private static ConcurrentDictionary<string, KeyObjSP> AllSP = new ConcurrentDictionary<string, KeyObjSP>();" + NL();
            source += "private static ConcurrentDictionary<string, uint> AllSPCounter = new ConcurrentDictionary<string, uint>();" + NL();
            source += "private static void UpdateAllSPCounter(string key)" + NL();
            source += "{" + NL();
            source += "CheckMemExceeded();" + NL();
            source += "uint iCounter = 0;" + NL();
            source += "    if (!AllSPCounter.TryGetValue(key, out iCounter))" + NL();
            source += "    {" + NL();
            source += "        if (iCounter == 0)" + NL();
            source += "        iCounter = 1;" + NL();
            source += "        AllSPCounter.TryAdd(key, iCounter);" + NL();
            source += "    }" + NL();
            source += "    else" + NL();
            source += "       AllSPCounter.TryUpdate(key, iCounter + 1, iCounter);" + NL();
            source += "}" + NL();

            source += "private static void CheckMemExceeded()" + NL();
            source += "{" + NL();
            source += "    KeyObjSP keyObj = null;" + NL();
            source += "    int ind = 0;" + NL();
            source += "    int iCounter=5;" + NL();
            source += "    uint iOut = 0;" + NL();
            source += "    if (MemoryObserver.IsexceededMemory())" + NL();
            source += "    {" + NL();
            source += "        IEnumerable<KeyValuePair<string,uint>> data = AllSPCounter.Where(z => z.Value >= 1).OrderBy(a=>a.Value);" + NL();
            source += "        foreach (KeyValuePair<string,uint> d in data)" + NL();
            source += "        {" + NL();
            source += "            if (ind > iCounter)" + NL();
            source += "            {" + NL();
            source += "                if (!MemoryObserver.IsexceededMemory())" + NL();
            source += "                {" + NL();
            source += "                    break;" + NL();
            source += "                }" + NL();
            source += "                iCounter += 5;" + NL();
            source += "            }" + NL();
            source += "            if (AllSP.TryGetValue(d.Key, out keyObj))" + NL();
            source += "            {" + NL();
            source += "                AllSP.TryRemove(d.Key, out keyObj);" + NL();
            source += "                AllSPCounter.TryRemove(d.Key, out iOut);" + NL();
            source += "            }" + NL();
            source += "            ind++;    " + NL();
            source += "        }       " + NL();
            source += "    }" + NL();
            source += "}" + NL();

            source += "public static object Get(string key)" + NL();
            source += "{" + NL();
            source += "    KeyObjSP keyObj = null;" + NL();
            source += "Task.Factory.StartNew(() => UpdateAllSPCounter(key));" + NL();
            source += "    if (!AllSP.TryGetValue(key, out keyObj))" + NL();
            source += "        return null;" + NL();
            source += "    TimeSpan span = DateTime.Now - keyObj.start;" + NL();
            source += "    if (span.Minutes > keyObj.time)" + NL();
            source += "    {" + NL();
            source += "        AllSP.TryRemove(key, out keyObj);" + NL();
            source += "        return null;" + NL();
            source += "    }" + NL();
            source += "    return keyObj.value;" + NL();
            source += "}" + NL();
            source += "public static bool Del(string key)" + NL();
            source += "{" + NL();
            source += "KeyObjSP keyObj = null;" + NL();
            source += "return AllSP.TryRemove(key, out keyObj);" + NL();
            source += "}" + NL();
            source += "public static bool Set(string key, object value, int time)" + NL();
            source += "{" + NL();
            source += "    KeyObjSP keyObj = new KeyObjSP();" + NL();
            source += "    keyObj.time = time;" + NL();
            source += "    keyObj.value = value;" + NL();
            source += "    keyObj.start = DateTime.Now;" + NL();
            source += "    KeyObjSP BalData = null;" + NL();
            source += "    AllSP.TryGetValue(key, out BalData);" + NL();
            source += "    if (AllSP.ContainsKey(key))" + NL();
            source += "        return AllSP.TryUpdate(key, keyObj, BalData);" + NL();
            source += "    return AllSP.TryAdd(key, keyObj);" + NL();
            source += "}" + NL();
            source += "}" + NL();

            source += "public class KeyObjSP" + NL();
            source += "{" + NL();
            source += "    public DateTime start { get; set; }" + NL();
            source += "   public int time { get; set; }" + NL();
            source += " public object value { get; set; }" + NL();
            source += "}" + NL();
            source += "public class SpManager{" + NL();

            source += "public static bool IsAnyNullOrEmpty(object myObject)" + NL();
            source += "{" + NL();
            source += "Type myType = myObject.GetType();" + NL();
            source += "IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());" + NL();
            source += "foreach (PropertyInfo prop in props)" + NL();
            source += "{" + NL();
            source += "    object propValue = prop.GetValue(myObject, null);" + NL();
            source += "    if ((int)propValue == 0)" + NL();
            source += "        return false;" + NL();
            source += "    return true;" + NL();
            source += "}" + NL();
            source += "return false;" + NL();
            source += "}" + NL();

            source += "public static object callSP(string spName,string key, DynamicParameters p)" + NL();
            source += "  {" + NL();
            source += "    Dictionary<string, EntityItem> dicEntities = MyEntitiesController.GetdicEntities();" + NL();
            source += "    string sConn = dicEntities[key].DataBaseConnectionString;" + NL();
            source += "    DataBaseRetriever dal = new DataBaseRetriever(sConn);" + NL();
            source += "    return dal.ExecuteStoredProcedure(spName, p, dicEntities[key].DataBaseTypeName);" + NL();
            source += "  }" + NL();
            source += " }" + NL();
            source += "}" + NL();

            WriteFileInAppCode("", "", source);
        }

        private static void CreateSpController(Dictionary<string, EntityItem> dic, Dictionary<string, StoredProcedure> dicStoredProc, CheckedItems checkedItems)
        {

            string source = "";
            source += "using Dapper;using System;using System.Collections.Generic;using System.Data;using System.Linq;using System.Web;using System.Web.Http;using System.Threading.Tasks;";
            source += "namespace Entities.table{" + NL();

            //create classes from parametrs for post
            for (int i = 0; i < dicStoredProc.Count; i++)
            {
                StoredProcedure sp = dicStoredProc.ElementAt(i).Value;
                if (sp.ParamaterNames.Count > 0)
                {
                    string key = dicStoredProc.ElementAt(i).Key;
                    source += "public class " + key + "obj";
                    source += "{" + NL();

                    int j = 0;
                    foreach (string sName in sp.ParamaterNames)
                    {
                        string n = sName.Replace("@", "");

                        source += "public " + GetTypeByColumn(sp.ParamaterTypes[j]) + " " + n + " { get; set; } " + NL();
                        j++;
                    }
                    source += "}" + NL();
                }
            }
            source += "public class SPController : ApiController{" + NL();

            //source += FunctionCreator.CreateCheckClearSpCache();

            for (int i = 0; i < dicStoredProc.Count; i++)
            {
                string key = dicStoredProc.ElementAt(i).Key;
                StoredProcedure sp = dicStoredProc.ElementAt(i).Value;

                int paramsCount = sp.ParamaterNames.Count;
                string plus = paramsCount > 0 ? " + " : "";
                if (paramsCount < 7)
                {
                    source += "[ActionName(\"" + key + "\")]" + NL();
                    source += "public object get" + key + "(" + GetFuncParams(paramsCount, sp.ParamaterTypes) + ")" + NL();
                    source += "{" + NL();
                    if (checkedItems.IsBalancer)
                    {
                        source += "return BalancerManager.RetrieveData(\"sp/" + key + GetCallParmatersForBalancer(paramsCount) + "); " + NL();
                    }
                    else
                    {
                        source += "    try" + NL();
                        source += "            {" + NL();
                        //source += " bool isDb = IsDb();" + NL();
                        string KeyParams = "\"SP_" + key + "\"" + plus + GetFuncParamsKeys(paramsCount, sp.ParamaterTypes);
                        //source += "CheckClearCahe(" + KeyParams + "); " + NL();
                        if (KeyParams == "")
                            KeyParams = "\"" + key + "\"";

                        source += "object obj = null; " + NL();
                        source += "if (obj == null)" + NL();
                        source += "{" + NL();
                        source += "var p = new DynamicParameters();" + NL();

                        string[] arrP = GetFuncParamsVal(paramsCount);
                        int j = 0;
                        bool isOutPut = false;
                        foreach (string sName in sp.ParamaterNames)
                        {
                            string outPutParam = "";
                            if (sp.ParamaterOutput!=null && sp.ParamaterOutput[j].ToUpper() == "OUT")
                            {
                                isOutPut = true;
                                outPutParam = ",direction: ParameterDirection.Output";
                            }
                            source += "p.Add(\"" + sName + "\", " + arrP[j] + outPutParam + "); " + NL();
                            j++;
                        }

                        source += "obj = SpManager.callSP(\"" + key + "\", \"" + key + "\", p);" + NL();

                        if (isOutPut)
                        {
                            string returnedObj = "";
                            int z = 0;
                            int k = 1;

                            foreach (string sName in sp.ParamaterNames)
                            {
                                if (sp.ParamaterOutput[z] != "")
                                {
                                    isOutPut = true;
                                    source += "object obj" + k.ToString() + " = p.Get<" + sp.ParamaterTypes[z] + ">(\"" + sName + "\");" + NL();
                                    returnedObj += sName + "=obj" + k.ToString() + ",";
                                    k++;
                                }
                                z++;
                            }

                            if (returnedObj.Length > 0)
                            {
                                returnedObj = returnedObj.Substring(0, returnedObj.Length - 1);
                                source += "obj = new { " + returnedObj + "};" + NL();
                            }
                        }

                        //source += "if (SpManager.IsAnyNullOrEmpty(obj))" + NL();
                        //source += "   KeySPManager.Set(" + KeyParams + ", obj, " + cacheTimeInMinutes + "); " + NL();
                        source += "}" + NL();
                        source += "return obj;" + NL();
                        source += "}" + NL();
                        source += "catch(Exception e)" + NL();
                        source += " {" + NL();
                        source += "Task.Factory.StartNew(() => { LogManager.WriteError(\"funcName : get" + key + " , Error - \" + e.Message); });" + NL();
                        source += "    return null;" + NL();
                        source += " }" + NL();

                    }
                    source += "}" + NL();
                }

                if (sp.ParamaterNames != null && sp.ParamaterNames.Count > 0)
                {

                    source += "public " + key + "obj get" + key + "obj()" + NL();
                    source += "{" + NL();
                    source += "    return new " + key + "obj();" + NL();
                    source += "}" + NL();

                    source += "[HttpPost]" + NL();
                    source += "[ActionName(\"" + key + "\")]" + NL();
                    source += "public object post" + key + "(" + key + "obj objX)" + NL();
                    source += "{" + NL();
                    if (checkedItems.IsBalancer)
                    {
                        source += "return BalancerManager.PostData(objX,\"sp/post" + key + "\"); " + NL();
                    }
                    else
                    {

                        source += "    try" + NL();
                        source += "    {" + NL();
                        //source += "bool isDb = IsDb();" + NL();
                        string KeyParams2 = "";
                        foreach (string sName in sp.ParamaterNames)
                        {
                            string n = sName.Replace("@", "");
                            KeyParams2 += "objX." + n + ".ToString()+";
                        }
                        if (KeyParams2.Length > 0)
                            KeyParams2 = KeyParams2.Substring(0, KeyParams2.Length - 1);

                        string keyPar = "\"SP_" + key + "\"" + plus + KeyParams2;

                        //source += "CheckClearCahe(" + keyPar + "); " + NL();
                        source += "object obj = null; " + NL();
                        source += "if (obj == null)" + NL();
                        source += "{" + NL();
                        source += "var p = new DynamicParameters();" + NL();

                        bool isOutPut = false;
                        int j = 0;
                        foreach (string sName in sp.ParamaterNames)
                        {
                            string n = sName.Replace("@", "");

                            string outPutParam = "";
                            if (sp.ParamaterOutput!=null && sp.ParamaterOutput[j].ToUpper() == "OUT")
                            {
                                isOutPut = true;
                                outPutParam = ",direction: ParameterDirection.Output";
                            }
                            source += "p.Add(\"" + sName + "\", objX." + n + outPutParam + "); " + NL();
                            j++;
                        }

                        source += " obj = SpManager.callSP(\"" + key + "\", \"" + key + "\", p);" + NL();

                        if (isOutPut)
                        {
                            string returnedObj = "";
                            int z = 0;
                            int k = 1;

                            foreach (string sName in sp.ParamaterNames)
                            {
                                if (sp.ParamaterOutput[z] != "")
                                {
                                    isOutPut = true;
                                    source += "object obj" + k.ToString() + " = p.Get<" + sp.ParamaterTypes[z] + ">(\"" + sName + "\");" + NL();
                                    returnedObj += sName + "=obj" + k.ToString() + ",";
                                    k++;
                                }
                                z++;
                            }

                            if (returnedObj.Length > 0)
                            {
                                returnedObj = returnedObj.Substring(0, returnedObj.Length - 1);
                                source += "obj = new { " + returnedObj + "};" + NL();
                            }
                        }

                        //source += "if (SpManager.IsAnyNullOrEmpty(obj))" + NL();
                        //source += "    KeySPManager.Set(" + keyPar + ", obj, " + cacheTimeInMinutes + "); " + NL();
                        source += "}" + NL();
                        source += "return obj;" + NL();
                        source += "   }" + NL();
                        source += "catch(Exception e)" + NL();
                        source += " {" + NL();
                        source += "    Task.Factory.StartNew(() => { LogManager.WriteError(\"funcName : get" + key + " , Error - \" + e.Message); });" + NL();
                        source += "    return null;" + NL();
                        source += "}" + NL();
                    }
                    source += "}" + NL();
                }


            }
            source += " }" + NL();
            source += "}" + NL();
            WriteFileInAppCode("", "", source);
        }

        private static string GetFuncParamsKeys(int paramLen, List<string> types)
        {
            if (paramLen == 1)
                return "id.ToString()";
            if (paramLen == 2)
                return "GT.ToString()+id.ToString()";
            if (paramLen == 3)
                return "MAX.ToString()+GT.ToString()+id.ToString()";

            if (paramLen == 4)
                return "GT.ToString()+id.ToString()+order.ToString()+Asc.ToString()";

            if (paramLen == 5)
                return "id.ToString()+order.ToString()+Asc.ToString()+paging.ToString()+size.ToString()";

            if (paramLen == 6)
                return "GT.ToString()+id.ToString()+order.ToString()+Asc.ToString()+paging.ToString()+size.ToString()";
            return "";
        }
        private static string GetFuncParams(int paramLen, List<string> types)
        {
            if (paramLen == 1)
                return GetTypeByColumn(types[0]) + " id";
            if (paramLen == 2)
                return GetTypeByColumn(types[0]) + " GT," + GetTypeByColumn(types[1]) + " id";
            if (paramLen == 3)
                return GetTypeByColumn(types[0]) + " MAX," + GetTypeByColumn(types[1]) + " GT," + GetTypeByColumn(types[2]) + " id";

            if (paramLen == 4)
                return GetTypeByColumn(types[0]) + " GT," + GetTypeByColumn(types[1]) + " id," + GetTypeByColumn(types[2]) + " order," + GetTypeByColumn(types[3]) + " Asc";

            if (paramLen == 5)
                return GetTypeByColumn(types[0]) + " id, " + GetTypeByColumn(types[1]) + " order, " + GetTypeByColumn(types[2]) + " Asc, " + GetTypeByColumn(types[3]) + " paging, " + GetTypeByColumn(types[4]) + " size";

            if (paramLen == 6)
                return GetTypeByColumn(types[0]) + " GT," + GetTypeByColumn(types[1]) + " id, " + GetTypeByColumn(types[2]) + " order, " + GetTypeByColumn(types[3]) + " Asc, " + GetTypeByColumn(types[4]) + " paging, " + GetTypeByColumn(types[5]) + " size";
            return "";
        }

        private static string[] GetFuncParamsVal(int paramLen)
        {
            string[] arr = new string[paramLen];
            if (paramLen == 1)
            {
                arr[0] = "id";
            }
            if (paramLen == 2)
            {
                arr[0] = "GT";
                arr[1] = "id";
            }
            if (paramLen == 3)
            {
                arr[0] = "MAX";
                arr[1] = "GT";
                arr[2] = "id";
            }
            if (paramLen == 4)
            {
                arr[0] = "GT";
                arr[1] = "id";
                arr[2] = "order";
                arr[3] = "Asc";

            }
            if (paramLen == 5)
            {
                arr[0] = "id";
                arr[1] = "order";
                arr[2] = "Asc";
                arr[3] = "paging";
                arr[4] = "size";
            }
            if (paramLen == 6)
            {
                arr[0] = "GT";
                arr[1] = "id";
                arr[2] = "order";
                arr[3] = "Asc";
                arr[4] = "paging";
                arr[5] = "size";
            }
            return arr;
        }
        private static void CreareSourceCode(string path, Dictionary<string, EntityItem> dic, CheckedItems checkedItems,
            Dictionary<string, Dictionary<string, string>> dicChanges = null, Dictionary<string, StoredProcedure> dicStoredProc = null)
        {
            //FunctionCreator.ExtraSource = "";
            //FunctionCreator.hashFuncNameList = new HashSet<string>();
            CreateDateFormatterClass(path, checkedItems.AppDateFormatType);

            if (dicStoredProc != null && dicStoredProc.Count > 0)
            {
                CreateSpManager();
                CreateSpController(dic, dicStoredProc, checkedItems);
            }
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (dic[key].IsProduce)
                {
                    CreateDataEntityFile(i, path, dic);//create entity class for table

                    CreatewebApiFileManager(i, path, dic, checkedItems);
                    CreatewebApiFile(i, path, dic, checkedItems);

                    if (dic[key].DbType == DbTypes.File)
                    {
                        if (!checkedItems.IsReadOnly && dicChanges != null && dicChanges.Count > 0)
                            ServerManager.ChangeDatabaseFileInRemoteServer(path, dic, dicChanges);
                        else
                            CopyMyDbFile(path, dic[key].DataSource);
                    }
                }
            }

            //CreateInitCs(path, dic, checkedItems);

            if (checkedItems.AppType == 1)
                CreateGlobalAsax(path, checkedItems.InitDataFromGlobalAsax, true, false, checkedItems);

            CreateWebConfig(path, checkedItems);

            CopyDlls(path);
            CopyCs(path, checkedItems);
            CopyProjectJsonFile(path);
            CopyPages(path, checkedItems);
        }

        private static void CopyMyDbFile(string DestinationPath, string fileName)
        {
            if (fileName == null)
                return;
            string f = fileName;
            string path = Common.GetPath();
            if (!Directory.Exists(DestinationPath + "App_Data"))
                Directory.CreateDirectory(DestinationPath + "App_Data");

            if (ServerManager.IsWorkLocal)
                File.Copy(path + "MyDb\\" + f, DestinationPath + "App_Data\\" + f, true);
            else
                File.Copy(path + "MyServerDb\\" + f, DestinationPath + "App_Data\\" + f, true);
        }

        private static void CopyProjectJsonFile(string DestinationPath)
        {
            string path = Common.GetPath();
            if (!Directory.Exists(DestinationPath + "App_Data"))
                Directory.CreateDirectory(DestinationPath + "App_Data");

            if (ServerManager.IsWorkLocal)
                File.Copy(path + "MyProjects\\" + projectName + Common.PreojectFileExtension(), DestinationPath + "App_Data\\jsonData" + Common.PreojectFileExtension(), true);
            else
            {
                ServerManager.SaveProjectFile(DestinationPath + "App_Data\\jsonData" + Common.PreojectFileExtension());
            }
        }

        private static void CopyProjectJsonServerData(string DestinationPath)
        {
            string path = Common.GetPath();
            if (!Directory.Exists(DestinationPath + "App_Data"))
                Directory.CreateDirectory(DestinationPath + "App_Data");
            File.Copy(path + "MyProjects\\" + projectName + "ServerData" + Common.PreojectFileExtension(), DestinationPath + "App_Data\\ServerData" + Common.PreojectFileExtension(), true);
        }

        private static Dictionary<string, List<Table>> ChangeDictionary(Dictionary<string, TableSp> dic)
        {
            Dictionary<string, List<Table>> dicChanged = new Dictionary<string, List<Table>>();
            for (int i = 0; i < dic.Count; i++)
            {
                List<Table> t = new List<Table>();
                TableSp tSp = dic.ElementAt(i).Value;
                for (int j = 0; j < tSp.lstTables.Count; j++)
                {
                    Table tab = new Table();
                    tab.Name = tSp.lstTables[j].Name;
                    tab.type = tSp.lstTables[j].type;
                    t.Add(tab);
                }
                dicChanged.Add(dic.ElementAt(i).Key, t);
            }
            return dicChanged;
        }

        private static void CopyPages(string DestinationPath, CheckedItems checkedItems)
        {
            string path = Common.GetPath();
            if (checkedItems.AppType == 1)//web pages
            {
                File.Copy(path + "MyPages\\MainWeb.html", DestinationPath + "\\Main.html", true);
                File.Copy(path + "MyPages\\ChartWeb.html", DestinationPath + "\\Chart.html", true);
                File.Copy(path + "MyPages\\MainWeb2.html", DestinationPath + "\\index.html", true);
            }
            else
            {
                File.Copy(path + "MyPages\\Main.html", DestinationPath + "\\Main.html", true);
                File.Copy(path + "MyPages\\Main2.html", DestinationPath + "\\index.html", true);
                File.Copy(path + "MyPages\\Chart.html", DestinationPath + "\\Chart.html", true);
            }

            Common.DirectoryCopy(path + "MyPages\\css", DestinationPath + "css", true);
            Common.DirectoryCopy(path + "MyPages\\js", DestinationPath + "js", true);
        }

        private static void CopyDlls(string DestinationPath)
        {
            string path = Common.GetPath();
            Common.DirectoryCopy(path + "MyDll", DestinationPath + "bin", false);
        }

        private static void CopyCs(string DestinationPath, CheckedItems checkedItems)
        {

            string path = Common.GetPath();
            if (IsOpenSource)
            {
                Common.DirectoryCopy(path + "MyCs", DestinationPath + "App_code\\Dal", false);
            }
            else
            {
                string[] arrFiles2 = Directory.GetFiles(path + "MyCs");
                string[] arrFiles = new string[arrFiles2.Length];

                SecureData sec = new SecureData();
                int i = 0;
                foreach (string f in arrFiles2)
                {
                    string s = File.ReadAllText(f);
                    arrFiles[i] = sec.DecryptText(s, getPass1() + getPass2() + getPass3());
                    i++;
                }

                if (checkedItems.IsBalancer || checkedItems.IsSharding)
                {

                    string[] arrFilesX = Directory.GetFiles(path + "MyCs");

                    if (checkedItems.IsReplicationBalancer || checkedItems.IsSharding)
                    {
                        string s1 = File.ReadAllText(path + "MyBal\\BMSBalancerManager.cs");
                        CodeList.Add(sec.DecryptText(s1, getPass1() + getPass2() + getPass3()));
                    }

                    foreach (string f in arrFilesX)
                    {

                        if (f.Contains("MyEntitiesManager.cs"))
                            continue;
                        if (checkedItems.IsReplicationBalancer && f.Contains("BalancerManager.cs"))
                            continue;

                        if (checkedItems.IsSharding && f.Contains("BalancerManager.cs"))
                            continue;

                        if (f.Contains("JoinManager") || f.Contains("Statistics") || f.Contains("BackupFactory"))
                            continue;

                        else if (f.Contains("MyEntitiesController"))
                        {
                            string s1 = File.ReadAllText(path + "MyWebFarmCs\\MyEntitiesController1.cs");
                            string s2 = File.ReadAllText(path + "MyWebFarmCs\\MyEntitiesController2.cs");
                            string dec1 = sec.DecryptText(s1, getPass1() + getPass2() + getPass3());
                            string dec2 = sec.DecryptText(s2, getPass1() + getPass2() + getPass3());
                            CodeList.Add(dec1 + dec2);
                        }
                        else if (f.Contains("MessageHandler"))
                        {
                            string s1 = File.ReadAllText(path + "MyWebFarmCs\\MessageHandler.cs");
                            CodeList.Add(sec.DecryptText(s1, getPass1() + getPass2() + getPass3()));
                        }
                        else
                        {
                            string s3 = File.ReadAllText(f);
                            CodeList.Add(sec.DecryptText(s3, getPass1() + getPass2() + getPass3()));
                        }
                    }
                }
                else
                {
                    foreach (string f in arrFiles)
                    {
                        //string s = File.ReadAllText(f);
                        CodeList.Add(f);
                    }
                }
            }
        }

        private static string getPass1()
        {
            return "WQA-shalomHa-ZXA";
        }
        private static string getPass2()
        {
            return "AZW-shalomHa-ADA";
        }
        private static string getPass3()
        {
            return "DCV-shalomHa-VFA";
        }

        private static void CopyWebFarmCs(string DestinationPath)
        {

        }

        private static void CreateInitCs(string path, Dictionary<string, EntityItem> dic, CheckedItems checkedItems)
        {
            string pathX = Common.GetPath();

            List<string> lst = new List<string>();
            List<string> lstDot = new List<string>();
            string source = "using System;" + NL() + "using System.Reflection;" + NL() + "using System.Collections;" + NL();

            source += "public class DataManager{" + NL();

            source += TAB() + "public static void CallFunc(string name)" + NL();
            source += TAB() + "{" + NL();
            //source += doubleTAB() + "Type type = typeof(DataManager);" + NL();
            //source += doubleTAB() + "MethodInfo info = type.GetMethod(name);" + NL();
            //source += doubleTAB() + "info.Invoke(null,null );" + NL();
            source += TAB() + "}" + NL();


            source += TAB() + "public static string SaveAll(){" + NL();
            source += TAB() + "string FailedStr = \"\";" + NL();
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (dic[key].IsProduce)
                    source += doubleTAB() + "bool Is" + key + " = " + dic[key].DbType + "." + dic[key].DbName + "." + key + "Manager.SaveToFile();" + NL();
            }

            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (dic[key].IsProduce)
                {
                    source += "if (!Is" + key + ")" + NL();
                    source += "{" + NL();
                    source += "FailedStr += \"Backup " + key + " data failed; \";" + NL();
                    source += "}" + NL();
                }
            }
            source += "return FailedStr;" + NL();
            source += "}";

            source += TAB() + "public static void CancelUpdate(){" + NL();
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (!dic[key].IsProduce)
                    continue;
                source += doubleTAB() + dic[key].DbType + "." + dic[key].DbName + "." + key + "Controller.GetIsUpdate(0);" + NL();
            }
            source += "}";

            source += TAB() + "public static void SetUpdate(){" + NL();
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (!dic[key].IsProduce)
                    continue;
                source += doubleTAB() + dic[key].DbType + "." + dic[key].DbName + "." + key + "Controller.GetIsUpdate(1);" + NL();
            }
            source += "}";

            source += TAB() + "public static void Init(){" + NL();

            source += TAB() + "ServerManager.GetAndSetServerTypeFromFile();" + NL();
            //source += TAB() + "if (ServerManager.IsSlave())" + NL();
            if (checkedItems.IsCluster)
                source += TAB() + "    ClusterMaster.CallMaster();" + NL();
            else
                source += TAB() + "    ServerManager.CallMaster();" + NL();

            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;

                if (!dic[key].IsProduce)
                    continue;

                EntityTypes eType = dic[key].EntityType;

                source += doubleTAB() + dic[key].DbType + "." + dic[key].DbName + "." + key + "Controller.Init();" + NL();
                string keyLine = dic[key].DbType + "_" + dic[key].DbName + "_" + key;
                lst.Add(keyLine);
                lstDot.Add(dic[key].DbType + "." + dic[key].DbName + "." + key);

            }

            source += "}";

            for (int j = 0; j < lst.Count; j++)
            {
                string strTmp = TAB() + "public static void " + lst[j] + "()" + NL() + "{";
                strTmp += doubleTAB() + lstDot[j] + "Controller.Init();" + NL() + "}";
                source += strTmp + NL();
            }

            //IsInMeomry
            source += "public static bool IsInMeomry(string controler)" + NL();
            source += "{" + NL();
            source += "switch (controler)" + NL();//start switch
            source += "{" + NL();
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (!dic[key].IsProduce)
                    continue;
                source += "case \"" + key + "\":" + NL();
                source += "{" + NL();
                source += "return " + dic[key].DbType + "." + dic[key].DbName + "." + key + "Controller.IsDataInMemory();" + NL();
                source += "}" + NL();
            }

            source += "}" + NL();//end switch
            source += "return false;" + NL();
            source += "}" + NL();
            //end 

            //GetEntity data
            source += "public static IEnumerable GetEntity(string controler)" + NL();
            source += "{" + NL();
            source += "switch (controler)" + NL();//start switch
            source += "{" + NL();
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (!dic[key].IsProduce)
                    continue;
                source += "case \"" + key + "\":" + NL();
                source += "{" + NL();
                source += "return " + dic[key].DbType + "." + dic[key].DbName + "." + key + "Manager.Get" + key + "List();" + NL();
                source += "}" + NL();
            }

            source += "}" + NL();//end switch
            source += "return null;" + NL();
            source += "}" + NL();
            //end 

            //Remove from Meomry
            source += "public static void Remove(string controler)" + NL();
            source += "{" + NL();
            source += "switch (controler)" + NL();//start switch
            source += "{" + NL();
            for (int i = 0; i < dic.Count; i++)
            {
                string key = dic.ElementAt(i).Key;
                if (!dic[key].IsProduce)
                    continue;
                source += "case \"" + key + "\":" + NL();
                source += "{" + NL();
                source += dic[key].DbType + "." + dic[key].DbName + "." + key + "Controller.ClearMemory();" + NL();
                source += "break;" + NL();
                source += "}" + NL();
            }

            source += "}" + NL();//end switch            
            source += "}" + NL();

            source += "}";
            WriteFileInAppCode(path, "DataManager", source);
        }


        private static void CreateGlobalAsax(string path, bool IsGlobalAsaxInit, bool IsLoadStatistics, bool IsWebFarm, CheckedItems checkedItems)
        {
            string globAsax = "Global.asax";

            string source = "<%@ Application Language=\"C#\" %>" + NL();
            //source += "<%@ Import Namespace=\"webapi\" %>" + NL();
            source += "<%@ Import Namespace=\"System.Web.Optimization\" %>" + NL();
            source += "<%@ Import Namespace=\"System.Web.Http\" %>" + NL();
            source += "<script runat=\"server\">" + NL();
            source += "void Application_Start(object sender, EventArgs e){" + NL();
            source += "GlobalConfiguration.Configure(WebApiConfig.Register);" + NL();
            source += "GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;" + NL();
            source += "GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);" + NL();

            source += "GlobalConfiguration.Configuration.MessageHandlers.Add(new MessageHandler());" + NL();

            //if(IsLoadStatistics)
            //    source += "Statistics.Run();" + NL();

            //if (IsGlobalAsaxInit && !checkedItems.IsBalancer && !checkedItems.IsSharding)
            //    source += "DataManager.Init();" + NL();

            source += "UserFactory uf = new UserFactory();" + NL();
            source += "uf.Init();" + NL();

            if (checkedItems.IsBackUp && !checkedItems.IsBalancer && !checkedItems.IsSharding)
                source += "BackupFactory.Run();" + NL();

            if (IsWebFarm)
                source += "UrlHandler.LoadUrls();" + NL();

            if (checkedItems.IsReplicate)
                source += "MasterServer.LoadSlaveList();" + NL();

            if (checkedItems.IsBalancer)
            {
                if (checkedItems.IsReplicationBalancer)
                    source += "ServerManager.GetAndSetServerTypeFromFile();" + NL();
                if (checkedItems.IsCluster)
                {
                    source += "ClusterManager.Init();" + NL();
                }
                else
                {
                    source += "BalancerManager bal = new BalancerManager();" + NL();
                    source += "bal.LoadData();" + NL();
                    source += "bal.Run();" + NL();
                }
            }
            else if (checkedItems.IsSharding)
            {
                source += "BalancerManager bal = new BalancerManager();" + NL();
                source += "bal.LoadData();" + NL();
            }
            else
            {
                source += "MemoryObserver mem = new MemoryObserver();" + NL();
                source += "mem.Run();" + NL();
                if (checkedItems.IsBalancerProccessActivated)
                {
                    source += "CpuObserver cp = new CpuObserver();" + NL();
                    source += "cp.Run();" + NL();
                }
                if (checkedItems.IsCluster)
                {
                    source += "ClusterMaster.InitServers();" + NL();
                }
            }

            source += "}" + NL();

            //if (checkedItems.IsBackUp && !checkedItems.IsBalancer && !checkedItems.IsSharding)
            //{
            //    source += "void Application_End(object sender, EventArgs e)" + NL();
            //    source += "{" + NL();
            //    source += "DataManager.SaveAll();" + NL();
            //    source += "}" + NL();
            //}

            source += "</script>";

            //check if exists
            if (File.Exists(path + globAsax))
            {
                string globAsaxData = File.ReadAllText(path + globAsax);
                File.WriteAllText(path + globAsax + "Old", globAsaxData);

            }

            WriteFile(path + globAsax, source);

        }

        private static void CreateWebConfig(string path, CheckedItems checkedItems)
        {
            string webconfig = "web.config";

            string source = "<?xml version=\"1.0\"?>" + NL();
            source += "<configuration>" + NL();
            source += "<system.web>" + NL();
            //source +="<compilation debug=\"true\" targetFramework=\"4.5\"/>"+ NL();
            source += "<compilation debug=\"false\" targetFramework=\"4.5.2\" />" + NL();
            string MaxSize = " maxRequestLength=\"" + checkedItems.MaxSizePost + "\"";

            source += "<httpRuntime targetFramework=\"4.5.2\"" + MaxSize + " />" + NL();

            if (checkedItems.IsAuth)
                source += "<authentication mode=\"Windows\" />" + NL();
            //source +="<identity impersonate=\"true\" />"+ NL();

            source += "</system.web>" + NL();

            //allow put an delete in iis
            source += "<system.webServer>" + NL();
            source += "<handlers>" + NL();
            source += "<remove name=\"WebDAV\" />" + NL();
            source += "<remove name=\"ExtensionlessUrlHandler-Integrated-4.0\" />" + NL();
            source += "<remove name=\"OPTIONSVerbHandler\" />" + NL();
            source += "<remove name=\"TRACEVerbHandler\" />" + NL();
            source += "<add name=\"ExtensionlessUrlHandler-Integrated-4.0\" path=\"*.\" verb=\"*\" type=\"System.Web.Handlers.TransferRequestHandler\" preCondition=\"integratedMode,runtimeVersionv4.0\" />" + NL();
            source += "</handlers>" + NL();
            source += "<modules>" + NL();
            source += "<remove name=\"WebDAVModule\" />" + NL();
            source += "</modules>" + NL();
            source += "</system.webServer>" + NL();
            source += "</configuration>" + NL();

            //check if exists
            if (File.Exists(path + webconfig))
            {
                string webconfigData = File.ReadAllText(path + webconfig);
                File.WriteAllText(path + webconfig + "Old", webconfigData);

            }

            WriteFile(path + webconfig, source);

        }


        private static string GetOrderAndPaging()
        {
            string source = "";
            source += "int page = 0;" + NL();
            source += "int amount = 0;" + NL();

            source += "if (Common.CheckPaging(paging, size, out page, out amount))" + NL();
            source += "{" + NL();
            source += "    items = items.Skip(amount * (page - 1)).Take(amount);" + NL();
            source += "}" + NL();
            return source;
        }


        private static string SetStoredProcedureCode(string tableName, CheckedItems checkedItems)
        {
            string source = "";
            if (!checkedItems.IsBalancer)
            {
                if (dicStored != null && dicStored.Count > 0)
                {
                    for (int i = 0; i < dicStored.Count; i++)
                    {
                        StoredProcedure sp = dicStored.ElementAt(i).Value;
                        if (sp.isExecute && sp.tableName == tableName)
                        {
                            if (checkedItems.IsReadOnly && (sp.code.Contains(".Add(") || sp.code.Contains(".TryAdd(")))
                                continue;
                            if (!sp.code.Contains("public static"))
                                source += "public static ";
                            source += sp.code;
                        }

                    }
                }
            }
            return source;
        }


        private static string GetParmatersName(string[] ArrNames)
        {
            string p = "";
            if (ArrNames == null)
                return "";

            foreach (string s in ArrNames)
            {
                p += s + ",";
            }

            if (p.Length > 1)
                p = p.Substring(0, p.Length - 1);

            return p;
        }
        private static string GetParmaters(int iCount, string[] ArrTypes)
        {

            switch (iCount)
            {
                case 1:
                    {
                        return ArrTypes[0] + " id";
                    }
                case 2:
                    {
                        return ArrTypes[0] + " GT, " + ArrTypes[1] + " id";
                    }
                case 3:
                    {
                        return ArrTypes[0] + " id, " + ArrTypes[1] + " order, " + ArrTypes[2] + " Asc";
                    }
                case 4:
                    {
                        return ArrTypes[0] + " GT," + ArrTypes[1] + " id, " + ArrTypes[2] + " order, " + ArrTypes[3] + " Asc";
                    }
                case 5:
                    {
                        return ArrTypes[0] + " id, " + ArrTypes[1] + " order, " + ArrTypes[2] + " Asc, " + ArrTypes[3] + " paging, " + ArrTypes[4] + " size";
                    }
                case 6:
                    {
                        return ArrTypes[0] + " GT," + ArrTypes[1] + " id, " + ArrTypes[2] + " order, " + ArrTypes[3] + " Asc, " + ArrTypes[4] + " paging, " + ArrTypes[5] + " size";
                    }
                case 7:
                    {
                        return ArrTypes[0] + " GT," + ArrTypes[1] + " id, " + ArrTypes[2] + " order, " + ArrTypes[3] + " Asc, " + ArrTypes[4] + " paging, " + ArrTypes[5] + " size , " + ArrTypes[6] + " size2";
                    }

            }

            return "";

        }
        private static string GetCallParmaters(int iCount)
        {
            switch (iCount)
            {
                case 1:
                    {
                        return "id";
                    }
                case 2:
                    {
                        return "GT,id";
                    }
                case 3:
                    {
                        return "id, order, Asc";
                    }
                case 4:
                    {
                        return "GT,id, order, Asc";
                    }
                case 5:
                    {
                        return "id, order, Asc, paging, size";
                    }
                case 6:
                    {
                        return "GT,id, order, Asc, paging, size";
                    }
                case 7:
                    {
                        return "GT,id, order, Asc, paging, size , size2";
                    }

            }

            return "";

        }
        private static string GetCallParmatersForBalancer(int iCount)
        {
            switch (iCount)
            {
                case 1:
                    {
                        return "/\" + id";
                    }
                case 2:
                    {
                        return "/\" + GT + \"/\" + id";
                    }
                case 3:
                    {
                        return "/\" + MAX + \"/\" + GT + \"/\" + id";
                    }
                case 4:
                    {
                        return "/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc";
                    }
                case 5:
                    {
                        return "/\" + id + \"/\" + order + \"/\" + Asc + \"/\" + paging + \"/\" + size";
                    }
                case 6:
                    {
                        return "/\" + GT + \"/\" + id + /\" + order + \"/\" + Asc + \"/\" + paging + \"/\" + size";
                    }
                case 7:
                    {
                        return "/\" + GT + \"/\" + id + /\" + order + \"/\" + Asc + \"/\" + paging + \"/\" + size + \"/\" + size2";

                    }

            }

            return "\"";

        }
        /// <summary>
        /// manager for web api
        /// </summary>
        /// <param name="i"></param>
        /// <param name="path"></param>
        /// <param name="dic"></param>
        private static void CreatewebApiFileManager(int i, string path, Dictionary<string, EntityItem> dic, CheckedItems checkedItems)
        {
            string source = "";
            string key = dic.ElementAt(i).Key;
            bool IsProcedure = dic[key].EntityType == EntityTypes.Procedure;
            bool IsTable = dic[key].EntityType == EntityTypes.Table;
            bool IsQuery = dic[key].EntityType == EntityTypes.Query;
            List<Table> foundPK = dic[key].lstTables.Where(x => x.IsPrimaryKey > 0).ToList();
            bool IsPKExist = false;

            //source += FunctionCreator.CreateCacheManger(key);

            string targetFile = dic[key].DataSource.Replace(Common.PreojectFileExtension(), "Old" + Common.PreojectFileExtension());

            //start web api class
            //using zone
            source = "using System;" + NL() + "using System.Collections.Generic;" + NL() + "using System.Linq; " + NL();
            source += "using System.Net; " + NL() + "using System.Net.Http; " + NL() + "using System.Web.Http;" + NL();
            source += "using System.Web;" + NL() + "using System.Data; " + NL() + "using System.Linq.Dynamic;" + NL();
            source += "using System.Globalization;" + NL();
            source += "using System.IO;" + NL();
            source += "using System.Threading.Tasks;" + NL();
            source += "using System.Collections.Concurrent;" + NL();
            source += "using CsvFiles;" + NL();
            source += "using Newtonsoft.Json;" + NL();
            source += "using ImFast.Dal;" + NL();
            //end using zone

            source += "namespace " + dic[key].DbType + "." + dic[key].DbName + NL() + "{" + NL();

            if (IsTable)
            {
                //source += FunctionCreator.CreateCacheManger(key);                

                source += "public class " + key + "Manager {" + NL();


                source += FunctionCreator.CreateSql(key);

                //create group by manager
                source += FunctionCreator.CreateGetGroupByManager(key);
                if (!checkedItems.IsReadOnly)
                {
                    source += Crud.CreateInsertManager(dic, key, dic[key].DbType, checkedItems);
                    source += Crud.CreateUpdateManager(dic, key, dic[key].DbType, checkedItems);
                    source += Crud.CreateDeleteManager(dic, key, dic[key].DbType, checkedItems);
                }

                //create calc
                //source += FunctionCreator.CreateCalcFunction(key);

                //create function global query
                source += FunctionCreator.CreateGlobalQuery(key, dic[key].DbType + "_" + dic[key].DbName);

                for (int j = 0; j < dic[key].lstTables.Count; j++)
                {
                    string objName = dic[key].lstTables[j].Name;

                    source += FunctionCreator.CreateWebApiFuncForFilesManager(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, false, false, IsPKExist, foundPK);//file web api
                    source += FunctionCreator.CreateWebApiFuncForFilesManager(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, false, true, IsPKExist, foundPK);//file web api orderby
                                                                                                                                                                                           //source += FunctionCreator.CreateWebApiFuncForFilesManager(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, false, true);//file web api orderby and paging
                    if (IsOperator(dic[key].lstTables[j].type))
                    {
                        source += FunctionCreator.CreateWebApiFuncForFilesManager(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, true, false, IsPKExist, foundPK);
                        source += FunctionCreator.CreateWebApiFuncForFilesManager(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, true, true, IsPKExist, foundPK);//orderby
                    }


                }

                //add extra code
                //contains get key function
                //source += FunctionCreator.ExtraSource;


                //common for table and key-val
                //source += FunctionCreator.CreateIsUpdateFuncion(checkedItems.IsBackUp);

                source += " }" + NL();//end class
            }


            source += " }" + NL();//end namespace
            WriteFileInAppCode(path, key + "Manager", source);
        }


        private static string GetCode_AddDataToInMemory(bool IsPKExist, string key, string keys, List<Table> foundPK, CheckedItems checkedItems)
        {
            string source = "";

            if (IsPKExist)
            {
                if (foundPK.Count == 1)
                    source += key + "List.TryAdd(" + GetTypeByColumn(foundPK) + "(obj." + keys + "), obj);" + NL();
                else
                {
                    string sKeys = "";
                    foreach (Table t in foundPK)
                    {
                        sKeys += "obj." + t.Name + ".ToString() + ";
                    }
                    sKeys = sKeys.Substring(0, sKeys.Length - 3);
                    source += key + "List.TryAdd(" + sKeys + ", obj);" + NL();
                }

            }


            if (!IsPKExist)
            {
                if (checkedItems.IsReadOnly)
                {
                    source += "if (obj != null)" + NL();
                    source += "tmpList.Add(obj);" + NL();
                    source += "    i++;" + NL();
                    source += "    if (i == 20000)" + NL();
                    source += "    {" + NL();
                    source += key + "List.AddRange(tmpList);" + NL();
                    source += "        i = 0;" + NL();
                    source += "        tmpList.Clear();" + NL();
                    source += "    }" + NL();
                    source += "}" + NL();
                    source += "if (i < 20000)" + NL();
                    source += "{" + NL();
                    source += key + "List.AddRange(tmpList);" + NL();
                    source += "    i = 0;" + NL();
                    source += "    tmpList.Clear();" + NL();
                }
                else
                {
                    source += "if (obj != null)" + NL();
                    source += key + "List.Add(obj);" + NL();
                }
            }
            return source;
        }

        private static void CreateDateFormatterClass(string path, int type)
        {
            string source = "";
            source += "using System;" + NL();

            source += "public class DateFormatter" + NL();
            source += "{" + NL();
            source += "    public static string getDateFormat()" + NL();
            source += "    {" + NL();
            switch (type)
            {
                case 0:
                    {
                        source += "        return \"d-M-yyyy hh-mm-ss\";" + NL();
                        break;
                    }
                case 1:
                    {
                        source += "        return \"M-d-yyyy hh-mm-ss\";" + NL();
                        break;
                    }
                case 2:
                    {
                        source += "        return \"yyyy-M-d hh-mm-ss\";" + NL();
                        break;
                    }
            }
            source += "    }" + NL();
            source += "}" + NL();
            WriteFileInAppCode(path, "DateFormatter", source);
        }

        private static string CreateCalcController(string key, CheckedItems checkedItems)
        {
            string source = "";

            if (checkedItems.IsBalancer)
            {
                source += "[ActionName(\"CALC\")]" + NL();
                source += "public async Task<HttpResponseMessage> GetCALC(string id)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GetCALC/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GetCALC/\" + id);" + NL();
                source += "}" + NL();

                source += "[ActionName(\"CALC\")]" + NL();
                source += "public async Task<HttpResponseMessage> GetCALC(string GT,string id)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GetCALC/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GetCALC/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();

            }
            else if (checkedItems.IsSharding)
            {
                source += "[ActionName(\"CALC\")]" + NL();
                source += "public HttpResponseMessage GetCALC(string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GetCALC/\" + id);" + NL();
                source += "}" + NL();

                source += "[ActionName(\"CALC\")]" + NL();
                source += "public HttpResponseMessage GetCALC(string GT,string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GetCALC/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "[ActionName(\"CALC\")]" + NL();
                source += "public HttpResponseMessage GetCALC(string id)" + NL();
                source += "{" + NL();
                source += "var data2 = " + key + "Manager.Calc(id,null);" + NL();
                source += "return Request.CreateResponse(data2);" + NL();
                source += "}" + NL();

                source += "[ActionName(\"CALC\")]" + NL();
                source += "public HttpResponseMessage GetCALC(string GT, string id)" + NL();
                source += "{" + NL();
                source += "var data2 = " + key + "Manager.Calc(GT,id);" + NL();
                source += "return Request.CreateResponse(data2);" + NL();
                source += "}" + NL();
            }
            return source;
        }
        private static string CreateGroupByController(string key, CheckedItems checkedItems)
        {
            string source = "";



            if (checkedItems.IsBalancer)
            {
                source += "[ActionName(\"groupby\")]" + NL();
                source += "public async Task<HttpResponseMessage> Getgroupby(string GT, string id)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/groupby/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/groupby/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();

                source += "[ActionName(\"groupby\")]" + NL();
                source += "public async Task<HttpResponseMessage> Getgroupby(string MAX, string GT, string id)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/groupby/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/groupby/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();

                source += "[ActionName(\"groupby\")]" + NL();
                source += "public async Task<HttpResponseMessage> Getgroupby(string GT, string id, string order, string Asc)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/groupby/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/groupby/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc);" + NL();


                source += "}" + NL();

            }
            else if (checkedItems.IsSharding)
            {
                source += "[ActionName(\"groupby\")]" + NL();
                source += "public HttpResponseMessage Getgroupby(string GT, string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/groupby/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();

                source += "[ActionName(\"groupby\")]" + NL();
                source += "public HttpResponseMessage Getgroupby(string MAX, string GT, string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/groupby/\" + MAX + \"/\" + GT + \"/\" + id;" + NL();
                source += "}" + NL();

                source += "[ActionName(\"groupby\")]" + NL();
                source += "public HttpResponseMessage Getgroupby(string GT, string id, string order, string Asc)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/groupby/\" + GT + \"/\" + id + \"/\" + order + \"/\" + Asc;" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "[ActionName(\"groupby\")]" + NL();
                source += "public object Getgroupby(string GT, string id)" + NL();
                source += "{" + NL();
                source += "if (!QueryManager.CheckForbidenChars(GT) || !QueryManager.CheckForbidenChars(id))" + NL();
                source += "   return null;" + NL();
                source += "return " + key + "Manager.GetGroupBy(GT, id, \"\"); " + NL();
                source += "//return Request.CreateResponse(data);" + NL();
                //source += "}" + NL();
                source += "}" + NL();

                source += "[ActionName(\"groupby\")]" + NL();
                source += "public object Getgroupby(string MAX, string GT, string id)" + NL();
                source += "{" + NL();
                source += "if (!QueryManager.CheckForbidenChars(MAX) || !QueryManager.CheckForbidenChars(id) || !QueryManager.CheckForbidenChars(GT))" + NL();
                source += " return null;" + NL();
                source += "return " + key + "Manager.GetGroupBy(MAX, GT, id);" + NL();
                source += "//return Request.CreateResponse(data);" + NL();
                source += "}" + NL();

                source += "[ActionName(\"groupby\")]" + NL();
                source += "public object Getgroupby(string GT, string id, string order, string Asc)" + NL();
                source += "{" + NL();
                source += "if (!QueryManager.CheckForbidenChars(Asc) || !QueryManager.CheckForbidenChars(order) || !QueryManager.CheckForbidenChars(id) || !QueryManager.CheckForbidenChars(GT))" + NL();
                source += "    return null;" + NL();
                source += "return " + key + "Manager.GetGroupBy(GT, id, order, Asc);" + NL();
                source += "//return Request.CreateResponse(data);" + NL();
                source += "}" + NL();


            }
            return source;
        }

        private static string CreateGroupBy2Controller(string key, CheckedItems checkedItems)
        {
            string source = "";

            source += "[ActionName(\"GROUPBY\")]" + NL();

            if (checkedItems.IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetGROUPBY(string GT, string id)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GROUPBY/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GROUPBY/\" + GT+ \"/\" + id);" + NL();
                source += "}" + NL();

            }
            else if (checkedItems.IsSharding)
            {
                source += "public HttpResponseMessage GetGROUPBY(string GT,string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GROUPBY/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetGROUPBY(string GT, string id)" + NL();
                source += "{" + NL();
                source += "var data2 = " + key + "Manager.GetGroupByData(GT, id, null);" + NL();
                source += "return Request.CreateResponse(data2);" + NL();
                source += "}" + NL();
            }
            return source;
        }

        private static string CreateGroupBy3Controller(string key, CheckedItems checkedItems)
        {
            string source = "";

            source += "[ActionName(\"GROUPBY\")]" + NL();

            if (checkedItems.IsBalancer)
            {
                source += "public async Task<HttpResponseMessage> GetGROUPBY(string MAX, string GT, string id)" + NL();
                source += "{" + NL();
                if (checkedItems.IsCluster)
                    source += "return await ClusterManager.RetrieveData(\"" + key + "/GROUPBY/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();
                else
                    source += "return await BalancerManager.RetrieveData(\"" + key + "/GROUPBY/\" + MAX + \"/\" + GT+ \"/\" + id);" + NL();
                source += "}" + NL();

            }
            else if (checkedItems.IsSharding)
            {
                source += "public HttpResponseMessage GetGROUPBY(string MAX,string GT, string id)" + NL();
                source += "{" + NL();
                source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/GROUPBY/\" + MAX + \"/\" + GT + \"/\" + id);" + NL();
                source += "}" + NL();
            }
            else
            {
                source += "public HttpResponseMessage GetGROUPBY(string MAX, string GT, string id)" + NL();
                source += "{" + NL();
                source += "var data2 = " + key + "Manager.GetGroupByData(MAX,GT, id);" + NL();
                source += "return Request.CreateResponse(data2);" + NL();
                source += "}" + NL();
            }
            return source;
        }

        /// <summary>
        /// web api
        /// </summary>
        /// <param name="i"></param>
        /// <param name="path"></param>
        /// <param name="dic"></param>
        private static void CreatewebApiFile(int i, string path, Dictionary<string, EntityItem> dic, CheckedItems checkedItems)
        {
            string key = dic.ElementAt(i).Key;

            if (dic[key].lstTables == null)
                return;

            string source = "";

            bool IsProcedure = dic[key].EntityType == EntityTypes.Procedure;
            bool IsTable = dic[key].EntityType == EntityTypes.Table;
            //bool IsQuery = dic[key].EntityType == EntityTypes.Query;
            List<Table> foundPk = null;

            if (dic[key].lstTables != null)
                foundPk = dic[key].lstTables.FindAll(x => x.IsPrimaryKey == 1);

            bool IsPkExist = foundPk != null && foundPk.Count > 0;
            //start web api class
            source = "using System;" + NL() + " using System.Collections.Generic;" + NL() + " using System.Linq; " + NL();
            source += " using System.Net; " + NL() + "using System.Net.Http; " + NL() + " using System.Web.Http;" + NL();
            source += " using System.Web;" + NL() + " using System.Data; " + NL() + "using System.Linq.Dynamic;" + NL();
            source += " using System.Threading.Tasks;" + NL();
            source += " using ImFast.Dal;" + NL();
            if (checkedItems.IsSharding)
                source += "using System.Collections.Concurrent;" + NL();

            if (checkedItems.IsEnableCors)
                source += "using System.Web.Http.Cors;" + NL();

            source += "namespace " + dic[key].DbType + "." + dic[key].DbName + NL() + "{" + NL();

            if (checkedItems.IsEnableCors)
                source += "[EnableCors(origins: \" * \", headers: \" * \", methods: \" * \")]";

            source += "public class " + key + "Controller : ApiController {" + NL();
            //source += "static bool IsUpdate = true;" + NL();//parameter for is update/insert/delete


            //source += FunctionCreator.CreateIsUpdate(checkedItems.IsBalancer, key, checkedItems.IsSharding, checkedItems.IsCluster);

            if (!checkedItems.IsReadOnly)
            {
                source += Crud.CreateInsert(dic, key, dic[key].DbType, checkedItems);
                source += Crud.CreateUpdate(dic, key, dic[key].DbType, checkedItems);
                source += Crud.CreateDelete(dic, key, dic[key].DbType, checkedItems);
                
            }
            source += FunctionCreator.CreateCheckCols(key);

            //source += FunctionCreator.CreateCheckClearCache(key);

            if (!checkedItems.IsSharding)
                source += FunctionCreator.CreateReplicateMasterCode(checkedItems.IsReplicate, key, checkedItems.IsCluster);

            //call getinmemory
            //source += FunctionCreator.CreategetDataInMemory(key, checkedItems.IsBalancer, checkedItems.IsSharding, checkedItems.IsCluster);

            if (IsTable)
            {

                //source += SetStoredProcedureWebApiCode(key, checkedItems);

                //source += FunctionCreator.CreateGetEmpty(checkedItems.IsBalancer, key, checkedItems.IsCluster);

                //source += FunctionCreator.CreateGetAll(checkedItems.IsBalancer, key, checkedItems.IsSharding);                

                //create group by
                source += CreateGroupByController(key, checkedItems);
                //source += CreateGroupBy2Controller(key, checkedItems);
                //source += CreateGroupBy3Controller(key, checkedItems);

                //create calc controller
                //source += CreateCalcController(key, checkedItems);

                //create function global query
                source += "[ActionName(\"Query\")]" + NL();

                if (checkedItems.IsBalancer)
                {
                    source += "public async Task<HttpResponseMessage> GetQueryX(string id)" + NL();
                    source += "{" + NL();
                    if (checkedItems.IsCluster)
                        source += "return await ClusterManager.RetrieveData(\"" + key + "/Query/\" + id);" + NL();
                    else
                        source += "return await BalancerManager.RetrieveData(\"" + key + "/Query/\" + id);" + NL();
                    source += "}" + NL();

                }
                else if (checkedItems.IsSharding)
                {
                    source += "public HttpResponseMessage GetQueryX(string id)" + NL();
                    source += "{" + NL();
                    source += "return BalancerManager.createSeveralGetRequests(\"" + key + "/Query/\" + id);" + NL();
                    source += "}" + NL();
                }
                else
                {
                    source += "public HttpResponseMessage GetQueryX(string id)" + NL();
                    source += "{" + NL();
                    //source += "return await Task<HttpResponseMessage>.Factory.StartNew(() =>" + NL();
                    //source += " {" + NL();
                    source += "try" + NL();
                    source += "{" + NL();
                    //source += "if (IsDb())" + NL();
                    //source += "{" + NL();
                    source += "    id = QueryManager.GetExpression(id);" + NL();
                    source += "    var d = " + key + "Manager.GetQueryX(id);" + NL();
                    source += "    return CheckCols(d);" + NL();
                    //source += "}" + NL();
                    //source += "CheckClearCahe(id);" + NL();
                    //source += "     var data = " + key + "Manager.GetQueryX(id);" + NL();
                    //source += "     return CheckCols(data);" + NL();
                    source += "}" + NL();

                    source += " catch" + NL();
                    source += " {" + NL();
                    source += "     return Request.CreateResponse(HttpStatusCode.BadRequest);" + NL();
                    source += " }" + NL();
                    //source += " });" + NL();
                    source += "}" + NL();
                }
                if (!checkedItems.IsBalancer && !checkedItems.IsSharding && !checkedItems.IsCluster)
                {
                    source += FunctionCreator.GetFunction_GetByFunc(key, dic[key]);
                }

                if (dic[key].lstTables != null)
                {
                    for (int j = 0; j < dic[key].lstTables.Count; j++)
                    {
                        string objName = dic[key].lstTables[j].Name;

                        source += FunctionCreator.CreateWebApiFuncForFiles(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, false, foundPk, j, checkedItems.IsBalancer, checkedItems.IsSharding, checkedItems.IsCluster);//file web api
                        if (IsOperator(dic[key].lstTables[j].type))
                            source += FunctionCreator.CreateWebApiFuncForFiles(objName, key, dic[key].lstTables[j].type, dic[key].DbName, dic[key].DbType, true, null, 1, checkedItems.IsBalancer, checkedItems.IsSharding, checkedItems.IsCluster);


                    }
                }
                //source += "public static void RefreshData(){" + NL();
                //source += key + "Manager.RefreshData();}" + NL() + NL();

            }//end is table


            //if (!checkedItems.IsBalancer && !checkedItems.IsSharding)
            //    source += FunctionCreator.CreateInitAndIsDataInMemory(key);

            source += " }" + NL();//end class
            source += " }" + NL();//end namespace
            WriteFileInAppCode(path, key + "Controller", source);
        }
        private static string GetBrackets(string text, string type)
        {
            switch (type)
            {
                case "SqlServer":
                    {
                        return "[" + text + "]";
                    }

            }
            return text;
        }



        /// <summary>
        /// Get DataBase initiate data query 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="query"></param>
        /// <param name="key"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        private static string GetDataBaseCall(string dbType, string query, string key, string where)
        {
            string source = "";
            if (where != null && where.Length > 0)
                query = "GetQuery()";

            //switch (dbType)
            //{
            //    case 1://"SqlServer":
            //        {

            source += "DalOleDb<string> sqlDb = new DalOleDb<string>(ConnectionStringManager.GetConnectionString(\"" + dbType + "\"));" + NL();
            source += "IEnumerable<" + key + "> enumerator = sqlDb.GetSqlData<" + key + ">(" + query + ");" + NL();

            //        }
            //}
            return source;
        }


        public static string CreateDataEntityFileStringOld(string key, Dictionary<string, EntityItem> dic, bool isCheckDate = true, bool isUseValidationRegex = true)
        {
            bool IsDate = false;
            bool IsString = false;
            string source = "";
            string type = "";
            string usingValidations = "";
            string usingStr = "using System;" + NL();
            usingStr += "using System.ComponentModel.DataAnnotations;";
            if (dic[key].lstTables == null)
                return "";

            string Nullablae = "";
            source += "namespace " + dic[key].DbType + "." + dic[key].DbName + NL() + "{" + NL();
            source += "public class " + key + " {" + NL();//start data class

            if (dic[key].DbName == DbEntityTypes.keyVal)
            {
                type = GetTypeByColumn(dic[key].lstTables[0].type);
                source += "[Required]" + NL();
                source += " public " + type + " " + dic[key].lstTables[0].Name + " { get; set; } " + NL();
                source += " public string value { get; set; }" + NL();
            }
            else
            {
                for (int j = 0; j < dic[key].lstTables.Count; j++)
                {
                    type = GetTypeByColumn(dic[key].lstTables[j].type);

                    if (isCheckDate && type == "DateTime")
                    {
                        IsDate = true;
                        source += "[JsonConverter(typeof(CustomDateTimeConverter2))]" + NL();
                    }
                    else if (type == "string")
                    {
                        IsString = true;
                        char c = (char)3;
                        //if (isUseValidationRegex)
                        //  source += "[RegularExpression(\"^[^" + c + "]*$\", ErrorMessage=\"Invalid character\")] " + NL();
                    }

                    Nullablae = IsInsertQuestionMark(type) ? "? " : " ";

                    if (dic[key].lstTables[j].IsPrimaryKey > 0)
                        source += "[Required]" + NL();
                    source += " public " + type + Nullablae + dic[key].lstTables[j].Name + " { get; set; } " + NL();
                }
            }
            source += " }";//end class
            source += " }";//end namspace

            if (isCheckDate && IsDate)
                usingStr += "using Newtonsoft.Json; using Newtonsoft.Json.Converters;";

            if (IsString)
                usingStr += usingValidations;

            string strSource = usingStr + source;

            return strSource;
        }



        private static void WriteFileInAppCode(string fileName, string className, string sourceCode)
        {
            string fName = fileName + @"App_Code\" + className + ".cs";
            if (IsOpenSource)
            {
                if (!Directory.Exists(fileName + "App_Code"))
                    Directory.CreateDirectory(fileName + "App_Code");

                WriteFile(fName, sourceCode);
            }
            else
                CodeList.Add(sourceCode);
        }

        private static void WriteFile(string fName, string sourceCode)
        {
            if (File.Exists(fName))
                File.Delete(fName);

            File.WriteAllText(fName, sourceCode);

        }



        private static string GetSelfHostWinAuth()
        {
            string source = "";
            source += "HttpListener listener = (HttpListener)appBuilder.Properties[\"System.Net.HttpListener\"];" + NL();
            source += "listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;" + NL();
            return source;
        }

        private static string SetWebApiConfig(CheckedItems checkedItems)
        {
            string source = "";
            source += "using System.Web.Http;" + NL();
            source += "using System.Web.Cors;" + NL();
            source += "using System.Web.Http.Cors;" + NL();

            source += "public static class WebApiConfig" + NL();
            source += "{" + NL();
            source += "public static void Register(HttpConfiguration config)" + NL();
            source += "{" + NL();

            source += "    config.MapHttpAttributeRoutes();" + NL();

            if (checkedItems.IsEnableCors)
            {
                source += "    var cors = new EnableCorsAttribute(\"*\", \"*\", \"*\");" + NL();

                source += "    config.EnableCors(cors);" + NL();
            }

            source += "    config.Filters.Add(new ValidationResponseFilter());" + NL();

            source += "    config.Routes.MapHttpRoute(name: \"DefaultApiJoin\", routeTemplate: \"{controller}/{action}/{id}/join/{entity2}/{id2}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"DefaultApiJoin3\", routeTemplate: \"{controller}/{action}/{GT}/{id}/join/{entity2}/{id2}\");" + NL();
            //source += "    config.Routes.MapHttpRoute(name: \"DefaultApiJoin2\", routeTemplate: \"{controller}/{action}/{id}/join/{entity2}/{id2}/{entity3}/{id3}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"DefaultApi\", routeTemplate: \"{controller}/{action}/{id}\", defaults: new { id = System.Web.Http.RouteParameter.Optional, action = System.Web.Http.RouteParameter.Optional });" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"LTGT\", routeTemplate: \"{controller}/{action}/{GT}/{id}\", defaults: new { id = System.Web.Http.RouteParameter.Optional, action = System.Web.Http.RouteParameter.Optional });" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"MAXLTGT\", routeTemplate: \"{controller}/{action}/{MAX}/{GT}/{id}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"LTGTOrder\", routeTemplate: \"{controller}/{action}/{GT}/{id}/{order}/{Asc}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"DefaultApiOrder\", routeTemplate: \"{controller}/{action}/{id}/{order}/{Asc}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"LTGTOrderPaging\", routeTemplate: \"{controller}/{action}/{GT}/{id}/{order}/{Asc}/{paging}/{size}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"LTGTOrderPaging2\", routeTemplate: \"{controller}/{action}/{GT}/{id}/{order}/{Asc}/{paging}/{size}/{size2}\");" + NL();
            source += "    config.Routes.MapHttpRoute(name: \"DefaultApiOrderPaging\", routeTemplate: \"{controller}/{action}/{id}/{order}/{Asc}/{paging}/{size}\");" + NL();

            source += "}" + NL();
            source += "}" + NL();
            return source;
        }

        private static void CreateWindowsServiceSelefHostCode(List<string> lst, CheckedItems checkedItems)
        {
            string ServiceXYZ = projectName;
            string source = "";

            //windows service area            

            source += "using Owin;" + NL();
            source += "using System;" + NL();
            source += "using System.Collections.Generic;" + NL();
            source += "using System.Linq;" + NL();
            source += "using System.Text;" + NL();
            source += "using System.Threading.Tasks;" + NL();
            source += "using System.Web.Http;" + NL();
            source += "using System.Net;" + NL();

            source += "namespace WindowsService1" + NL();
            source += "{" + NL();
            source += "public class Startup" + NL();
            source += "{" + NL();
            source += "public void Configuration(IAppBuilder appBuilder)" + NL();
            source += "{" + NL();

            if (checkedItems.IsAuth)
                source += GetSelfHostWinAuth();

            source += "HttpConfiguration config = new HttpConfiguration();" + NL();
            source += "config.Formatters.Remove(config.Formatters.XmlFormatter);" + NL();

            source += getwebApiRoutLinks();
            source += "config.MessageHandlers.Add(new MessageHandler());" + NL();

            //if (!checkedItems.IsBalancer && !checkedItems.IsSharding)
            //    source += "DataManager.Init();" + NL();

            source += "UserFactory uf = new UserFactory();" + NL();
            source += "uf.Init();" + NL();

            if (checkedItems.IsBackUp && !checkedItems.IsBalancer)
                source += "BackupFactory.Run();" + NL();

            if (checkedItems.IsReplicate)
                source += "MasterServer.LoadSlaveList();" + NL();

            if (checkedItems.IsBalancer)
            {
                if (checkedItems.IsCluster)
                {
                    source += "ClusterManager.Init();" + NL();
                }
                else
                {
                    source += "BalancerManager bal = new BalancerManager();" + NL();
                    source += "bal.LoadData();" + NL();
                    source += "bal.Run();" + NL();
                }
            }

            else if (checkedItems.IsSharding)
            {
                source += "BalancerManager bal = new BalancerManager();" + NL();
                source += "bal.LoadData();" + NL();
            }
            else
            {
                source += "MemoryObserver mem = new MemoryObserver();" + NL();
                source += "mem.Run();" + NL();

                if (checkedItems.IsBalancerProccessActivated)
                {
                    source += "CpuObserver cp = new CpuObserver();" + NL();
                    source += "cp.Run();" + NL();
                }
                if (checkedItems.IsCluster)
                {
                    source += "ClusterMaster.InitServers();" + NL();
                }
            }
            source += "appBuilder.UseWebApi(config);" + NL();
            source += "}" + NL();
            source += "}" + NL();
            source += "}" + NL();

            lst.Add(source);
            source = "";

            source += "using System;" + NL();
            source += "using System.Collections.Generic;" + NL();
            source += "using System.ComponentModel;" + NL();
            source += "using System.Data;" + NL();
            source += "using System.Diagnostics;" + NL();
            source += "using System.Linq;" + NL();
            source += "using System.ServiceProcess;" + NL();
            source += "using System.Text;" + NL();
            source += "using System.Threading.Tasks;" + NL();
            source += "using Microsoft.Owin.Hosting;" + NL();
            source += "namespace WindowsService1" + NL();
            source += "{" + NL();
            source += "public partial class " + ServiceXYZ + ": ServiceBase" + NL();
            source += "{" + NL();
            source += "  const string baseAddress = \"http://localhost:" + CodeManager.portNum + "/\";" + NL();
            source += "private IDisposable _server = null;" + NL();
            source += "public " + ServiceXYZ + "()" + NL();
            source += "{" + NL();
            source += "InitializeComponent();" + NL();
            source += "}" + NL();
            source += "protected override void OnStart(string[] args)" + NL();
            source += "{" + NL();
            source += "StartOptions options = new StartOptions();" + NL();
            source += "options.Urls.Add(\"http://localhost:" + CodeManager.portNum + "\");" + NL();
            source += "options.Urls.Add(\"http://127.0.0.1:" + CodeManager.portNum + "\");" + NL();
            source += "options.Urls.Add(string.Format(\"http://{0}:" + CodeManager.portNum + "\", Environment.MachineName));" + NL();

            source += "_server = WebApp.Start<Startup>(options);" + NL();
            source += "}" + NL();
            source += "protected override void OnStop()" + NL();
            source += "{" + NL();
            source += "    if (_server != null)" + NL();
            source += "    {" + NL();
            //if (checkedItems.IsBackUp && !checkedItems.IsBalancer)
            //{
            //    source += "DataManager.SaveAll();" + NL();
            //}
            source += "    _server.Dispose();" + NL();
            source += "    }" + NL();
            source += "base.OnStop();" + NL();
            source += "  }" + NL();
            source += " }" + NL();
            source += "}" + NL();

            lst.Add(source);
            source = "";

            source += "using System.ComponentModel;" + NL();
            source += "using System.ServiceProcess;" + NL();
            source += "namespace WindowsService1" + NL();
            source += "{" + NL();
            source += "public partial class " + ServiceXYZ + NL();
            source += "{" + NL();
            source += "private System.ComponentModel.IContainer components = null;" + NL();

            source += "protected override void Dispose(bool disposing)" + NL();
            source += "{" + NL();
            source += "if (disposing && (components != null))" + NL();
            source += "{" + NL();
            source += "    components.Dispose();" + NL();
            source += "}" + NL();
            source += "base.Dispose(disposing);" + NL();
            source += "}" + NL();
            source += "private void InitializeComponent()" + NL();
            source += "{" + NL();
            source += "    components = new System.ComponentModel.Container();" + NL();
            source += "    this.ServiceName = \"" + ServiceXYZ + "\";" + NL();
            source += "}" + NL();
            source += "}" + NL();
            source += "}" + NL();

            lst.Add(source);
            source = "";


            ///windows servicw installer
            source += "using System.ComponentModel;" + NL();
            source += "using System.Configuration.Install;" + NL();
            source += "namespace WindowsService1" + NL();
            source += "{" + NL();
            source += "[RunInstaller(true)]" + NL();
            source += "public partial class ProjectInstaller : System.Configuration.Install.Installer" + NL();
            source += "{" + NL();
            source += "public ProjectInstaller()" + NL();
            source += "{" + NL();
            source += "InitializeComponent();" + NL();
            source += "}" + NL();
            source += "}" + NL();
            source += "}" + NL();

            lst.Add(source);
            source = "";

            source += "using System.ComponentModel;" + NL();
            source += "using System.Configuration.Install;" + NL();
            source += "namespace WindowsService1" + NL();
            source += "{" + NL();
            source += "public partial class ProjectInstaller" + NL();
            source += "{" + NL();
            source += "private System.ComponentModel.IContainer components = null;" + NL();
            source += "protected override void Dispose(bool disposing)" + NL();
            source += "{" + NL();
            source += "    if (disposing && (components != null))" + NL();
            source += "    {" + NL();
            source += "    components.Dispose();" + NL();
            source += "    }" + NL();
            source += "base.Dispose(disposing);" + NL();
            source += "}" + NL();

            source += "private void InitializeComponent()" + NL();
            source += "{" + NL();
            source += "this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();" + NL();
            source += "this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();" + NL();

            source += "this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;" + NL();
            source += "this.serviceProcessInstaller1.Password = null;" + NL();
            source += "this.serviceProcessInstaller1.Username = null;" + NL();

            source += "this.serviceInstaller1.ServiceName = \"" + ServiceXYZ + "\";" + NL();

            source += "this.Installers.AddRange(new System.Configuration.Install.Installer[] {" + NL();
            source += "this.serviceProcessInstaller1," + NL();
            source += "this.serviceInstaller1});" + NL();

            source += "}" + NL();

            source += "private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;" + NL();
            source += "private System.ServiceProcess.ServiceInstaller serviceInstaller1;" + NL();
            source += "}" + NL();
            source += "}" + NL();

            lst.Add(source);
            source = "";


            ///main program windows service
            source += "using System;" + NL();
            source += "using System.Collections.Generic;" + NL();
            source += "using System.Linq;" + NL();
            source += "using System.ServiceProcess;" + NL();
            source += "using System.Text;" + NL();
            source += "using System.Threading.Tasks;" + NL();
            source += "namespace WindowsService1" + NL();
            source += "{" + NL();
            source += "public static class Program" + NL();
            source += "{" + NL();
            source += "static void Main()" + NL();
            source += "{" + NL();
            source += "   ServiceBase[] ServicesToRun;" + NL();
            source += "   ServicesToRun = new ServiceBase[]" + NL();
            source += "{" + NL();
            source += "    new " + ServiceXYZ + "()" + NL();
            source += "};" + NL();
            source += "ServiceBase.Run(ServicesToRun);" + NL();
            source += "  }" + NL();
            source += " }" + NL();
            source += "}" + NL();

            lst.Add(source);
            source = "";
        }


        private static string getwebApiRoutLinks()
        {
            string source = "";
            source += "config.Routes.MapHttpRoute(name: \"Api\", routeTemplate: \"{controller}/{action}/{id}\", defaults: new { id = System.Web.Http.RouteParameter.Optional, action = System.Web.Http.RouteParameter.Optional });" + NL();
            source += "config.Routes.MapHttpRoute(name: \"LTGT\", routeTemplate: \"{controller}/{action}/{GT}/{id}\", defaults: new { id = System.Web.Http.RouteParameter.Optional, action = System.Web.Http.RouteParameter.Optional });" + NL();
            source += "config.Routes.MapHttpRoute(name: \"MAXLTGT\", routeTemplate: \"{controller}/{action}/{MAX}/{GT}/{id}\");" + NL();
            source += "config.Routes.MapHttpRoute(name: \"LTGTOrder\", routeTemplate: \"{controller}/{action}/{GT}/{id}/{order}/{Asc}\");" + NL();
            source += "config.Routes.MapHttpRoute(name: \"DefaultApiOrder\", routeTemplate: \"{controller}/{action}/{id}/{order}/{Asc}\");" + NL();
            source += "config.Routes.MapHttpRoute(name: \"LTGTOrderPaging\", routeTemplate: \"{controller}/{action}/{GT}/{id}/{order}/{Asc}/{paging}/{size}\");" + NL();
            source += "config.Routes.MapHttpRoute(name: \"DefaultApiOrderPaging\", routeTemplate: \"{controller}/{action}/{id}/{order}/{Asc}/{paging}/{size}\");" + NL();
            return source;
        }

        private static void CreateSelefHostCode(List<string> lst, CheckedItems checkedItems)
        {

            string source = "";
            source += "using Owin;" + NL();
            source += "using System.Web.Http;" + NL();
            source += "using System.Net;" + NL();
            source += "public class Startup" + NL();
            source += "{" + NL();
            source += "public void Configuration(IAppBuilder appBuilder)" + NL();
            source += "{" + NL();
            if (checkedItems.IsAuth)
                source += GetSelfHostWinAuth();
            source += "HttpConfiguration config = new HttpConfiguration();" + NL();
            source += "config.Formatters.Remove(config.Formatters.XmlFormatter);" + NL();
            source += getwebApiRoutLinks();
            source += "config.MessageHandlers.Add(new MessageHandler());" + NL();
            source += "appBuilder.UseWebApi(config);" + NL();


            source += "}" + NL();
            source += "}" + NL();

            lst.Add(source);

            source = "";

            source += "using System.Threading.Tasks;" + NL();
            source += "using Microsoft.Owin.Hosting;" + NL();
            source += "using System.Net.Http;" + NL();
            source += "using System;" + NL();
            source += "class Program" + NL();
            source += "{" + NL();
            source += "static void Main(string[] args)" + NL();
            source += "{" + NL();
            //todo:IsForceHTTPS ?
            source += "const string baseAddress = \"http://localhost:" + CodeManager.portNum + "/\";" + NL();//port is changable from gui
            source += "StartOptions options = new StartOptions();" + NL();
            source += "options.Urls.Add(\"http://localhost:" + CodeManager.portNum + "\");" + NL();
            source += "options.Urls.Add(\"http://127.0.0.1:" + CodeManager.portNum + "\");" + NL();
            source += "options.Urls.Add(string.Format(\"http://{0}:" + CodeManager.portNum + "\", Environment.MachineName));" + NL();

            source += "using (WebApp.Start<Startup>(options))" + NL();
            source += "{" + NL();
            //source += "Statistics.Run();" + NL();            
            //if (!checkedItems.IsBalancer && !checkedItems.IsSharding)
            //    source += "DataManager.Init();" + NL();

            source += "UserFactory uf = new UserFactory();" + NL();
            source += "uf.Init();" + NL();

            if (checkedItems.IsBackUp && !checkedItems.IsBalancer)
                source += "BackupFactory.Run();" + NL();

            if (checkedItems.IsReplicate)
                source += "MasterServer.LoadSlaveList();" + NL();

            if (checkedItems.IsBalancer)
            {
                if (checkedItems.IsCluster)
                {
                    source += "ClusterManager.Init();" + NL();
                }
                else
                {
                    source += "BalancerManager bal = new BalancerManager();" + NL();
                    source += "bal.LoadData();" + NL();
                    source += "bal.Run();" + NL();
                }
            }
            else if (checkedItems.IsSharding)
            {
                source += "BalancerManager bal = new BalancerManager();" + NL();
                source += "bal.LoadData();" + NL();
            }
            else
            {
                source += "MemoryObserver mem = new MemoryObserver();" + NL();
                source += "mem.Run();" + NL();

                if (checkedItems.IsBalancerProccessActivated)
                {
                    source += "CpuObserver cp = new CpuObserver();" + NL();
                    source += "cp.Run();" + NL();
                }
                if (checkedItems.IsCluster)
                {
                    source += "ClusterMaster.InitServers();" + NL();
                }
            }

            source += "Console.WriteLine(\"This is an in-memory no sql database, if you want to stop this proccess please press enter to exit\");" + NL();
            source += "Console.ReadLine();" + NL();
            source += "}" + NL();
            source += "}" + NL();
            source += "}" + NL();

            lst.Add(source);

        }


        public static bool CreateDll(CodeWrapper code)
        {
            bool ret = false;
            if (!code.CheckedItems.IsReadOnly && code.dicChanges != null && code.dicChanges.Count > 0)
            {
                SaveChangesFile(code);
            }

            ret = CreateDll(code.path, code.dicEntities, code.lstDbTypeItem, code.CheckedItems,
                                code.projName, code.dicFarmEntitiens, code.dicStoredProc, code.dicChanges);


            return ret;
        }

        private static bool SaveChangesFile(CodeWrapper code)
        {


            try
            {
                if (code.dicChanges != null && code.dicChanges.Count > 0 && code.dicLastEntities != null && code.dicLastEntities.Count > 0)
                {
                    for (int i = 0; i < code.dicChanges.Count; i++)
                    {
                        string key = code.dicChanges.ElementAt(i).Key;
                        Dictionary<string, string> dicCH = code.dicChanges.ElementAt(i).Value;
                        Dictionary<string, int> dic = new Dictionary<string, int>();
                        for (int j = 0; j < code.dicLastEntities[key].lstTables.Count; j++)
                        {
                            string fieldName = code.dicLastEntities[key].lstTables[j].Name;
                            if (!dicCH.ContainsKey(fieldName))
                            {
                                dic.Add(fieldName, j);
                            }
                        }
                        if (dic.Count > 0)
                        {
                            //save changedDB file
                            string path = code.path + "App_Data\\" + key + "changedDB";
                            string content = JsonConvert.SerializeObject(dic);
                            File.WriteAllText(path, content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Write(ex);
                return false;
            }

            return true;
        }
        public static bool CreateDll(string path, Dictionary<string, EntityItem> dicEntities,
            Dictionary<string, string> lstDbTypeItem, CheckedItems CheckedItems, string projName,
            Dictionary<string, DividedEntities> dicFarmEntitiens,
            Dictionary<string, StoredProcedure> dicStoredProc, Dictionary<string, Dictionary<string, string>> dicChanges = null)
        {
            dicStored = dicStoredProc;
            projectName = projName;

            CodeList.Clear();//reset code list - self host

            cacheTimeInMinutes = CheckedItems.CacheTime;

            if (CheckedItems.AppType == 2)
                CreateSelefHostCode(CodeList, CheckedItems);//init code - self host

            if (CheckedItems.AppType == 3)
                CreateWindowsServiceSelefHostCode(CodeList, CheckedItems);//init code - Windows Service

            dicConStr = lstDbTypeItem;
            //isAllDataInMemory = CheckedItems.InMemory;
            bool IsGlobalAsaxInit = CheckedItems.InitDataFromGlobalAsax;
            //InsertData = CheckedItems.InsertData;
            //UpdateData = CheckedItems.UpdateData;
            ////DeleteData = CheckedItems.DeleteData;
            //IsBackUp = CheckedItems.IsBackUp;
            //AppDateFormatType = CheckedItems.AppDateFormatType;
            //IsReplicate = CheckedItems.IsReplicate;

            //IsReplicationBalancer = CheckedItems.IsReplicationBalancer;
            //IsEnableCors = CheckedItems.IsEnableCors;


            //serverType = CheckedItems.serverType;
            //checkedItems.IsSharding = CheckedItems.IsSharding;




            if (CheckedItems.IsBalancer || CheckedItems.IsSharding)
            {
                CreareBalancerSourceCode(path, dicEntities, CheckedItems, dicStoredProc);

            }
            else
                CreareSourceCode(path, dicEntities, CheckedItems, dicChanges, dicStoredProc);


            CodeList.Add(SetWebApiConfig(CheckedItems));

            string[] arrCode = CodeList.ToArray();


            bool isCompile = Compile.ComplierManager.Compile(arrCode, projectName, path, CheckedItems.AppType);//- self host or dll web site

            if (!isCompile)
            {
                return false;
            }

            if (CheckedItems.AppType == 1 && !CheckedItems.IsReadOnly)
            {
                //string p = path.Substring(0, path.Length - 1);
                //FileManager.FileCreate(path + "permissions.bat");
                //string s = "icacls \"" + p + "\" /grant Everyone:(OI)(CI)F\n";
                //s += "appcmd add apppool /name:" + projectName + " /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated\n";
                //s += "appcmd add site /name:" + projectName + " /id:1 /physicalPath:" + p + " /bindings:http/*:80:" + projectName + "\n";

                //File.WriteAllText(path + "permissions.bat", s);

            }

            if (CheckedItems.AppType > 1 && !CheckedItems.IsReadOnly)
            {
                //string p = path.Substring(0, path.Length - 1);
                //FileManager.FileCreate(path + "permissions.bat");
                //string s = "icacls \"" + p + "\" /grant Everyone:(OI)(CI)F\n";

                //File.WriteAllText(path + "permissions.bat", s);
                string inst = "";

                if (CheckedItems.AppType == 3)
                {
                    if (path.IndexOf(" ") > 0)
                    {
                        string spath = "\"" + path;
                        inst = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "\\installutil " + spath + projectName + ".exe" + "\"";
                    }
                    else
                        inst = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "\\installutil " + path + projectName + ".exe";

                    File.WriteAllText(path + "installWindowsService.bat", inst);

                    string startInst = "net start " + projectName;
                    File.WriteAllText(path + "startWindowsService.bat", startInst);
                    string startUp = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\";
                    try
                    {
                        File.WriteAllText(startUp + "startWindowsService.bat", startInst);
                    }
                    catch { }
                }
            }
            return true;
        }

        public static void PrepereCodeForStoreProcedures(TreeView trvDb, Dictionary<string, StoredProcedure> dicStoredProc)
        {

            string sp = DbEntityTypes.Procedures;
            if (trvDb.Nodes[sp] != null)
            {
                foreach (TreeNode tn in trvDb.Nodes[sp].Nodes)
                {
                    foreach (TreeNode tnIn in tn.Nodes)
                    {
                        string key = tnIn.Text;
                        if (dicStoredProc.ContainsKey(key))
                        {
                            dicStoredProc[key].isExecute = tnIn.Checked;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prepere Code For File Db
        /// </summary>
        /// <param name="trvDb"></param>
        /// <param name="dbName"></param>
        /// <param name="dicEntities"></param>
        public static void PrepereCodeForFileDb(TreeView trvDb, string dbName, Dictionary<string, EntityItem> dicEntities)
        {
            string Tables = DbEntityTypes.Tables;
            string keyVal = DbEntityTypes.keyVal;
            string Databases = DbEntityTypes.Databases;
            string dbType = DbTypes.File;

            if (trvDb.Nodes[dbType].Nodes[Tables] != null)
            {
                foreach (TreeNode tn in trvDb.Nodes[dbType].Nodes[Tables].Nodes)
                {

                    ///Tables
                    if (tn.Parent != null && tn.Parent.Name == Tables && tn.Checked)
                    {
                        string table = tn.Text;
                        List<Table> SelectedTables = new List<Table>();
                        int i = 0;

                        foreach (var item in dicEntities[table].lstTables)
                        {
                            if (tn.Nodes[i].Checked)
                            {
                                SelectedTables.Add(item);
                            }

                            if (item.IsPrimaryKey > 0)
                            {
                                dicEntities[table].IsUpdate = true;
                            }
                            i++;
                        }
                        if (SelectedTables.Count > 0)
                            dicEntities[table].IsProduce = true;
                    }
                }
            }

            if (trvDb.Nodes[dbType].Nodes[keyVal] != null)
            {
                foreach (TreeNode tn in trvDb.Nodes[dbType].Nodes[keyVal].Nodes)
                {

                    ///Tables
                    if (tn.Parent != null && tn.Parent.Name == keyVal && tn.Checked)
                    {
                        string table = tn.Text;
                        List<Table> SelectedTables = new List<Table>();
                        int i = 0;

                        foreach (var item in dicEntities[table].lstTables)
                        {
                            if (tn.Nodes[i].Checked)
                            {
                                SelectedTables.Add(item);
                            }

                            if (item.IsPrimaryKey > 0)
                            {
                                dicEntities[table].IsUpdate = true;
                            }
                            i++;
                        }
                        if (SelectedTables.Count > 0)
                            dicEntities[table].IsProduce = true;
                    }
                }
            }
        }

    }
}