using UnityEngine;
using System.Collections;

namespace iCanScript.Editor {
    
    public partial class iCS_IStorage {
        // -------------------------------------------------------------------------------
    	public void CleanupMuxPort(iCS_EditorObject port) {
    		// Make certain we have the parent mux port.
            while(port.IsChildMuxPort) port= port.Parent;
            if(!port.IsParentMuxPort) {
                Debug.LogWarning("iCanScript: Invalid object given in CleanupMuxPort");
                return;
            }
    		// Determine # of child mux ports.
    		int nbChildPorts= 0;
    		iCS_EditorObject aChild= null;
    		port.ForEachChildPort(
    			c=> {
    				++nbChildPorts;
    				aChild= c;
    			}
    		);
    		// Remove mux port if no children exist.
    		if(nbChildPorts == 0) {
    			DestroyInstance(port);
    			return;
    		}
    		// Transform mux port to standard dynamic port if only one child port exist.
    		if(nbChildPorts == 1) {
    			var source= aChild.ProducerPort;
    			DestroyInstance(aChild);
    			if(source != null) {
    				port.ObjectType= port.IsOutMuxPort ? iCS_ObjectTypeEnum.OutDynamicDataPort : iCS_ObjectTypeEnum.InDynamicDataPort;
    				SetSource(port, source);
                    RecalculatePortIndexes(port.ParentNode);
    			} else {
    				DestroyInstance(port);
    			}
    			return;
    		}
    		// Adjust the indexes of parent & child ports.
            int idx= 0;
            port.PortIndex= (int)iCS_PortIndex.Return;
            port.ForEachChildPort(
                c=> {
                    c.PortIndex= idx++;
                }
            );
    	}
    }

}
