using UnityEngine;
using System.Collections;

[UK_Class(Company="Infaunier", Package="Input")]
public sealed class UK_GameController {
    [UK_Function(Return="analog1", Icon="UK_JoystickIcon.psd",
                 ToolTip="RawAnalog returns the raw joystick value while Analog returns the time compensated joystick value ajusted with the input speed.")]
    public static Vector2 GameController(out Vector2 rawAnalog1, float speed= 1.0f) {
        rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        return Time.deltaTime*speed*rawAnalog1;
    }
}
