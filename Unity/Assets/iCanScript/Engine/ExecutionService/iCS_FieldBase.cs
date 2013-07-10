using UnityEngine;
using System.Reflection;
using System.Collections;

public class iCS_FieldBase : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected FieldInfo myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldBase(FieldInfo fieldInfo, string name, int priority, int nbOfParameters, bool hasReturn, bool hasThis)
    : base(name, priority, nbOfParameters, hasReturn, hasThis) {
        myFieldInfo= fieldInfo;
    }
}
