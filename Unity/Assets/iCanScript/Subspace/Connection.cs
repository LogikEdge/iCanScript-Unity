using UnityEngine;
using System.Collections;
using Subspace;

namespace Subspace {

    public class Connection {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        ISignature  mySignature    = null;
        int         myPortIndex    = -1;
        bool        myIsAlwaysReady= false;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public SSAction Action {
            get { return mySignature != null ? mySignature.GetAction() : null; }
        }
        public SignatureDataSource Signature {
            get { return mySignature != null ? mySignature.GetSignatureDataSource() : null; }
        }
        public int PortIndex {
            get { return myPortIndex; }
        }
    //#if UNITY_EDITOR
        public string PortFullName {
            get {
                var nodeName= Action.FullName;
                var port= Action.GetPortWithIndex(myPortIndex);
                var portName= port == null ? "["+myPortIndex+"]" : port.Name;
                return nodeName+"."+portName;            
            }
        }
    //#endif
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public Connection() { }
        public Connection(ISignature signature, int portIndex, bool isAlwaysReady= false, bool isControlFlow= false) {
            mySignature    = signature;
            myPortIndex    = portIndex;
            myIsAlwaysReady= isAlwaysReady;
        }

        public bool IsConnected             { get{ return mySignature != null; }}
        public object Value                 {
            get { return Signature.GetValue(PortIndex); }
            set { Signature.SetValue(PortIndex, value); }
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
