using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_IStorage {
    	// ----------------------------------------------------------------------
        // This function will attempt to remove dupliacte ports on the same module.
        public void OptimizeDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort) {
            var provider= inPort.ProducerPort;
            if(provider == null) return;
            if(provider != outPort) {
                OptimizeDataConnection(provider, outPort);
            } 
            OptimizeDataConnection(provider);
        }
    	// ----------------------------------------------------------------------
        public void OptimizeDataConnection(iCS_EditorObject port) {
            iCS_EditorObject[] allConnections= port.ConsumerPorts;
            for(int i= 0; i < allConnections.Length-1; ++i) {
                for(int j= i+1; j < allConnections.Length; ++j) {
                    if(allConnections[i].ParentId == allConnections[j].ParentId && allConnections[i].DisplayName == allConnections[j].DisplayName) {
                        iCS_EditorObject[] portsToRelocate= allConnections[j].ConsumerPorts;
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

}
