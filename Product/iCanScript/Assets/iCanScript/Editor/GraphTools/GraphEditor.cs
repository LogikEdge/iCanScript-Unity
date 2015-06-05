using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

	public static class GraphEditor {

		// ===================================================================
        /// Sets the port specififcation.
		///
		/// @param vsObject An object that is part of the connection.
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

		// ===================================================================
        /// Refreshes the port specififcation of a connection.
		///
		/// @param vsObject An object that is part of the connection.
		///
        public static void RefreshPortSpec(iCS_EditorObject vsObject) {
            var producerPort= GraphInfo.GetProducerPort(vsObject);
            SetPortSpec(producerPort, producerPort.PortSpec);
        }
	}
	
}
