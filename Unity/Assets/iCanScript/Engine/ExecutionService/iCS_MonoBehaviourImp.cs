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
    public string                   GetHostName()            { return name; }
    public void                     SetHostName(string nm)   { name= nm; }
    public uint                     GetMajorVersion()        { return MajorVersion; }
    public uint                     GetMinorVersion()        { return MinorVersion; }
    public uint                     GetBugFixVersion()       { return BugFixVersion; }
    public void                     SetMajorVersion(uint v)  { MajorVersion = v; }
    public void                     SetMinorVersion(uint v)  { MinorVersion = v; }
    public void                     SetBugFixVersion(uint v) { BugFixVersion= v; }
    public List<iCS_EngineObject>   GetEngineObjects()       { return EngineObjects; }
    public List<Object>             GetUnityObjects()        { return UnityObjects; }
    public int                      GetUndoRedoId()          { return UndoRedoId; }
    public void                     SetUndoRedoId(int id)    { UndoRedoId= id; }
    
    // ======================================================================
    // Visual Script Editor Data Interface Implementation
    // ----------------------------------------------------------------------
    public int     GetDisplayRoot()                     { return DisplayRoot; }
    public void    SetDisplayRoot(int id)               { DisplayRoot= id; }
    public int     GetSelectedObject()                  { return SelectedObject; }
    public void    SetSelectedObject(int id)            { SelectedObject= id; }
    public bool    GetShowDisplayRootNode()             { return ShowDisplayRootNode; }
    public void    SetShowDisplayRootNode(bool show)    { ShowDisplayRootNode= show; }
    public float   GetGuiScale()                        { return GuiScale; }
    public void    SetGuiScale(float scale)             { GuiScale= scale; }
    public Vector2 GetScrollPosition()                  { return ScrollPosition; }
    public void    SetScrollPosition(Vector2 pos)       { ScrollPosition= pos; }
    public iCS_NavigationHistory GetNavigationHistory() { return NavigationHistory; }
    
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
