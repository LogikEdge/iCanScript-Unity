using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_PortIndex  {
    // .NET Signature
    ParametersStart= 0,
    ParametersEnd=399,
    Return= 400,
    This= 401,

    // Extended Signature
    OutThis= 500,
    
    // Control Ports
    ControlStart= 1000,
    Trigger= 1000,
    EnableStart= 1001,
    EnableEnd= 1100,
    ControlEnd= 1100
}

