using UnityEngine;
using System.Collections;

namespace Subspace {
    
    public class SSObject {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        protected int       myInstanceId= -1;
        protected string    myName      = null;
        protected SSObject  myParent    = null;
        protected iCS_VisualScriptImp  VisualScript    { get; set; }

        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public int      InstanceId  { get { return myInstanceId; }}
        public string   Name        { get { return myName; }}
        public SSObject Parent      { get { return myParent; }}
        public int      ParentId    { get { return myParent.InstanceId; }}
        public string   FullName    { get { return GetFullName("."); }}

        public iCS_EngineObject EngineObject {
            get {
                int id= InstanceId;
                return id != -1 ? VisualScript.EngineObjects[InstanceId] : null;
            }
        }
        public string           TypeName        { get { return GetType().Name; }}

    //#if UNITY_EDITOR
        public iCS_EngineObject GetPortWithIndex(int idx) {
            var ourId= InstanceId;
            for(int i= 0; i < VisualScript.EngineObjects.Count; ++i) {
                var eObj= VisualScript.EngineObjects[i];
                if(eObj != null && eObj.InstanceId != -1) {
                    if(eObj.IsPort && eObj.ParentId == ourId && eObj.PortIndex == idx) {
                        return eObj;
                    }
                }
            }
            return null;
        }
    //#endif
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSObject(int instanceId, string name, iCS_VisualScriptImp visualScript) {
            myInstanceId= instanceId;
            myName      = name;
            VisualScript= visualScript;
        }

        // ----------------------------------------------------------------------
        // Returns the absolute name using the given separator between levels.
        public string GetFullName(string separator= ".") {
            var parentName= myParent != null ? myParent.GetFullName(separator) : "";
            return parentName+separator+myName;
        }

        // ----------------------------------------------------------------------
        public override string ToString() {
            return Name;
        }
    }
    
}
