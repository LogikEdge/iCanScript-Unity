using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject  myFunctionNode= null;  ///< VS objects associated with code context
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the VS objects associated with code context
        public iCS_EditorObject FunctionNode {
            get { return myFunctionNode; }
        }
        
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created code context.
        ///
        public FunctionDefinition(iCS_EditorObject functionNode)
        : base(CodeType.FUNCTION) {
            myFunctionNode= functionNode;
        }

    }

}