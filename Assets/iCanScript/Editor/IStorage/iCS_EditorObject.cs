using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_IStorage    myIStorage  = null;
    int             myId        = -1;
    bool            myIsFloating= false;
    bool            myIsDirty   = false;
    
    // ======================================================================
    // Conversion Utilities
    // ----------------------------------------------------------------------
    iCS_EngineObject EngineObject         { get { return myIStorage.Storage[myId]; }}
    List<iCS_EngineObject> EditorToEngineList(List<iCS_EditorObject> editorObjects) {
        return Prelude.map(eo=> eo.EngineObject, editorObjects);
    }
    
    // ======================================================================
    // Proxy Methods
    // Common attributes ----------------------------------------------------
    public bool   IsValid                   { get { return myId != -1 && EngineObject.IsValid; }}
    public iCS_ObjectTypeEnum ObjectType    { get { return EngineObject.ObjectType; } set { EngineObject.ObjectType= value; }}
    public int    InstanceId                { get { return EngineObject.InstanceId; }}
    public int    ParentId                  { get { return EngineObject.ParentId; }}
    public Type   RuntimeType               { get { return EngineObject.RuntimeType; }}
    public string RawName                   { get { return EngineObject.RawName; } set { EngineObject.RawName= value; }}
    public string Name                      { get { return EngineObject.Name; } set { EngineObject.Name= value; }}
    public bool   IsNameEditable            { get { return EngineObject.IsNameEditable; }}
    public string RawTooltip                { get { return EngineObject.RawTooltip; } set { EngineObject.RawTooltip= value; }}
    public string Tooltip                   { get { return EngineObject.Tooltip; } set { EngineObject.Tooltip= value; }}
    public bool   IsFloating                { get { return myIsFloating; } set { myIsFloating= value; }}
    public bool   IsDrity                   { get { return myIsDirty; } set { myIsDirty= value; }}
    public Rect   LocalPosition             { get { return EngineObject.LocalPosition; } set { EngineObject.LocalPosition= value; }}
    
    // Node specific attributes ---------------------------------------------
    public string IconGUID                  { get { return EngineObject.IconGUID; } set { EngineObject.IconGUID= value; }}
    
	// Port specific attributes ---------------------------------------------
    public iCS_EdgeEnum Edge                { get { return EngineObject.Edge; }}
    public int          SourceId            { get { return EngineObject.SourceId; } set { EngineObject.SourceId= value; }}

    // State specific attributes ---------------------------------------------
    public bool IsEntryState                { get { return EngineObject.IsEntryState; } set { EngineObject.IsEntryState= value; }}
    
    // Object Type Queries ---------------------------------------------------
    public bool IsPort                      { get { return EngineObject.IsPort; }}
    public bool IsDataPort                  { get { return EngineObject.IsDataPort; }}
    public bool IsInDataPort                { get { return EngineObject.IsInDataPort; }}
    public bool IsOutDataPort               { get { return EngineObject.IsOutDataPort; }}
    public bool IsModulePort                { get { return EngineObject.IsModulePort; }}
    public bool IsStatePort                 { get { return EngineObject.IsStatePort; }}
    public bool IsInStatePort               { get { return EngineObject.IsInStatePort; }}
    public bool IsOutStatePort              { get { return EngineObject.IsOutStatePort; }}
    public bool IsTransitionPort            { get { return EngineObject.IsTransitionPort; }}
    public bool IsInTransitionPort          { get { return EngineObject.IsInTransitionPort; }}
    public bool IsOutTransitionPort         { get { return EngineObject.IsOutTransitionPort; }}
    public bool IsInputPort                 { get { return EngineObject.IsInputPort; }}
    public bool IsOutputPort                { get { return EngineObject.IsOutputPort; }}
    public bool IsClassModule               { get { return EngineObject.IsClassModule; }}
    public bool IsTransitionModule          { get { return EngineObject.IsTransitionModule; }}
    public bool IsTransitionGuard           { get { return EngineObject.IsTransitionGuard; }}
    public bool IsTransitionAction          { get { return EngineObject.IsTransitionAction; }}
    public bool IsNode                      { get { return EngineObject.IsNode; }}
    public bool IsModule                    { get { return EngineObject.IsModule; }}
    public bool IsFunction                  { get { return EngineObject.IsFunction; }}
    public bool IsBehaviour                 { get { return EngineObject.IsBehaviour; }}
    public bool IsConstructor               { get { return EngineObject.IsConstructor; }}
    public bool IsStateChart                { get { return EngineObject.IsStateChart; }}
    public bool IsState                     { get { return EngineObject.IsState; }}
    public bool IsEnablePort                { get { return EngineObject.IsEnablePort; }}
    public bool IsInMuxPort                 { get { return EngineObject.IsInMuxPort; }}
    public bool IsOutMuxPort                { get { return EngineObject.IsOutMuxPort; }}
    
    // Layout attributes ----------------------------------------------------
    public bool IsUnfolded                  { get { return EngineObject.IsUnfolded; }}
    public bool IsFolded                    { get { return EngineObject.IsFolded; }}
    public bool IsIconized                  { get { return EngineObject.IsIconized; }}
    
    // ----------------------------------------------------------------------
    public MethodBase GetMethodBase(List<iCS_EditorObject> editorObjects) {
        return EngineObject.GetMethodBase(EditorToEngineList(editorObjects));
    }
    
    // ======================================================================
    // Constructors
    // ----------------------------------------------------------------------
    public iCS_EditorObject(iCS_IStorage iStorage, int id) {
        myIStorage= iStorage;
        myId= id;
        myIsDirty= true;
    }
    
    // ----------------------------------------------------------------------
    // Reinitialize the editor object to its default values.
    public void Reset() {
        IsFloating= false;
        IsDirty= false;
        EngineObject.Reset();
    }
}
