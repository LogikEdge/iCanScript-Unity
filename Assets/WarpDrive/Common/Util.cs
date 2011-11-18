using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Inf {
#if UNITY_EDITOR
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Debug Utilities
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // ----------------------------------------------------------------------
    // Executes the Debug.Log if condition is true.
    public static bool DebugLog(bool cond, object obj) {
        if(cond) {
            Debug.Log(obj);
            return true;
        }
        return false;
    }
    public static bool DebugLog(System.Func<bool> cond, object obj) {
        return DebugLog(cond(), obj);
    }

    // ----------------------------------------------------------------------
    // Executes the Debug.LogWarning if condition is true.
    public static bool DebugWarning(bool cond, object obj) {
        if(cond) {
            Debug.LogWarning(obj);
            return true;
        }
        return false;
    }
    public static bool DebugWarning(System.Func<bool> cond, object obj) {
        return DebugWarning(cond(), obj);
    }

    // ----------------------------------------------------------------------
    // Executes the Debug.LogError if condition is true.
    public static void DebugError(bool cond, object obj) {
        if(cond) {
            Debug.LogError(obj);
        }
    }
    public static void DebugError(System.Func<bool> cond, object obj) {
        DebugError(cond(), obj);
    }
#endif

    
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Code Utilities
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [System.Serializable]
    public class Reference<T> where T : class {
        public T Value= default(T);
    }
    // ----------------------------------------------------------------------
    public static bool IsA(Type topType, Type baseType) {
        for(; topType != null; topType= topType.BaseType) {
            if(topType == baseType) return true;
        }
        return false;        
    } 
    
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Basic Utilities
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public static int Min(int a, int b) { return a < b ? a : b; }
    public static int Max(int a, int b) { return a > b ? a : b; }
    public static int Min(int a, int b, int c) { return a < b ? (a < c ? a : c) : (b < c ? b : c); }
    public static int Max(int a, int b, int c) { return a > b ? (a > c ? a : c) : (b > c ? b : c); }
    public static int Min(int a, int b, int c, int d) { return Min(a, Min(b,c,d)); }
    public static int Max(int a, int b, int c, int d) { return Max(a, Max(b,c,d)); }
    public static int Min(int a, int b, int c, int d, int e) { return Min(a, Min(b,c,d), e); }
    public static int Max(int a, int b, int c, int d, int e) { return Max(a, Max(b,c,d), e); }
    
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// Dictionary
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [System.Serializable]
    public class Dictionary<TKey, TValue> : ICollection< KeyValuePair<TKey, TValue> >, IEnumerable<KeyValuePair<TKey,TValue>>
        where TKey   : System.IComparable<TKey> {

        // ======================================================================
        // ICollection implementation.
        // ----------------------------------------------------------------------        
        public void Add(KeyValuePair<TKey, TValue> item) {
            int idx= IndexOf(item);
            if(idx >= 0) throw new System.ArgumentException();
            myDictionary.Insert(~idx, item);
        }

        // ----------------------------------------------------------------------
        public void Clear() {
            myDictionary.Clear();
        }
        
        // ----------------------------------------------------------------------
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return IndexOf(item) >= 0;
        }
        
        // ----------------------------------------------------------------------
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            myDictionary.CopyTo(array, arrayIndex);
        }

        // ----------------------------------------------------------------------
        public bool Remove(KeyValuePair<TKey, TValue> item) {
            return myDictionary.Remove(item);
        }
        
        // ----------------------------------------------------------------------
        public int Count       { get { return myDictionary.Count; }}
        public bool IsReadOnly { get { return false; }}        


        // ======================================================================
        // IEnumerable implementation.
        // ----------------------------------------------------------------------
        IEnumerator<KeyValuePair<TKey,TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator() {
            return myDictionary.GetEnumerator();
        } 
        IEnumerator IEnumerable.GetEnumerator() {
            return myDictionary.GetEnumerator();
        }
        
        
        // ======================================================================
        // IDictionary implementation.
        // ----------------------------------------------------------------------
        public void Add(TKey _key, TValue _value) {
            if(_key == null) throw new System.ArgumentNullException();
            Add(new KeyValuePair<TKey, TValue>(_key, _value));
        }
        
        // ----------------------------------------------------------------------
        public bool ContainsKey(TKey _key) {
            return IndexOf(_key) >= 0;
        }
        
        // ----------------------------------------------------------------------
        public bool Remove(TKey _key) {
            int idx= IndexOf(_key);
            if(idx < 0) return false;
            myDictionary.RemoveAt(idx);
            return true;
        }
        
        // ----------------------------------------------------------------------
        public TValue this[TKey _key] {
            get {
                int idx= IndexOf(_key);
                if(idx < 0) throw new KeyNotFoundException();
                return myDictionary[idx].Value;
            }
            set {
                KeyValuePair<TKey, TValue> item= new KeyValuePair<TKey,TValue>(_key, value);
                int idx= IndexOf(_key);
                if(idx < 0) { myDictionary.Insert(~idx, item); return; }
                myDictionary[idx]= item;
            }
        }
        
        // ----------------------------------------------------------------------
        public bool TryGetValue(TKey _key, out TValue _outValue) {
            int idx= IndexOf(_key);
            if(idx < 0) { _outValue= default(TValue); return false; }
            _outValue= myDictionary[idx].Value;
            return true;
        }

        // ----------------------------------------------------------------------
        public int IndexOf(TKey _key) {
            return IndexOf(new KeyValuePair<TKey, TValue>(_key, default(TValue)));
        }

        // ----------------------------------------------------------------------
        public int IndexOf(KeyValuePair<TKey, TValue> pair) {
            return myDictionary.BinarySearch(pair, new KeyComparison());
        }
        class KeyComparison : IComparer<KeyValuePair<TKey,TValue>> {
            public int Compare(KeyValuePair<TKey,TValue> a, KeyValuePair<TKey, TValue> b) {
                return a.Key.CompareTo(b.Key);
            }
        }
        
        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        [SerializeField]
        List<KeyValuePair<TKey, TValue>>  myDictionary= new List<KeyValuePair<TKey, TValue>>();
    }

}
