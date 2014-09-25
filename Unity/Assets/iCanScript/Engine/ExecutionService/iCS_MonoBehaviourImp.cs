using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("")]
public class iCS_MonoBehaviourImp : MonoBehaviour, iCS_IVisualScriptData {
    // ======================================================================
    // Storage
    // ----------------------------------------------------------------------
    [HideInInspector] public int			          MajorVersion          = iCS_Config.MajorVersion;
    [HideInInspector] public int    		          MinorVersion          = iCS_Config.MinorVersion;
    [HideInInspector] public int    		          BugFixVersion         = iCS_Config.BugFixVersion;
    [HideInInspector] public int                      DisplayRoot           = -1;	
	[HideInInspector] public int    		          SelectedObject        = -1;
    [HideInInspector] public Vector2                  SelectedObjectPosition= Vector2.zero;
	[HideInInspector] public bool                     ShowDisplayRootNode   = true;
	[HideInInspector] public float  		          GuiScale              = 1f;	
	[HideInInspector] public Vector2		          ScrollPosition        = Vector2.zero;
    [HideInInspector] public int                      UndoRedoId            = 0;
    [HideInInspector] public List<iCS_EngineObject>   EngineObjects         = new List<iCS_EngineObject>();
    [HideInInspector] public List<Object>             UnityObjects          = new List<Object>();
    [HideInInspector] public iCS_NavigationHistory    NavigationHistory     = new iCS_NavigationHistory();
    [HideInInspector] public iCS_EngineObject         EngineObject          = null;

    // ======================================================================
    // Visual Script Data Interface Implementation
    // ----------------------------------------------------------------------
    int iCS_IVisualScriptData.MajorVersion {
        get { return MajorVersion; }
        set { MajorVersion= value; }
    }
    int iCS_IVisualScriptData.MinorVersion {
        get { return MinorVersion; }
        set { MinorVersion= value; }
    }
    int iCS_IVisualScriptData.BugFixVersion {
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
    Vector2 iCS_IVisualScriptData.SelectedObjectPosition {
        get { return SelectedObjectPosition; }
        set { SelectedObjectPosition= value; }
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
    // Storage Redirect
    // ----------------------------------------------------------------------
    public string GetFullName(iCS_EngineObject obj) {
        return iCS_VisualScriptData.GetFullName(this, this, obj);
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
    public int GetIndexFromName(string fullName) {
        return iCS_VisualScriptData.GetIndexFromName(this, fullName);
    }
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // DATABASE CONVERSION
    // ======================================================================
    // OLD STORAGE (TO BE REMOVED)
    // ----------------------------------------------------------------------
    [HideInInspector] public iCS_StorageImp myStorage= null;
    public void v1_1_2_Upgrade() {
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
        v1_1_2_Upgrade();
    }

 }
