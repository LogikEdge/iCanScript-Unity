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
    
    [iCS_Function(Return="analog1", Icon="iCS_JoystickIcon.psd",
                 Tooltip="Game Controller with configurable names.  Analog1 & 2 are time compensated and ajusted with the input associated speed.")]
    public static Vector2 GameControllerExt(out Vector2 rawAnalog1, out Vector2 rawAnalog2, out Vector2 analog2,
                                         out bool b1, out bool b2, out bool b3, out bool b4,
                                         out bool b5, out bool b6, out bool b7, out bool b8,
                                         string analog1_x_name= "Horizontal",
                                         string analog1_y_name= "Vertical",
                                         float  analog1_speed= 1.0f,
                                         string analog2_x_name= null,
                                         string analog2_y_name= null,
                                         float  analog2_speed= 1.0f,
                                         string b1_name= "Fire1",
                                         string b2_name= "Fire2",
                                         string b3_name= "Fire3",
                                         string b4_name= "Jump",
                                         string b5_name= null,
                                         string b6_name= null,
                                         string b7_name= null,
                                         string b8_name= null) {
        float dt= Time.deltaTime;
        rawAnalog1= new Vector2(
            string.IsNullOrEmpty(analog1_x_name) ? 0f : Input.GetAxis(analog1_x_name),
            string.IsNullOrEmpty(analog1_y_name) ? 0f : Input.GetAxisRaw(analog1_y_name)
        );
        rawAnalog2= new Vector2(
            string.IsNullOrEmpty(analog2_x_name) ? 0f : Input.GetAxis(analog2_x_name),
            string.IsNullOrEmpty(analog2_y_name) ? 0f : Input.GetAxisRaw(analog2_y_name)
        );
        analog2= dt*analog2_speed*rawAnalog2;
        b1= string.IsNullOrEmpty(b1_name) ? false : Input.GetButton(b1_name);
        b2= string.IsNullOrEmpty(b2_name) ? false : Input.GetButton(b2_name);
        b3= string.IsNullOrEmpty(b3_name) ? false : Input.GetButton(b3_name);
        b4= string.IsNullOrEmpty(b4_name) ? false : Input.GetButton(b4_name);
        b5= string.IsNullOrEmpty(b5_name) ? false : Input.GetButton(b5_name);
        b6= string.IsNullOrEmpty(b6_name) ? false : Input.GetButton(b6_name);
        b7= string.IsNullOrEmpty(b7_name) ? false : Input.GetButton(b7_name);
        b8= string.IsNullOrEmpty(b8_name) ? false : Input.GetButton(b8_name);
        return dt*analog1_speed*rawAnalog1;
    }
    
}
