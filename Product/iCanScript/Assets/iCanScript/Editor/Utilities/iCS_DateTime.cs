using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public static class iCS_DateTime {
        // ---------------------------------------------------------------------------------
    	// Convert current date & time to string
    	public static string DateAsString() {
    		return DateAsString(DateTime.Now);
    	}
    	public static string TimeAsString() {
    		return TimeAsString(DateTime.Now);
    	}
    	public static string DateTimeAsString() {
    		return DateTimeAsString(DateTime.Now);
    	}
        // ---------------------------------------------------------------------------------
    	// Convert specified date & time to string
    	public static string DateAsString(DateTime dateTime) {
    		return dateTime.Year.ToString()+"-"+dateTime.Month.ToString("00")+"-"+dateTime.Day.ToString("00");
    	}
    	public static string TimeAsString(DateTime dateTime) {
    		return dateTime.Hour.ToString("00")+"h"+dateTime.Minute.ToString("00")+"m"+dateTime.Second.ToString("00")+"s";
    	}
    	public static string DateTimeAsString(DateTime dateTime) {
    		return DateAsString(dateTime)+" "+TimeAsString(dateTime);
    	}
    }

}
