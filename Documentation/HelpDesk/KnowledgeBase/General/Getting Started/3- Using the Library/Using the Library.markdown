Author: Michel Launier
Date: 2014-06-02
Title: Getting Started -- Using the Library
CSS: GitHub.css

# Using the Library #

**Notice:** This article builds upon the visual script created in the [_Your First Visual Script_][YourFirstVisualScriptArticle] article.

In this article, you will learn the four (4) methods of accessing the library content.  In doing so, you will update the sphere movement created in the previous article to orbit the world center (0,0,0) using simple mathematical function nodes.

Starting from [_Your First Visual Script_][YourFirstVisualScriptArticle] example, perform the following steps:


## 1- Method #1: Using the Instance Wizard

The _**Instance Wizard**_ give you access to the variables and operations available on a specific class of object instance.  You will use the _**Instance Wizard**_ to remove the _Translate(...)_ function and add the _set\_position_ property of the _**Transform**_ component attached to the sphere.

In the visual editor, select the _**Transform**_ node to activate the _**Instance Wizard**_.

![][Remove Translate Function]

In the _operations_ section of the _**Instance Wizard**_, scroll to and click the highlighted _**Translate(x:float, y:float, z:float)**_ to remove it from the _**Transform**_.

![][Add SetPosition Property]

In the _variables_ section of the _**Instance Wizard**_, click the input checkbox of the _**position**_ property to expose it.
 

## 2- Method #2: Browsing the Library Content

The sinus and cosinus functions will be used to orbit the sphere around (0,0,0).  These functions will be extracted from the _Mathf_ class included in the _Unity Engine_ assembly.

![][Add Sin Function]

To add the _**sin**_ function:

1. In the _**Library Panel**_, open the _**Unity**_ top level icon then the _**UnityEngine**_ package;
2. Scroll until you see the _**Mathf**_ class;
3. Drag the _**Sin(f:float)->:float**_ from under _**Mathf**_ into the _**Visual Editor**_ to create the _**Sin**_ function node.

## 3- Method #3: Searching the Library Content

![][Add Cos Function]

Adding the _**cos**_ function:

1. Enter the word "_cos_" in the search field in the _**Library Panel**_;
2. Drag the _**Cos(f:float)->:float**_ item from the library panel into the _**Visual Editor**_ to create the _**Cos**_ function node. 


## 4- Method #4: Contextual Library Menu
 
 The _**Sin**_ and _**Cos**_ mathematical nodes produce separate floating point values. These values must be collated in a _:Vector3_ type to be bound with the _**position**_ property of the _**Transform**_.
 
 
 ![][Add ToVector3 Adaptor]
 
 Creating the _**ToVector3(x:float, y:float, z:float)**_ adaptor node:
 
 1. Drag and release the _**position**_ port of the _**Transform**_ inside the _**Update**_ node;
 2. From the contextual menu, select _**iCanScript | From<->To | FromTo | ToVector(x:float, y:float, z:float)->:Vector3**_ to add the node.

 
![][Bind Sin and Cos]

Cleanup and bind the nodes:

1. Drag the nodes from the title bar to reposition them;
2. Bind the _**Sin**_ output port to the _**x**_ input of the _**ToVector**_ node;
3. Bind the _**Cos**_ output port to the _**z**_ input of the _**ToVector**_ node.

## 5- Complete the Visual Script
 To complete your script, you need to drive the input port of the _**Sin**_ and _**Cos**_ nodes with a time base function.
 
![][Add RealtimeSinceStartup]
 
 1. Search for "_realtime_" in the _**Library panel**_;
 2. Drag the _**realtimeSinceStartup**_ function into the _Update_ node;
 3. Bind the output port of the _**realtimeSinceStartup**_ node to the input ports of both the _**Sin**_ and _**Cos**_ nodes.
 
## 6- Run the Visual Script
 
 The script is complete and can be ran by pressing the _play_ button.  The sphere orbits the position (0,0,0) with a radius of 1.
 
 
## 7- What Have You Learned?
 
 1. Remove the _**translate**_ function using the _**Instance Wizard**_;
 2. Add the set _**position**_ property using the _**Instance Wizard**_;
 3. Add the _**Sin(...)**_ function by browsing the library;
 4. Add the _**Cos(...)**_ function by searching the library;
 5. Add the _**ToVector(...)**_ function using the library contextual menu triggered by dragging a port out in the open.
 
 
 [YourFirstVisualScriptArticle]: http://icanscript.com/support?view=kb&kbartid=4

[Remove Translate Function]:  http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--RemoveTranslateFunction.png
[Add SetPosition Property]: http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--AddSetPositionProperty.png
[Add Sin Function]: http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--AddSinFunction.png
[Add Cos Function]: http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--AddCosFunction.png
[Add ToVector3 Adaptor]: http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--AddToVector3Adaptor.png
[Bind Sin and Cos]: http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--BindSinAndCos.png
[Add RealtimeSinceStartup]: http://www.icanscript.com/images/support/kb/using-the-library/iCanScript--AddRealtimeSinceStartup.png
 
 