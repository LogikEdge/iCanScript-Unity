using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

	public static class GraphEditor {

		// ===================================================================
        /// Sets the port specififcation.
		///
		/// @param portSpec The new port specification.
		///
        public static void SetPortSpec(iCS_EditorObject vsObject, PortSpecification portSpec) {
			var allConnectedPorts= GraphInfo.GetAllConnectedPorts(vsObject);
			foreach(var p in allConnectedPorts) {
                if(p.IsEnablePort) {
                    p.PortSpec= PortSpecification.Enable;
                }
                else {
    				p.PortSpec= portSpec;
                }
			}
        }

	}
	
}
