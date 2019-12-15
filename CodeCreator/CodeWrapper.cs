using ImFast.BL;
using ImFast.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImFast.CodeCreator
{
    public class CodeWrapper
    {
        public string path {get;set;}
        
        public string projName {get;set;}
        
        /// <summary>
        /// all entiteis per db
        /// </summary>
        public Dictionary<string, EntityItem> dicEntities{get;set;}

        /// <summary>
        /// last entiteis per db
        /// </summary>
        public Dictionary<string, EntityItem> dicLastEntities { get; set; }


        public Dictionary<string, string> lstDbTypeItem {get;set;}
        
        
        public CheckedItems CheckedItems {get;set;}


        public Dictionary<string, DividedEntities> dicFarmEntitiens {get;set;}

        /// <summary>
        /// Stored Procedures
        /// </summary>
        public Dictionary<string, StoredProcedure> dicStoredProc { get; set; }
        
        /// <summary>
        /// is any change in data structure
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> dicChanges { get; set; }
    }
}
