using UnityEngine;
using System;
using System.Collections;

public class iCS_SignatureDataSource {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    // .NET Signature
    object              myThis                  = null;  
    object[]            myParameters            = null;
    object              myReturnValue           = null;
    iCS_Connection[]    myParameterConnections  = null;
    iCS_Connection      myThisConnection        = null;
    // Extended Signature
    object              myOutThis               = null;
    // Controls
    bool                myTrigger               = false;
    bool[]              myEnables               = null;
    iCS_Connection[]    myEnableConnections     = null;
    
#if UNITY_EDITOR
    string[]         myParameterNames= null;
#endif
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
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
    }
    public iCS_Connection[] ParameterConnections {
        get { return myParameterConnections; }
    }
    public object OutThis {
        get { return myOutThis; }
        set { myOutThis= value; }
    }
    public bool Trigger {
        get { return myTrigger; }
        set { myTrigger= value; }
    }
    public bool[] Enables {
        get { return myEnables; }
    }
    public iCS_Connection[] EnableConnections {
        get { return myEnableConnections; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_SignatureDataSource(int nbOfParameters) {
        myParameters = new object[nbOfParameters];
        myParameterConnections= new iCS_Connection[nbOfParameters];
        for(int i= 0; i < nbOfParameters; ++i) {
            myParameters[i]= null;
            myParameterConnections[i]= null;
        }
    }

    // ======================================================================
    // Functions to configure and access the signature parameters and 
    // return values.
    // ----------------------------------------------------------------------
#if UNITY_EDITOR
	public string GetName(int idx) {
        if(myParameterNames == null || idx >= myParameterNames.Length || myParameterNames[idx] == null) {
            return "["+idx+"]";
        }
	    return myParameterNames[idx];
	}
	// -------------------------------------------------------------------------
    public void SetParameterName(int idx, string value) {
        // Dynamically allocate parameter name array.
        if(myParameterNames == null) {
            var len= myParameters.Length;
            myParameterNames= new string[len];
            for(int i= 0; i < len; ++i) {
                myParameterNames[i]= null;
            }
        }
        if(idx < myParameterNames.Length) {
            myParameterNames[idx]= value;
        }
        if(idx != (int)iCS_PortIndex.This) {
            Debug.LogWarning("iCanScript: Trying to set signature name with wrong index: "+idx+" name= "+value);
        }
	}
#endif
	// -------------------------------------------------------------------------
    public object GetValue(int idx) {
        if(idx < myParameters.Length) return myParameters[idx];
        if(idx == (int)iCS_PortIndex.Return) return myReturnValue;
        if(idx == (int)iCS_PortIndex.This) return myThis;
		if(idx == (int)iCS_PortIndex.OutThis) return myOutThis;
		if(idx == (int)iCS_PortIndex.Trigger) return myTrigger;
		if(idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
			return GetEnable(idx);
		}
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to get a signature value with wrong index: "+idx);
#endif
        return null;		
	}
	// -------------------------------------------------------------------------
    public void SetValue(int idx, object value) {
        if(idx < myParameters.Length)  {
            myParameters[idx]= value;
            return;
        }
        if(idx == (int)iCS_PortIndex.Return) {
            myReturnValue= value;
            return;
        }
        if(idx == (int)iCS_PortIndex.This) {
            myThis= value;
            return;
        }
		if(idx == (int)iCS_PortIndex.OutThis) {
			myOutThis= value;
			return;
		}
		if(idx == (int)iCS_PortIndex.Trigger) {
			myTrigger= value;
			return;
		}
		if(idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
			SetEnable(idx, value);
			return;
		}
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to set a signature value with wrong index: "+idx);
#endif
	}
	// -------------------------------------------------------------------------
    public object GetParameter(int idx) {
        if(idx < myParameters.Length) return myParameters[idx];
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to get a signature value with wrong index: "+idx);
#endif
        return null;		        
    }
	// -------------------------------------------------------------------------
    public void SetParameter(int idx, object value) {
        if(idx < myParameters.Length)  {
            myParameters[idx]= value;
            return;
        }
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to set a signature value with wrong index: "+idx);
#endif        
    }
	// -------------------------------------------------------------------------
	public bool GetEnable(int idx) {
		if(myEnables == null) return true;
		var i= idx-(int)iCS_PortIndex.EnablesStart;
		if(i >= myEnables.Length) return true;
		return myEnables[i];
	}
	// -------------------------------------------------------------------------
	public void SetEnable(int idx, bool value) {
		if(myEnables == null) return;
		var i= idx-(int)iCS_PortIndex.EnablesStart;
		if(i >= myEnables.Length) return;
		myEnables[i]= value;
	}
	// -------------------------------------------------------------------------
    public iCS_Connection GetConnection(int idx) {
        if(idx < myParameterConnections.Length) return myParameterConnections[idx];
        if(idx == (int)iCS_PortIndex.This) return myThisConnection;
		if(idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
			var i= idx-(int)iCS_PortIndex.EnablesStart;
			if(myEnableConnections != null && i < myEnableConnections.Length) {
				return myEnableConnections[i];
			}
		}
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to get a signature connection with wrong index: "+idx);
#endif
        return null;
    }
    // -------------------------------------------------------------------------
    public void SetConnection(int idx, iCS_Connection connection) {
        if(idx < myParameterConnections.Length) {
    		myParameterConnections[idx]= connection;            
            return;
        }
        if(idx == (int)iCS_PortIndex.This) {
            myThisConnection= connection;
            return;
        }
		if(idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
			var i= idx-(int)iCS_PortIndex.EnablesStart;
			if(myEnableConnections != null && i < myEnableConnections.Length) {
				myEnableConnections[i]= connection;
			}
			return;
		}
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to set a signature connection with wrong index: "+idx);
#endif
	}

    // ======================================================================
    // Iteration
    // ----------------------------------------------------------------------
    public bool ForEachConnection(Func<int,iCS_Connection,bool> test) {
        if(myThisConnection != null) {
            if(test((int)iCS_PortIndex.This, myThisConnection) == false) {
                return false;
            }
        }
        if(ForEachParameterConnection(test) == false) {
			return false;
		}
		return ForEachEnableConnection(test);
    }
    // ----------------------------------------------------------------------
    public bool ForEachParameterConnection(Func<int,iCS_Connection,bool> test) {
        var cLen= myParameterConnections.Length;
	    for(int i= 0; i < cLen; ++i) {
	        var connection= myParameterConnections[i];
	        if(connection != null) {
        	    if(test(i, connection) == false) {
        	        return false;	                    
    	        }
	        }
	    }
	    return true;        
    }
    // ----------------------------------------------------------------------
    public bool ForEachEnableConnection(Func<int,iCS_Connection,bool> test) {
		if(myEnableConnections == null) return true;
        var cLen= myEnableConnections.Length;
	    for(int i= 0; i < cLen; ++i) {
	        var connection= myEnableConnections[i];
	        if(connection != null) {
        	    if(test(i, connection) == false) {
        	        return false;	                    
    	        }
	        }
	    }
	    return true;        
	}
	
    // ======================================================================
    // Functions to fetch the runtime inputs needed to execute the
    // associated action.
    // ----------------------------------------------------------------------
	public void ForcedFetchConnections() {
        // Force fetch "this".
        if(myThisConnection != null) {
            myThis= myThisConnection.Value;
        }
        // Force all parameters.
        var cLen= myParameterConnections.Length;
	    for(int i= 0; i < cLen; ++i) {
	        var c= myParameterConnections[i];
	        if(c != null) {
        	    myParameters[i]= c.Value;	                    
	        }
	    }
        // Force all enables.
        var cLen= myEnableConnections.Length;
	    for(int i= 0; i < cLen; ++i) {
	        var c= myEnableConnections[i];
	        if(c != null) {
        	    myEnables[i]= c.Value;	                    
	        }
	    }
	}
    // ----------------------------------------------------------------------
    // Returns true if all connections are ready
	public bool AreAllConnectionsReady(int frameId) {
        return ForEachConnection((idx, connection)=> connection.IsReady(frameId));
	}
    // ----------------------------------------------------------------------
    // Returns true if the given parameter is ready for the given frameId.
    public bool IsReady(int idx, int frameId) {
        return IsConnectionReady(idx, frameId);
    }
    // ----------------------------------------------------------------------
    // Returns true if the given parameter is ready for the given frameId.
    public bool IsConnectionReady(int idx, int frameId) {
        var connection= GetConnection(idx);
        if(connection == null) return true;
        return connection.IsReady(frameId);
    }
    // ----------------------------------------------------------------------
    // Returns the final value of a connection or port if no connection.
	public object FetchValue(int idx) {
	    var connection= GetConnection(idx);
        if(connection == null) return GetValue(idx);
        return connection.Value;
	}
	
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get { return GetValue(idx); }
        set { SetValue(idx, value); }
    }
}
