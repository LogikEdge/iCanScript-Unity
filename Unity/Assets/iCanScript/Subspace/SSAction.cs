using UnityEngine;
using System.Collections;

namespace Subspace {
    
    // =========================================================================
    /// An action is the base class of the execution.  It includes a frame
    /// identifier that is used to indicate if the action has been run.  This
    /// indicator is the bases for the execution synchronization.
    public abstract class SSAction : SSActionBase {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        int     myPriority      = 0;
        int     myEvaluatedRunId= 0;
        int     myExecutedRunId = 0;
        bool    myIsStalled     = false;
        bool    myIsActive      = true;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public int  Priority            { get { return myPriority; } set { myPriority= value; }}
        public int  EvaluatedRunId      { get { return myEvaluatedRunId; }}
        public int  ExecutedRunId       { get { return myExecutedRunId; }}

        // ----------------------------------------------------------------------
        public bool IsWaiting           { get { return myEvaluatedRunId != Context.RunId; }}
        public bool IsEvaluated         { get { return myEvaluatedRunId == Context.RunId; }}
        public bool IsExecuted          { get { return myExecutedRunId == Context.RunId; }}
        public void MarkAsEvaluated()   { myEvaluatedRunId= Context.RunId; myIsStalled= false; }
        public void MarkAsExecuted()    { myExecutedRunId= Context.RunId; MarkAsEvaluated(); }

        // ----------------------------------------------------------------------
        public SSAction ParentAction    { get { return myParent as SSAction; } set { myParent= value; }}
        public bool     IsStalled       { get { return myIsStalled; } set { myIsStalled= value; }}
        public bool IsActive            {
            get {
                if(myIsActive == false) {
                    return false;
                }
                var pAction= ParentAction;
                return pAction == null ? true : pAction.IsActive;
            }
            set { myIsActive= value; }
        }
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSAction(string name, SSObject parent, int priority)
        : base(name, parent) {
            myPriority= priority;
        }
     
        // ======================================================================
        // Execution
        // ----------------------------------------------------------------------
        public abstract SSPullBinding      GetStalledProducerPort();
    
    }
    
}

