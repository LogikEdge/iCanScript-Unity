using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;

public class Test : MonoBehaviour
{
    // Declare a delegate that will be used to execute the completed 
    // dynamic method.  
//    private delegate Vector3 MethodInvoker();
    private delegate Vector3 MethodInvoker();

//    static Delegate hi= null;
    static DynamicMethod hi;
    public Vector3  v3;

    static Test()
    {
        // Create an array that specifies the parameter types of the 
        // overload of Console.WriteLine to be used in Hello.
//        Type[] writeStringArgs = {typeof(string)};
        // Get the overload of Console.WriteLine that has one 
        // String parameter.
        MethodInfo methodInfo = 
            typeof(UnityEngine.Random).GetMethod("get_insideUnitSphere");

        // Create a delegate that represents the dynamic method. This 
        // action completes the method, and any further attempts to 
        // change the method will cause an exception.
        hi = Compile(methodInfo);
   }
   static DynamicMethod Compile(MethodInfo methodInfo) {
        // Create a dynamic method with the name "Hello", a return type
        // of int, and two parameters whose types are specified by the 
        // array helloArgs. Create the method in the module that 
        // defines the Test class.
        DynamicMethod newMethod = new DynamicMethod("insideUnitSphere", 
            methodInfo.ReturnType,
            null);

        // Get an ILGenerator and emit a body for the dynamic method.
        ILGenerator il = newMethod.GetILGenerator();
        // Load the first argument, which is a string, onto the stack.
        // Call the overload of Console.WriteLine that prints a string.
        il.Emit(OpCodes.Jmp, methodInfo);

        return newMethod;
        // Create a delegate that represents the dynamic method. This 
        // action completes the method, and any further attempts to 
        // change the method will cause an exception.
//        return newMethod.CreateDelegate(typeof(MethodInvoker));
   }
   public void Update() {
        // Use the delegate to execute the dynamic method. Save and 
        // print the return value. 
       v3= (Vector3)hi.Invoke(null, null);
   }
}