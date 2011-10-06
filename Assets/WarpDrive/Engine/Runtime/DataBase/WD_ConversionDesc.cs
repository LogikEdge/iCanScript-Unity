using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ConversionDesc : WD_BaseDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public MethodInfo  Method;
    public Type        FromType;
    public Type        ToType;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_ConversionDesc(string company, string package, MethodInfo methodInfo, Type fromType, Type toType) : base(company, package, fromType.Name+"->"+toType.Name) {
        Method= methodInfo;
        FromType= fromType;
        ToType= toType;
    }
    // ----------------------------------------------------------------------
    public override WD_EditorObject CreateInstance(WD_EditorObjectMgr editorObjects, int parentId, Vector2 initialPos) {
        // Create the function node.
        int funcId= editorObjects.GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= editorObjects.IsValid(parentId) ? editorObjects.GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        editorObjects[funcId]= new WD_EditorObject(funcId, Name, Method.DeclaringType, parentId, WD_DisplayTypeEnum.Conversion, localPos);
        // Create the function ports.
        int portId= editorObjects.GetNextAvailableId();
        editorObjects[portId]= new WD_EditorObject(portId, FromType.Name, FromType, funcId, WD_DisplayTypeEnum.InFunctionPort, new Rect(0,0,0,0));
        portId= editorObjects.GetNextAvailableId();
        editorObjects[portId]= new WD_EditorObject(portId, ToType.Name, ToType, funcId, WD_DisplayTypeEnum.OutFunctionPort, new Rect(0,0,0,0));

        return editorObjects[funcId];
    }
}
