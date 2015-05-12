using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
public static class iCS_Debug {	
	public enum Flags { CodeGeneration };

	public static string Message(string message) {
		return "iCanScript: "+message;
	}
	public static string NullMessage(string message) {
		return Message(message)+" is Null";
	}

	public static void Log(string message) {
		Debug.Log(Message(message));
	}
	public static void LogWarning(string message) {
		Debug.LogWarning(Message(message));
	}
	public static void LogError(string message) {
		Debug.LogError(Message(message));
	}
	public static void Log(Flags flag, string s, Action<string> fnc) {
		if(flag == Flags.CodeGeneration) {
			fnc("iCanScript: "+s);			
		}
	}
    public static void Profile(string idStr, Action action) {
        var time= Time.realtimeSinceStartup;
        action();
        Debug.Log("Profile: "+idStr+"=> "+(Time.realtimeSinceStartup-time));
    }
    public static void Profile(string idStr, float threashold, Action action) {
        var time= Time.realtimeSinceStartup;
        action();
        var deltaTime= Time.realtimeSinceStartup-time;
        if(deltaTime >= threashold) {
            Debug.Log("Profile: "+idStr+"=> "+deltaTime);            
        }
    }
    public static T Profile<T>(string idStr, Func<T> action) {
        var time= Time.realtimeSinceStartup;
        T result= action();
        Debug.Log("Profile: "+idStr+"=> "+(Time.realtimeSinceStartup-time));
        return result;
    }
    public static T Profile<T>(string idStr, float threashold, Func<T> action) {
        var time= Time.realtimeSinceStartup;
        T result= action();
        var deltaTime= Time.realtimeSinceStartup-time;
        if(deltaTime >= threashold) {
            Debug.Log("Profile: "+idStr+"=> "+deltaTime);            
        }
        return result;
    }
}

} // Namespace iCanScript
