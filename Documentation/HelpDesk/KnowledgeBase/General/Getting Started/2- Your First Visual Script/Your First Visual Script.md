# Your First Visual Script
In this document, you will learn the core construct of iCanScript by creating a visual script to move a sphere.  We assume you have already [installed iCanScript][install iCanScript] and you have opened a new Unity scene.

    
## 1- Open the iCanScript Editors

iCanScript comes with the following editor windows:

- _**Visual Editor**_ (main edition window);
- _**Library Tree**_ (from where you get your nodes);
- _**Instance Wizard**_ (object-oriented panel to work with object instances);
- _**Tree View**_ (same as Visual Editor but in tree format);
- _**User Preferences Panel**_ (to personalize the look and behaviour).


![][Window Menu]

Open the _**Visual Editor**_, the _**Library Tree**_, the _**Instance Wizard**_ and the _**Tree View**_ and arrange the layout to your liking.

![][Layout Example]

## 2- Create a Sphere Component ##

![][Create Sphere]

Use the menu _**GameObject->Create Other->Sphere**_ to create the sphere object that will host the visual script. 

## 3- Add the Visual Script to the Sphere ##

![][Create Visual Script]

A Visual Script is a component that needs to be added to a game object:

1. Select the _Sphere_ game object and;
2. Create the Visual Script using _**Component->iCanScript->Visual Script**_.

![][Visual Editor Canvas]

The _**Visual Editor**_ will now display a canvas and is now ready to be populated with nodes.

## 4- Every Script Starts With a Message Handler... ##

![][Create Update Message Handler]

Every script in Unity is invoked from a message sent by the Unity framework.  Use the following steps to create the _Update_ message handler for your visual script:

1. Right click (or two finger click for Mac users) on the canvas of the Visual Editor;
2. Select from the menu that appears the "_**+ Update**_"

![][Empty Update Message Handler]

The Visual Script now contains a message handler that will execute when Unity send the _Update_ message.

## 5- Accessing the Sphere Transform

Moving the sphere requires that you modify its transform component.  Before you can do so, you will need to create a node representing its transform.

![][Create Transform Instance Node]

1. Click and drag the _**transform**_ port from the edge into the _**Update**_ node;
2. Release the port inside the _**Update**_ node;
3. Select "_**+ Object Instance**_" from the menu to create a _**transform**_ instance node. 

![][Empty Transform Instance Node]

Have you noticed that the instance port (i.e. "_**this**_" port) is connected to the _**Update::transform**_ port.  This type of connection is called a binding.

## 6- Move the Sphere ##

![][Add Translate Function]

Add the _**translate**_ function to your _**transform**_ instance node using the _**Instance Wizard**_ editor by:

1. Selecting the _**Transform**_ instance node;
	- This will display the available _variables_ and _operations_ in the _**Instance Wizard**_.
2. Scroll the _Operations_ section of the _**Instance Wizard**_ until you see "_**Translate(x:float, y:float, z:float)**_";
3. Click the "_**Translate(x:float, y:float, z:float)**_" to add the function to the node.

![][Change X Translate]

In this example, you will move the sphere in the X-Axis only with constant velocity.  This can be realized by configuring the input ports of the _**translate**_ function of the _**transform**_:

1. Select the _**x.translate**_ port of the _**Transform**_ node;
2. In the _Inspector_, open the iCS_Visual Script component;
3. Modify the _**x.Translate**_ attribute with the value of 0.2 (displacement/frame).

## 7- Run the Visual Script ##
Your visual script is now ready to run.  Just press the run button and watch your sphere move in the x-axis.
 
![][Running Your First Script]

## 8- What Have You Learned? ##

1. Setup your environment to use iCanScript editors;
2. Install a Visual Script;
3. Create a message handler node to respond to Unity _**Update**_ messages;
4. Extract the _**transform**_ instance node from the game object;
5. Populate the _**transform**_ node with the _**translate(...)**_ function using the _**Instance Wizard**_;
6. Configure the _**translate(...)**_ function to move the sphere;
7. Run the visual script.



[install iCanScript]: http://www.icanscript.com/support?view=kb&kbartid=2

[Window Menu]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--WindowMenu.png
[Layout Example]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--LayoutExample.png
[Create Sphere]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--CreateSphere.png
[Create Visual Script]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--CreateVisualScript.png
[Visual Editor Canvas]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--VisualEditorCanvas.png
[Create Update Message Handler]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--CreateUpdateMessageHandler.png
[Empty Update Message Handler]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--EmptyUpdateMessageHandler.png
[Create Transform Instance Node]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--CreateTransformInstanceNode.png
[Empty Transform Instance Node]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--EmptyTransformInstanceNode.png
[Add Translate Function]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--AddTranslateFunction.png
[Change X Translate]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--ChangeXTranslate.png
[Running Your First Script]: http://www.icanscript.com/images/support/kb/your-first-visual-script/iCanScript--GettingStarted--RunningYourFirstScript.png