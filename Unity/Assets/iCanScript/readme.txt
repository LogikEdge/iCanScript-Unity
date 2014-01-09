Installation:
-------------

You will find in attachment iCanScript v0.9.7 (RC1.0e).  Use the following steps to install iCanScript:

1) Save the attached files on your hard drive;
2) Open the Unity game engine.
3) Delete the iCanScript folder if it already exists in your project panel;
4) In the project panel, import the iCanScript package that you have saved on your hard drive (Custom Package);
4.1) If you desire, install the iCanScript_Nodes package.  It includes the source code for basic nodes);
4.2) If you desire, install the iCanScript_Examples packages (the examples require that the node package has been installed).
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
iCanScript offers the ability to directly publish your code has node.  This is realize by tagging your classes, attribute, properties, and functions with iCanScript attributes.  You can find more information on the iCanScript attributes in the user guide in section “Extending iCanScript”

	http://www.icanscript.com/user_guide


Documentation:
--------------
Major sections of the user and programmer documentation are still work in progress.  Please excuse us for the delay.


Additional Packages:
--------------------
iCanScript base node library and examples are available as a Unity package named:

	iCanScript_Nodes.unitypackage
        iCanScript_Examples.unitypackage

The nodes in that package are provided as source code and can be used as examples to create your own nodes.  Please note that these nodes will change and mature.   They are currently in their infancy state.

Getting the Latest Packages:
----------------------------
The iCanScript node & example packages are maintain on Github and can be downloaded using git.  The release versions can be downloaded using the associated "tag".  You may also obtain the latest development loads by cloning the "master" branch.  The URL for the git repositories are:

	https://github.com/Disruptive-Software/iCanScript_Nodes
	https://github.com/Disruptive-Software/iCanScript_Examples


Enjoy and help us make iCanScript better by submitting customer request for bugs and new features.

