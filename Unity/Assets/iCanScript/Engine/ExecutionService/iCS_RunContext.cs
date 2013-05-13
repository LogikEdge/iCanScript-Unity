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
    public void RunEvent() {
        if(myAction == null) return;
        ++myFrameId;
        do {
            myAction.Execute(myFrameId);                                
            if(myAction.IsStalled) {
                myAction.ForceExecute(myFrameId);
            }
        } while(!myAction.IsCurrent(myFrameId));        
    }
}
