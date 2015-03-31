using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class IfDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject[]  myAssociatedObjects= null;                  ///< VS objects associated with code context
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the VS objects associated with code context
        public iCS_EditorObject[] AssociatedObjects {
            get { return myAssociatedObjects; }
        }
        
        // -------------------------------------------------------------------
        /// Builds a _If_ specific code context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created code context.
        ///
        public IfDefinition(iCS_EditorObject[] associatedObjects)
        : base(CodeType.IF) {
            myAssociatedObjects= associatedObjects;
        }

    }

}