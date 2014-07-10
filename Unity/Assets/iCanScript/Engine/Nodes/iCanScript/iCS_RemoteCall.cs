using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Library="Communication")]
public static class iCS_RemoteCall {
    [iCS_Function]
    public static void SendMessage(iCS_VisualScriptImp targetVisualScript, string messageName) {
        if(targetVisualScript != null) {
            targetVisualScript.RunMessage(messageName);
        }
    }
}
