using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_MenuProxy {
    // ======================================================================
	// iCanScript Window Menu.
	[MenuItem("Window/iCanScript Editor")]
	public static void ShowiCanScriptEditor() {
        iCS_Editor editor= EditorWindow.GetWindow(typeof(iCS_Editor), false, "iCanScript") as iCS_Editor;
        editor.hideFlags= HideFlags.DontSave;
	}

    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("iCanScript/Create Behaviour #&b", false, 1)]
	public static void CreateBehaviour() {
        iCS_Menu.CreateBehaviour();
	}
	[MenuItem("iCanScript/Create Behaviour #&b", true, 1)]
	public static bool ValidateCreateBehaviour() {
        return iCS_Menu.ValidateCreateBehaviour();
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("iCanScript/Create Module Library", false, 2)]
	public static void CreateModuleLibrary() {
        iCS_Menu.CreateModuleLibrary();
	}
	[MenuItem("iCanScript/Create Module Library", true, 2)]
	public static bool ValidateCreateModuleLibrary() {
        return iCS_Menu.ValidateCreateModuleLibrary();
	}

    // ======================================================================
	// Create a library module to selected game object.
	[MenuItem("iCanScript/Create State Chart Library", false, 3)]
	public static void CreateStateChartLibrary() {
        iCS_Menu.CreateStateChartLibrary();
	}
	[MenuItem("iCanScript/Create State Chart Library", true, 3)]
	public static bool ValidateCreateStateChartLibrary() {
        return iCS_Menu.ValidateCreateStateChartLibrary();
	}

    // ======================================================================
    // Navigation
    [MenuItem("iCanScript/",false,20)]
    [MenuItem("iCanScript/Center Graph _#&f",false,21)]
    public static void CenterGraph() {
        iCS_Menu.CenterGraph();
    }
    [MenuItem("iCanScript/Center On Selected _&f",false,22)]
    public static void CenterOnSelected() {
        iCS_Menu.CenterOnSelected();
    }
    // ======================================================================
    // Documentation Access
    [MenuItem("iCanScript/",false,30)]
    [MenuItem("iCanScript/Documentation/Home Page",false,31)]
    public static void HomePage() {
        Application.OpenURL("http://www.icanscript.com/index.html");
    }
    [MenuItem("iCanScript/Documentation/User's Manual",false,32)]
    public static void UserManual() {
        Application.OpenURL("http://www.icanscript.com/Documentation/UserManual/index.html");
    }
    [MenuItem("iCanScript/Documentation/Programmer's Guide",false,33)]
    public static void ProgrammerGuide() {
        Application.OpenURL("http://www.icanscript.com/Documentation/ProgrammerGuide/index.html");
    }
    [MenuItem("iCanScript/Documentation/Release Notes",false,34)]
    public static void ReleaseNotes() {
        Application.OpenURL("http://www.icanscript.com/Documentation/ReleaseNotes/index.html");
    }
    // ======================================================================
    // Support Access
    [MenuItem("iCanScript/Report a Bug",false,40)]
    public static void ReportBug() {
        Application.OpenURL("http://www.icanscript.com/Support/ReportBug/index.html");
    }
}
