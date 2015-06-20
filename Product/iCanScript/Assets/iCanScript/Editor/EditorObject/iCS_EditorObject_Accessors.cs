using UnityEngine;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ==================================================================
        // Port / Node accessor queries
        public bool IsStatic {
            get {
                if(IsDataPort) {
                    return IsStaticPrivateVariable || IsStaticPublicVariable;                    
                }
                if(IsNode) {
                    return IsStaticFunction;
                }
                return false;
            }
        }
        public bool IsPublic {
            get {
                if(IsDataPort) {
                    return IsPublicVariable || IsStaticPublicVariable;
                }
                if(IsNode) {
                    return IsPublicFunction;
                }
                return false;
            }
        }
        public bool IsProtected {
            get {
                if(IsNode) {
                    return IsProtectedFunction;
                }
                return false;
            }
        }
        public bool IsPrivate {
            get {
                if(IsDataPort) {
                    return IsPrivateVariable || IsStaticPrivateVariable;                    
                }
                if(IsNode) {
                    return IsPrivateFunction;
                }
                return false;
            }
        }
        
    	// ======================================================================
        // Port accessor queries.
        public bool IsOwner {
            get {
                if(!IsDataPort) return false;
                return PortSpec == PortSpecification.Owner;
            }
        }
        public bool IsConstant {
            get {
                if(!IsDataPort) return false;
                return PortSpec == PortSpecification.Constant;
            }
        }
        public bool IsPublicVariable {
            get {
                if(!IsDataPort) return false;
                return PortSpec == PortSpecification.PublicVariable;
            }
        }
        public bool IsStaticPublicVariable {
            get {
                if(!IsDataPort) return false;
                return PortSpec == PortSpecification.StaticPublicVariable;
            }
        }
        public bool IsPrivateVariable {
            get {
                if(!IsDataPort) return false;
                return PortSpec == PortSpecification.PrivateVariable;
            }
        }
        public bool IsStaticPrivateVariable {
            get {
                if(!IsDataPort) return false;
                return PortSpec == PortSpecification.StaticPrivateVariable;
            }
        }
        public bool IsTypeVariable {
            get {
                return IsConstant || IsPublicVariable || IsPrivateVariable
                    || IsStaticPublicVariable || IsStaticPrivateVariable;
            }
        }
        
    	// ======================================================================
        // Node accessor queries.
        public bool IsPublicFunction {
            get {
                if(!IsNode) return false;
                switch(NodeSpec) {
                    case NodeSpecification.Default:
                    case NodeSpecification.Constructor:
                    case NodeSpecification.StaticConstructor:
                    case NodeSpecification.PublicFunction:
                    case NodeSpecification.PublicStaticFunction:
                    case NodeSpecification.PublicVirtualFunction:
                    case NodeSpecification.PublicOverrideFunction:
                    case NodeSpecification.PublicNewFunction:
                    case NodeSpecification.PublicNewStaticFunction:
                        return true;
                }
                return false;
            }
        }
        public bool IsProtectedFunction {
            get {
                if(!IsNode) return false;
                switch(NodeSpec) {
                    case NodeSpecification.ProtectedFunction:
                    case NodeSpecification.ProtectedVirtualFunction:
                    case NodeSpecification.ProtectedOverrideFunction:
                    case NodeSpecification.ProtectedNewFunction:
                        return true;
                }
                return false;
            }
        }
        public bool IsPrivateFunction {
            get {
                if(!IsNode) return false;
                switch(NodeSpec) {
                    case NodeSpecification.PrivateFunction:
                    case NodeSpecification.PrivateStaticFunction:
                        return true;
                }
                return false;
            }
        }
        public bool IsVirtualFunction {
            get {
                if(!IsNode) return false;
                switch(NodeSpec) {
                    case NodeSpecification.PublicVirtualFunction:
                    case NodeSpecification.ProtectedVirtualFunction:
                        return true;
                }
                return false;
            }
        }
        public bool IsOverrideFunction {
            get {
                if(!IsNode) return false;
                switch(NodeSpec) {
                    case NodeSpecification.PublicOverrideFunction:
                    case NodeSpecification.ProtectedOverrideFunction:
                        return true;
                }
                return false;
            }
        }
        public bool IsNewFunction {
            get {
                if(!IsNode) return false;
                switch(NodeSpec) {
                    case NodeSpecification.PublicNewFunction:
                    case NodeSpecification.PublicNewStaticFunction:
                    case NodeSpecification.ProtectedNewFunction:
                    case NodeSpecification.ProtectedNewStaticFunction:
                        return true;
                }
                return false;
            }
        }

    }
}