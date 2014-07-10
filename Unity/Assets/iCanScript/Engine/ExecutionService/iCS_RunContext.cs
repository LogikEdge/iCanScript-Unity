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
                        var nodeName= node == null ? "unknown node" : node.FullName;
                        var port= node == null ? null : node.GetPortWithIndex(stalledProducerPort.PortIndex);
                        var portName= port == null ? "" : port.Name;
                        var waitingNodeName= stalledProducerPort.Signature.GetAssociatedNodeName();
//                        Debug.Log("Found stalled port=> "+nodeName+"."+portName);
                        node.IsActive= false;
                        Debug.LogWarning("Deactivating=> "+node.FullName+" ("+myFrameId+") NODE WAITING ON PORT=> "+waitingNodeName);
                        myAction.Execute(myFrameId);
                        Debug.LogWarning("Activating=> "+node.FullName+" ("+myFrameId+")");
                        node.IsActive= true;
                    }                    
                    else {
                        Debug.Log("DID NOT FIND STALLED PORT");
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
