//#define EXECUTION_TRACE
using UnityEngine;

public class iCS_RunContext {
    // ======================================================================
    // Fields
    int          myFrameId= 0;
    iCS_Action   myAction= null;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_Action Action {
        get { return myAction; }
        set { myAction= value; myFrameId= 0; }
    }
    public int FrameId {
        get { return myFrameId; }
    }
    
    // ======================================================================
    // Methods
    // ----------------------------------------------------------------------
    public iCS_RunContext(iCS_Action action) {
        Action= action;
    }

    // ----------------------------------------------------------------------
    // Executes the run context (if it is valid)
    public void Run() {
        if(myAction == null) return;
        ++myFrameId;
        int retry= 0;
        do {
            myAction.Execute(myFrameId);                                
            if(myAction.IsStalled) {
                if(++retry < 100) {
                    var stalledProducerPort= myAction.GetStalledProducerPort(myFrameId);
                    if(stalledProducerPort != null) {
                        var node= stalledProducerPort.Action;
#if UNITY_EDITOR
                        if(myAction.VisualScript.IsTraceEnabled) {
                        
//                            var nodeName= nodeName == null ? "unknown node" : node.FullName;
//                            var port= node == null ? null : node.GetPortWithIndex(stalledProducerPort.PortIndex);
//                            var portName= port == null ? "" : port.Name;
                            var waitingNodeName= stalledProducerPort.Signature.GetAssociatedNodeName();
//                            Debug.Log("Found stalled port=> "+nodeName+"."+portName);
                            Debug.LogWarning("Deactivating=> "+node.FullName+" ("+myFrameId+") NODE WAITING ON PORT=> "+waitingNodeName);
                        }
#endif
                        node.IsActive= false;
                        myAction.Execute(myFrameId);
                        node.IsActive= true;
#if UNITY_EDITOR
                        if(myAction.VisualScript.IsTraceEnabled) {
                            Debug.LogWarning("Activating=> "+node.FullName+" ("+myFrameId+")");
                        }
#endif
                    }                    
                    else {
#if UNITY_EDITOR
                        if(myAction.VisualScript.IsTraceEnabled) {
                            Debug.Log("EXECUTION RECOVERY: DID NOT FIND STALLED PORT");
                        }
#endif
                        myAction.ForceExecute(myFrameId);                    
                    }
                }
                else {
                    myAction.ForceExecute(myFrameId);                    
                }
            }
        } while(!myAction.IsCurrent(myFrameId));        
    }
}
