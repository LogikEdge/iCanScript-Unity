using UnityEngine;

[iCS_Class(Company="iCanScript")]
public sealed class iCS_Component {
    // Mesh
    [iCS_Function(Return="meshFilter", Tooltip="Returns the MeshFilter associated with the game object.")]
    public static MeshFilter GetMeshFilter(GameObject gameObject) {
        return gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
    }
    [iCS_Function(Return="textMesh", Tooltip="Returns the TextMesh associated with the game object.")]
    public static TextMesh GetTextMesh(GameObject gameObject) {
        return gameObject.GetComponent(typeof(TextMesh)) as TextMesh;
    }

    // Physics
    [iCS_Function(Return="rigidbody", Tooltip="Returns the Rigidbody associated with the game object.")]
    public static Rigidbody GetRigidBody(GameObject gameObject) {
        return gameObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
    }
    [iCS_Function(Return="characterController", Tooltip="Returns the CharacterController associated with the game object.")]
    public static CharacterController GetCharacterController(GameObject gameObject) {
        return gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
    }
}
