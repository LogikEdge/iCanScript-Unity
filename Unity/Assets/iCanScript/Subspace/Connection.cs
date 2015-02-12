using UnityEngine;
using System.Collections;
using Subspace;

namespace Subspace {

    public class Connection {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        SSActionWithSignature  myAction    = null;
        int         myPortIndex    = -1;
        bool        myIsAlwaysReady= false;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public SSAction Action          { get { return myAction; }}
        public int      PortIndex       { get { return myPortIndex; }}
        public string   PortFullName    { get { return Action.FullName+"["+myPortIndex+"]"; }}
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public Connection() { }
        public Connection(SSActionWithSignature action, int portIndex, bool isAlwaysReady= false, bool isControlFlow= false) {
            myAction       = action;
            myPortIndex    = portIndex;
            myIsAlwaysReady= isAlwaysReady;
        }

        public bool IsConnected             { get{ return myAction != null; }}
        public object Value                 {
            get { return myAction.GetValue(PortIndex); }
            set { myAction.SetValue(PortIndex, value); }
        }
        public bool IsReady(int runId)    {
            if(myIsAlwaysReady || !IsConnected) return true;
            return Action.ArePortsCurrent(runId);
        }
        public bool IsCurrent(int runId) {
        	return Action.ArePortsCurrent(runId);
        }
        public bool DidExecute(int runId) {
        	return Action.ArePortsExecuted(runId);
        }
     }
    
}

