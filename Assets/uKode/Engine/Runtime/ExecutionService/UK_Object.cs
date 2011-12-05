using UnityEngine;
using System.Collections;

public class UK_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string  Name     { get; set; }
    public string  TypeName { get { return GetType().Name; }}
    public Vector2 Layout   { get; set; }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Object(string name, Vector2 layout) {
        Name= name;
        Layout= layout;
    }
}
