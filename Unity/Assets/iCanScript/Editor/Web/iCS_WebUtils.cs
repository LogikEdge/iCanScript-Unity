using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using iCanScript;
using P=Prelude;

public static class iCS_WebUtils {
    // ----------------------------------------------------------------------
    // Performs a Web form request on the given url.
    public static WWW WebRequest(string url, WWWForm form, float waitTime= 2f) {
        return WaitForTransaction(new WWW(url, form), waitTime);
    }
    // ----------------------------------------------------------------------
    // Performs a Web request on the given url.
    public static WWW WebRequest(string url, float waitTime= 500f) {
        return WaitForTransaction(new WWW(url), waitTime);
    }
    // ----------------------------------------------------------------------
	// Wait for Web transaction to complete.
    public static WWW WaitForTransaction(WWW transaction, float waitTime= 2f) {
        var startTime= Time.realtimeSinceStartup;
        while(!transaction.isDone) {
            if((Time.realtimeSinceStartup-startTime) > waitTime) {
                Debug.LogWarning("iCanScript: Timeout waiting for URL: "+transaction.url);
                break;
            }
        }
        return transaction;	
	}
}
