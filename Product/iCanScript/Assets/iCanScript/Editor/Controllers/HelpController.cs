using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using iCanScript.Internal.JSON;
using iCanScript.Internal.Engine;


namespace iCanScript.Internal.Editor {
    
    public static class HelpController {
    	static private string unityHelpIndex= null;
    	static private Dictionary<string, string> unityHelpSummary= new Dictionary<string, string>();
    	static string unityHelpPath = EditorApplication.applicationContentsPath + "/Documentation/html/en/ScriptReference";
    
    	const string noHelp= "no help available" ;
    	
    	// ---------------------------------------------------------------------------------
    	static HelpController() {
    		buildUnityHelpIndex();
    		buildUnityHelpSummary();
    	}
    
    	// ---------------------------------------------------------------------------------
    	public static void Start() {}
    
    	// ---------------------------------------------------------------------------------	
    	public static void Shutdown() {}
    
    
    	// =================================================================================
    	// Convert the JavaScript unity help index file "index.js" to a json string.
    	// ---------------------------------------------------------------------------------
    	private static void buildUnityHelpIndex() {
    		StreamReader fileStream;
    		try {
    			fileStream = new StreamReader (unityHelpPath + "/index.js");
    		}
    		catch {
				// TODO: Fix help file access
//    			Debug.LogWarning("iCanScript: unable to open Unity help.");
    			return;
    		}
    		
    		StringBuilder unityHelpIndexBuilder = new StringBuilder("");
    		string line;
    		
    		// Read the info file one line at a time (stripping spaces), and convert the java script formating to json where needed.
    		using(fileStream) { 
    			while( (line= fileStream.ReadLine()) != null ) {
    				line= line.Trim();
    				// Remove comments which are not valid for JSON.  Warning there are some "//" in the strings, so do not use regex through entire line to find comments.
    				if( !(line.StartsWith("//"))) {
    					// Change ' var xxx = ' format to ' {"xxx": '
    					Regex regex = new Regex("(var )(.*?)(=)");
    					Match match = regex.Match(line);
    					if (match.Success) {
    						string name= match.Groups[2].ToString().Trim();	
    						line= Regex.Replace(line, "var .*?=", "{\"" + name + "\":");
    					}
    					
    					// Change "};" to "}", and "];" to ]}". 
    					line= Regex.Replace(line, "};", "}");
    					line= Regex.Replace(line, "];", "]}");
    					unityHelpIndexBuilder.Append(line + "\n");
    				}
    			}
    			fileStream.Close ();
    		}
    		unityHelpIndex= unityHelpIndexBuilder.ToString();
    	}
    
    	private static string formatFromHTML(string stringToFormat)
    	{
    		// TODO: can support rich text, but at present rich text does not seem to work in 
    		// Just strip out HTML tags for now
    		return Regex.Replace(stringToFormat, "<.*?>", string.Empty);
    	}	
    		
    
    	// =================================================================================
    	// Get the summary descriptin for a unity API by providing class.function
    	// ---------------------------------------------------------------------------------
    	private static void buildUnityHelpSummary()
    	{
    		JArray  arrayOfEntries;
    		try {
    			JObject rootObject= JSON.JSON.GetRootObject(unityHelpIndex);	
    			arrayOfEntries= rootObject.GetValueFor("info") as JArray;
    		}
    		catch {
				// TODO: Fix Unity help file access.
//    			Debug.LogWarning("iCanScript: Error reading unity help index as JSON format");
    			return;
    		}
    		
    		foreach (JObject jObject in arrayOfEntries.value) {
    			// Get the url and summary as per the index.js file.
    			JString jSummary;
    			JString jUrl;
    			try {
    				jUrl= jObject.GetValueFor("url") as JString;
    				jSummary= jObject.GetValueFor("summary") as JString;
    			}
    			catch {
    				Debug.Log("iCanScript: Error reading line in JSON unity help index");
    				return;
    			}
    	
    			// replace HTML formating in summary
    			string summary= formatFromHTML(jSummary.value);
    			
    			// Transfer information to dictionary
    			// Warning: "title" and "url" are not all unique in current index.js
    			try {
    				unityHelpSummary.Add(jUrl.value, summary);
    			}
    			catch {
    				// Duplicate URL's are caught here.  This is known to happen in regular case.
    				//Debug.Log("Duplicate: "+ jUrl.value + "\nsummary: " + summary);
    			}
    		}
    	}
    	
    	
    	// =================================================================================
    	// Get the summary help description 
    	// ---------------------------------------------------------------------------------
    	
