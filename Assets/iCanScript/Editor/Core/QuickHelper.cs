// EditorScript that quickly searchs for a help page
// of the selected Object.
//
// If there is no page found on the Manual it opens the Unity forum.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;

public class QuickHelper : EditorWindow {

    Object  source= null;
    string  search= "";
    int     popup= 0;
    
    [MenuItem("Example/QuickHelper _h")]
    static void Init() {
        QuickHelper window= EditorWindow.GetWindowWithRect(typeof(QuickHelper), new Rect(0,0,165,100)) as QuickHelper;
        window.Show();
    }
    
    void OnGUI() {
        Rect toolbarRect= iCS_ToolbarUtility.BuildToolbar(position.width);
        popup= iCS_ToolbarUtility.Popup(ref toolbarRect, 100, "Create", popup, new string[2]{"t1","t2"}, 0, 0);
        search= iCS_ToolbarUtility.Search(ref toolbarRect, 150, search, 0, 0);

//        EditorGUILayout.BeginHorizontal();
//        source = EditorGUILayout.ObjectField(source, typeof(Object), true);
//        EditorGUILayout.EndHorizontal();

//        if(GUILayout.Button("Search!")) {
//            if(source == null) {
//                ShowNotification(new GUIContent("No object selected for searching"));
//            } else {
//                if(Help.HasHelpForObject(source)) {
////                    Help.ShowHelpForObject(source);
//                    Help.ShowHelpPage("file:///unity/ScriptReference/CharacterController-isGrounded");
//                    wwwTest();
//                } else {
//                    Help.BrowseURL("http://forum.unity3d.com/search.php");
//                }
//            }
//        }
    }
    void wwwTest() {
        WWW www = new WWW("http://unity3d.com/support/documentation/ScriptReference/CharacterController-isGrounded.html");
//        yield return www;
        while(!www.isDone);
        Debug.Log(www.text);
    }
}