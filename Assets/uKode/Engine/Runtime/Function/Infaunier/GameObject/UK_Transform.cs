using UnityEngine;
using System.Collections;

[UK_Class(Company="Infaunier", Package="GameObject", Name="Transform")]
public sealed class UK_Transform {
    [UK_Function]
    public static void Transform(GameObject gameObject, Vector3 translation) {
        if(gameObject == null) return;
        gameObject.transform.Translate(translation);
    }

}

