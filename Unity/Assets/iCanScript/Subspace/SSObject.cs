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
        public string   FullName    { get { return GetFullName("."); }}
        public SSObject Parent      { get { return myParent; }}
        public int      ParentId    { get { return myParent.InstanceId; }}
    
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

