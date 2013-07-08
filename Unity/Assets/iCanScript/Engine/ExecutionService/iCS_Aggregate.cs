using UnityEngine;

public class iCS_Aggregate : iCS_ParallelDispatcher, iCS_IParams {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object[]  myParameters;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Aggregate(string name, int priority, int nbOfParameters= 0)
    : base(name, priority) {
        myParameters= new object[nbOfParameters];
    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get { return GetParameter(idx); }
        set { SetParameter(idx, value); }
    }
    // ======================================================================
    // IParams implementation
    // An aggregate only support value input parameters.  All other types of
    // parameters are ignored.
    // ----------------------------------------------------------------------
	public string GetParameterName(int idx) { return Name+"["+idx+"]"; }
	public object GetParameter(int idx) {
        return idx < myParameters.Length ? myParameters[idx] : null;		
	}
	public void SetParameter(int idx, object value) {
        if(idx < myParameters.Length)  { myParameters[idx]= value; return; }
	}
    public bool IsParameterReady(int idx, int frameId) {
        return true;
    }
	public void SetParameterConnection(int idx, iCS_Connection connection) {
	}
}
