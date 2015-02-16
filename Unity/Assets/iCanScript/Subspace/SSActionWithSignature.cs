using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Subspace;

namespace Subspace {

    public abstract class SSActionWithSignature : SSAction {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        // .NET Signature
        object              myThis                 = null;  
        Connection          myThisConnection       = null;
        object[]            myParameters           = null;
        Connection[]        myParameterConnections = null;
        object              myReturnValue          = null;
        // Controls
        bool                myTrigger              = false;
        bool[]              myEnables              = null;
        Connection[]        myEnableConnections    = null;
    
        // ======================================================================
        // Filler when enables or connections not used.
        static bool[]       ourEmptyEnables    = new bool[0];
        static Connection[]	ourEmptyConnections= new Connection[0];
    
        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public object This {
            get { return myThisConnection == null ? myThis : myThisConnection.Value; }
        }
        public object ReturnValue {
            get { return myReturnValue; }
            set { myReturnValue= value; }
        }
        public bool Trigger {
            get { return myTrigger; }
            set { myTrigger= value; }
        }
        public object this[int portIdx] {
            get { return GetValue(portIdx); }
            set { SetValue(portIdx, value); }
        }
        public object[] Parameters {
            get { return myParameters; }
        }
        public Connection[] ParameterConnections {
            get { return myParameterConnections; }
        }
        public void SetConnection(int portIdx, Connection connection) {
            if(portIdx < myParameterConnections.Length) {
        		myParameterConnections[portIdx]= connection;            
                return;
            }
            if(portIdx == (int)iCS_PortIndex.InInstance) {
                myThisConnection= connection;
                return;
            }
    		if(myEnableConnections != null && portIdx >= (int)iCS_PortIndex.EnablesStart && portIdx <= (int)iCS_PortIndex.EnablesEnd) {
    			var i= portIdx-(int)iCS_PortIndex.EnablesStart;
    			if(i < myEnableConnections.Length) {
    				myEnableConnections[i]= connection;
    			}
    			return;
    		}
            Debug.LogWarning("iCanScript: Trying to set a signature connection with wrong index: "+portIdx);
        }
        public bool IsParameterReady(int idx, int runId) {
            if(idx >= myParameters.Length) {
                Debug.LogWarning("iCanScript: Trying to access a signature parameter with wrong index: "+idx);
                return false;
            }
            var connection= myParameterConnections[idx];
            if(connection == null) return true;
            return connection.IsReady(runId);
        }
        public object UpdateParameter(int idx) {
            if(idx >= myParameters.Length) {
                Debug.LogWarning("iCanScript: Trying to access a signature parameter with wrong index: "+idx);
                return null;
            }
            var connection= myParameterConnections[idx];
            if(connection != null) {
                myParameters[idx]= connection.Value;
            }
            return myParameters[idx];
        }
        public bool IsThisReady(int runId) {
            if(myThisConnection == null) return true;
            return myThisConnection.IsReady(runId);
        }
        // =========================================================================
        // Enables Query
        // ----------------------------------------------------------------------
        // Return true if the enable state can be assertained with the isEnabled
        // output parameter set appropriatly.  Otherwise, false is returned.
    	public bool GetIsEnabledIfReady(int runId, out bool isEnabled) {
            bool needToWait= false;
    	    int len= myEnables.Length;
            for(int i= 0; i < len; ++i) {
                var connection= myEnableConnections[i];
                if(connection != null) {
                    if(connection.IsCurrent(runId)) {
                        if((bool)connection.Value == false) {
                            isEnabled= false;
                            return true;
                        }
                    }
                    else {
                        needToWait= true;
                    }
                } else {
                    if(myEnables[i] == false) {
                        isEnabled= false;
                        return true;
                    }
                }
            }
            if(!needToWait) {
                isEnabled= true;
                return true;
            }
            isEnabled= false;
    	    return false;
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
    

        // ======================================================================
        // Functions to configure and access the signature parameters and 
        // return values.
    	// -------------------------------------------------------------------------
        // Returns one of the signature outputs.
        public object GetValue(int portIdx) {
    		if(portIdx < myParameters.Length) return myParameters[portIdx];
    		if(portIdx == (int)iCS_PortIndex.OutInstance) return This;
            if(portIdx == (int)iCS_PortIndex.Return) return ReturnValue;
    		if(portIdx == (int)iCS_PortIndex.Trigger) return Trigger;
            if(portIdx == (int)iCS_PortIndex.InInstance) return This;
    		if(portIdx >= (int)iCS_PortIndex.EnablesStart && portIdx <= (int)iCS_PortIndex.EnablesEnd) {
                int i= portIdx-(int)iCS_PortIndex.EnablesStart;
                if(i < myEnables.Length) {
                    var connection= myEnableConnections[i];
                    return connection == null ? myEnables[i] : myEnableConnections[i].Value;
                }
    		}
    		throw new System.Exception("Invalid signature access: ["+portIdx+"]");
    	}
    	// -------------------------------------------------------------------------
        // Sets the value of the object in the signature.  This should be called
        // by the compiler to initialize the signature.
        public void SetValue(int portIdx, object value) {
            if(portIdx < myParameters.Length)  {
                myParameters[portIdx]= value;
                return;
            }
            if(portIdx == (int)iCS_PortIndex.Return) {
                myReturnValue= value;
                return;
            }
            if(portIdx == (int)iCS_PortIndex.InInstance) {
                myThis= value;
                return;
            }
            if(portIdx == (int)iCS_PortIndex.OutInstance) {
                return;
            }
    		if(portIdx == (int)iCS_PortIndex.Trigger) {
    			myTrigger= (bool)value;
    			return;
    		}
    		if(portIdx >= (int)iCS_PortIndex.EnablesStart && portIdx <= (int)iCS_PortIndex.EnablesEnd) {
                var i= portIdx-(int)iCS_PortIndex.EnablesStart;
    			myEnables[i]= (bool)value;
    			return;
    		}
    		throw new System.Exception("Attempting to access a port that does not exists: "+GetPortFullName(portIdx));
    	}
        public string GetPortFullName(int portIdx) {
            string portName= "(parameter: "+portIdx+")";
            switch((iCS_PortIndex)portIdx) {
                case iCS_PortIndex.InInstance: {
                    var function= this as iCS_FunctionBase;
                    if(function != null) {
                        portName= "(in: <"+function.methodBase.DeclaringType.Name+" &>)";
                    }
                    else {
                        portName= "(in: this)";                        
                    }
                    break;
                }
                case iCS_PortIndex.OutInstance: {
                    var function= this as iCS_FunctionBase;
                    if(function != null) {
                        portName= "(out: <"+function.methodBase.DeclaringType.Name+" &>)";
                    }
                    else {
                        portName= "(out: this)";                        
                    }
                    break;
                }
                case iCS_PortIndex.Return: {
                    portName= "(return)";
                    break;
                }
                case iCS_PortIndex.Trigger: {
                    portName= "(trigger)";
                    break;
                }
                default: {
                    if(portIdx >= (int)iCS_PortIndex.EnablesStart && portIdx <= (int)iCS_PortIndex.EnablesEnd) {
                        portName= "(enable)";
                    }
                    break;
                }
            } 
            return FullName+portName;
        }

        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public SSActionWithSignature(string name, SSObject parent, int priority, int nbOfParameters, int nbOfEnables)
        : base(name, parent, priority) {
            myParameters = new object[nbOfParameters];
            myParameterConnections= new Connection[nbOfParameters];
            for(int i= 0; i < nbOfParameters; ++i) {
                myParameters[i]= null;
                myParameterConnections[i]= null;
            }
            if(nbOfEnables == 0) {
                myEnables= ourEmptyEnables;
                myEnableConnections= ourEmptyConnections;            
            } else {
                myEnables= new bool[nbOfEnables];
                myEnableConnections= new Connection[nbOfEnables];
                for(int i= 0; i < nbOfEnables; ++i) {
                    myEnables[i]= true;
                    myEnableConnections[i]= null;
                }
            }
        }
    
        // ======================================================================
        // Execution
        // ----------------------------------------------------------------------
        public override void Execute(int runId) {
            // Don't execute if action disabled.
            if(!IsActive) {
                return;
            }
    //        if(VisualScript.IsTraceEnabled) {
    //            Debug.Log("Executing=> "+FullName+" ("+runId+")");            
    //        }        

            // Clear the output trigger flag.
            myTrigger= false;
            // Wait until the enables can be resolved.
            bool isEnabled;
            if(!GetIsEnabledIfReady(runId, out isEnabled)) {
    			IsStalled= true;
    //            if(VisualScript.IsTraceEnabled) {
    //                Debug.Log("Executing=> "+FullName+" is waiting on the enables");
    //            }
    			return;
    		}
    		// Skip execution if this action is disabled.
            if(isEnabled == false) {
                MarkAsCurrent();
                if(Context.IsTraceEnabled) {
                    Debug.Log("Executing=> "+FullName+" is disabled"+" ("+runId+")");
                }
                return;
            }
            // Invoke derived class to execute normally.
            IsStalled= true;
            DoExecute(runId);
            if(Context.IsTraceEnabled) {    
                if(DidExecute()) {
                    Debug.Log("Executing=> "+FullName+" was executed sucessfully"+" ("+runId+")");
                }
    //            else if(IsCurrent(runId)){
    //                Debug.Log("Executing=> "+FullName+" is Current");
    //            }
    //            else {
    //                Debug.Log("Executing=> "+FullName+" is waiting for an input");
    //            }
            }
        }
        // ----------------------------------------------------------------------
        public Connection GetStalledEnablePort(int runId) {
            if(IsCurrent) {
                return null;
            }
            // Let's first verify the enables.
            int len= myEnableConnections.Length;
            for(int i= 0; i < len; ++i) {
                var connection= myEnableConnections[i];
                if(connection != null) {
                    if(!connection.IsReady(runId)) {
                        return connection;
                    }
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public override Connection GetStalledProducerPort(int runId) {
            if(IsCurrent) {
                return null;
            }
            // Let's first verify the enables.
            var stalledEnable= GetStalledEnablePort(runId);
            if(stalledEnable != null) {
                return stalledEnable;
            }
            // Verify intance connection
            if(myThisConnection != null && !myThisConnection.IsReady(runId)) {
                return myThisConnection;
            }
            // Verify parameter connections
            var len= myParameterConnections.Length;
            for(int i= 0; i < len; ++i) {
                var connection= myParameterConnections[i];
                if(connection != null) {
                    if(!connection.IsReady(runId)) {
                        return connection;
                    }
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public override void ForceExecute(int runId) {
            if(Context.IsTraceEnabled) {
                var stalledPort= GetStalledProducerPort(runId);
                var stalledPortName= stalledPort == null ? "" : stalledPort.PortFullName;
                if(stalledPort != null) {
                    var stalledNode= stalledPort.Action;
                    Debug.LogWarning("Force execute=> "+FullName+" STALLED PORT=> "+stalledPortName+" STALLED PORT NODE STATE=> "+stalledNode.IsCurrent);            
                    var stalledNodeParent= stalledNode.Parent as SSActionWithSignature;
                    if(stalledNodeParent != null) {
                        Debug.LogWarning("STALLED PORT NODE PARENT ENABLE=> "+stalledNodeParent.FullName+"("+stalledNodeParent.GetIsEnabled()+")");
                    }
                }
                Debug.LogWarning("Force Execute=> "+FullName);
            }
            // Force verify enables.
            if(GetIsEnabled() == false) {
                MarkAsCurrent();
                return;
            }
            // Invoke derived class to force execute.
            IsStalled= true;
            DoForceExecute(runId);
        }
        // ----------------------------------------------------------------------
        // Override the execute marker to set the output trigger.
        public new void MarkAsExecuted() {
            myTrigger= true;
            base.MarkAsExecuted();
        }
        // =========================================================================
        // Functions to override to provide specific behaviours.
        // ----------------------------------------------------------------------
        protected abstract void DoExecute(int runId);
        protected abstract void DoForceExecute(int runId);
    }
    
}

