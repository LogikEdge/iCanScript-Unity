using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="GameObject", Name="Transform")]
public sealed class WD_Transform {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public static void Transform(GameObject gameObject, Vector3 translation) {
        if(gameObject == null) return;
        gameObject.transform.Translate(translation);
    }

}

