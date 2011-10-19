using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // filterWith :: (f a->())->(g a->bool)->[a]->()
    // filter     :: (f a->bool)->[a]->[a]
    public static void filterWith<T>(System.Action<T> fnc, System.Func<T,bool> cond, T[] l) {
        foreach(var obj in l) {
            if(cond(obj)) {
                fnc(obj);
            }
        }
    }
    public static void filterWith<T>(System.Action<T> fnc, System.Func<T,bool> cond, List<T> l) {
        foreach(var obj in l) {
            if(cond(obj)) {
                fnc(obj);
            }
        }
    }
    public static T[] filter<T>(System.Func<T,bool> cond, T[] l) {
        List<T> result= new List<T>();
        filterWith<T>( (obj)=> { result.Add(obj); }, cond, l);
        return result.ToArray();
    }
    public static List<T> filter<T>(List<T> l, System.Func<T,bool> cond) {
        List<T> result= new List<T>();
        filterWith<T>( (obj)=> { result.Add(obj); }, cond, l);
        return result;
    }
    
}
