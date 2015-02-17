using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Subspace;
using P=Prelude;

[AddComponentMenu("")]
public partial class iCS_VisualScriptImp : iCS_MonoBehaviourImp {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    [System.NonSerialized]
    SSContext                           myContext         = null;
    SSObject[]                          myRuntimeNodes    = new SSObject[0];
    List<int>                           myPublicInterfaces= new List<int>();
    List<int>                           myPublicVariables = new List<int>();
    Dictionary<string,iCS_VSContext>    myMessageContexts = new Dictionary<string,iCS_VSContext>();
    P.TimerService                      myTimerService    = new P.TimerService();
    

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public SSObject[]  RuntimeNodes     { get { return myRuntimeNodes; }}
    public List<iCS_EngineObject> PublicInterfaces    { get { return P.map(id=> EngineObjects[id], myPublicInterfaces); }}
    public List<iCS_EngineObject> PublicVariables     { get { return P.filter(pi=> pi.IsConstructor, PublicInterfaces); }}
    public List<iCS_EngineObject> PublicUserFunctions { get { return P.filter(pi=> pi.IsPackage, PublicInterfaces); }}
    public SSContext              Context {
        get {
            if(myContext == null) {
                myContext= new SSContext(this);
                myContext.ErrorDelegate  = RuntimeErrorDelegate;
                myContext.WarningDelegate= RuntimeWarningDelegate;
            }
            return myContext;
        }
    }
    public int UpdateFrameId {
        get {
            iCS_VSContext updateContext= null;
            if(myMessageContexts.TryGetValue("Update", out updateContext)) {
                return updateContext.FrameId;
            }
            return 0;
        }
    }
    public int FixedUpdateFrameId {
        get {
            iCS_VSContext fixedUpdateContext= null;
            if(myMessageContexts.TryGetValue("FixedUpdate", out fixedUpdateContext)) {
                return fixedUpdateContext.FrameId;
            }
            return 0;
        }
    }
    void RuntimeErrorDelegate(string msg, SSObject obj) {
        int instanceId= GetInstanceId(obj);
		ErrorControllerProxy.AddError("Runtime", msg, Context.UserData as iCS_VisualScriptImp, instanceId);   
    }
    void RuntimeWarningDelegate(string msg, SSObject obj) {
        int instanceId= GetInstanceId(obj);
		ErrorControllerProxy.AddWarning("Runtime", msg, myContext.UserData as iCS_VisualScriptImp, instanceId);        
    }
    int GetInstanceId(SSObject obj) {
        return Array.FindIndex(myRuntimeNodes, x=> obj.Equals(x));        
    }
    
    // ======================================================================
    // Message Handler
    // ----------------------------------------------------------------------
    // Run messages without parameter.
    public void RunMessage(string messageName) {
        // Run timer service on Update
        if(messageName == "Update") {
            myTimerService.Update();
        }
        // Run the context.
        iCS_VSContext runContext;
        if(myMessageContexts.TryGetValue(messageName, out runContext)) {
            runContext.Run();
        }
    }
    // ----------------------------------------------------------------------
    public void InvokeVisualScript(string methodName) {
        RunMessage(methodName);
    }
    public void InvokeVisualScript(string methodName, float delay) {
        myTimerService.CreateTimedAction(delay, ()=> RunMessage(methodName)).Schedule();
    }
    // ----------------------------------------------------------------------
    // Run message with one parameter.
    public void RunMessage(string messageName, object p1) {
        iCS_VSContext runContext;
        if(myMessageContexts.TryGetValue(messageName, out runContext)) {
            var rtMessage= runContext.Action as iCS_Message;
            if(rtMessage != null) {
                rtMessage.Parameters[0]= p1;
                runContext.Run();
            }
        }
    }
    // ----------------------------------------------------------------------
    // Run message with two parameters.
    public void RunMessage(string messageName, object p1, object p2) {
        iCS_VSContext runContext;
        if(myMessageContexts.TryGetValue(messageName, out runContext)) {
            var rtMessage= runContext.Action as iCS_Message;
            if(rtMessage != null) {
                rtMessage.Parameters[0]= p1;
                rtMessage.Parameters[1]= p2;
                runContext.Run();
            }
        }
    }
    
    // ----------------------------------------------------------------------
    void Reset() {
        myMessageContexts= new Dictionary<string,iCS_VSContext>();
    }
    
    // ----------------------------------------------------------------------
    // Awake is called when the script instance is being loaded.
    void Awake() {
//        base.Awake();  // This generates an internal compiler error
        GenerateRuntimeObjects();        
        iCS_VSContext awakeContext= null;
        myMessageContexts.TryGetValue("Awake", out awakeContext);
        if(awakeContext != null) {
            awakeContext.Action.Context.RunId= -2;
            awakeContext.Action.IsActive= true;
            do {
                awakeContext.Action.Evaluate();
                if(awakeContext.Action.IsStalled) {
                    Debug.LogError("The Awake() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!awakeContext.Action.IsEvaluated);
            awakeContext.Action.IsActive= false;
        }
    }
    // ----------------------------------------------------------------------
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {
        ConnectRuntimeObjects();
        // Build public variables
        foreach(var i in myPublicVariables) {
            var constructor= myRuntimeNodes[i] as SSAction;
            constructor.Evaluate();
        }
        myPublicVariables.Clear();
        // Run the Start message handler.
        iCS_VSContext startContext= null;
        myMessageContexts.TryGetValue("Start", out startContext);
        if(startContext != null) {
            startContext.Action.Context.RunId= -2;
            startContext.Action.IsActive= true;
            do {
                startContext.Action.Evaluate();
                if(startContext.Action.IsStalled) {
                    Debug.LogError("The Start() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!startContext.Action.IsEvaluated);
            startContext.Action.IsActive= false;
        }
    }
        
    // ======================================================================
    // Dynamic Message Receiver Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        iCS_Package message= obj as iCS_Package;
        if(message == null) return;
        var messageName= message.Name;
        AddChildWithName(obj, messageName);
    }
    // ----------------------------------------------------------------------
    public void AddChildWithName(object obj, string messageName) {
        iCS_Package message= obj as iCS_Package;
        if(message == null) return;
        if(!myMessageContexts.ContainsKey(messageName)) {
            // Build a specific context for all public functions.
            var context= new SSContext(this);
            context.ErrorDelegate  = RuntimeErrorDelegate;
            context.WarningDelegate= RuntimeWarningDelegate;
            message.Context= context;
            myMessageContexts.Add(messageName, new iCS_VSContext(message));
        }
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(object obj) {
        iCS_Package message= obj as iCS_Package;
        if(message == null) return;
        var messageName= message.Name;
        if(myMessageContexts.ContainsKey(messageName)) {
            myMessageContexts.Remove(messageName);
        }
    }

}
