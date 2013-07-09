using UnityEngine;
using System.Collections;

public class iCS_SignatureDataSource {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    private   int              myIndex= 0;
    private   int              myCachedFrameId= -2;
    protected object[]         myParameters;
    protected object           myReturnValue;
    protected iCS_Connection[] myConnections;
    
    // ======================================================================
    // Functions to configure and access the signature parameters and 
    // return values.
    // ----------------------------------------------------------------------
//	public string GetParameterName(int idx) { return Name+"["+idx+"]"; }
	public object GetParameter(int idx) {
        var len= myParameters.Length;
        if(idx < len) return myParameters[idx];
        if(idx == len) return myReturnValue;
        return null;		
	}
	public void SetParameter(int idx, object value) {
        var len= myParameters.Length;
        if(idx < len)  {
            myParameters[idx]= value;
            return;
        }
        if(idx == len) {
            myReturnValue= value;
        }
	}
	public void SetParameterConnection(int idx, iCS_Connection connection) {
        if(idx >= myParameters.Length) return;
		myConnections[idx]= connection;
	}

    // ======================================================================
    // Functions to fetch the runtime inputs needed to execute the
    // associated action.
    // ----------------------------------------------------------------------
	public bool FetchParameters(int frameId) {
        if(myCachedFrameId != frameId) {
            myCachedFrameId= frameId;
            myIndex= 0;
        }
        var len= myConnections.Length;
	    for(; myIndex < len; ++myIndex) {
	        var c= myConnections[myIndex];
	        if(c != null) {
	            if(c.IsReady(frameId)) {
    	            myParameters[myIndex]= c.Value;
	            } else {
	                return false;
	            }
	        }
	    }
	    return true;
	}
//    public bool IsParameterReady(int idx, int frameId) {
//        var len= myParameters.Length;
//        if(idx >= len) return true;
//        if(myConnections[idx] == null) return true;
//        return myConnections[idx].IsReady(frameId);
//    }
	
//    // ======================================================================
//    // Accessors
//    // ----------------------------------------------------------------------
//    public object this[int idx] {
//        get { return GetParameter(idx); }
//        set { SetParameter(idx, value); }
//    }
}
