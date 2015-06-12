Title: iCanScript Release Notes
Author: Michel Launier
Version: v2.0.20
CSS: Github.css


#  iCanScript Release Notes [Back to top]

<!-- PDF: <<[front-matter/pdf.md] -->
<!-- HTML: <<[front-matter/html.md] -->
![](images/iCanScript-logo_512x512.png)

## iCanScript v2.0.20 ##

_June 8, 2015_

V2.0.20 add the ability to define the generated code variable specification associated with the ports.

With V2.0.20, the user can create public or private ports as well as determine if the port is static (class scope) or not (instance scope).  Support for Constant, Parameters, and Local Variables is also provided. 

**<span style="color: red">\[WARNING\]:</span>**  V2.0.20 requires Unity V5.0.2 or later.  Please upgrade Unity before using this version of iCanScript.

### List of Changes

- **<span style="color: green">\[NEW\] </span>** Added support for selecting the type of variable to be generated in the port dialog editor:
	- Constant;
	- Public & Private Variables;
	- Static Public & Private Variables;
	- Local Variables;
	- Function Definition Parameters.
- **<span style="color: green">\[NEW\] </span>** New port visuals to easily indentify the variable type associated with the port. 
- **<span style="color: blue">\[IMPROVED\] </span>** Improve contrast of the selected node.
- **<span style="color: blue">\[IMPROVED\] </span>** Increase port size to improve variable type visibility.
- **<span style="color: red">\[FIXED\] </span>** Remove unneeded casts in the generated code.
- **<span style="color: red">\[FIXED\] </span>** Visual script data upgrade in now perform on import.

* * *

## iCanScript v2.0.19 ##

_May 28, 2015_

V2.0.19 adds the support for C# primitive variables _**Bool**_, _**Int**_, _**Float**_, and _**String**_.

### List of Changes

- **<span style="color: green">\[NEW\] </span>** Added support for the creation and usage of C# primitive types: _bool_, _int_, _float_, _string_.  The C# primitive types are located in namespace _**CSharp.Primitives**_.
	- **IMPORTANT:** The variables in namespace _**iCanScript.Variables**_ should be replaced by their C# equivalent.  The iCanScript variables are now obsoleted and will be removed in a future release.
- **<span style="color: blue">\[IMPROVED\] </span>** Partial conversion of the API that changed between Unity 5.0.1 and 5.0.2.
	- **NOTE:** To remain compatible with Unity 5.0.1, not all of the API changes have been applied.
- **<span style="color: blue">\[IMPROVED\] </span>** Avoid generating variable and function names that collide with C# reserved keywords.
- **<span style="color: red">\[FIXED\] </span>** Null exception when generating comments for a function parameter that has no description field.

* * *

## iCanScript v2.0.18 ##

_May 25, 2015_

V2.0.18 is a minor bug fix and code generation optimization release.


### List of Changes

- **<span style="color: green">\[NEW\] </span>** Added support for _**operator +(string, Object)**_ in the C# primitive namespace.
- **<span style="color: blue">\[IMPROVED\] </span>**  The length of the generated comments have been readjusted to match the forum post width.
- **<span style="color: blue">\[IMPROVED\] </span>** Removed unnecessary _"using CSharp.Primitive;"_ generated for each file.
- **<span style="color: blue">\[IMPROVED\] </span>** Code generation optimization when target port of a function is connected to the output of a property or field.
- **<span style="color: red">\[FIXED\] </span>** Invalid generated code for the _**Get**_  of a _Field_ variable.

* * *


## iCanScript v2.0.17 ##

_May 20, 2015_

V2.0.17 adds the operators for simple primitive types such as Boolean, Int, and Float.

**<span style="color: red">\[WARNING\]:</span>**  The library _**iCanScript.Logic.Boolean**_ has been replaced by _**CSharp.Primitives.Bool**_.  Note that you may have to regenerate the source code from the visual script if you see errors after you upgrade.

### List of Changes

- **<span style="color: green">\[NEW\]</span>** Added support for C# primitive variable operators. The new operators can be access from the library under namespace: _**CSharp.Primitives**_.  The primitive types that are affected are:
	- _**Bool**_, _**Int**_, and _**Float**_.
- **<span style="color: green">\[NEW\]</span>** An option to _**Show / Hide the Unity Editor library**_ has been added to the library toolbar.
- **<span style="color: blue">\[IMPROVED\]</span>** Properly indent the generated code for the first line of the class declaration.
- **<span style="color: blue">\[IMPROVED\]</span>** The Unity event handlers in the dynamic menu are now sorted.
- **<span style="color: blue">\[IMPROVED\]</span>** The library options _**Show Inherited**_ is now preserved after a recompile or a restart of Unity.
- **<span style="color: blue">\[CHANGED\]</span>** The library options _**Show Protected**_ has been removed from the library toolbar.
- **<span style="color: red">\[REMOVED\]</span>** The library node _**iCanScript.Logic.Boolean**_ has been removed.  Use _**CSharp.Primitives.Bool**_ instead.

* * *

## iCanScript v2.0.16 ##

_May 20, 2015_

