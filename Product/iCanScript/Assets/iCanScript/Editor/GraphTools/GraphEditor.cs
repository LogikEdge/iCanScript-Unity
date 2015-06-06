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

        // ======================================================================
        /// Sets the default port specification for the given port.
        ///
        /// @param p The port on which to set the default port specification.
        ///
        public static void SetDefaultPortSpec(iCS_EditorObject p) {
            // -- Setup spec for control ports. --
            var parentNode= p.ParentNode;
            var producerPort= GraphInfo.GetProducerPort(p);
            // -- Only valid for the producer port. --
            if(producerPort != p) { return; }
            // -- Determine default port spec. --
            if(parentNode.IsFunctionDefinition) {
                if(p.IsInDataOrControlPort) {
                    GraphEditor.SetPortSpec(p, PortSpecification.Parameter);
                }
            }
            else if(parentNode.IsEventHandler) {
                if(p.IsFixDataPort) {
                    GraphEditor.SetPortSpec(p, PortSpecification.Parameter);
                }
                else {
                    GraphEditor.SetPortSpec(p, PortSpecification.PublicVariable);
                }
            }
            else if(parentNode.IsVariableDefinition) {
                if(p.IsOutDataOrControlPort) {
                    GraphEditor.SetPortSpec(p, PortSpecification.PublicVariable);
                }
                else if(p.IsInDataOrControlPort) {
                    GraphEditor.SetPortSpec(p, PortSpecification.Constant);
                }
            }
            // TODO: Needs to be verified...
            else if(parentNode.IsKindOfFunction) {
                if(p.IsInDataOrControlPort) {
                    var initialValue= p.InitialValue;
                    if(initialValue != null) {
                        GraphEditor.SetPortSpec(p, PortSpecification.Constant);
                    }
                    else {
                        var runtimeType= p.RuntimeType;
                        if(runtimeType == typeof(GameObject) ||
                           runtimeType == typeof(Transform) ||
                           GraphInfo.IsLocalType(p)) {
                            GraphEditor.SetPortSpec(p, PortSpecification.Owner);
                            p.InitialValue= null;
                        }
                        else {
                            GraphEditor.SetPortSpec(p, PortSpecification.PublicVariable); 
                        }
                    }
                }
                else if(p.IsOutDataOrControlPort) {
                    if(GraphInfo.MustBeATypeVariable(p)) {
                        GraphEditor.SetPortSpec(p, PortSpecification.PrivateVariable);
                    }
                    else {
                        GraphEditor.SetPortSpec(p, PortSpecification.LocalVariable);
                    }
                }
            }
            else if(parentNode.IsInstanceNode) {
                if(p.IsInDataOrControlPort) {
                    var runtimeType= p.RuntimeType;
                    if(runtimeType == typeof(GameObject) ||
                       runtimeType == typeof(Transform) ||
                       GraphInfo.IsLocalType(p)) {
                        GraphEditor.SetPortSpec(p, PortSpecification.Owner);
                        p.InitialValue= null;
                    }
                    else {
                        GraphEditor.SetPortSpec(p, PortSpecification.PublicVariable); 
                    }
                }
            }
            else if(parentNode.IsKindOfPackage) {
                if(p.IsInDataOrControlPort) {
                    GraphEditor.SetPortSpec(p, PortSpecification.PublicVariable); 
                }
            }
            else {
                GraphEditor.SetPortSpec(p, PortSpecification.LocalVariable);
            }
        }

	}
	
}
