using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// ===========================================================================
// Set of global utilities used by multiple WarpDrive classes.
public class WarpDrive {
    // ---------------------------------------------------------------------------
    // Add the given item to the list if it does not already exist.  True is
    // returned if the item was added.  False is returned otherwise.
    public static bool AddUniqu<T>(T item, List<T> container) where T : IComparable {
        foreach(var obj in container) {
            if(item.CompareTo(obj) == 0) return false;
        }
        container.Add(item);
        return true;
    }
}
