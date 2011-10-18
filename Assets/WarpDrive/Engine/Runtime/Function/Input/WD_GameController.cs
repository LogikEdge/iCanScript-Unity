using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Input")]
public sealed class WD_GameController {
    [WD_Function(Return="analog1", Icon="Assets/WarpDrive/Editor/Resources/WD_JoystickIcon.psd")]
    public static Vector2 GameController(out Vector2 rawAnalog1, float speed= 1.0f) {
        rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        return Time.deltaTime*speed*rawAnalog1;
    }
}
