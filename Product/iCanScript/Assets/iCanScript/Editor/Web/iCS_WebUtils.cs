//#define DEBUG

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_WebUtils {
        // ----------------------------------------------------------------------
        // Performs a Web form request on the given url.
        public static WWW WebRequest(string url, WWWForm form, float waitTime= 2f) {
            WWW response= null;
    		try {
    			response= WaitForTransaction(new WWW(url, form), waitTime);			
    		}
    #if DEBUG
    		catch(System.Exception e) { Debug.LogWarning("iCanScript: Error accessing WWW: "+e.Message); }
    #else
    		catch(System.Exception) {}
    #endif
    		return response;
        }
        // ----------------------------------------------------------------------
        // Performs a Web request on the given url.
        public static WWW WebRequest(string url, float waitTime= 500f) {
            WWW response= null;
    		try {
    			response= WaitForTransaction(new WWW(url), waitTime);			
    		}
    #if DEBUG
    		catch(System.Exception e) { Debug.LogWarning("iCanScript: Error accessing WWW: "+e.Message); }
    #else
    		catch(System.Exception) {}
    #endif
    		return response;
        }
        // ----------------------------------------------------------------------
    	// Wait for Web transaction to complete.
        public static WWW WaitForTransaction(WWW transaction, float waitTime= 2f) {
            var startTime= Time.realtimeSinceStartup;
            while(!transaction.isDone) {
                if((Time.realtimeSinceStartup-startTime) > waitTime) {
    #if DEBUG
                    Debug.LogWarning("iCanScript: Timeout waiting for URL: "+transaction.url);
    #endif
                    break;
                }
            }
            return transaction;	
    	}
    }

}
