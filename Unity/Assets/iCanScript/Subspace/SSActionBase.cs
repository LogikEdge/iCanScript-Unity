namespace Subspace {

    // ====================================================================
    /// Defines the base class for _Subspace_ actions.
    ///
    /// SSBaseAction defines the context of an evaluation followed by an execution
    /// if the evaluation succeeds.  The _Evaluate()_ is defaulted to run the
    /// _Execute()_ if not overriden.
    public abstract class SSActionBase : SSObject {
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSActionBase(string name, SSObject parent)
            : base(name, parent) {}

        // ======================================================================
        // Execution
        // ----------------------------------------------------------------------
        public virtual  void Evaluate()     { Execute(); }
        public abstract void Execute();
    }
    
}
