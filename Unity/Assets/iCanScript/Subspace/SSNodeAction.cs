using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Subspace;

namespace Subspace {

    public abstract class SSNodeAction : SSAction {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        bool    myPortsAreAlwaysCurrent= false;

        // .NET Signature
        object              myThis             = null;  
        SSPullBinding       myThisBinding      = null;
        object[]            myParameters       = null;
        SSPullBinding[]     myParameterBindings= null;
        object              myReturnValue      = null;
        // Controls
        bool                myTrigger          = false;
        bool[]              myEnables          = null;
        SSPullBinding[]     myEnableBindings   = null;
    
        // ======================================================================
        // Filler when enables or connections not used.
        static bool[]           ourEmptyEnables = new bool[0];
        static SSPullBinding[]	ourEmptyBindings= new SSPullBinding[0];
    
        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public object This {
            get { return myThisBinding == null ? myThis : myThisBinding.Value; }
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
        public SSPullBinding[] ParameterConnections {
            get { return myParameterBindings; }
        }
        public void SetConnection(int portIdx, SSPullBinding connection) {
            if(portIdx < myParameterBindings.Length) {
        		myParameterBindings[portIdx]= connection;            
                return;
            }
            if(portIdx == (int)iCS_PortIndex.InInstance) {
                myThisBinding= connection;
                return;
            }
    		if(myEnableBindings != null && portIdx >= (int)iCS_PortIndex.EnablesStart && portIdx <= (int)iCS_PortIndex.EnablesEnd) {
    			var i= portIdx-(int)iCS_PortIndex.EnablesStart;
    			if(i < myEnableBindings.Length) {
    				myEnableBindings[i]= connection;
    			}
    			return;
    		}
            Debug.LogWarning("iCanScript: Trying to set a signature connection with wrong index: "+portIdx);
        }
        public bool IsParameterReady(int idx) {
            if(idx >= myParameters.Length) {
                Debug.LogWarning("iCanScript: Trying to access a signature parameter with wrong index: "+idx);
                return false;
            }
            var connection= myParameterBindings[idx];
            if(connection == null) return true;
            return connection.IsReady;
        }
        public object UpdateParameter(int idx) {
            if(idx >= myParameters.Length) {
                Debug.LogWarning("iCanScript: Trying to access a signature parameter with wrong index: "+idx);
                return null;
            }
            var connection= myParameterBindings[idx];
            if(connection != null) {
                myParameters[idx]= connection.Value;
            }
            return myParameters[idx];
        }
        public bool IsThisReady {
            get {
                if(myThisBinding == null) return true;
                return myThisBinding.IsReady;
            }
        }

        // ----------------------------------------------------------------------
        public bool ArePortsEvaluated   { get { return IsEvaluated || ArePortsAlwaysCurrent || !IsActive; }}
        public bool ArePortsExecuted    { get { return IsExecuted; }}
        public bool ArePortsAlwaysCurrent {
			get { return myPortsAreAlwaysCurrent; }
			set { myPortsAreAlwaysCurrent= value; }
		}
        

        // =========================================================================
        // Enables Query
        // ----------------------------------------------------------------------
        // Return true if the enable state can be assertained with the isEnabled
        // output parameter set appropriatly.  Otherwise, false is returned.
    	public bool GetIsEnabledIfReady(out bool isEnabled) {
            bool needToWait= false;
    	    int len= myEnables.Length;
            for(int i= 0; i < len; ++i) {
                var connection= myEnableBindings[i];
                if(connection != null) {
                    if(connection.IsEvaluated) {
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
                var connection= myEnableBindings[i];
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
                    var connection= myEnableBindings[i];
                    return connection == null ? myEnables[i] : myEnableBindings[i].Value;
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
        public SSNodeAction(string name, SSObject parent, int priority, int nbOfParameters, int nbOfEnables)
        : base(name, parent, priority) {
            myParameters = new object[nbOfParameters];
            myParameterBindings= new SSPullBinding[nbOfParameters];
            for(int i= 0; i < nbOfParameters; ++i) {
                myParameters[i]= null;
                myParameterBindings[i]= null;
            }
            if(nbOfEnables == 0) {
                myEnables= ourEmptyEnables;
                myEnableBindings= ourEmptyBindings;            
            } else {
                myEnables= new bool[nbOfEnables];
                myEnableBindings= new SSPullBinding[nbOfEnables];
                for(int i= 0; i < nbOfEnables; ++i) {
                    myEnables[i]= true;
                    myEnableBindings[i]= null;
                }
            }
        }
    
        // ======================================================================
        // Execution
        // ----------------------------------------------------------------------
        public override void Evaluate() {
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
            if(!GetIsEnabledIfReady(out isEnabled)) {
    			IsStalled= true;
    //            if(VisualScript.IsTraceEnabled) {
    //                Debug.Log("Executing=> "+FullName+" is waiting on the enables");
    //            }
    			return;
    		}
    		// Skip execution if this action is disabled.
            if(isEnabled == false) {
                MarkAsEvaluated();
                if(Context.IsTraceEnabled) {
                    Debug.Log("Executing=> "+FullName+" is disabled"+" ("+Context.RunId+")");
                }
                return;
            }
            // Invoke derived class to execute normally.
            IsStalled= true;
            DoEvaluate();
            if(Context.IsTraceEnabled) {    
                if(IsExecuted) {
                    Debug.Log("Executing=> "+FullName+" was executed sucessfully"+" ("+Context.RunId+")");
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
        public SSPullBinding GetStalledEnablePort() {
            if(IsEvaluated) {
                return null;
            }
            // Let's first verify the enables.
            int len= myEnableBindings.Length;
            for(int i= 0; i < len; ++i) {
                var connection= myEnableBindings[i];
                if(connection != null) {
                    if(!connection.IsReady) {
                        return connection;
                    }
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public override SSPullBinding GetStalledProducerPort() {
            if(IsEvaluated) {
                return null;
            }
            // Let's first verify the enables.
            var stalledEnable= GetStalledEnablePort();
            if(stalledEnable != null) {
                return stalledEnable;
            }
            // Verify intance connection
            if(myThisBinding != null && !myThisBinding.IsReady) {
                return myThisBinding;
            }
            // Verify parameter connections
            var len= myParameterBindings.Length;
            for(int i= 0; i < len; ++i) {
                var connection= myParameterBindings[i];
                if(connection != null) {
                    if(!connection.IsReady) {
                        return connection;
                    }
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public override void Execute() {
            if(Context.IsTraceEnabled) {
                var stalledPort= GetStalledProducerPort();
                var stalledPortName= stalledPort == null ? "" : stalledPort.PortFullName;
                if(stalledPort != null) {
                    var stalledNode= stalledPort.Action;
                    Debug.LogWarning("Force execute=> "+FullName+" STALLED PORT=> "+stalledPortName+" STALLED PORT NODE STATE=> "+stalledNode.IsEvaluated);            
                    var stalledNodeParent= stalledNode.Parent as SSNodeAction;
                    if(stalledNodeParent != null) {
                        Debug.LogWarning("STALLED PORT NODE PARENT ENABLE=> "+stalledNodeParent.FullName+"("+stalledNodeParent.GetIsEnabled()+")");
                    }
                }
                Debug.LogWarning("Force Execute=> "+FullName);
            }
            // Force verify enables.
            if(GetIsEnabled() == false) {
                MarkAsEvaluated();
                return;
            }
            // Invoke derived class to force execute.
            IsStalled= true;
            DoExecute();
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
        protected abstract void DoEvaluate();
        protected abstract void DoExecute();
    }
    
}

