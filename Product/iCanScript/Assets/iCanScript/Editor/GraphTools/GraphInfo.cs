using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P= iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class provides information about the iCanScript graph.
    public static partial class GraphInfo {

        // ===================================================================
		/// Returns the _Target_ port on the given node.
		///
		/// @param node The node to seach.
		/// @return The Target port if found. _null_ otherwise.
		///
		public static iCS_EditorObject GetTargetPort(iCS_EditorObject node) {
			return node.TargetPort;
		}
		
        // ===================================================================
		/// Returns the port that produces the data.
		///
		/// @param port One of the connected ports.
		/// @return The port that produces the data.
		///
		public static iCS_EditorObject GetProducerPort(iCS_EditorObject port) {
            var producerPort= port.SegmentProducerPort;
            // -- Follow the target/self port chain. --
            while(producerPort.IsSelfPort) {
                var targetPort= GetTargetPort(producerPort.ParentNode);
                if(targetPort == null) {
                    break;
                }
                producerPort= targetPort.SegmentProducerPort;                    
            }
			// Follow the trigger/enable chain.
            while(producerPort.IsTriggerPort && producerPort.ParentNode.IsKindOfPackage) {
                var package= producerPort.ParentNode;
                var nestedFunctions= VSStructure.GetListOfFunctions(package);
                if(nestedFunctions.Count != 0) {
                    // FIXME: Need to create a trigger variable.
                    return producerPort;
                }
                var enables= ControlFlow.GetEnablePorts(package);
                switch(enables.Length) {
                    case 0: {
                        // FIXME: Should remove enable to nothing.
                        return producerPort;
                    }
                    case 1: {
                        return GetProducerPort(enables[0]);
                    }
                    default: {
                        // FIXME: Need to create a trigger variable.
                        return producerPort;
                    }
                }
            }
            return producerPort;			
		}
		
        // ===================================================================
		/// Returns a list of all ports in the connection.
		///
		/// @param port One of the connected ports.
		/// @return The list of all ports in the connection.
		///
		public static iCS_EditorObject[] GetAllConnectedPorts(iCS_EditorObject port) {
			var allPorts= new List<iCS_EditorObject>();
			// -- Follow the end consumer chain. --
			var consumerPorts= new List<iCS_EditorObject>();
			var nextConsumerPorts= new List<iCS_EditorObject>();
			consumerPorts.Add(GetProducerPort(port));
			do {
				foreach(var p in consumerPorts) {
					allPorts.Add(p);
					// -- Bridge the Target/Self ports. --
					if(p.IsTargetPort) {
						var selfPort= p.ParentNode.SelfPort;
						if(selfPort != null) {
							nextConsumerPorts.Add(selfPort);
						}
					}
					else {
						nextConsumerPorts.AddRange(p.ConsumerPorts);
					}
				}				
				consumerPorts.Clear();
				consumerPorts.AddRange(nextConsumerPorts);
				nextConsumerPorts.Clear();
			} while(consumerPorts.Count != 0);
			return allPorts.ToArray();
		}
		
        // ===================================================================
        /// Builds a list of parent nodes.
        ///
        /// The list is sorted from the top most parent to the bottom most leaf.
        ///
        /// @param vsObject The object from which to build the list.
        /// @return An array of parents where index 0 is the top most parent.
        //
        public static iCS_EditorObject[] BuildListOfParentNodes(iCS_EditorObject vsObject) {
            var result= new List<iCS_EditorObject>();
            for(var parent= vsObject.ParentNode; parent != null; parent= parent.ParentNode) {
                result.Add(parent);
            }
            return P.reverse(result).ToArray();
        }

        // ===================================================================
        /// Return the first shared parent between two visual script objects.
        ///
        /// @param vsObj1 The first visual script object.
        /// @param vsObj2 The second visual script object.
        /// @return The first shared parent.
        ///
        public static iCS_EditorObject GetCommonParent(iCS_EditorObject vsObj1, iCS_EditorObject vsObj2) {
            if(vsObj1 == null) return vsObj2;
            if(vsObj2 == null) return vsObj1;
            var l1= BuildListOfParentNodes(vsObj1);
            var l2= BuildListOfParentNodes(vsObj2);
            l1= P.insertAt(vsObj1, l1.Length, l1);
            l2= P.insertAt(vsObj2, l2.Length, l2);
            iCS_EditorObject commonParent= null;
            for(int i= 0; i < l1.Length && i < l2.Length; ++i) {
                if(l1[i] != l2[i]) break;
                commonParent= l1[i];
            }
            return commonParent;
        }

        // ===================================================================
        /// Returns the common parent of a list of visual script objects.
        ///
        /// @param vsObjects The list of visual script objects.
        /// @return The first shared parent.
        ///
        public static iCS_EditorObject GetCommonParent(iCS_EditorObject[] vsObjects) {
            if(vsObjects == null) return null;
            var len= vsObjects.Length;
            if(len == 0) return null;
            if(len == 1) return vsObjects[0];
            var commonParent= vsObjects[0];
            for(int i= 1; i < len; ++i) {
                commonParent= GetCommonParent(commonParent, vsObjects[i]);
            }
            return commonParent;
        }

        // ----------------------------------------------------------------------
    	/// Determines if the vsObject refers to an element of the given type.
    	///
    	/// @param vsObj The object on which the search occurs.
    	///
    	public static bool IsLocalType(iCS_EditorObject vsObj) {
    		// First determine if the type is included inside the GameObject.
            var iStorage= vsObj.IStorage;
            var baseType= CodeGenerationUtility.GetBaseType(iStorage);
    		if(vsObj.IsIncludedInType(baseType)) return true;
    		var typeNode= vsObj.ParentTypeNode;
    		if(typeNode == null) return false;
    		if(vsObj.Namespace == CodeGenerationUtility.GetNamespace(iStorage)) {
    			return vsObj.TypeName == typeNode.CodeName;
    		}
    		return false;
    	}

        // ===================================================================
        /// Determine if the given output port must be promoted to a type
        /// variable.
        ///
        /// @param port The output port to be verified.
        /// @return _True_ if the only valid variable type for this port is
        ///         a type variable.
        ///
        public static bool MustBeATypeVariable(iCS_EditorObject port) {
            var commonParent= GetCommonParent(GetAllConnectedPorts(port));
            commonParent= GetCommonParent(port, commonParent);
            return commonParent.IsTypeDefinitionNode;
        }

        // ===================================================================
        /// Determines if the given output port must be a parameter.
        ///
        /// @param port The output port to be verified.
        /// @return _True_ if the given port must be a parameter.
        ///         _False_ otherwise.
        ///
        public static bool MustBeAParameter(iCS_EditorObject port) {
            var consumerPorts= port.SegmentEndConsumerPorts;
            foreach(var p in consumerPorts) {
                if(p.IsFixDataPort && p.ParentNode.IsEventHandler) {
                    return true;
                }
            }
            return false;
        }

        // ===================================================================
        /// Determines if the given output port can be a parameter.
        ///
        /// @param port The output port to be verified.
        /// @return _True_ if the given port can be a parameter.
        ///         _False_ otherwise.
        ///
        public static bool CanBeAParameter(iCS_EditorObject port) {
            var consumerPorts= port.SegmentEndConsumerPorts;
            foreach(var p in consumerPorts) {
                var parentNode= p.ParentNode;
                if(p.IsFixDataPort && parentNode.IsEventHandler) {
                    return true;
                }
                if(parentNode.IsFunctionDefinition) {
                    return true;
                }
            }
            return false;
        }

    }
    
}
