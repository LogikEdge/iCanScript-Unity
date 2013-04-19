Installation:
-------------

You will find in attachment iCanScript v0.8.6 (Beta #6).  Use the following steps to install iCanScript:

1) Save the attached file on your hard drive;
2) Open the Unity game engine.
3) Delete the iCanScript folder if it already exists in your project panel;
4) In the project panel, import the iCanScript package that you have saved on your hard drive (Custom Package);
5) Create a game object of your choice;
6) Attach the iCanScript behaviour (from the iCanScript menu) to your game object;
7) You are now ready to create your first visual script program…

You can find some tutorials at:
www.icanscript.com/screencasts

This version is a beta version that has been tested on OSX and Windows XP, Windows Vista & Windows 7.  Disruptive software provides the software as-is and is not responsible for issues associated with it's usage.


iCanScript Package:
-------------------
The iCanScript unity package can be import in your project as any other type of package.  It will automatically create a Gizmos folder and install the iCanScriptGizmo graphic file (used to show in the scene editor which GameObject has a Unity script).

The iCanScript package has several files and most should not be modified. The exception is the files under "iCanScript/Editor/NodeInstaller".  This files can be modified to include your own libraries.

Installing Your Own Library:
----------------------------
You can install your own libraries to be used in iCanScript.  To do so, you can customize the file "iCanScript/Editor/NodeInstaller" to invoke your installer.

iCanScript comes with some predefined nodes that are extracted from Unity and .NET.  The installation of these nodes are performed in the files "iCanScript/Editor/NodeInstaller/iCS_UnityClasses" and "iCanScript/Editor/NodeInstaller/iCS_NETClasses".  These files should not be modified since that may evolve as iCanScript matures.  However, you may find it worthwhile to examine them for an example how to install your own library.

Creating Nodes from Your Source Code:
-------------------------------------
iCanScript offers the ability to directly publish your code has node.  This is realize by tagging your classes, attribute, properties, and functions with iCanScript attributes.  You can find more information on the iCanScript attributes in the programming guide at:

	http://www.icanscript.com/documentation/programmer_guide/creating_nodes


Documentation:
--------------
Major sections of the user and programmer documentation are still work in progress.  Please excuse us for the delay.


Additional Packages:
--------------------
iCanScript base node library is available as a Unity package called:

	iCanScript_Nodes.unitypackage

The nodes in that package are provided as source code and can be used as examples to create your own nodes.  Please note that these nodes will change and mature.   They are currently in their infancy state.


Enjoy and help us make iCanScript better by submitting customer request for bugs and new features.