    	public static string getHelp(iCS_EditorObject edObj )
    	{	
    		if(edObj != null) {
    			// Get the EditorObject tooltip if there is one.
    			string help= edObj.Description;
    			if(!String.IsNullOrEmpty(help)) {
    				return help;
    			}
    					
    			// otherwise try and get help based on the MemberInfo,
    			help= getHelp(getAssociatedHelpMemberInfo(edObj));
    			if(!String.IsNullOrEmpty(help)) return help;
    			
    			// Otherwise try and get help based on engineObject Type.
    			// Carefull with the order, since for example some specific types are also a package, class, builder, or function!
    			if (edObj.IsNode) {
    				if(edObj.IsFunctionDefinition)
    					return getHelp("PublicFunction");
    				else if(edObj.IsVariableDefinition)
    					return getHelp("PublicVariable");
    				else if(edObj.IsConstructor)
    					return getHelp("Constructor");
    				else if(edObj.IsKindOfFunction)
    					return getHelp("Function");
    				else if(edObj.IsInstanceNode)
    					return getHelp("Instance");
    			}
    			else if (edObj.IsPort) {
    				if (edObj.IsTargetPort)
    					return getHelp("TargetPort");
                    else if(edObj.IsSelfPort)
                        return getHelp("SelfPort");
    				else if(edObj.IsTriggerPort)
    					return getHelp("TriggerPort");
    				else if(edObj.IsEnablePort)
    					return getHelp("EnablePort");
    			}
    			// otherwise try and get help bases on edObj type.
    			return getHelp(edObj.RuntimeType);
    		}
    		return null;
    	}
    	
    	
    	public static string getHelp(LibraryMemberInfo memberInfo )
    	{
    		if(memberInfo != null) {
//    			// Return help string if there is already one in the memberInfo
//    			if (memberInfo.HelpSummaryCache != null) {
//    				return memberInfo.HelpSummaryCache;
//    			}
//    			
//    			// Try and use MemberInfo Description
//    			if (!String.IsNullOrEmpty(memberInfo.StoredDescription)) {
//    				return memberInfo.StoredDescription;
//    			}
//    			
//    			// If there is no help string already in MemberInfo, try and look up Unity help
//    			if (memberInfo.HelpSummaryCache==null && memberInfo.Company == "Unity") {
//    				string search= getHelpUrl(memberInfo);
//    				string summary=null;
//    				unityHelpSummary.TryGetValue(search, out summary);
//    				if (!String.IsNullOrEmpty(summary)) {
//    					// cache and return the found help string
//    					memberInfo.HelpSummaryCache= summary;
//    					return summary;
//    				}
//    			}
//    			
//    			// If there is no help found yet, try and return help based on type
//    			String typeHelp= getHelp(memberInfo.GetType());
//    			if (!String.IsNullOrEmpty(typeHelp)) {
//    				memberInfo.HelpSummaryCache= typeHelp;
//    			}	
//    			else
//    			{
//    				// Mark cache as empty string (vs null), so we do not search again 
//    				memberInfo.HelpSummaryCache= "";	
//    			}				
//    			return typeHelp;
    		}
    		return null;
    	}
    	
    	public static string getHelp(Type type)
    	{
            if(type == null) return "";
    		return getHelp(type.ToString());
    	}
    	
    	public static string getHelp(string typeName)
    	{
    		string help=null;
    		HelpDictionary.typeHelp.TryGetValue(typeName, out help);
    		return help;
    	}
    		
