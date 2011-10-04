using UnityEngine;
using System;
using System.Collections;

public class WD_RuntimeClass : WD_Runtime {
    // ======================================================================
    // Attributes
    // ----------------------------------------------------------------------
    public object              Instance= null;
    public WD_RuntimeMethod[]  Methods = null;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_RuntimeClass(WD_ClassDesc classDesc) {
        // Create instance.
        Instance= Inf.IsA(classDesc.ClassType, typeof(ScriptableObject)) ?
            ScriptableObject.CreateInstance(classDesc.ClassType) :
            Activator.CreateInstance(classDesc.ClassType);            

        // Create runtime methods.
        Methods= new WD_RuntimeMethod[classDesc.MethodInfos.Length];
        for(int i= 0; i < classDesc.MethodInfos.Length; ++i) {
            Methods[i]= WD_RuntimeMethod.CreateMethod(Instance, classDesc.MethodInfos[i].Invoke, classDesc.ParameterNames[i].Length);
        }
    }

}
