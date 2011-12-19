using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="GameObject", Name="Transform")]
public sealed class iCS_Transform {
    [iCS_Function]
    public static void Transform(GameObject gameObject, Vector3 translation) {
        if(gameObject == null) return;
        gameObject.transform.Translate(translation);
    }

}

