using UnityEngine;
using System.Collections;

namespace Subspace {
    
    // ==========================================================================
    /// Defines the base class for all Objects in Subspace.
    ///
    /// All objects in Subspace are part of a hierarchy and have a parent.  In
    /// addition, all objects referer a SSContext to share information within
    /// the group of objects it belongs to.  By default, the parent SSContext
    /// is used.
    public class SSObject {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        protected string    myName      = null;
        protected SSObject  myParent    = null;
        private   SSContext myContext   = null;

        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public string       Name        { get { return myName; }}
        public string       FullName    { get { return GetFullName("/"); }}
        public SSObject     Parent      { get { return myParent; }}
        public SSContext    Context     {
            get { return myContext ?? myParent.Context; }
            set { myContext= value; }
        }
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSObject(string name, SSObject parent) {
            myName   = name;
			myParent = parent;
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

