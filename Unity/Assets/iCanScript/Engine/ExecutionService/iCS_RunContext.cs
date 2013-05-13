public class iCS_RunContext {
    // ======================================================================
    // Fields
    int          myFrameId= 0;
    iCS_Action   myAction= null;
    
    // ======================================================================
    // Methods
    // ----------------------------------------------------------------------
    public iCS_RunContext(iCS_Action action) {
        myAction= action;
        myFrameId= 0;
    }

    // ----------------------------------------------------------------------
    // Executes the run context (if it is valid)
    void RunEvent() {
        if(myAction == null) return;
        do {
            myAction.Execute(myFrameId);                                
            if(myAction.IsStalled) {
                myAction.ForceExecute(myFrameId);
            }
        } while(!myAction.IsCurrent(myFrameId));        
    }
}
