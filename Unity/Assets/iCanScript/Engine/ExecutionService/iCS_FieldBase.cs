using UnityEngine;
using System.Reflection;
using System.Collections;
using Subspace;

public abstract class iCS_FieldBase : SSNodeAction {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected FieldInfo myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldBase(string name, SSObject parent, FieldInfo fieldInfo, int priority,
                         int nbOfParameters, int nbOfEnables)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {
        myFieldInfo= fieldInfo;
    }
}
