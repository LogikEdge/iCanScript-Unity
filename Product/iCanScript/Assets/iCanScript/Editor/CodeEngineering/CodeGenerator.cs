using UnityEngine;
using iCanScript.Engine;

namespace iCanScript.Editor.CodeEngineering {

    public class CodeGenerator {    ///< @reviewed 2015-03-31
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        GlobalDefinition    myCodeRoot= null;   ///< Code global definition.

    	// -------------------------------------------------------------------------
        /// Builds global scope code definition.
        ///
        /// @param iStorage The VS storage to convert to code.
        /// @return The complete visual script code.
        ///
        public void GenerateCodeFor(iCS_IStorage iStorage) {
            // -- Nothing to do if no or empty Visual Script. --
            if(iStorage == null || iStorage.EditorObjects.Count == 0) {
                return;
            }

            // -- Build code global scope. --
            var codeGenerationNamespace= iCS_Config.kCodeGenerationNamespace;
            myCodeRoot= new GlobalDefinition(iStorage.EditorObjects[0], codeGenerationNamespace);
            
            // -- Generate code. --
            var result= myCodeRoot.GenerateCode(0);
            
            // -- Write final code to file. --
            var fileName= iCS_ObjectNames.ToTypeName(iStorage.EditorObjects[0].CodeName);
            CSharpFileUtils.WriteCSharpFile("iCanScript Generated Code", fileName, result.ToString());
            
            // -- Update the type information --
            GameObject go= iStorage.HostGameObject;
            if(go != null) {
                iStorage.TypeName= fileName;
                iStorage.BaseTypeName= PreferencesController.CodeGenerationBaseTypeName;
            }
        }

    	// -------------------------------------------------------------------------
        /// Deletes the generate code files.
        ///
        /// @param iStorage The VS storage to convert to code.
        ///
        public void DeleteGeneratedFilesFor(iCS_IStorage iStorage) {
            var fileName= iCS_ObjectNames.ToTypeName(iStorage.EditorObjects[0].CodeName);
            CSharpFileUtils.DeleteCSharpFile("iCanScript Generated Code", fileName);            
        }

    }
    
}