    	public static LibraryMemberInfo getAssociatedHelpMemberInfo(iCS_EditorObject edObj) 
    	{
    		if(edObj != null) {
    			if (edObj.IsPort) {	
    				// check for special types of ports. 
    				// TODO: support these.
    				if (edObj.PortIndex == (int)iCS_PortIndex.Return && edObj.ParentNode.IsStaticField) {
    					// return port will be same as parent node description.
    				}
    				else if(edObj.IsKindOfPackagePort && !edObj.IsInstanceNodePort && !edObj.IsProposedDataPort)
    					return null;
    				else if( edObj.PortIndex >= (int)iCS_PortIndex.ParametersEnd ) {
    					return null;
    				}				
    				
    				// Following port will contain the member info
    				if (edObj.IsProposedDataPort) {
    					// contained in edObj
    				}
    				else if (edObj.IsInputPort) {
                        var consumerPorts= edObj.SegmentEndConsumerPorts;
                        var aConsumer= consumerPorts.Length != 0 ? consumerPorts[0] : edObj;
    					edObj= aConsumer.Parent;
    				}
    				else if (edObj.IsOutputPort) {
    					edObj= edObj.SegmentProducerPort.Parent;
    				}							
    			}
    			
    			LibraryMemberInfo memberInfo=null;
    			
    			// Try and Get Member Info from GetAssociatedDescriptor 
    			if(edObj.IsKindOfFunction || edObj.IsEventHandler) {
    				memberInfo= edObj.GetLibraryObject() as LibraryMemberInfo;
    			}
    			
    			return memberInfo;
    			
    		}
    		return null;
    	}
    	
    	
    	public static string GetHelpTitle(iCS_EditorObject edObj, bool displayType, bool displayParentNode) {
    		if (edObj==null)
    			return null;
    		
    		string parentName= null;
    		string typeName= null;
    		string displayName= edObj.DisplayName;
    		// Variable name "<Color>" interferes with the RTF ... so modify it.
    		displayName= Regex.Replace(displayName, "<Color>", "< Color >");	
    		
    		if (edObj.IsNode) {
    			if(displayType) {
    				// Type names to be displayed in front of node name.
    				// Carefull with the order, since for example some specific types are also a package, class, builder, or function!
    				if(edObj.IsEventHandler)
    					typeName= "Unity Event Handler";
    				else if(edObj.IsFunctionDefinition)
    					typeName= "Function Definition";
    				else if(edObj.IsVariableDefinition)
    					typeName= "Variable Definition";
    				else if(edObj.IsConstructor)
    					typeName= "Variable Builder";
    				else if(edObj.IsKindOfFunction)
    					typeName= "Function";
    				else if(edObj.IsInstanceNode)
    					typeName= "Class Instance";
    				else if(edObj.IsPackage)
    					typeName= "Package";
    				else
    					return null;
    			}
    		}
    		else if (edObj.IsPort) {
    			if(displayType) {
    				// change Type for special types of ports. 
    				if (edObj.IsTargetPort) {
    					typeName= "Target";	
    				}
    				else if(edObj.IsSelfPort) {
    				    typeName= "Self";
    				}
                    else {
    					typeName= iCS_Types.TypeName(edObj.RuntimeType);
    				}
    			}
    		}
    		
    		if(displayParentNode) {
    			parentName= edObj.Parent.DisplayName + ".";
    		}
    			
    		return "<b>" + typeName + "</b><iCS_highlight> " + parentName + displayName + "</iCS_highlight>";
    	}	
    	
    	
    	public static string GetHelpTitle(LibraryObject libraryObject) {
    		string title= "<iCS_highlight>" + libraryObject.nodeName + "</iCS_highlight>";
    		string typeName= null;
			string className= null;
			if(libraryObject is LibraryMemberInfo) {
				var memberInfo= libraryObject as LibraryMemberInfo;
	    		className= iCS_Types.TypeName(memberInfo.declaringType);				
			}
    	
    		if(libraryObject is LibraryField) {
    			typeName="Property of " + className;
    		}
    		else if(libraryObject is LibraryProperty) {
    			typeName= "Property of " + className;
    		}
          	else if(libraryObject is LibraryConstructor) {
    			typeName= "Variable Builder";
            } 
    		else if(libraryObject is LibraryFunction) {
    			typeName="Function of " + className;             
            } 
    		else if(libraryObject is LibraryEventHandler) {
    			typeName="Event Handler for " + className;
            }
    		else if(libraryObject is LibraryType) {
    			typeName="Class Instance";
    		}
    
    		return "<b>" + typeName + " " + title + "</b>";
    	}
    	
