using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_ErrorController {
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    static iCS_ErrorController() {}
    public static void Start() {}
    public static void Shutdown() {}

    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public class ErrorWarning {
		string				myServiceId   = null;
		string 				myMessage     = null;
		iCS_VisualScriptImp	myVisualScript= null;
		int					myObjectId    = -1;

		public string 				ServiceId 		{ get { return myServiceId; }}
		public string 				Message			{ get { return myMessage; }}
		public iCS_VisualScriptImp	VisualScript	{ get { return myVisualScript; }}
		public int					ObjectId		{ get { return myObjectId; }}
		
		public ErrorWarning(string serviceId, string msg, iCS_VisualScriptImp vs, int objectId) {
			myServiceId= serviceId;
			myMessage= msg;
			myVisualScript= vs;
			myObjectId= objectId;
		}
	};
	
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	static List<ErrorWarning>	myErrors  = new List<ErrorWarning>();
	static List<ErrorWarning>	myWarnings= new List<ErrorWarning>();
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public static int 					NumberOfErrors 		{ get { return myErrors.Count; }}
	public static int 					NumberOfWarnings	{ get { return myWarnings.Count; }}
	public static List<ErrorWarning>	Errors 				{ get { return myErrors; }}
	public static List<ErrorWarning>	Warnings 			{ get { return myWarnings; }}

    // ======================================================================
    // Operations
    // ----------------------------------------------------------------------
	public static void Clear(string serviceId) {
		myErrors  = P.filter(d=> d.ServiceId != serviceId, myErrors);
		myWarnings= P.filter(d=> d.ServiceId != serviceId, myWarnings);
	}
	public static void AddError(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
		myErrors.Add(new ErrorWarning(serviceId, message, vs, objectId));
	}
	public static void AddWarning(string serviceId, string message, iCS_VisualScriptImp vs, int objectId) {
		myWarnings.Add(new ErrorWarning(serviceId, message, vs, objectId));
	}
}
