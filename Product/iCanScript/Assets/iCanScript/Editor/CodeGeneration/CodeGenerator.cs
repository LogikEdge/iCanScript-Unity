using UnityEngine;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class CodeGenerator {    ///< @reviewed 2015-03-31
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        FileDefinition    myCodeRoot= null;   ///< Code global definition.

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
            var typeName= NameUtility.ToTypeName(iStorage.TypeName);
            var namespaceName= CodeGenerationUtility.GetNamespace(iStorage);
            var baseType= CodeGenerationUtility.GetBaseType(iStorage);
            myCodeRoot= new FileDefinition(typeName, namespaceName, baseType, iStorage);
            
            // -- Generate code. --
            var result= myCodeRoot.GenerateCode(0);
            
            // -- Write final code to file. --
            var fileName= typeName;
            var folder= CodeGenerationUtility.GetCodeGenerationFolder(iStorage);
            CSharpFileUtils.WriteCSharpFile(folder, fileName, result.ToString());
        }

    	// -------------------------------------------------------------------------
        /// Deletes the generate code files.
        ///
        /// @param iStorage The VS storage to convert to code.
        ///
        public void DeleteGeneratedFilesFor(iCS_IStorage iStorage) {
            var fileName= NameUtility.ToTypeName(iStorage.TypeName);
            var folder= CodeGenerationUtility.GetCodeGenerationFolder(iStorage);
            CSharpFileUtils.DeleteCSharpFile(folder, fileName);
        }

    }
    
}