    	public static string GetHelpParameters(LibraryMethodBase libraryMethodBase) {
    		string parameters= null;
    
//          	if(memberInfo.IsConstructor) {
//                parameters= memberInfo.ToConstructorInfo.FunctionParameters();
//            } 
//    		else if(memberInfo.IsMethod) {
//                parameters= memberInfo.ToMethodInfo.FunctionParameters();                
//            } 
//    		else if(memberInfo.IsMessage) {
//                parameters= memberInfo.ToMessageInfo.FunctionParameters();
//            }
    
    		return parameters;
    	}
    	
    	
    	// =================================================================================
    	// Open web browser for specific help
    	// ---------------------------------------------------------------------------------		
    
    	public static void openDetailedHelp(iCS_EditorObject edObj ) {
    		openDetailedHelp(getAssociatedHelpMemberInfo(edObj));
    	}	
    
    	public static void openDetailedHelp(LibraryMemberInfo memberInfo )	
    	{	
//    		if(memberInfo != null) {
//    			if (memberInfo.Company == "Unity") {
//    				string search= getHelpUrl(memberInfo);
//    				if (search != null)
//    					Help.ShowHelpPage("file:///unity/ScriptReference/" + search + ".html");
//    			}
//    		}
    	}	
    
    	// =================================================================================
    	// Get the Unity help file url 
    	// ---------------------------------------------------------------------------------		
    	private static string getHelpUrl(LibraryMemberInfo memberInfo)	
    	{		
			return "";
			
//			string className="";
//			string demarcator="";
//			string methodName="";
//			
//			if (memberInfo.IsMessage) {
//				className = memberInfo.ParentTypeInfo.ClassName;
//				demarcator= ".";
//				methodName= memberInfo.DisplayName; 
//			}
//			else if (memberInfo.IsTypeInfo) {
//				// First level libray entries (classes and packages), just return className
//				className = memberInfo.ToTypeInfo.ClassName;
//			}
//			else if (memberInfo.IsMethod) {
//				if(memberInfo.ToMethodInfo.DeclaringType.Name == null)
//					className= memberInfo.ParentTypeInfo.ClassName;
//				else
//					className= memberInfo.ToMethodInfo.DeclaringType.Name;
//				
//				methodName= memberInfo.ToMethodInfo.MethodName;
//				if (memberInfo.IsProperty) {
//					// Property Nodes
//					demarcator="-";
//					methodName= Regex.Replace(methodName, "get_", string.Empty);
//					methodName= Regex.Replace(methodName, "set_", string.Empty);
//				}
//				else if(memberInfo.IsConstructor) {
//					// Builders
//					demarcator="-ctor";
//					methodName= "";
//				}
//				else {
//					// Functions, etc.
//					demarcator= ".";
//					// Remap arithmetic operator names
//					if (methodName.Contains("op_")) {
//						demarcator="-";
//						methodName= Regex.Replace(methodName, "op_Addition", "operator_add");
//						methodName= Regex.Replace(methodName, "op_Division", "operator_divide");
//						methodName= Regex.Replace(methodName, "op_Equality", "operator_eq");
//						methodName= Regex.Replace(methodName, "op_Inequality", "operator_ne");
//						methodName= Regex.Replace(methodName, "op_Multiply", "operator_multiply");
//						methodName= Regex.Replace(methodName, "op_Subtraction", "operator_subtract");
//						//methodName= Regex.Replace(methodName, "op_UnaryNegation", ???);
//						// TODO: else Debug.Log 
//						// TODO: More opertators .. csharp operators.
//					}
//				}
//			}
//			else if (memberInfo.IsField) {
//				// Field Nodes
//				className= memberInfo.ParentTypeInfo.ClassName;
//				methodName= memberInfo.ToFieldInfo.MethodName;	
//				if(memberInfo.ToFieldInfo.IsClassMember)
//					demarcator= ".";
//				else if(memberInfo.ToFieldInfo.IsInstanceMember) 
//					demarcator= "-";
//				else 
//					demarcator= ".";
//			}		
//	
//			return className + demarcator + methodName;
    	}
    
   }

}