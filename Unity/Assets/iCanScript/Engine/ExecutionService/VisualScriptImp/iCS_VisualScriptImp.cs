using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Subspace;
using P=Prelude;

[AddComponentMenu("")]
public partial class iCS_VisualScriptImp : iCS_MonoBehaviourImp {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    [System.NonSerialized]
    SSObject[]                          myRuntimeNodes    = new SSObject[0];    

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public SSObject[]  RuntimeNodes     { get { return myRuntimeNodes; }}
                
}
