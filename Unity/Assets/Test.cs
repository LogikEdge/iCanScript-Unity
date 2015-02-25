using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;

public class Test : MonoBehaviour
{
    // Declare a delegate that will be used to execute the completed 
    // dynamic method.  
//    private delegate Vector3 MethodInvoker();
    private delegate R       iCS_Delegate<R>();
    private delegate Vector3 MethodInvoker();

    static iCS_Delegate<Vector3> hi= null;
    public Vector3  v3;

    static Test()
    {
        // Get the overload of Console.WriteLine that has one 
        // String parameter.
        MethodInfo methodInfo = 
            typeof(UnityEngine.Random).GetMethod("get_insideUnitSphere");

        // Create a delegate that represents the dynamic method. This 
        // action completes the method, and any further attempts to 
        // change the method will cause an exception.
        var delegateType= typeof(iCS_Delegate<>).MakeGenericType(new Type[]{methodInfo.ReturnType});
        hi = Compile(methodInfo, delegateType) as iCS_Delegate<Vector3>;
   }
   static Delegate Compile(MethodInfo methodInfo, Type delegateType) {
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

        // Create a delegate that represents the dynamic method. This 
        // action completes the method, and any further attempts to 
        // change the method will cause an exception.
        return newMethod.CreateDelegate(delegateType);
   }
   static Type GetDelegateType(MethodInfo methodInfo) {
       var parameters= methodInfo.GetParameters();
       var len= parameters.Length;
       var genericTypes= new Types[len+1];
       for(int i= 0; i < len; ++i) {
           genericTypes[i]= parameters[i].ParameterType;
       }
       genericTypes[len]= methodInfo.returnType;
       Type baseType= null;
       switch(len) {
           case 0: {
               baseType= typeof(iCS_Delegate<>);
               break;
           }
           case 1:
           case 2:
           case 3:
           case 4:
           case 5:
           case 6:
       }
       return baseType.MakeGenericType(genericTypes);
   }
   public void Update() {
        // Use the delegate to execute the dynamic method. Save and 
        // print the return value. 
       v3= hi();
   }
}