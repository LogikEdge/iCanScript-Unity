using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Component")]
public sealed class iCS_Component {
    // Mesh
    [iCS_Function(Return="meshFilter", ToolTip="Returns the MeshFilter associated with the game object.")]
    public static MeshFilter GetMeshFilter(GameObject gameObject) {
        return gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
    }
    [iCS_Function(Return="textMesh", ToolTip="Returns the TextMesh associated with the game object.")]
    public static TextMesh GetTextMesh(GameObject gameObject) {
        return gameObject.GetComponent(typeof(TextMesh)) as TextMesh;
    }

    // Physics
    [iCS_Function(Return="rigidbody", ToolTip="Returns the Rigidbody associated with the game object.")]
    public static Rigidbody GetRigidBody(GameObject gameObject) {
        return gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
    }
    [iCS_Function(Return="characterController", ToolTip="Returns the CharacterController associated with the game object.")]
    public static CharacterController GetCharacterController(GameObject gameObject) {
        return gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
    }
}
