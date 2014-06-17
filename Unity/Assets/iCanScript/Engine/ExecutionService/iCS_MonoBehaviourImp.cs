using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("")]
public class iCS_MonoBehaviourImp : MonoBehaviour, iCS_IVisualScriptData {
    // ======================================================================
    // Storage
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
    string iCS_IVisualScriptEngineData.HostName {
        get { return name; }
        set { name= value; }
    }
    uint iCS_IVisualScriptEngineData.MajorVersion {
        get { return MajorVersion; }
        set { MajorVersion= value; }
    }
    uint iCS_IVisualScriptEngineData.MinorVersion {
        get { return MinorVersion; }
        set { MinorVersion= value; }
    }
    uint iCS_IVisualScriptEngineData.BugFixVersion {
        get { return BugFixVersion; }
        set { BugFixVersion= value; }
    }
    List<iCS_EngineObject>  iCS_IVisualScriptEngineData.EngineObjects {
        get { return EngineObjects; }
    }
    List<Object> iCS_IVisualScriptEngineData.UnityObjects {
        get { return UnityObjects; }
    }
    int iCS_IVisualScriptEngineData.UndoRedoId {
        get { return UndoRedoId; }
        set { UndoRedoId= value; }
    }
    
    
    // ======================================================================
    // Visual Script Editor Data Interface Implementation
    // ----------------------------------------------------------------------
    int iCS_IVisualScriptEditorData.DisplayRoot {
        get { return DisplayRoot; }
        set { DisplayRoot= value; }
    }
    int iCS_IVisualScriptEditorData.SelectedObject {
        get { return SelectedObject; }
        set { SelectedObject= value; }
    }
    bool iCS_IVisualScriptEditorData.ShowDisplayRootNode {
        get { return ShowDisplayRootNode; }
        set { ShowDisplayRootNode= value; }
    }
    float iCS_IVisualScriptEditorData.GuiScale {
        get { return GuiScale; }
        set { GuiScale= value; }
    }
    Vector2 iCS_IVisualScriptEditorData.ScrollPosition {
        get { return ScrollPosition; }
        set { ScrollPosition= value; }
    }
    iCS_NavigationHistory iCS_IVisualScriptEditorData.NavigationHistory {
        get { return NavigationHistory; }
    }

    
    // ======================================================================
    // Storage Redirect
    // ----------------------------------------------------------------------
    public string GetFullName(iCS_EngineObject obj) {
        return iCS_VisualScriptData.GetFullName(this, obj);
    }
    public iCS_EngineObject GetParent(iCS_EngineObject obj) {
        return iCS_VisualScriptData.GetParent(this, obj);
    }
    public iCS_EngineObject GetSourceEndPort(iCS_EngineObject port) {
        return iCS_VisualScriptData.GetFirstProviderPort(this, port);
    }
    public iCS_EngineObject GetSourcePort(iCS_EngineObject port) {
        return iCS_VisualScriptData.GetSourcePort(this, port);
    }
    public iCS_EngineObject GetParentNode(iCS_EngineObject obj) {
        return iCS_VisualScriptData.GetParentNode(this, obj);
    }
    public bool IsOutPackagePort(iCS_EngineObject port) {
        return iCS_VisualScriptData.IsOutPackagePort(this, port);
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // DATABASE CONVERSION
    // ======================================================================
    // OLD STORAGE (TO BE REMOVED)
    // ----------------------------------------------------------------------
    public iCS_StorageImp myStorage= null;
//    public iCS_StorageImp Storage {
//        get { return myStorage; }
//    }
    void PerfromUpgrade() {
        if(myStorage != null) {
            iCS_VisualScriptData.CopyFromTo(myStorage, this);
        }
        myStorage= null;
    }
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    void Awake() {
        PerfromUpgrade();
    }

 }