V2.0.16 is a code generation bug fix release.

### List of Changes

- **<span style="color: green">\[NEW\]</span>** Support code generation for function definition input parameters.
	- **NOTE:** Output parameters and return value is not yet supported.

* * *

## iCanScript v2.0.15 ##

_May 15, 2015_

V2.0.15 is a library database bug fix release.

### List of Changes

- **<span style="color: blue">\[IMPROVED\]</span>** Don't show the inherited members by default in the library window.
- **<span style="color: blue">\[IMPROVED\]</span>** Rename library branches for element without a namespace to "_**-- no namespace --**_" instead of _\<empty\>_.

* * *

## iCanScript v2.0.14 ##

_May 15, 2015_

V2.0.14 is a critical bug fix release.

 
### List of Changes

- **<span style="color: red">\[FIXED\]</span>** Fix issue introduced in v2.0.12 where new visual script could not be created.
- **<span style="color: red">\[FIXED\]</span>** Fix "Unable to find suitable parent" error message when drawing from the library.

* * *

## iCanScript v2.0.13 ##

_May 15, 2015_

V2.0.13 was rejected due to critical bugs found.

* * *

## iCanScript v2.0.12 ##

_May 13, 2015_

V2.0.12 introduces a redesigned library capable of supporting all features of an object-oriented programming language (ex: C# or C++).

The new library component automatically imports any library added to the project and therefore includes all functionality available in the Unity Engine, Unity Editor, and .NET libraries.

**<span style="color: red">WARNING:</span>** The option to display in the library the functions with a protected accessibility scope is not functional in this release.
 
### List of Changes

- **<span style="color: green">\[NEW\]</span>** The library database and its selection window has been fully designed to support all features of an object-oriented language (C# or C++).
	- **<span style="color: green">\[NEW\]</span>** Imports all public libraries and source code available in the Unity project;
	- **<span style="color: green">\[NEW\]</span>** Library database hierarchy is structured based on the namespaces and types imported from the libraries.
	- **<span style="color: green">\[NEW\]</span>** Three search fields allow to filter the library database content based on the _**namespace**_, the _**type**_, and the _**member (field / property / function)**_ names.

- **<span style="color: blue">\[CHANGED\]</span>** The iCanScript nodes were relocated under the iCanScript namespace to conform with the new library database structure.  A data upgrade will be performed the first a scene created with an earlier version of iCanScript.

- **<span style="color: blue">\[REMOVED\]</span>** The ability to programatically import external libraries into the iCanScript library database has been removed (since all libraries are automatically imported).

- **<span style="color: blue">\[REMOVED\]</span>** The option to include/exclude the Unity editor library using the _Global Configuration panel_ has been removed (the Unity Editor library is always imported).

* * *


## iCanScript v2.0.11 ##

_May 8, 2015_

V2.0.11 was rejected and therefore not  released.

* * *

## iCanScript v2.0.10 ##

_May 1st, 2015_

V2.0.10 was rejected and therefore not  released.

* * *

## iCanScript v2.0.9 ##

_April 24, 2015_

V2.0.9 is a development release of iCanScript2.
### List of Changes

- **<span style="color: green">\[NEW\]</span>** The **Type Name** of the visual script can now be configured in the _Visual Script Configuration_ panel.
- **<span style="color: green">\[NEW\]</span>** iCanScript2 can now be used to create _Editor Scripts_.
	- Enable/Disable the _Editor Script_ option in the _Visual Script Configuration_ to switch between engine and editor scripts.
	- The _Unity Editor Library_ must be enabled to create _Editor Visual Scripts_.
- **<span style="color: green">\[NEW\]</span>** An option to include the **Unity Editor Library** is now available in the _Code Generation_ section of the _Global Preferences_. 
	- **NOTE:** A restart is required for this option to take effect.
- **<span style="color: green">\[NEW\]</span>** The user can now customize the _**code generation folders**_ for the _Editor_ and _Engine_ visual scripts from the _Global Preferences_.
- **<span style="color: green">\[NEW\]</span>** The user can now customize the _**namespace**_ for code generation:
	- **<span style="color: green">\[NEW\]</span>** Separate _Namespaces_ for _Editor_ & _Engine_ code generation can be customized from the _Global Preferences_;
	- **<span style="color: green">\[NEW\]</span>** The _Namespace_ can be overridden on a visual script basis using the _Visual Script Configuration Panel_.
- **<span style="color: blue">\[IMPROVED\]</span>** The _**"Add Unity Event Handler"**_ menu item is no longer displayed if the script does not inherit from _MonoBehaviour_.
- **<span style="color: blue">\[IMPROVED\]</span>** Warning provided to help simplify situation where data and control flow overlap.
- **<span style="color: red">\[FIXED\]</span>** The name of the type can now be configured when a port required a Type value.  The type name format is: _"namespace.type"_.
- **<span style="color: red">\[FIXED\]</span>** Invalid code generated for static functions of _"GameObject"_ and _"Transform"_.


* * *

## iCanScript v2.0.8 ##

_April 22, 2015_

V2.0.8 is a development release of iCanScript2.
### List of Changes

- **<span style="color: green">\[NEW\]</span>** Added a _Visual Script Configuration Panel_ accessible from the visual script toolbar.
- **<span style="color: green">\[NEW\]</span>** The user can now configure the _**base type (inheritance)**_ for the type defined by the visual script.
	- **<span style="color: green">\[NEW\]</span>** The global _Base Type_ can be configured in the _Global Preferences_;
	- **<span style="color: green">\[NEW\]</span>** The _Base Type_ can be overridden for each visual script in the new _Visual Script Configuration Panel_.
- **<span style="color: red">\[FIXED\]</span>** Null exception when attempting to import a field with its value set _null_.
- **<span style="color: red">\[FIXED\]</span>** Fields defined in a base class not properly imported in derived classes resulting in a type not found error.


* * *

## iCanScript v2.0.7 ##

_April 20, 2015_

V2.0.7 resolves two major issues introduced in release v2.0.6.  Sorry for the inconvenience.

### List of Changes

- **<span style="color: red">\[FIXED\]</span>** Enumeration null exception on data upgrade of visual script.
- **<span style="color: red">\[FIXED\]</span>** Invalid error generated on _get_ accessor of fields.

* * *

## iCanScript v2.0.6 ##

_April 20, 2015_

V2.0.6 is a development release of iCanScript2.

### List of Changes

- **<span style="color: blue">\[IMPROVED\]</span>** Add the _'Self'_ port on all function nodes.
- **<span style="color: red">\[FIXED\]</span>** Disabled nodes are now displayed with half intensity in editor mode.


* * *

## iCanScript v2.0.5 ##

_April 19, 2015_

V2.0.5 is a development release of iCanScript2.

### List of Changes

- **<span style="color: blue">\[IMPROVED\]</span>** Automatically generated type-cast now uses the most specialized type instead of the common base type.
- **<span style="color: red">\[FIXED\]</span>** Fix code generation when multiple enables exists on a single node.


* * *

## iCanScript v2.0.4 ##

_April 17, 2015_

V2.0.4 is a development release of iCanScript2.

### List of Changes

- **<span style="color: blue">\[IMPROVED\]</span>** Performance of panning canvas.
- **<span style="color: blue">\[IMPROVED\]</span>** Improve name of trigger port in generated code.
- **<span style="color: blue">\[IMPROVED\]</span>** Restructuring of the dynamic menu for future support of variable & nested type creation.
- **<span style="color: blue">\[IMPROVED\]</span>** Remove illegal option to add _enable/trigger_ ports on a Unity event handler.
- **<span style="color: blue">\[IMPROVED\]</span>** Remove unused menu item _'+ Iterator'_ from contextual menu.
- **<span style="color: red">\[FIXED\]</span>** Fix code generation when _Target_ port is connected to a package input port that is initialized to _Owner_.


* * *

## iCanScript v2.0.3 ##

_April 16, 2015_

V2.0.3 is a development release of iCanScript2.

V2.0.3 introduces the concept of the script _Owner_ which is used to configure input ports to designate the game object on which the script is installed.


### List of Changes

- **<span style="color: green">\[NEW\]</span>** Default _Target_ port value to script _Owner_ if not connected and of type _Transform_ or _GameObject_.
-  **<span style="color: blue">\[IMPROVED\]</span>** Prefix all constructors with the keyword _New_ in the library.
- **<span style="color: red">\[FIXED\]</span>** Opening the port/node editor now requires a double click.
- **<span style="color: red">\[FIXED\]</span>** Remove generation of extra function parameters when input port is connected to a variable defined outside of the function.
- **<span style="color: red">\[FIXED\]</span>** Generate upcast when the _Target_ port when needed.

* * *


## iCanScript v2.0.2 ##

_April 15, 2015_

V2.0.2 is a development release of iCanScript2.

### List of Changes

- **<span style="color: green">\[NEW\]</span>** Error display when a function node cannot find its runtime code. 
- **<span style="color: blue">\[IMPROVED\]</span>** In the contextual menu, rename menu item _**"+ Out Instance Port"**_ to _**"+ Add Self Port"**_.
- **<span style="color: blue">\[IMPROVED\]</span>** In the contextual menu, rename menu item _**"+ Enable Port"**_ to _**"+ Add Enable Port"**_.
- **<span style="color: blue">\[IMPROVED\]</span>** In the contextual menu, rename menu item _**"+ Trigger Port"**_ to _**"+ Add Trigger Port"**_.
- **<span style="color: red">\[FIXED\]</span>** Prepend namespace to generated type names when type exists in more than one namespace.

* * *

## iCanScript v2.0.1 ##
 _April 14, 2015_

V2.0.1 is a development release of iCanScript2.

### List of Changes
- **<span style="color: red">\[FIXED\]</span>** Generation of the _"using"_ directive is now dependent on the content of the visual script.
- **<span style="color: blue">\[IMPROVED\]</span>** Addition of the Unity _"Awake()"_ event handler in the dynamic menu.

* * *

## iCanScript v2.0.0 ##
 _March 20, 2015_

V2.0.0 is the initial development release of iCanScript2.
