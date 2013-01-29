using UnityEngine;
using System.Collections;

public class iCS_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string  Name     { get; set; }
    public string  TypeName { get { return GetType().Name; }}
    public int     Priority { get; set; }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Object(string name, int priority) {
        Name= name;
        Priority= priority;
    }
}
