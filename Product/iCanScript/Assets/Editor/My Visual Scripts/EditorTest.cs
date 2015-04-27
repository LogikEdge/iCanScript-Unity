using UnityEditor;

namespace MyVisualScripts.Editor {

    [iCS_Class(Library="My Visual Scripts")]
    public  class EditorTest : EditorWindow {

        // =========================================================================
        // PUBLIC FUNCTIONS
        // -------------------------------------------------------------------------

        [iCS_Function]
        [MenuItem("MyMenu/Object Cloner")]
        public static void Init() {
            var theOutput= GetWindow(typeof(MyVisualScripts.Editor.EditorTest));
            theOutput.title= "EditorTest";
            theOutput.ShowUtility();
        }
    }
}
