using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;

public class Test : MonoBehaviour
{
    // Declare a delegate that will be used to execute the completed 
    // dynamic method.  
    private delegate Vector3 HelloInvoker();
    static HelloInvoker hi= null;
    public Vector3  v3;

    static Test()
    {
        // Create an array that specifies the types of the parameters 
        // of the dynamic method. This method has a string parameter 
        // and an int parameter.
//        Type[] helloArgs = {typeof(string), typeof(int)};

        // Create a dynamic method with the name "Hello", a return type
        // of int, and two parameters whose types are specified by the 
        // array helloArgs. Create the method in the module that 
        // defines the Test class.
        DynamicMethod hello = new DynamicMethod("Hello", 
            typeof(Vector3), 
            null, 
            typeof(Test).Module);

        // Create an array that specifies the parameter types of the 
        // overload of Console.WriteLine to be used in Hello.
//        Type[] writeStringArgs = {typeof(string)};
        // Get the overload of Console.WriteLine that has one 
        // String parameter.
        MethodInfo writeString = 
            typeof(UnityEngine.Random).GetMethod("get_insideUnitSphere");

        // Get an ILGenerator and emit a body for the dynamic method.
        ILGenerator il = hello.GetILGenerator();
        // Load the first argument, which is a string, onto the stack.
        // Call the overload of Console.WriteLine that prints a string.
        il.EmitCall(OpCodes.Call, writeString, null);
        // The Hello method returns the value of the second argument; 
        // to do this, load the onto the stack and return.
        il.Emit(OpCodes.Ret);

        // Create a delegate that represents the dynamic method. This 
        // action completes the method, and any further attempts to 
        // change the method will cause an exception.
        hi = (HelloInvoker) hello.CreateDelegate(typeof(HelloInvoker));

        // Use the delegate to execute the dynamic method. Save and 
        // print the return value. 
//        Vector3 retval = hi();
   }
   public void Update() {
        // Use the delegate to execute the dynamic method. Save and 
        // print the return value. 
       v3= hi();
   }
}