using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

[AddComponentMenu("")]
public partial class iCS_VisualScriptImp : iCS_MonoBehaviourImp {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    [System.NonSerialized]
    public bool                         IsTraceEnabled    = false;
    object[]                            myRuntimeNodes    = new object[0];
    List<int>                           myPublicInterfaces= new List<int>();
    Dictionary<string,iCS_RunContext>   myMessageContexts = new Dictionary<string,iCS_RunContext>();
    

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object[]  RuntimeNodes     { get { return myRuntimeNodes; }}
    public List<iCS_EngineObject> PublicInterfaces    { get { return P.map(id=> EngineObjects[id], myPublicInterfaces); }}
    public List<iCS_EngineObject> PublicVariables     { get { return P.filter(pi=> pi.IsConstructor, PublicInterfaces); }}
    public List<iCS_EngineObject> PublicUserFunctions { get { return P.filter(pi=> pi.IsPackage, PublicInterfaces); }}
    public int UpdateFrameId {
        get {
            iCS_RunContext updateContext= null;
            if(myMessageContexts.TryGetValue("Update", out updateContext)) {
                return updateContext.FrameId;
            }
            return 0;
        }
    }
    public int FixedUpdateFrameId {
        get {
            iCS_RunContext fixedUpdateContext= null;
            if(myMessageContexts.TryGetValue("FixedUpdate", out fixedUpdateContext)) {
                return fixedUpdateContext.FrameId;
            }
            return 0;
        }
    }
    
    // ======================================================================
    // Message Handler
    // ----------------------------------------------------------------------
    // Run messages without parameter.
    public void RunMessage(string messageName) {
        iCS_RunContext runContext;
        if(myMessageContexts.TryGetValue(messageName, out runContext)) {
//            var rtMessage= runContext.Action as iCS_Message;
//            rtMessage.GetSignatureDataSource().This= this;
            runContext.Run();
        }
    }
    // ----------------------------------------------------------------------
    public void InvokeVisualScript(string methodName) {
        RunMessage(methodName);
    }
    // ----------------------------------------------------------------------
    // Run message with one parameter.
    public void RunMessage(string messageName, object p1) {
        iCS_RunContext runContext;
        if(myMessageContexts.TryGetValue(messageName, out runContext)) {
            var rtMessage= runContext.Action as iCS_Message;
            if(rtMessage != null) {
                rtMessage.GetSignatureDataSource().SetParameter(0, p1);
                runContext.Run();
            }
        }
    }
    // ----------------------------------------------------------------------
    // Run message with two parameters.
    public void RunMessage(string messageName, object p1, object p2) {
        iCS_RunContext runContext;
        if(myMessageContexts.TryGetValue(messageName, out runContext)) {
            var rtMessage= runContext.Action as iCS_Message;
            if(rtMessage != null) {
                rtMessage.GetSignatureDataSource().SetParameter(0, p1);
                rtMessage.GetSignatureDataSource().SetParameter(1, p2);
                runContext.Run();
            }
        }
    }
    
    // ----------------------------------------------------------------------
    void Reset() {
        myMessageContexts= new Dictionary<string,iCS_RunContext>();
    }
    
    // ----------------------------------------------------------------------
    // Awake is called when the script instance is being loaded.
    void Awake() {
//        base.Awake();  // This generates an internal compiler error
        GenerateRuntimeObjects();        
        iCS_RunContext awakeContext= null;
        myMessageContexts.TryGetValue("Awake", out awakeContext);
        if(awakeContext != null) {
            awakeContext.Action.IsActive= true;
            do {
                awakeContext.Action.Execute(-2);
                if(awakeContext.Action.IsStalled) {
                    Debug.LogError("The Awake() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!awakeContext.Action.IsCurrent(-2));
            awakeContext.Action.IsActive= false;
        }
    }
    // ----------------------------------------------------------------------
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {
        ConnectRuntimeObjects();
        iCS_RunContext startContext= null;
        myMessageContexts.TryGetValue("Start", out startContext);
        if(startContext != null) {
            startContext.Action.IsActive= true;
            do {
                startContext.Action.Execute(-2);
                if(startContext.Action.IsStalled) {
                    Debug.LogError("The Start() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!startContext.Action.IsCurrent(-2));
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
            myMessageContexts.Add(messageName, new iCS_RunContext(message));
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
