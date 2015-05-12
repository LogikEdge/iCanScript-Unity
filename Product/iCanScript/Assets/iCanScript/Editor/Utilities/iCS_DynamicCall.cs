using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    /// This class is used to dynamically find and invoke
    /// functionality that is not directly reachable from
    /// the current assembly.
    public static class iCS_DynamicCall {
        // ----------------------------------------------------------------------
        /// Adds a viusal script on the givne GameObject.
        /// @param go   The GameObject on which to add a new viusal script.
        public static iCS_VisualScriptImp AddVisualScript(GameObject go) {
            if(myAddVisualScriptFnc == null) {
                myAddVisualScriptFnc= new DynamicInvoke("iCanScript.Internal.Editor.DynamicInterface", "AddVisualScript");
            }
            return myAddVisualScriptFnc.Invoke(null, go) as iCS_VisualScriptImp;
        }
        static DynamicInvoke    myAddVisualScriptFnc= null;
        // ----------------------------------------------------------------------
        /// Adds an iCS Library component on the givne GameObject.
        /// @param go   The GameObject on which to add a new iCS library.
        public static iCS_LibraryImp AddLibrary(GameObject go) {
            if(myAddLibraryFnc == null) {
                myAddLibraryFnc= new DynamicInvoke("iCanScript.Internal.Editor.DynamicInterface", "AddLibrary");
            }
            return myAddLibraryFnc.Invoke(null, go) as iCS_LibraryImp;
        }
        static DynamicInvoke    myAddLibraryFnc= null;
    }

}
