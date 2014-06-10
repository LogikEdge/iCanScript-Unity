using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("")]
public class iCS_MonoBehaviourImp : MonoBehaviour, iCS_IVisualScript {
    // ======================================================================
    // Storage
    // ----------------------------------------------------------------------
    [SerializeField]
    public iCS_StorageImp myStorage= null;
    public iCS_StorageImp Storage {
        get {
            if(myStorage == null) {
                myStorage= ScriptableObject.CreateInstance("iCS_Storage") as iCS_StorageImp;
                myStorage.name= name;
            }
            return myStorage;
        }
    }
    
    // ======================================================================
    // Visual Script Data Interface Implementation
    // ----------------------------------------------------------------------
    public string                   GetHostName()            { return name; }
    public void                     SetHostName(string nm)   { name= nm; }
    public uint                     GetMajorVersion()        { return Storage.MajorVersion; }
    public uint                     GetMinorVersion()        { return Storage.MinorVersion; }
    public uint                     GetBugFixVersion()       { return Storage.BugFixVersion; }
    public void                     SetMajorVersion(uint v)  { Storage.MajorVersion = v; }
    public void                     SetMinorVersion(uint v)  { Storage.MinorVersion = v; }
    public void                     SetBugFixVersion(uint v) { Storage.BugFixVersion= v; }
    public List<iCS_EngineObject>   GetEngineObjects()       { return Storage.EngineObjects; }
    public List<Object>             GetUnityObjects()        { return Storage.UnityObjects; }
    public int                      GetUndoRedoId()          { return Storage.UndoRedoId; }
    public void                     SetUndoRedoId(int id)    { Storage.UndoRedoId= id; }
    
    // ======================================================================
    // Visual Script Editor Data Interface Implementation
    // ----------------------------------------------------------------------
    public int     GetDisplayRoot()                     { return Storage.DisplayRoot; }
    public void    SetDisplayRoot(int id)               { Storage.DisplayRoot= id; }
    public int     GetSelectedObject()                  { return Storage.SelectedObject; }
    public void    SetSelectedObject(int id)            { Storage.SelectedObject= id; }
    public bool    GetShowDisplayRootNode()             { return Storage.ShowDisplayRootNode; }
    public void    SetShowDisplayRootNode(bool show)    { Storage.ShowDisplayRootNode= show; }
    public float   GetGuiScale()                        { return Storage.GuiScale; }
    public void    SetGuiScale(float scale)             { Storage.GuiScale= scale; }
    public Vector2 GetScrollPosition()                  { return Storage.ScrollPosition; }
    public void    SetScrollPosition(Vector2 pos)       { Storage.ScrollPosition= pos; }
    public iCS_NavigationHistory GetNavigationHistory() { return Storage.NavigationHistory; }
    public iCS_NavigationHistory NavigationHistory {
        get { return Storage.NavigationHistory; }
    }
    
    // ======================================================================
    // Storage Redirect
    // ----------------------------------------------------------------------
    public List<iCS_EngineObject> EngineObjects {
        get { return Storage.EngineObjects; }
    }
    public string GetFullName(iCS_EngineObject obj) {
        return Storage.GetFullName(obj);
    }
    public iCS_EngineObject GetParent(iCS_EngineObject obj) {
        return Storage.GetParent(obj);
    }
    public iCS_EngineObject GetSourceEndPort(iCS_EngineObject port) {
        return Storage.GetFirstProviderPort(port);
    }
    public iCS_EngineObject GetSourcePort(iCS_EngineObject port) {
        return Storage.GetSourcePort(port);
    }
    public iCS_EngineObject GetParentNode(iCS_EngineObject obj) {
        return Storage.GetParentNode(obj);
    }
    public bool IsOutPackagePort(iCS_EngineObject port) {
        return Storage.IsOutPackagePort(port);
    }
    
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    void Awake() {
//        Debug.Log("Awakening=> "+name);
//    }

 }
