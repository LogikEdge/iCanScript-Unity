using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Engine {

    [Serializable]
    public enum AccessSpecifier   { Public, Private, Protected, Internal };
    [Serializable]
    public enum ScopeSpecifier    { NonStatic, Static, Virtual, Abstract, Override, New, Const };
    
}
