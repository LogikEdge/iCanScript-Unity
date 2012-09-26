using UnityEngine;
using UnityEditor;

public static class iCS_iCanScriptMenu {
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
        iCS_VisualEditor graphEditor= iCS_EditorMgr.FindGraphEditor();
        if(graphEditor != null) graphEditor.CenterOnRoot();
    }
    [MenuItem("iCanScript/Center On Selected _&f",false,22)]
    public static void CenterOnSelected() {
        iCS_VisualEditor graphEditor= iCS_EditorMgr.FindGraphEditor();
        if(graphEditor != null) graphEditor.CenterOnSelected();
    }
//    // ======================================================================
//    [MenuItem("iCanScript/",false,40)]
//    [MenuItem("iCanScript/Reload Libraries",false,41)]
//    public static void ReloadLibraries() {
//        iCS_Installer.Install();
//    }
    // ======================================================================
    // Documentation Access
    [MenuItem("iCanScript/",false,50)]
    [MenuItem("iCanScript/Documentation/Home Page",false,51)]
    public static void HomePage() {
        Application.OpenURL("http://www.icanscript.com");
    }
    [MenuItem("iCanScript/Documentation/User's Manual",false,52)]
    public static void UserManual() {
        Application.OpenURL("http://www.icanscript.com/documentation/user_guide");
    }
    [MenuItem("iCanScript/Documentation/Programmer's Guide",false,53)]
    public static void ProgrammerGuide() {
        Application.OpenURL("http://www.icanscript.com/documentation/programmer_guide");
    }
    [MenuItem("iCanScript/Documentation/Release Notes",false,54)]
    public static void ReleaseNotes() {
        Application.OpenURL("http://www.icanscript.com/support/release_notes");
    }
    // ======================================================================
    // Support Access
    [MenuItem("iCanScript/Customer Request",false,60)]
    public static void ReportBug() {
        Application.OpenURL("http://www.disruptive-sw.com/support/customer_request");
    }
}
