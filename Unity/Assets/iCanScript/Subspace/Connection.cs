using UnityEngine;
using System.Collections;
using Subspace;

namespace Subspace {

    public class Connection {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        SSActionWithSignature   myAction       = null;
        int                     myPortIndex    = -1;
        bool                    myIsAlwaysReady= false;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public SSAction Action          { get { return myAction; }}
        public int      PortIndex       { get { return myPortIndex; }}
        public string   PortFullName    { get { return Action.FullName+"["+myPortIndex+"]"; }}
    
        // ======================================================================
        /// Creation/Destruction
        // ----------------------------------------------------------------------
        public Connection() { }
        public Connection(SSActionWithSignature action, int portIndex, bool isAlwaysReady= false, bool isControlFlow= false) {
            myAction       = action;
            myPortIndex    = portIndex;
            myIsAlwaysReady= isAlwaysReady;
        }

        /// Returns **TRUE** if this connection is connect to an _SSAction_.
        public bool IsConnected             { get{ return myAction != null; }}
        /// Get/Sets the value associated with the connection.
        public object Value                 {
            get { return myAction.GetValue(PortIndex); }
            set { myAction.SetValue(PortIndex, value); }
        }
        /// Returns **TRUE** if the value is available for this _runId_.
        public bool IsReady    {
            get {
                if(myIsAlwaysReady || !IsConnected) return true;
                return Action.ArePortsEvaluated;                
            }
        }
        /// Returns **TRUE** if the connected _SSAction_ has been evaluated or executed for this _runId_.
        public bool IsCurrent {
        	get { return Action.ArePortsEvaluated; }
        }
        /// Returns **TRUE** if the connected _SSAction_ has ran for this _runId_.
        public bool DidExecute {
        	get { return Action.ArePortsExecuted; }
        }
     }
    
}

