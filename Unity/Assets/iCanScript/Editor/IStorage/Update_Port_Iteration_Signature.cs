using UnityEngine;
using System.Collections;
using iCanScriptEngine;

public static class Update_Port_Iteration_Signature {

    public static void Apply(iCS_EditorObject port) {
        if(port == null) return;
        var providerPort= port.ProducerPort;
        // Use port type if no provider port
        bool portIsArray= port.RuntimeType.IsArray;
        if(providerPort == null) {
            if(portIsArray == false) {
                port.PortIterationSignature= PortIterationSignatureEnum.Dont_iterate;
            }
        }
        else {
            var providerPortIsArray= providerPort.RuntimeType.IsArray;
            if(portIsArray == true && providerPortIsArray == false) {
                port.PortIterationSignature= PortIterationSignatureEnum.Dont_iterate;
            }
        }
    }
}
