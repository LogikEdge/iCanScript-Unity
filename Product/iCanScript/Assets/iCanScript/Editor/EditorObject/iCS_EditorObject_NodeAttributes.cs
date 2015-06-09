using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //  NODE ATTRIBUTES
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public partial class iCS_EditorObject {
        // Node specific attributes ---------------------------------------------
        public string MethodName {
    		get { return EngineObject.MethodName; }
    		set {
    			var engineObject= EngineObject;
                if(engineObject.MethodName == value) return;
    		    engineObject.MethodName= value;
    		}
    	}
        public int NbOfParams {
    		get { return EngineObject.NbOfParams; }
    		set {
    			var engineObject= EngineObject;
                if(engineObject.NbOfParams == value) return;
    		    engineObject.NbOfParams= value;
    		}
    	}
    	public string IconPath {
    		get {
    			var guid= IconGUID;
    			if(guid == null) return null;
    			return AssetDatabase.GUIDToAssetPath(guid);
    		}
    		set { 
    			if(string.IsNullOrEmpty(value)) {
    				IconGUID= null;
    				return;
    			}
    			var guid= TextureCache.IconPathToGUID(value);
    			if(guid != null) {
    				IconGUID= guid;
    			}
    		}
    	}
        public string IconGUID {
    		get { return EngineObject.IconGUID; }
    		set {
    			var engineObject= EngineObject;
    			if(engineObject.IconGUID == value) return;
    			engineObject.IconGUID= value;
    		}
    	}
        public MethodBase GetMethodBase(List<iCS_EditorObject> editorObjects) {
            return EngineObject.GetMethodBase(EditorToEngineList(editorObjects));
        }
        public FieldInfo GetFieldInfo() {
            return EngineObject.GetFieldInfo();
        }
    
    	// -------------------------------------------------------------------------
        /// Returns the target port of a node.
        ///
        /// @return The _'target'_ port if found. _'null'_ otherwise.
        ///
        public iCS_EditorObject TargetPort {
            get {
                iCS_EditorObject result= null;
                ForEachChildPort(
                    p=> {
                        if(p.PortIndex == (int)iCS_PortIndex.Target) {
                            result= p;
                        }
                    }
                );
                return result;
            }
        }
    	// -------------------------------------------------------------------------
        /// Returns the self port of a node.
        ///
        /// @return The _'target'_ port if found. _'null'_ otherwise.
        ///
        public iCS_EditorObject SelfPort {
            get {
                iCS_EditorObject result= null;
                ForEachChildPort(
                    p=> {
                        if(p.PortIndex == (int)iCS_PortIndex.Self) {
                            result= p;
                        }
                    }
                );
                return result;
            }
        }
    	// -------------------------------------------------------------------------
        /// Returns the return port of a node.
        ///
        /// @return The _'return'_ port if found. _'null'_ otherwise.
        ///
        public iCS_EditorObject ReturnPort {
            get {
                iCS_EditorObject result= null;
                ForEachChildPort(
                    p=> {
                        if(p.PortIndex == (int)iCS_PortIndex.Return) {
                            result= p;
                        }
                    }
                );
                return result;
            }
        }
    
    
        // =======================================================================
        // Proxy Node
        // -----------------------------------------------------------------------
        public int ProxyOriginalNodeId {
            get { return EngineObject.ProxyOriginalNodeId; }
            set { EngineObject.ProxyOriginalNodeId= value; }
        }
        public string ProxyOriginalVisualScriptTag {
            get { return EngineObject.ProxyOriginalVisualScriptTag; }
            set { EngineObject.ProxyOriginalVisualScriptTag= value; }
        }
    }
}

