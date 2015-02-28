using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;

public class Test : MonoBehaviour
{
    // Declare a delegate that will be used to execute the completed 
    // dynamic method.  
//    private delegate Vector3 MethodInvoker();
    private delegate R	iCS_Delegate<R>();
    private delegate R	iCS_Delegate<P1, R>(P1 p1);
    private delegate R	iCS_Delegate<P1, P2, R>(P1 p1, P2 p2);
    private delegate R	iCS_Delegate<P1, P2, P3, R>(P1 p1, P2 p2, P3 p3);
    private delegate R	iCS_Delegate<P1, P2, P3, P4, R>(P1 p1, P2 p2, P3 p3, P4 p4);
    private delegate R	iCS_Delegate<P1, P2, P3, P4, P5, R>(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);
    private delegate R	iCS_Delegate<P1, P2, P3, P4, P5, P6, R>(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);
	
	
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
        hi = Compile(methodInfo) as iCS_Delegate<Vector3>;
   }
   static Delegate Compile(MethodInfo methodInfo) {
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
        return newMethod.CreateDelegate(typeof(iCS_Delegate<Vector3>));
   }
   static Type GetDelegateType(MethodInfo methodInfo) {
       var parameters= methodInfo.GetParameters();
       var len= parameters.Length;
       var genericTypes= new Type[len+1];
       for(int i= 0; i < len; ++i) {
           genericTypes[i]= parameters[i].ParameterType;
       }
       genericTypes[len]= methodInfo.ReturnType;
       Type baseType= null;
       switch(len) {
           case 0: { baseType= typeof(iCS_Delegate<>);  break; }
           case 1: { baseType= typeof(iCS_Delegate<,>); break; }
           case 2: { baseType= typeof(iCS_Delegate<,,>); break; }
           case 3: { baseType= typeof(iCS_Delegate<,,,>); break; }
           case 4: { baseType= typeof(iCS_Delegate<,,,,>); break; }
           case 5: { baseType= typeof(iCS_Delegate<,,,,,>); break; }
           case 6: { baseType= typeof(iCS_Delegate<,,,,,,>); break; }
       }
       return baseType.MakeGenericType(genericTypes);
   }
   public void Update() {
        // Use the delegate to execute the dynamic method. Save and 
        // print the return value. 
//       v3= hi();
   }
}