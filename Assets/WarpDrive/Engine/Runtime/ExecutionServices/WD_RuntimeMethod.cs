using UnityEngine;
using System.Collections;

public delegate object MethodInvoke(object self, object[] args);
public class WD_RuntimeMethod : WD_Runtime {
    // ======================================================================
    // Attributes
    // ----------------------------------------------------------------------
    public   object         Self= null;
    public   object[]       Args= null;
    public   object         Value= null;
    internal MethodInvoke   Method;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public static WD_RuntimeMethod CreateValue(object value) {
        WD_RuntimeMethod m= new WD_RuntimeMethod();
        m.Value= value;
        m.Method= GetValue;
        return m;
    }  
    // ----------------------------------------------------------------------
    public static WD_RuntimeMethod CreateFunction(MethodInvoke method, int nbOfArgs) {
        WD_RuntimeMethod m= new WD_RuntimeMethod();
        m.Args= new object[nbOfArgs];
        m.Method= method;
        return m;
    }
    // ----------------------------------------------------------------------
    public static WD_RuntimeMethod CreateMethod(object self, MethodInvoke method, int nbOfArgs) {
        WD_RuntimeMethod m= CreateFunction(method, nbOfArgs);
        m.Self= self;
        return m;        
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public object Invoke() {
        return Value= Method(Self, Args);
    }

    // ----------------------------------------------------------------------
    private WD_RuntimeMethod() {}
    private static object GetValue(object self, object[] args) {
        return (self as WD_RuntimeMethod).Value;
    }
}
