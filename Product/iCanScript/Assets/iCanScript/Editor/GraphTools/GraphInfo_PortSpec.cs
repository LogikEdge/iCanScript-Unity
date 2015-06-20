using UnityEngine;
using System;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class provides information about the iCanScript graph.
    public static partial class GraphInfo {

        // ===================================================================
		// VARIABLE TYPE ENUMS
		public enum TypeVariables {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
		};
		public enum ParameterVariable {
			Parameter= PortSpecification.Parameter
		};
        public enum InVariableType {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable,
            Constant=              PortSpecification.Constant
        };
        public enum InTypeVariableWithOwner {
            Owner=                 PortSpecification.Owner,
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum InFunctionDefinitionVariableType {
            Parameter=             PortSpecification.Parameter,
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable,
            Constant=              PortSpecification.Constant
        };
        public enum InUnityObjectVariableType {
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum InFunctionDefinitionUnityObjectVariableType {
            Parameter=             PortSpecification.Parameter,
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum InOwnerAndUnityObjectVariableType {
			Owner=				   PortSpecification.Owner,
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum InFunctionDefinitionOwnerAndUnityObjectVariableType {
            Parameter=             PortSpecification.Parameter,
			Owner=				   PortSpecification.Owner,
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable
        };
        public enum OutVariableType {
            LocalVariable=         PortSpecification.LocalVariable,
            PublicVariable=        PortSpecification.PublicVariable,
            PrivateVariable=       PortSpecification.PrivateVariable,
            StaticPublicVariable=  PortSpecification.StaticPublicVariable,
            StaticPrivateVariable= PortSpecification.StaticPrivateVariable            
        };
        public enum FredVariableType {
            LocalVariable=         PortSpecification.LocalVariable,
        };
        
        // ===================================================================
		/// Returns the allowed types of variable the given port can support.
		///
		/// @param port The port used to filter the types of variable.
		/// @return The filtered port specification.
		///
		public static Enum GetAllowedPortSpecification(iCS_EditorObject port) {
            var producerPort= GetProducerPort(port);
			if(MustBeATypeVariable(producerPort)) {
				return TypeVariables.PublicVariable;
			}
			if(MustBeAParameter(producerPort)) {
				return ParameterVariable.Parameter;
			}
            if(producerPort.IsInDataOrControlPort) {
                var isFunctionDefinition= producerPort.ParentNode.IsFunctionDefinition;
				var runtimeType= producerPort.RuntimeType;
                if(iCS_Types.IsA<UnityEngine.Object>(runtimeType)) {
					if(runtimeType == typeof(GameObject) ||
					   runtimeType == typeof(Transform)) {
                        if(isFunctionDefinition) {
    	                  	return InFunctionDefinitionOwnerAndUnityObjectVariableType.PublicVariable;
                        }
	                  	return InOwnerAndUnityObjectVariableType.PublicVariable;
					}
					else {
                        if(isFunctionDefinition) {
    	                    return InFunctionDefinitionUnityObjectVariableType.PublicVariable;
                        }
                        if(GraphInfo.IsLocalType(producerPort)) {
            				return InOwnerAndUnityObjectVariableType.Owner;                                
                        }
	                    return InUnityObjectVariableType.PublicVariable;
					}
                }
                else {
                    if(isFunctionDefinition) {
                        return InFunctionDefinitionVariableType.PublicVariable;
                    }
                    if(GraphInfo.IsLocalType(producerPort)) {
        				return InTypeVariableWithOwner.Owner;                           
                    }
                    return InVariableType.PublicVariable;
                }
            }
            else if(producerPort.IsOutDataOrControlPort) {
                return OutVariableType.PublicVariable;
            }
			
			return TypeVariables.PublicVariable;
		}
		
	}

}

