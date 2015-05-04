using UnityEngine;
using System.Collections;
using iCanScript.Engine;

namespace iCanScript.Editor {
    public static class DynamicInterface {

        public static iCS_VisualScript AddVisualScript(GameObject go) {
            return go.AddComponent<iCS_VisualScript>();
        }
        public static iCS_Library AddLibrary(GameObject go) {
            return go.AddComponent<iCS_Library>();
        }
    
    }    
}
