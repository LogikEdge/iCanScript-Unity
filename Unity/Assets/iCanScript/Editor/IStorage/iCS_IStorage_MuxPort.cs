using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	void CleanupMuxPort(iCS_EditorObject muxPort) {
		// Make certain we have the parent mux port.
		while(muxPort != null && muxPort.IsPort && !muxPort.IsParentMuxPort) muxPort= muxPort.Parent;
		if(!muxPort.IsParentMuxPort) return;
		// Determine # of child mux ports.
		int nbChildPorts= 0;
		iCS_EditorObject aChild= null;
		muxPort.ForEachChildPort(
			c=> {
				++nbChildPorts;
				aChild= c;
			}
		);
		// Remove mux port if no children exist.
		if(nbChildPorts == 0) {
			DestroyInstance(muxPort);
			IsDirty= true;
			return;
		}
		// Transform mux port to standard dynamic port if only one child port exist.
		if(nbChildPorts == 1) {
			var source= aChild.Source;
			DestroyInstance(aChild);
			if(source != null) {
				muxPort.ObjectType= iCS_ObjectTypeEnum.OutDynamicDataPort;
				SetSource(muxPort, source);				
			} else {
				DestroyInstance(muxPort);
			}
			IsDirty= true;
			return;
		}
	}
}
