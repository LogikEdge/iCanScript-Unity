using UnityEngine;
using System.Reflection;
using System.Collections;

public abstract class iCS_FieldBase : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected FieldInfo myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldBase(FieldInfo fieldInfo, iCS_Storage storage, int instanceId, int priority,
                         int nbOfParameters, int nbOfEnables)
    : base(storage, instanceId, priority, nbOfParameters, nbOfEnables) {
        myFieldInfo= fieldInfo;
    }
}
