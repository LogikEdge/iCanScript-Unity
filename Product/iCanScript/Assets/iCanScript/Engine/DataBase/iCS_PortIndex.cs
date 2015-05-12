using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Engine {
    
    [System.Serializable]
    public enum iCS_PortIndex  {
        // .NET Signature
        ParametersStart= 0,
        ParametersEnd=399,
        Return= 400,
        Target= 401,

        // Extended Signature
        Self= 500,
    
        // Control Ports
        ControlStart= 1000,
        Trigger= 1000,
        EnablesStart= 1001,
        EnablesEnd= 1100,
        ControlEnd= 1100
    }

}
