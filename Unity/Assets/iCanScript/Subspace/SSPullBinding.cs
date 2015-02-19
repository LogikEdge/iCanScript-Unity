using UnityEngine;
using System.Collections;

namespace Subspace {

    public class SSPullBinding : SSBinding {
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
        public string       PortFullName    { get { return Action.FullName+"["+myPortIndex+"]"; }}
    
        // ======================================================================
        /// Creation/Destruction
        // ----------------------------------------------------------------------
        public SSPullBinding(string name, SSObject parent,
                             SSNodeAction action, int portIndex, bool isAlwaysReady= false, bool isControlFlow= false)
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
            get {
                if(myIsAlwaysReady) return true;
                return Action.ArePortsEvaluated;                
            }
        }
        /// Returns **TRUE** if the connected _SSAction_ has been evaluated or executed for this _runId_.
        public bool IsEvaluated {
        	get { return Action.ArePortsEvaluated; }
        }
        /// Returns **TRUE** if the connected _SSAction_ has ran for this _runId_.
        public bool DidExecute {
        	get { return Action.ArePortsExecuted; }
        }
     }
    
}

