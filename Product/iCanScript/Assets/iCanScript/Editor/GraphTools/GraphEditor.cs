using UnityEngine;
using System;
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
                AdjustPortIndexes(p.ParentNode);
			}
        }

		// ===================================================================
        /// Adjust the port index of the given node.
        ///
        /// @param node The node that needs to reorganize the port indexes.
        ///
        public static void AdjustPortIndexes(iCS_EditorObject node) {
            // -- Assure continuous port index for data ports. --
            if(!node.IsKindOfFunction) {
                var dataPorts= node.BuildListOfChildPorts(p=> p.IsDataPort && !p.IsTargetOrSelfPort && !p.IsReturnPort);
                Array.Sort(dataPorts,
                    (x,y)=> {
                        if(x.PortSpec == PortSpecification.Parameter &&
                           y.PortSpec != PortSpecification.Parameter) return -1;
                        if(x.PortSpec != PortSpecification.Parameter &&
                           y.PortSpec == PortSpecification.Parameter) return 1;
                        return x.PortIndex - y.PortIndex;
                    
                    }
                );
                for(int i= 0; i < dataPorts.Length; ++i) {
                    dataPorts[i].PortIndex= i;
                }                
            }
            
            // -- Assure continuous port index for enable ports. --
            var enablePorts= node.BuildListOfChildPorts(p=> p.IsEnablePort);
            Array.Sort(enablePorts, (x,y)=> x.PortIndex - y.PortIndex);
            for(int i= 0; i < enablePorts.Length; ++i) {
                enablePorts[i].PortIndex= i + (int)iCS_PortIndex.EnablesStart;
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
            else if(parentNode.IsConstructor) {
                if(p.IsInDataOrControlPort) {
                    GraphEditor.SetPortSpec(p, PortSpecification.Constant);
                }
                else if(p.IsOutDataOrControlPort) {
                    // -- Determine if this is a local variable of not. --
                    bool isLocal= false;
                    parentNode.ForEachChildPort(
                        cp=> {
                            if(cp.IsInDataOrControlPort) {
                                if(cp.ProducerPort != null) {
                                    isLocal= true;
                                }
                            }
                        }
                    );
                    if(isLocal) {
                        GraphEditor.SetPortSpec(p, PortSpecification.LocalVariable);
                    }
                    else {
                        GraphEditor.SetPortSpec(p, PortSpecification.PublicVariable);
                    }
                }
            }
            // TODO: Needs to be verified...
            else if(parentNode.IsKindOfFunction) {
                if(p.IsInDataOrControlPort) {
                    var initialValue= p.Value;
                    if(initialValue != null) {
                        GraphEditor.SetPortSpec(p, PortSpecification.Constant);
                    }
                    else {
                        var runtimeType= p.RuntimeType;
                        if(p.IsTargetPort
                           && (runtimeType == typeof(GameObject)
                           || runtimeType == typeof(Transform)
                           || GraphInfo.IsLocalType(p))) {
                            GraphEditor.SetPortSpec(p, PortSpecification.Owner);
                            p.Value= null;
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
                    if(p.IsTargetPort
                       && (runtimeType == typeof(GameObject)
                       || runtimeType == typeof(Transform)
                       || GraphInfo.IsLocalType(p))) {
                        GraphEditor.SetPortSpec(p, PortSpecification.Owner);
                        p.Value= null;
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
