using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public static class iCS_PortUtility {

    // -----------------------------------------------------------------------
    /// Assures that the parameter index of each port are properly set.
    public static void RepairPorts(iCS_IVisualScriptData vsd, iCS_EngineObject obj) {
        // Extract and classify ports.
        var ports= iCS_VisualScriptData.GetChildPorts(vsd, obj);
        var parameterPorts= P.filter(p=> p.IsDataPort && !p.IsInstancePort, ports);
        var enablePorts   = P.filter(p=> p.IsEnablePort, ports);

        // Move all uninitialized indexes at the end.
        int badIndex= 10000;
        foreach(var p in parameterPorts) {
            if((int)p.PortIndex < (int)iCS_PortIndex.ParametersStart) {
                p.PortIndex= (int)iCS_PortIndex.ParametersStart + (++badIndex);
            }
        }
        foreach(var p in enablePorts) {
            if((int)p.PortIndex < (int)iCS_PortIndex.EnablesStart) {
                p.PortIndex= (int)iCS_PortIndex.EnablesStart + (++badIndex);
            }
        }
        
        // Sort parameters & enable ports.
        Comparison<iCS_EngineObject> indexCompare= (x,y)=> (int)x.PortIndex - (int)y.PortIndex;
        Array.Sort(parameterPorts, indexCompare);
        Array.Sort(enablePorts   , indexCompare);

        // Assure continuous and proper index sequence
        for(int i= 0; i < parameterPorts.Length; ++ i) {
            parameterPorts[i].PortIndex= (int)iCS_PortIndex.ParametersStart + i;
        }
        for(int i= 0; i < enablePorts.Length; ++i) {
            enablePorts[i].PortIndex= (int)iCS_PortIndex.EnablesStart + i;
        }
    }
}
