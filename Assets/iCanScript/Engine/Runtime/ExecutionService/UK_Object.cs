using UnityEngine;
using System.Collections;

public class iCS_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string  Name     { get; set; }
    public string  TypeName { get { return GetType().Name; }}
    public Vector2 Layout   { get; set; }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Object(string name, Vector2 layout) {
        Name= name;
        Layout= layout;
    }
}
