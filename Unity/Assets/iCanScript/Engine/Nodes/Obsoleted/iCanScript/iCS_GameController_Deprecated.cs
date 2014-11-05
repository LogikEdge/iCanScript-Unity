using UnityEngine;
using System.Collections;

//[iCS_Class(Company="iCanScript", Library="Input", Icon="iCS_JoystickIcon.psd")]
/// -- Deprecated --
public static partial class iCS_GameController {
    // -- Deprecated --
    public static Vector2 GameController(out Vector2 analog1,
                                         out bool b1, out bool b2, out bool b3,
                                         float scale= 1.0f) {
        analog1= new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        b1= Input.GetButton("Fire1");
        b2= Input.GetButton("Fire2");
        b3= Input.GetButton("Fire3");
        return scale*analog1;
    }
    
    // -- Deprecated --
    public static Vector2 GameControllerExt(out Vector2 scaledAnalog2,
                                         out bool b1, out bool b2, out bool b3, out bool b4,
                                         out bool b5, out bool b6, out bool b7, out bool b8,
                                         string analog1_x_name= "Horizontal",
                                         string analog1_y_name= "Vertical",
                                         float  analog1_scale= 1.0f,
                                         string analog2_x_name= null,
                                         string analog2_y_name= null,
                                         float  analog2_scale= 1.0f,
                                         string b1_name= "Fire1",
                                         string b2_name= "Fire2",
                                         string b3_name= "Fire3",
                                         string b4_name= "Jump",
                                         string b5_name= null,
                                         string b6_name= null,
                                         string b7_name= null,
                                         string b8_name= null) {
        b1= string.IsNullOrEmpty(b1_name) ? false : Input.GetButton(b1_name);
        b2= string.IsNullOrEmpty(b2_name) ? false : Input.GetButton(b2_name);
        b3= string.IsNullOrEmpty(b3_name) ? false : Input.GetButton(b3_name);
        b4= string.IsNullOrEmpty(b4_name) ? false : Input.GetButton(b4_name);
        b5= string.IsNullOrEmpty(b5_name) ? false : Input.GetButton(b5_name);
        b6= string.IsNullOrEmpty(b6_name) ? false : Input.GetButton(b6_name);
        b7= string.IsNullOrEmpty(b7_name) ? false : Input.GetButton(b7_name);
        b8= string.IsNullOrEmpty(b8_name) ? false : Input.GetButton(b8_name);
        bool isAnalog1XNameEmpty= string.IsNullOrEmpty(analog1_x_name);
        bool isAnalog1YNameEmpty= string.IsNullOrEmpty(analog1_y_name);
        var analog1= new Vector2(
            isAnalog1XNameEmpty ? 0f : Input.GetAxis(analog1_x_name),
            isAnalog1YNameEmpty ? 0f : Input.GetAxis(analog1_y_name)            
        );
        bool isAnalog2XNameEmpty= string.IsNullOrEmpty(analog2_x_name);
        bool isAnalog2YNameEmpty= string.IsNullOrEmpty(analog2_y_name);
        var analog2= new Vector2(
            isAnalog2XNameEmpty ? 0f : Input.GetAxis(analog2_x_name),
            isAnalog2YNameEmpty ? 0f : Input.GetAxis(analog2_y_name)            
        );
        scaledAnalog2= analog2*analog2_scale;
        return analog1*analog1_scale;
    }    
}
