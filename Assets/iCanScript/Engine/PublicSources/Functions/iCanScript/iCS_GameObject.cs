using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public static class iCS_GameObject {
    // Global transform attributes
    [iCS_Function(ToolTip="Returns the global position of the given game object.")]
    public static Vector3 GetPosition(GameObject gameObject) {
        return gameObject.transform.position;
    }
    [iCS_Function(ToolTip="Returns the global rotation of the given game object.")]
    public static Quaternion GetRotation(GameObject gameObject) {
        return gameObject.transform.rotation;
    }
    [iCS_Function(ToolTip="Returns the global scale of the given game object.")]
    public static Vector3 GetScale(GameObject gameObject) {
        return gameObject.transform.lossyScale;
    }

    // Local transform attributes
    [iCS_Function(ToolTip="Returns the local position of the given game object.")]
    public static Vector3 GetLocalPosition(GameObject gameObject) {
        return gameObject.transform.localPosition;
    }
    [iCS_Function(ToolTip="Returns the local rotation of the given game object.")]
    public static Quaternion GetLocalRotation(GameObject gameObject) {
        return gameObject.transform.localRotation;
    }
    [iCS_Function(ToolTip="Returns the local scale of the given game object.")]
    public static Vector3 GetLocalScale(GameObject gameObject) {
        return gameObject.transform.localScale;
    }

    // Translation
    [iCS_Function]
    public static void Translate(GameObject gameObject, Vector3 translation) {
        if(gameObject == null) return;
        gameObject.transform.Translate(translation);
    }
    [iCS_Function(Name="Translate")]
    public static void TransformUsingVelocity(GameObject gameObject, Vector3 velocity) {
        if(gameObject == null) return;
        gameObject.transform.Translate(Time.deltaTime*velocity);
    }
    
    // Rotation
    [iCS_Function]
    public static void Rotate(GameObject gameObject, Vector3 axis, float angle) {
        gameObject.transform.Rotate(axis, angle);
    }
    [iCS_Function]
    public static void RotateAround(GameObject gameObject, Vector3 point, Vector3 axis, float angle) {
        gameObject.transform.RotateAround(point, axis, angle);
    }
    [iCS_Function(Name="Rotate")]
    public static void RotateUsingAngleVelocity(GameObject gameObject, Vector3 axis, float angleVelocity) {
        gameObject.transform.Rotate(axis, Time.deltaTime*angleVelocity);
    }
    [iCS_Function(Name="RotateAround")]
    public static void RotateAroundUsingAngleVelocity(GameObject gameObject, Vector3 point, Vector3 axis, float angleVelocity) {
        gameObject.transform.RotateAround(point, axis, Time.deltaTime*angleVelocity);
    }
}

