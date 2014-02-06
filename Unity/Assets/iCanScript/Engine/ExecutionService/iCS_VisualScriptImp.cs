using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public partial class iCS_VisualScriptImp : iCS_Storage {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object[]                            myRuntimeNodes   = new object[0];
    Dictionary<string,iCS_RunContext>   myMessageContexts= new Dictionary<string,iCS_RunContext>();
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object[] RuntimeNodes { get { return myRuntimeNodes; }}
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
    // This function should be used to pass information between objects.  It
    // is invoked after Awake and before any Update call.
    void Start() {
        GenerateCode();
        iCS_RunContext startContext= null;
        myMessageContexts.TryGetValue("Start", out startContext);
        if(startContext != null) {
            do {
                startContext.Action.Execute(-2);
                if(startContext.Action.IsStalled) {
                    Debug.LogError("The Start() of "+name+" is stalled. Please remove any dependent processing !!!");
                    return;
                }
            } while(!startContext.Action.IsCurrent(-2));
        }
    }
        
    // ======================================================================
    // Dynamic Message Receiver Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        iCS_Message message= obj as iCS_Message;
        if(message == null) return;
        var messageName= message.Name;
        if(myMessageContexts.ContainsKey(messageName)) {
            myMessageContexts[messageName]= new iCS_RunContext(message);
        } else {
            myMessageContexts.Add(messageName, new iCS_RunContext(message));
        }
    }
    // ----------------------------------------------------------------------
    public void RemoveChild(object obj) {
        iCS_Message message= obj as iCS_Message;
        if(message == null) return;
        var messageName= message.Name;
        if(myMessageContexts.ContainsKey(messageName)) {
            myMessageContexts.Remove(messageName);
        }
    }

}
