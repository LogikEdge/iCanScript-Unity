using UnityEngine;
using System.Collections;

namespace Subspace {
    
    /// =========================================================================
    // An action is the base class of the execution.  It includes a frame
    // identifier that is used to indicate if the action has been run.  This
    // indicator is the bases for the execution synchronization.
    public abstract class SSAction : SSObject {
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        int     myPriority      = 0;
        int     myEvaluatedRunId= 0;
        int     myExecutedRunId = 0;
        bool    myIsStalled     = false;
        bool    myIsActive      = true;
        bool    myPortsAreAlwaysCurrent= false;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public int      Priority            { get { return myPriority; } set { myPriority= value; }}
        public int      EvaluatedRunId      { get { return myEvaluatedRunId; }}
        public int      ExecutedRunId       { get { return myExecutedRunId; }}
        public bool     IsStalled           { get { return myIsStalled; } set { myIsStalled= value; }}
        public SSAction ParentAction        { get { return myParent as SSAction; } set { myParent= value; }}
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
        public bool ArePortsAlwaysCurrent {
			get { return myPortsAreAlwaysCurrent; }
			set { myPortsAreAlwaysCurrent= value; }
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
        public abstract void            Evaluate();
        public abstract void            Execute();
        public abstract Connection      GetStalledProducerPort();
    
        // ----------------------------------------------------------------------
        public bool IsWaiting           { get { return myEvaluatedRunId != myContext.RunId; }}
        public bool IsDisabled          { get { return IsEvaluated && !IsExecuted; }}
        public bool IsEvaluated         { get { return myEvaluatedRunId == myContext.RunId; }}
        public bool IsExecuted          { get { return myExecutedRunId == myContext.RunId; }}
        public void MarkAsEvaluated()   { myEvaluatedRunId= myContext.RunId; myIsStalled= false; }
        public void MarkAsExecuted()    { myExecutedRunId= myContext.RunId; MarkAsEvaluated(); }

        // ----------------------------------------------------------------------
        public bool ArePortsEvaluated   { get { return IsEvaluated || ArePortsAlwaysCurrent || !IsActive; }}
        public bool ArePortsExecuted    { get { return IsExecuted; }}
    }
    
}

