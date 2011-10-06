using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_FunctionDesc : WD_BaseDesc {
    public Type         ClassType;
    public string       ToolTip;
    public string[]     ParameterNames;
    public Type[]       ParameterTypes;
    public bool[]       ParameterInOuts;
    public MethodInfo   Method;
    public WD_FunctionDesc(string company, string package, Type classType,
                           string methodName, string toolTip,
                           string[] paramNames, Type[] paramTypes, bool[] paramInOuts,
                           MethodInfo methodInfo) : base(company, package, methodName) {
        ClassType = classType;
        ToolTip   = toolTip;
        ParameterNames = paramNames;
        ParameterTypes = paramTypes;
        ParameterInOuts= paramInOuts;
        Method    = methodInfo;
    }
    public WD_RuntimeMethod CreateRuntime() {
        return WD_RuntimeMethod.CreateFunction(Method.Invoke, ParameterNames.Length);
    }

    // -----------------------------------------------------------------------
    public override int CreateInstance(WD_EditorObjectMgr editorObjects, int parentId, Vector2 initialPos) {
        // Create the function node.
        int funcId= editorObjects.GetNextAvailableId();
        // Calcute the desired screen position of the new object.
        Rect parentPos= editorObjects.IsValid(parentId) ? editorObjects.GetPosition(parentId) : new Rect(0,0,0,0);
        Rect localPos= new Rect(initialPos.x-parentPos.x, initialPos.y-parentPos.y,0,0);
        // Create new EditorObject
        editorObjects[funcId]= new WD_EditorObject(funcId, Name, ClassType, parentId, WD_DisplayTypeEnum.Function, localPos);
        // Create the function ports.
        for(int i= 0; i < ParameterNames.Length; ++i) {
            int portId= editorObjects.GetNextAvailableId();
            WD_DisplayTypeEnum portType= ParameterInOuts[i] ? WD_DisplayTypeEnum.OutFunctionPort : WD_DisplayTypeEnum.InFunctionPort;
            editorObjects[portId]= new WD_EditorObject(portId, ParameterNames[i], ParameterTypes[i], funcId, portType, new Rect(0,0,0,0));
        }        
        return funcId;
    }
}
