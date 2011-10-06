using UnityEngine;
using System.Collections;

[System.Serializable]
public enum WD_DisplayTypeEnum {
    State, Module, Class, Function, Conversion,
    InFieldPort,    OutFieldPort,
    InPropertyPort, OutPropertyPort,
    InFunctionPort, OutFunctionPort,
    InModulePort,   OutModulePort,
    InStatePort,    OutStatePort,
    EnablePort,
    Unknown
}

