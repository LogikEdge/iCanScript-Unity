using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[iCS_Class(Company="iCanScript", Library="Communication")]
public static class iCS_RemoteCall {
    [iCS_Function]
    public static Component FindWithTag(string tag, string componentType) {
        var go= GameObject.FindWithTag(tag);
        if(go == null) {
            return null;
        }
        return go.GetComponent(componentType);
    }
    
    [iCS_Function]
    public static void SendMessage(iCS_VisualScriptImp targetVisualScript, string messageName) {
        if(targetVisualScript != null) {
            targetVisualScript.RunMessage(messageName);
        }
    }
    [iCS_Function]
    public static void SendMessage(iCS_VisualScriptImp targetVisualScript, string messageName, object p1) {
        if(targetVisualScript != null) {
            targetVisualScript.RunMessage(messageName, p1);
        }
    }
    [iCS_Function]
    public static void SendMessage(iCS_VisualScriptImp targetVisualScript, string messageName, object p1, object p2) {
        if(targetVisualScript != null) {
            targetVisualScript.RunMessage(messageName, p1, p2);
        }
    }
    
    // ------------------------------------------------------------------------
    // Build a dictionary of all the visual script we are communicating with.
    static Dictionary<string, iCS_VisualScriptImp> myDictionary= new Dictionary<string, iCS_VisualScriptImp>();

    // ------------------------------------------------------------------------
    // Send a message to the visual script with the corresponding tag.
    [iCS_Function]        
    public static void SendMessageUsingTag(string tag, string messageName) {
        iCS_VisualScriptImp vs= null;
        var isFound= myDictionary.TryGetValue(tag, out vs);
        if(vs == null) {
            var go= GameObject.FindWithTag(tag);
            if(go != null) {
                vs= go.GetComponent<iCS_VisualScriptImp>();
                if(vs != null) {
                    if(isFound) {
                        myDictionary[tag]= vs;
                    }
                    else {
                        myDictionary.Add(tag, vs);
                    }
                }
            }            
        }
        if(vs != null) {
            vs.RunMessage(messageName);            
        }
    }
}
