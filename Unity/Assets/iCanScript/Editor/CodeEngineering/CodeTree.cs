using UnityEngine;
using System.Collections;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class CodeTree {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        GlobalDefinition    myCodeRoot= null;
    
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a _class_ specific code context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created code context.
        ///
        public CodeTree(iCS_IStorage iStorage) {
            // Build code root context.
            myCodeRoot= new GlobalDefinition();
            myCodeRoot.Namespace= "iCanScript.Engine.GeneratedCode";
            myCodeRoot.AddUsingDirective("UnityEngine");
            
            // Add the root node has the class context.
            var classDefinition= new ClassDefinition(iStorage.EditorObjects[0], typeof(MonoBehaviour));            
            myCodeRoot.AddClassDefinition(classDefinition);
        }
    
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generates the CSharp code.
        public string GenerateCode() {
            return myCodeRoot.GenerateCode();
        }
    }
    
}