using UnityEngine;

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
            // Nothing to do if no or empty Visual Script.
            if(iStorage == null || iStorage.EditorObjects.Count == 0) {
                return;
            }

            // Build code global scope.
            BuildGlobalDefinition(iStorage);
            
            // Generate code.
            var result= GenerateCode();
            
            // Write final code to file.
            var fileName= iCS_ObjectNames.ToTypeName(iStorage.EditorObjects[0].CodeName);
            CSharpFileUtils.WriteCSharpFile("iCanScript Generated Code", fileName, result.ToString());
        }

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the definition of the global code.
        ///
        /// @param iStorage The VS storage to convert to code.
        ///
        void BuildGlobalDefinition(iCS_IStorage iStorage) {
            // Build global context.
            myCodeRoot= new GlobalDefinition();
            myCodeRoot.Namespace= "iCanScript.Engine.GeneratedCode";
            myCodeRoot.AddUsingDirective("UnityEngine");
            
            // Add root class defintion.
            var classDefinition= new TypeDefinition(iStorage.EditorObjects[0],
                                                    typeof(MonoBehaviour),
                                                    CodeContext.AccessType.PUBLIC,
                                                    CodeContext.ScopeType.NONSTATIC);
            myCodeRoot.AddType(classDefinition);
        }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generates the CSharp code.
        public string GenerateCode() {
            return myCodeRoot.GenerateCode(0);
        }
    
    }
    
}
