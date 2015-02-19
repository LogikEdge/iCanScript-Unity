using UnityEngine;
using System.Collections;

namespace Subspace {

    public class SSBinding : SSObject {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        SSNodeAction   myAction       = null;
        int            myPortIndex    = -1;
        bool           myIsAlwaysReady= false;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public SSNodeAction Action          { get { return myAction; }}
        public int          PortIndex       { get { return myPortIndex; }}
        public string       PortFullName    { get { return Action.FullName+"["+Name+"]"; }}
    
        // ======================================================================
        /// Creation/Destruction
        // ----------------------------------------------------------------------
        public SSBinding(string name, SSObject parent,
                             SSNodeAction action, int portIndex, bool isAlwaysReady= false)
        : base(name, parent) {
            myAction       = action;
            myPortIndex    = portIndex;
            myIsAlwaysReady= isAlwaysReady;
        }

        /// Get/Sets the value associated with the connection.
        public object Value                 {
            get { return myAction.GetValue(PortIndex); }
            set { myAction.SetValue(PortIndex, value); }
        }
        /// Returns **TRUE** if the value is available for this _runId_.
        public bool IsReady    {
            get { return myIsAlwaysReady || IsEvaluated; }
        }
        /// Returns **TRUE** if the connected _SSAction_ has been evaluated or executed for this _runId_.
        public bool IsEvaluated {
        	get { return Action.ArePortsEvaluated; }
        }
        /// Returns **TRUE** if the connected _SSAction_ has ran for this _runId_.
        public bool IsExecuted {
        	get { return Action.ArePortsExecuted; }
        }
     }
    
}

