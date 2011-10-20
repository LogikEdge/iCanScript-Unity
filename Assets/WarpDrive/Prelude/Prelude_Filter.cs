using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ----------------------------------------------------------------------
    // filterWith :: (f a->bool)->(f a->())->[a]->()
    // filter     :: (f a->bool)->[a]->[a]
    public static void filterWith<T>(System.Func<T,bool> cond, System.Action<T> fnc, T[] l) {
        foreach(var obj in l) {
            if(cond(obj)) {
                fnc(obj);
            }
        }
    }
    public static void filterWith<T>(System.Func<T,bool> cond, System.Action<T> fnc, List<T> l) {
        foreach(var obj in l) {
            if(cond(obj)) {
                fnc(obj);
            }
        }
    }
    public static T[] filter<T>(System.Func<T,bool> cond, T[] l) {
        List<T> result= new List<T>();
        filterWith<T>(cond, (obj)=> { result.Add(obj); }, l);
        return result.ToArray();
    }
    public static List<T> filter<T>(System.Func<T,bool> cond, List<T> l) {
        List<T> result= new List<T>();
        filterWith<T>(cond, (obj)=> { result.Add(obj); }, l);
        return result;
    }
    
}
