using UnityEngine;
using System.Collections;

namespace iCanScript.Nodes {
	public static class GameControllerUtility {
	    [iCS_Function(Return="scaledAnalog1", Icon="iCS_JoystickIcon.psd",
	                 Description="Game Controller with configurable names.  Analog1 & 2 are time compensated and ajusted with the input associated speed.")]
	    public static Vector2 GameController(out Vector2 scaledAnalog2,
	                                         out bool action1, out bool action2, out bool action3, out bool action4,
	                                         out bool action5, out bool action6, out bool action7, out bool action8,
	                                         string analog1_x_name= "Horizontal",
	                                         string analog1_y_name= "Vertical",
	                                         float  analog1_scale= 1.0f,
	                                         string analog2_x_name= null,
	                                         string analog2_y_name= null,
	                                         float  analog2_scale= 1.0f,
	                                         string action1_name= "Fire1",
	                                         string action2_name= null,
	                                         string action3_name= null,
	                                         string action4_name= null,
	                                         string action5_name= null,
	                                         string action6_name= null,
	                                         string action7_name= null,
	                                         string action8_name= null) {
	        action1= string.IsNullOrEmpty(action1_name) ? false : Input.GetButton(action1_name);
	        action2= string.IsNullOrEmpty(action2_name) ? false : Input.GetButton(action2_name);
	        action3= string.IsNullOrEmpty(action3_name) ? false : Input.GetButton(action3_name);
	        action4= string.IsNullOrEmpty(action4_name) ? false : Input.GetButton(action4_name);
	        action5= string.IsNullOrEmpty(action5_name) ? false : Input.GetButton(action5_name);
	        action6= string.IsNullOrEmpty(action6_name) ? false : Input.GetButton(action6_name);
	        action7= string.IsNullOrEmpty(action7_name) ? false : Input.GetButton(action7_name);
	        action8= string.IsNullOrEmpty(action8_name) ? false : Input.GetButton(action8_name);
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
}