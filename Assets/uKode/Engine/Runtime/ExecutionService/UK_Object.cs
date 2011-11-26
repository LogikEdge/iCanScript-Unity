using UnityEngine;
using System.Collections;

public class UK_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Name     { get; set; }
    public string TypeName { get { return GetType().Name; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Object(string name) { Name= name; }
}
