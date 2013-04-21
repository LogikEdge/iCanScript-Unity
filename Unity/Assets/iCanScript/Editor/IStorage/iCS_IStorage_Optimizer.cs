using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_IStorage {
	// ----------------------------------------------------------------------
    // This function will attempt to remove dupliacte ports on the same module.
    public void OptimizeDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort) {
        var source= inPort.Source;
        if(source == null) return;
        if(source != outPort) {
            OptimizeDataConnection(source, outPort);
        } 
        OptimizeDataConnection(source);
    }
	// ----------------------------------------------------------------------
    public void OptimizeDataConnection(iCS_EditorObject port) {
        iCS_EditorObject[] allConnections= port.Destinations;
        for(int i= 0; i < allConnections.Length-1; ++i) {
            for(int j= i+1; j < allConnections.Length; ++j) {
                if(allConnections[i].ParentId == allConnections[j].ParentId && allConnections[i].Name == allConnections[j].Name) {
                    iCS_EditorObject[] portsToRelocate= allConnections[j].Destinations;
                    foreach(var toRelocate in portsToRelocate) {
                        SetSource(toRelocate, allConnections[i]);
                    }
                    DestroyInstance(allConnections[j]);
                    OptimizeDataConnection(port);
                    return;
                }
            }
        }
    }
}
