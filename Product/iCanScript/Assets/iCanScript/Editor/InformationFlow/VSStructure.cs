using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    // ===================================================================
    /// This class implements utilties to navigate and query the visual
    /// script structure.
    public static class VSStructure {

    	// -------------------------------------------------------------------------
        /// Returns the list of functions inside the given package.
        ///
        /// @param package The package to examine.
        /// @return The list of all functions nested inside the package.
        ///
        public static List<iCS_EditorObject> GetListOfFunctions(iCS_EditorObject package) {
            var childFunctions= new List<iCS_EditorObject>();
            package.ForEachChildRecursiveDepthFirst(
                c=> {
                    if(c.IsKindOfFunction) {
                        childFunctions.Add(c);
                    }
                }
            );
            return childFunctions;
        }        

    }

}