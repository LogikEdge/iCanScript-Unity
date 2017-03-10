using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {

    public static class SaveAndLoad {
        // =================================================================================
        /// Saves the visual script.
        ///
        /// @param The IStorage to save.
        ///
        public static void Save(iCS_IStorage iStorage) {
            if(iStorage == null) return;
            var package= iStorage.Package;
            var folder= iStorage.IsEditorScript ?
                package.GetEditorVisualScriptFolder(true) :
                package.GetEngineVisualScriptFolder(true);
            var path= folder+"/"+NameUtility.ToTypeName(iStorage.TypeName)+".ics2";
            iCS_VisualScriptImportExport.Export(iStorage.Storage, path);                        
        }
    }
    
}
