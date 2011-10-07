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
    public override int CreateInstance(WD_EditorObjectMgr editorObjects, int parentId, Vector2 initialPos) {
        // Create the function node.
        int funcId= editorObjects.GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= editorObjects.IsValid(parentId) ? editorObjects.GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        editorObjects[funcId]= new WD_EditorObject(funcId, Name, Method.DeclaringType, parentId, WD_ObjectTypeEnum.Conversion, localPos);
        // Create the function ports.
        CreatePort(editorObjects, FromType.Name, funcId, FromType, WD_ObjectTypeEnum.InFunctionPort);
        CreatePort(editorObjects, ToType.Name, funcId, ToType, WD_ObjectTypeEnum.OutFunctionPort);

        return funcId;
    }
}
