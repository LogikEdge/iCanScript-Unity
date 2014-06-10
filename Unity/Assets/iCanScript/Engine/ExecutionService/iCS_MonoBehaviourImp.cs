using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("")]
public class iCS_MonoBehaviourImp : MonoBehaviour, iCS_IVisualScriptData {
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
