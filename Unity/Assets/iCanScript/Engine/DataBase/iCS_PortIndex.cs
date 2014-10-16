using UnityEngine;
using System.Collections;

[System.Serializable]
public enum iCS_ParameterIndex  {
    // .NET Signature
    ParametersStart= 0,
    ParametersEnd=399,
    Return= 400,
    InInstance= 401,

    // Extended Signature
    OutInstance= 500,
    
    // Control Ports
    ControlStart= 1000,
    Trigger= 1000,
    EnablesStart= 1001,
    EnablesEnd= 1100,
    ControlEnd= 1100
}

