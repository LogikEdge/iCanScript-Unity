using UnityEngine;
using System.Collections;

public class iCS_SignatureDataSource {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object           myThis         = null;  
    object[]         myParameters   = null;
    object           myReturnValue  = null;
    iCS_Connection[] myConnections  = null;
#if UNITY_EDITOR
    string[]         myNames        = null;
#endif
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int ReturnIndex     { get { return myParameters.Length; }}
    public int ThisIndex       { get { return myParameters.Length+1; }}
    public int NbOfParameters  { get { return myParameters.Length; }}
    public int NbOfConnections { get { return myConnections.Length; }}
    public object This {
        get { return myThis; }
        set { myThis= value; }
    }
    public object ReturnValue {
        get { return myReturnValue; }
        set { myReturnValue= value; }
    }
    public object[] Parameters {
        get { return myParameters; }
        set { myParameters= value; }
    }
    public iCS_Connection[] Connections {
        get { return myConnections; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_SignatureDataSource(int nbOfParameters, bool hasReturn, bool hasThis) {
        myParameters = new object[nbOfParameters];
        myConnections= new iCS_Connection[nbOfParameters+(hasThis ? 2 : 0)];
        for(int i= 0; i < nbOfParameters; ++i) {
            myParameters[i] = null;
            myConnections[i]= null;
        }
    }

    // ======================================================================
    // Functions to configure and access the signature parameters and 
    // return values.
    // ----------------------------------------------------------------------
#if UNITY_EDITOR
	public string GetName(int idx) {
        if(myNames == null || idx >= myNames.Length || myNames[idx] == null) {
            return "["+idx+"]";
        }
	    return myNames[idx];
	}
	public void SetParameterName(int idx, string value) {
        if(myNames == null) {
            var len= myConnections.Length;
            myNames= new string[len];
            for(int i= 0; i < len; ++i) {
                myNames[i]= null;
            }
        }
        if(idx >= myNames.Length) return;
        myNames[idx]= value;
	}
#endif
	public object GetValue(int idx) {
        var len= myParameters.Length;
        if(idx < len) return myParameters[idx];
        if(idx == len) return myReturnValue;
        if(idx == len+1) return myThis;
        return null;		
	}
	public void SetValue(int idx, object value) {
        var len= myParameters.Length;
        if(idx < len)  {
            myParameters[idx]= value;
            return;
        }
        if(idx == len) {
            myReturnValue= value;
            return;
        }
        if(idx == len+1) {
            myThis= value;
        }
	}
	public void SetConnection(int idx, iCS_Connection connection) {
        if(idx >= myConnections.Length) return;
		myConnections[idx]= connection;
	}
	public object GetThis() {
	    return myThis;
	}
    public void SetThis(object value) {
        myThis= value;
    }
    public object GetReturnValue() {
        return myReturnValue;
    }
    public void SetReturnValue(object value) {
        myReturnValue= value;
    }

    // ======================================================================
    // Functions to fetch the runtime inputs needed to execute the
    // associated action.
    // ----------------------------------------------------------------------
	public void ForcedFetchConnections() {
        var cLen= myConnections.Length;
        var pLen= myParameters.Length;
	    for(int i= 0; i < cLen; ++i) {
	        var c= myConnections[i];
	        if(c != null) {
	           if(i < pLen) {
        	       myParameters[i]= c.Value;	                    
	           } else if(i == pLen+1) {
	               myThis= c.Value;
	           }
	        }
	    }
	}
    // ----------------------------------------------------------------------
    // Returns true if all connections are ready
	public bool AreAllConnectionsReady(int frameId) {
        var cLen= myConnections.Length;
	    for(int i= 0; i < cLen; ++i) {
	        var c= myConnections[i];
	        if(c != null && !c.IsReady(frameId)) {
	                return false;
	        }
	    }
	    return true;
	}
    // ----------------------------------------------------------------------
    // Returns true if the given parameter is ready for the given frameId.
    public bool IsReady(int idx, int frameId) {
        return IsConnectionReady(idx, frameId);
    }
    // ----------------------------------------------------------------------
    // Returns true if the given parameter is ready for the given frameId.
    public bool IsConnectionReady(int idx, int frameId) {
        var len= myConnections.Length;
        if(idx >= len) return true;
        if(myConnections[idx] == null) return true;
        return myConnections[idx].IsReady(frameId);
    }
    // ----------------------------------------------------------------------
    // Returns the final value of a connection or port if no connection.
	public object FetchValue(int idx) {
        if(idx < 0 || idx >= myConnections.Length) return null;
	    var c= myConnections[idx];
        if(c == null) return GetValue(idx);
        return myConnections[idx].Value;
	}
	
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get { return GetValue(idx); }
        set { SetValue(idx, value); }
    }
}
