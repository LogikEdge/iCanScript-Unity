using UnityEngine;
using System;
using System.Collections;

public class iCS_SignatureDataSource {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    // .NET Signature
    object              myThis                  = null;  
    iCS_Connection      myThisConnection        = null;
    object[]            myParameters            = null;
    iCS_Connection[]    myParameterConnections  = null;
    object              myReturnValue           = null;
    // Extended Signature
    object              myOutThis               = null;
    // Controls
    bool                myTrigger               = false;
    bool[]              myEnables               = null;
    iCS_Connection[]    myEnableConnections     = null;
    
    // ======================================================================
    static bool[]           ourEmptyEnables    = new bool[0];
    static iCS_Connection[] ourEmptyConnections= new iCS_Connection[0];
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object This {
        set { myThis= value; }
        get { return myThisConnection == null ? myThis : myThisConnection.Value; }
    }
    public object ReturnValue {
        get { return myReturnValue; }
        set { myReturnValue= value; }
    }
    public object OutThis {
        get { return myOutThis; }
        set { myOutThis= value; }
    }
    public bool Trigger {
        get { return myTrigger; }
        set { myTrigger= value; }
    }
    public object this[int idx] {
        get { return GetValue(idx); }
        set { SetValue(idx, value); }
    }
    public int ParametersStart { get { return 0; }}
    public int ParametersEnd   { get { return myParameters.Length-1; }}
    public object[] Parameters {
        get { return myParameters; }
    }
    public iCS_Connection[] ParameterConnections {
        get { return myParameterConnections; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public iCS_SignatureDataSource(int nbOfParameters, int nbOfEnables) {
        myParameters = new object[nbOfParameters];
        myParameterConnections= new iCS_Connection[nbOfParameters];
        for(int i= 0; i < nbOfParameters; ++i) {
            myParameters[i]= null;
            myParameterConnections[i]= null;
        }
        if(nbOfEnables == 0) {
            myEnables= ourEmptyEnables;
            myEnableConnections= ourEmptyConnections;            
        } else {
            myEnables= new bool[nbOfEnables];
            myEnableConnections= new iCS_Connection[nbOfEnables];
            for(int i= 0; i < nbOfEnables; ++i) {
                myEnables[i]= true;
                myEnableConnections[i]= null;
            }
        }
    }

    // ======================================================================
    // Functions to configure and access the signature parameters and 
    // return values.
	// -------------------------------------------------------------------------
    // Returns one of the signature outputs.
    public object GetValue(int idx) {
        if(idx == (int)iCS_PortIndex.Return) return ReturnValue;
		if(idx == (int)iCS_PortIndex.OutThis) return OutThis;
		if(idx == (int)iCS_PortIndex.Trigger) return Trigger;
        if(idx == (int)iCS_PortIndex.This) return This;
		if(idx < myParameters.Length) return GetParameter(idx);
		if(idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
            int i= idx-(int)iCS_PortIndex.EnablesStart;
            if(i < myEnables.Length) {
                var connection= myEnableConnections[i];
                return connection == null ? myEnables[i] : myEnableConnections[i].Value;
            }
		}
#if UNITY_EDITOR
		throw new System.Exception("Invalid signature access: ["+idx+"]");
#else
        return null;		
#endif
	}
	// -------------------------------------------------------------------------
    // Sets the value of the object in the signature.  This should be called
    // by the compiler to initialize the signature.
    public void SetValue(int idx, object value) {
        if(idx < myParameters.Length)  {
            SetParameter(idx, value);
            return;
        }
        if(idx == (int)iCS_PortIndex.Return) {
            ReturnValue= value;
            return;
        }
        if(idx == (int)iCS_PortIndex.This) {
            This= value;
            return;
        }
		if(idx == (int)iCS_PortIndex.OutThis) {
			OutThis= value;
			return;
		}
		if(idx == (int)iCS_PortIndex.Trigger) {
			Trigger= (bool)value;
			return;
		}
		if(idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
			SetEnable(idx, (bool)value);
			return;
		}
#if UNITY_EDITOR
		throw new System.Exception("Invalid signature access: ["+idx+"]");
#endif
	}
	// -------------------------------------------------------------------------
    public void SetParameter(int idx, object value) {
#if UNITY_EDITOR
        if(idx >= myParameters.Length) {
			Debug.LogWarning("iCanScript: Invalid signature access: ["+idx+"]");
			throw new System.Exception("Invalid signature access: ["+idx+"]");
        }
#endif        
        myParameters[idx]= value;
    }
	// -------------------------------------------------------------------------
	public void SetEnable(int idx, bool value) {
		var i= idx-(int)iCS_PortIndex.EnablesStart;
#if UNITY_EDITOR
		if(i >= myEnables.Length) {
			Debug.LogWarning("iCanScript: Invalid signature access: ["+idx+"]");
			throw new System.Exception("Invalid signature access: ["+idx+"]");
	    }
#endif
		myEnables[i]= value;
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
		if(myEnableConnections != null && idx >= (int)iCS_PortIndex.EnablesStart && idx <= (int)iCS_PortIndex.EnablesEnd) {
			var i= idx-(int)iCS_PortIndex.EnablesStart;
			if(i < myEnableConnections.Length) {
				myEnableConnections[i]= connection;
			}
			return;
		}
#if UNITY_EDITOR
        Debug.LogWarning("iCanScript: Trying to set a signature connection with wrong index: "+idx);
#endif
	}

    // =========================================================================
    // Enables Query
    // ----------------------------------------------------------------------
    // Return true if the enable state can be assertained with the isEnabled
    // output parameter set appropriatly.  Otherwise, false is returned.
	public bool GetIsEnabledIfReady(int frameId, out bool isEnabled) {
	    int len= myEnables.Length;
        for(int i= 0; i < len; ++i) {
            var connection= myEnableConnections[i];
            if(connection != null) {
                if(connection.DidExecute(frameId)) {
                    if((bool)connection.Value == false) {
                        isEnabled= false;
                        return true;
                    }
                
                } else {
                    isEnabled= false;
                    return false;
                }
            } else {
                if(myEnables[i] == false) {
                    isEnabled= false;
                    return true;
                }
            }
        }
        isEnabled= true;
	    return true;
	}
	// ----------------------------------------------------------------------
    public bool GetIsEnabled() {
        int len= myEnables.Length;
        for(int i= 0; i < len; ++i) {
            var connection= myEnableConnections[i];
            if(connection == null) {
                if(myEnables[i] == false) {
                    return false;
                }
            } else {
				var v= connection.Value;
				if(v == null) {
					Debug.LogWarning("iCanScript: Invalid connection for enabled sourced from: "+connection.Action.FullName);
					continue;
				}
                if(/*v != null && */(bool)v == false) {
                    return false;
                }
            }
        }
        return true;
    }
    
    // =========================================================================
    // This Queries
    // -------------------------------------------------------------------------
    // Return 'true' if instance pointer is ready for the given frameId.
    // The 'This' object is also updated if the connection is ready.
    public bool IsThisReady(int frameId) {
        if(myThisConnection == null) return true;
        return myThisConnection.IsReady(frameId);
    }
    
	// -------------------------------------------------------------------------
    public bool IsParameterReady(int idx, int frameId) {
#if UNITY_EDITOR
        if(idx >= myParameters.Length) {
            Debug.LogWarning("iCanScript: Trying to access a signature parameter with wrong index: "+idx);
            return false;
        }
#endif
        var connection= myParameterConnections[idx];
        if(connection == null) return true;
        return connection.IsReady(frameId);
    }
    // ----------------------------------------------------------------------
    // Forces the update of the given parameter.
    public object UpdateParameter(int idx) {
#if UNITY_EDITOR
        if(idx >= myParameters.Length) {
            Debug.LogWarning("iCanScript: Trying to access a signature parameter with wrong index: "+idx);
            return null;
        }
#endif
        var connection= myParameterConnections[idx];
        if(connection != null) {
            myParameters[idx]= connection.Value;
        }
        return myParameters[idx];
    }
	// -------------------------------------------------------------------------
    public object GetParameter(int idx) {
#if UNITY_EDITOR
        if(idx >= myParameters.Length) {
            Debug.LogWarning("iCanScript: Trying to get a signature value with wrong index: "+idx);
            return null;		                    
        }
#endif
        return myParameters[idx];
    }
    
}
