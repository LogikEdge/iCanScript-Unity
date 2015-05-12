using System.Collections.Generic;
using System;
using UnityEditor;

namespace iCanScript.Internal.Editor {
    
    public static class HelpDictionary {
    	
    	public static Dictionary<string, string> typeHelp = new Dictionary<string, string>()
    	{
    	    { "Constructor", 
    			"A Variable Builder node is used to construct and initialise a class or variable."
    		},
    	    { "Instance", 
    			"Class nodes have an instance port, you can use a Variable Builder node to create an instance."
    		},
    	    { "Function", 
    			"Function nodes are used to execute behaviours."
    		},
    	    { "VariableReference", 
    			"A Variable Reference is used to accesses a Public Variable.\n\n" +
    			"Create Public Variable: drag a variable from the Library to the Canvas.\n" +
    			"Create Variable Reference: drag a Game Object containing a Public Variable from the Hierarchy into a Message Handler and select."
    		},
    	    { "PublicVariable", 
    			"A Public Variable can be accessed from any Game Object.\n\n" +
    			"Create Public Variable: drag a variable from the Library to the Canvas.\n" +
    			"Create Variable Reference: drag a Game Object containing a Public Variable from the Hierarchy into a Message Handler and select."
    		},	
    	    { "FunctionCall", 
    			"A Function Call is used to invoke a Public Function.\n\n" + 
    			"Create Public Function: right click on the canvas and select.\n" +
    			"Create Function Call: drag a Game Object containing a Public Function from the Hierarchy into a Message Handler and select."
    		},	
    	    { "PublicFunction", 
    			"A Public Function can be invoked from any Game Object.\n\n" +
    			"Create Public Function: right click on the canvas and select.\n" +
    			"Create Function Call: drag a Game Object containing a Public Function from the Hierarchy into a Message Handler and select."
    		},									
    	    { "InstancePort", 
    			"Instance ports are used to connect instances to functions, classes, and variables."
    		},		
    	    { "TriggerPort", 
    			"Trigger Ports are used for flow control.  Will be set to true when execution of the attached node is complete."
    		},		
    	    { "EnablePort", 
    			"Enable ports are used for flow control.  When set to true, attached node will execute"
    		},		
    	    { "UnityEditor.SceneHierarchyWindow", 
    			"<iCS_highlight>Scene Hierarchy tips:</iCS_highlight>\n" +
    			"You can drag iCanScript Objects from the Hierarchy into a message handler to add Variables and User Functions."
    		},
    		{ "UnityEditor.ProjectBrowser", 
    			"<iCS_highlight>Project Browser tips:</iCS_highlight>\n" +
    			"Find prefabs in the Project Browser."
    		},
    		{ "UnityEditor.ConsoleWindow", 
    			"<iCS_highlight>ConsoleWindow tips:</iCS_highlight>\n" +
    			"The Console Window is where you can find details on your iCanScript errors."
    		},
    		{ "UnityEditor.InspectorWindow", 
    			"<iCS_highlight>Inspector Window:</iCS_highlight>\n" +
    			"You can edit parameters of your iCanScript Object in the inspector window under 'iCS_VisualScript (Script)'."
    		},
    		{ "iCS_InstanceEditorWindow",
    			"<iCS_highlight>Instance Wizard tips:</iCS_highlight>\n" +
    			"The Instance Wizard is used to add and remove ports in a class node.  Select a class node in the visual editor to use.\n"	
    		},		
    		{ "iCS_LibraryEditorWindow",
    			"<iCS_highlight>Library tips:</iCS_highlight>\n" +
    			"From the library window, you can drag objects into a message handler in the Visual Editor window to create nodes.\n" +
    			"\n" +
    			"<iCS_highlight>Keyboard shortcuts:</iCS_highlight>\n" +
    			"<iCS_highlight>H:</iCS_highlight> additional help for selected<iCS_x=200><iCS_highlight>Enter:</iCS_highlight> fold/unfold"
    		},																			
    		{ "iCS_VisualEditorWindow",
    			"<iCS_highlight>Keyboard shortcuts (for selected):</iCS_highlight>\n" +
    			"<iCS_highlight>H:</iCS_highlight> more Help<iCS_x=180><iCS_highlight>drag:</iCS_highlight> moves node\n" +
    			"<iCS_highlight>F:</iCS_highlight> Focus in view<iCS_x=180><iCS_highlight>ctrl-drag:</iCS_highlight> moves node outside\n" +
    			"<iCS_highlight>L:</iCS_highlight> auto Layout<iCS_x=180><iCS_highlight>shift-drag:</iCS_highlight> move copies node\n" +
    			"<iCS_highlight>B:</iCS_highlight> Bookmark<iCS_x=180><iCS_highlight>del:</iCS_highlight> deletes object\n" +
    			"<iCS_highlight>G:</iCS_highlight> move to bookmark<iCS_x=180><iCS_highlight>C:</iCS_highlight> Connect to bookmark\n"
    		},
    		{ "iCS_Package", 
    			"The Package is a flexible node. It can contain complex graphs and expose only those ports that are made public by the visual script designer."
    		}
    	};
    
    }
    
}