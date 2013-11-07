using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Library="Input", Icon="iCS_JoystickIcon.psd")]
public static class iCS_GameController {
    [iCS_Function(Return="analog1", Icon="iCS_JoystickIcon.psd",
                 Tooltip="RawAnalog returns the raw joystick value while Analog returns the time compensated joystick value ajusted with the input speed.")]

    public static Vector2 GameController(out Vector2 rawAnalog1,
                                         out bool b1, out bool b2, out bool b3,
                                         float speed= 1.0f) {
        float dt= Time.deltaTime;
        float cdt= dt*speed;
        rawAnalog1= new Vector2(Input.GetAxis("Horizontal"), Input.GetAxisRaw("Vertical"));
        b1= Input.GetButton("Fire1");
        b2= Input.GetButton("Fire2");
        b3= Input.GetButton("Fire3");
        return cdt*rawAnalog1;
    }
}
