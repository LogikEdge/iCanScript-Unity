using UnityEngine;
using System.Collections;

namespace Subspace {
    
    public class SSObject {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        protected string    myName      = null;
        protected SSObject  myParent    = null;
        protected SSContext myContext   = null;

        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public string       Name        { get { return myName; }}
        public string       FullName    { get { return GetFullName("/"); }}
        public SSObject     Parent      { get { return myParent; }}
        public SSContext    Context     { get { return myContext; }}
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSObject(string name, SSObject parent, SSContext context) {
            myName      = name;
			myParent    = parent;
            myContext   = context;
        }

        // ----------------------------------------------------------------------
        /// Returns the absolute name using the given separator between levels.
        public string GetFullName(string separator= ".") {
            var parentName= myParent != null ? myParent.GetFullName(separator) : "";
            return parentName+separator+myName;
        }

        // ----------------------------------------------------------------------
        /// Allow for printing using standard IO stream.
        public override string ToString() {
            return Name;
        }
    }
    
}

