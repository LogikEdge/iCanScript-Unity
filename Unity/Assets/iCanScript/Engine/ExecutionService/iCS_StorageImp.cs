using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of iCanScript.  All object are derived
// from this storage class.
[AddComponentMenu("")]
public class iCS_StorageImp : ScriptableObject, iCS_IVisualScriptData {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    [HideInInspector] public uint			          MajorVersion       = iCS_Config.MajorVersion;
    [HideInInspector] public uint    		          MinorVersion       = iCS_Config.MinorVersion;
    [HideInInspector] public uint    		          BugFixVersion      = iCS_Config.BugFixVersion;
    [HideInInspector] public int                      DisplayRoot        = -1;	
	[HideInInspector] public int    		          SelectedObject     = -1;
	[HideInInspector] public bool                     ShowDisplayRootNode= true;
	[HideInInspector] public float  		          GuiScale           = 1f;	
	[HideInInspector] public Vector2		          ScrollPosition     = Vector2.zero;
    [HideInInspector] public int                      UndoRedoId         = 0;
    [HideInInspector] public List<iCS_EngineObject>   EngineObjects      = new List<iCS_EngineObject>();
    [HideInInspector] public List<Object>             UnityObjects       = new List<Object>();
    [HideInInspector] public iCS_NavigationHistory    NavigationHistory  = new iCS_NavigationHistory();
                      public iCS_EngineObject         EngineObject       = null;
    

    // ======================================================================
    // Visual Script Data Interface Implementation
    // ----------------------------------------------------------------------
    string iCS_IVisualScriptData.HostName {
        get { return name; }
        set { name= value; }
    }
    uint iCS_IVisualScriptData.MajorVersion {
        get { return MajorVersion; }
        set { MajorVersion= value; }
    }
    uint iCS_IVisualScriptData.MinorVersion {
        get { return MinorVersion; }
        set { MinorVersion= value; }
    }
    uint iCS_IVisualScriptData.BugFixVersion {
        get { return BugFixVersion; }
        set { BugFixVersion= value; }
    }
    List<iCS_EngineObject>  iCS_IVisualScriptData.EngineObjects {
        get { return EngineObjects; }
    }
    List<Object> iCS_IVisualScriptData.UnityObjects {
        get { return UnityObjects; }
    }
    int iCS_IVisualScriptData.UndoRedoId {
        get { return UndoRedoId; }
        set { UndoRedoId= value; }
    }
    
    
    // ======================================================================
    // Visual Script Editor Data Interface Implementation
    // ----------------------------------------------------------------------
    int iCS_IVisualScriptData.DisplayRoot {
        get { return DisplayRoot; }
        set { DisplayRoot= value; }
    }
    int iCS_IVisualScriptData.SelectedObject {
        get { return SelectedObject; }
        set { SelectedObject= value; }
    }
    bool iCS_IVisualScriptData.ShowDisplayRootNode {
        get { return ShowDisplayRootNode; }
        set { ShowDisplayRootNode= value; }
    }
    float iCS_IVisualScriptData.GuiScale {
        get { return GuiScale; }
        set { GuiScale= value; }
    }
    Vector2 iCS_IVisualScriptData.ScrollPosition {
        get { return ScrollPosition; }
        set { ScrollPosition= value; }
    }
    iCS_NavigationHistory iCS_IVisualScriptData.NavigationHistory {
        get { return NavigationHistory; }
    }


    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool IsValidEngineObject(int id) {
		return iCS_VisualScriptData.IsValidEngineObject(this, id);
	}
    public bool IsValidUnityObject(int id)  {
        return iCS_VisualScriptData.IsValidUnityObject(this, id);
	}

    // ======================================================================
    // Duplication Utilities
    // ----------------------------------------------------------------------
    public static void CopyFromTo(iCS_IVisualScriptData from, iCS_IVisualScriptData to) {
        iCS_VisualScriptData.CopyFromTo(from, to);
    }

    // ======================================================================
    // Unity Object Utilities
    // ----------------------------------------------------------------------
    public void ClearUnityObjects() {
        iCS_VisualScriptData.ClearUnityObjects(this);
    }
    // ----------------------------------------------------------------------
    public int AddUnityObject(Object obj) {
        return iCS_VisualScriptData.AddUnityObject(this, obj);
    }
    // ----------------------------------------------------------------------
    public Object GetUnityObject(int id) {
        return iCS_VisualScriptData.GetUnityObject(this, id);
    }

    // ======================================================================
    // Tree Navigation Queries
    // ----------------------------------------------------------------------
    public iCS_EngineObject GetParent(iCS_EngineObject child) {
        return iCS_VisualScriptData.GetParent(this, child);
    }
    // ----------------------------------------------------------------------
	public iCS_EngineObject GetParentNode(iCS_EngineObject child) {
        return iCS_VisualScriptData.GetParentNode(this, child);
	}
    // ----------------------------------------------------------------------
	public string GetFullName(iCS_EngineObject obj) {
        return iCS_VisualScriptData.GetFullName(this, obj);
	}
	
    // ======================================================================
    // Connection Queries
    // ----------------------------------------------------------------------
    // Returns the immediate source of the port.
    public iCS_EngineObject GetSourcePort(iCS_EngineObject port) {
        return iCS_VisualScriptData.GetSourcePort(this, port);
    }
    // ----------------------------------------------------------------------
    // Returns the endport source of a connection.
    public iCS_EngineObject GetFirstProviderPort(iCS_EngineObject port) {
        return iCS_VisualScriptData.GetFirstProviderPort(this, port);
    }
    // ----------------------------------------------------------------------
    // Returns the list of consumer ports.
    public iCS_EngineObject[] GetConsumerPorts(iCS_EngineObject port) {
        return iCS_VisualScriptData.GetConsumerPorts(this, port);
    }
    // ----------------------------------------------------------------------
    public bool IsEndPort(iCS_EngineObject port) {
        return iCS_VisualScriptData.IsEndPort(this, port);
    }
    // ----------------------------------------------------------------------
    public bool IsRelayPort(iCS_EngineObject port) {
        return iCS_VisualScriptData.IsRelayPort(this, port);
    }
    // ----------------------------------------------------------------------
    public bool HasASource(iCS_EngineObject port) {
        return iCS_VisualScriptData.HasASource(this, port);
    }
    // ----------------------------------------------------------------------
    public bool HasADestination(iCS_EngineObject port) {
        return iCS_VisualScriptData.HasADestination(this, port);
    }
    
    // ======================================================================
    // EnginObject Utilities
    // ----------------------------------------------------------------------
	public bool IsInPackagePort(iCS_EngineObject obj) {
        return iCS_VisualScriptData.IsInPackagePort(this, obj);
	}
    // ----------------------------------------------------------------------
	public bool IsOutPackagePort(iCS_EngineObject obj) {
        return iCS_VisualScriptData.IsOutPackagePort(this, obj);
	}
    
}
