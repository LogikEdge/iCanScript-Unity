using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using iCanScript;

public class iCS_CSGenerateBehaviour {
	// ----------------------------------------------------------------------
    public static void UpdateBehaviourCode() {
        // Retrieve all messages needed by behaviour.
        var messages= GetMessagesForBehaviour();
        
        // Build class name and file path.
        var fileName= iCS_EditorStrings.DefaultBehaviourFilePath;
        var className= Path.GetFileNameWithoutExtension(fileName);
        
        // Build behaviour code.
        var code= BehaviourMessageProxy(className, messages);
        string codeManifestHash= iCS_TextUtility.CalculateMD5Hash(code);

        // Extract file manifest hash value
        JObject jsonHeader= null;
        jsonHeader= iCS_CSFileTemplates.ExtractJSON(iCS_TextFileUtility.ReadFile(fileName));
        string fileManifestHash= null;
        if(jsonHeader != null) {
            JString jsonContentHashValue= jsonHeader.GetValueFor("ContentHash") as JString;
            if(jsonContentHashValue != null) {
                fileManifestHash= jsonContentHashValue.value;
            }
        }

        // Update file if manifest has changed
//        Debug.Log("iCanScript: Behaviour file manifest hash: "+fileManifestHash);
//        Debug.Log("iCanScript: Behaviour code manifest hash: "+codeManifestHash);
        if(string.Compare(codeManifestHash, fileManifestHash) != 0) {
            Debug.Log("iCanScript: Rebuilding Behaviour code ...");
            var jsonManifestHash= new JObject(new JNameValuePair("ContentHash", codeManifestHash));
            iCS_TextFileUtility.WriteFile(fileName, iCS_CSFileTemplates.PrependJSON(jsonManifestHash, code));            
        }
    }
    
    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    static string GenerateBehaviourClassImports(string className) {
        return "\nusing UnityEngine;\n";
    }
	// ----------------------------------------------------------------------
    static string GenerateBehaviourClassHeader(string className) {
        return "\npublic sealed class "+className+" : MonoBehaviour {\n"+
               "\tiCS_VisualScript[]   allVisualScripts= null;\n\n"+
               "\tvoid Start()\n"+
               "\t{\n"+
               "\t\tallVisualScripts= gameObject.GetComponents<iCS_VisualScript>();\n"+
               "\t}\n";
    }
    static string GenerateBehaviourClassTrailer(string className) {
        return "\n}\n";
    }
	// ----------------------------------------------------------------------
    public static string BehaviourMessageProxy(string className, iCS_MessageInfo[] messages) {
        var fileHeader= iCS_CSFileTemplates.FileHeader(className+".cs", className);
        var imports= GenerateBehaviourClassImports(className);
        var classHeader= GenerateBehaviourClassHeader(className);
        var classTrailer= GenerateBehaviourClassTrailer(className);
        var messageImpls= "";
        foreach(var msg in messages) {
            messageImpls+= GenerateMessageReceiver(msg)+"\n";
        }
        return fileHeader+imports+classHeader+messageImpls+classTrailer;
    }

    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    public static string GenerateMessageReceiver(iCS_MessageInfo message) {
        // Start message is already handle...
        if("Start" == message.DisplayName) return "";
        return "\n\t"+GenerateMessageSignature(message)+GenerateMessageBody(message);
    }
	// ----------------------------------------------------------------------
    public static string GenerateMessageSignature(iCS_MessageInfo message) {
        return MethodSignature(message);
    }
	// ----------------------------------------------------------------------
    public static string GenerateMessageBody(iCS_MessageInfo message) {
        var parameters= message.Parameters;
        var len= parameters.Length;
        var msgParamStr= "";
        for(int i= 0; i < len; ++i) {
            msgParamStr+= parameters[i].name;
            if(i < len-1) {
                msgParamStr+= ", ";
            }
        }
        var paramStr= "\""+message.DisplayName+"\""+(msgParamStr.Length != 0 ? ", ":"")+msgParamStr;
        // Special case for the Start() message that generates the code.
        return "\n\t{\n"+
               "\t\tif(allVisualScripts != null) {\n"+
               "\t\t\tforeach(var vs in allVisualScripts) {\n"+
               "\t\t\t\tvs.RunMessage("+paramStr+");\n"+
               "\t\t\t}\n"+
               "\t\t}\n"+
               "\t}\n";            
    }

    // ======================================================================
    // Method/Function code generation
	// ----------------------------------------------------------------------
    public static string MethodSignature(iCS_MethodBaseInfo method) {
        var storageClass= "";
        if(method.IsClassMember) storageClass= "static ";
        var returnType= "void";
        if(method.FunctionReturn != null) {
            returnType= TypeName(method.FunctionReturn.type);
        }
        var name= method.DisplayName;
        var parameters="";
        if(method.Parameters != null) {
            foreach(var p in method.Parameters) {
				parameters+= TypeName(p.type)+" "+p.name+", ";
            }            
        }
        if(parameters.Length != 0) parameters= parameters.Substring(0, parameters.Length-2);
        return storageClass+returnType+" "+name+"("+parameters+")";
    }
	// ----------------------------------------------------------------------
    public static string MethodStorageClass(iCS_MethodBaseInfo method) {
        return method.IsClassMember ? "static" : "";
    }
	// ----------------------------------------------------------------------
    public static string[] MethodParameters(iCS_MethodBaseInfo method) {
        var methodParameters= method.Parameters;
        if(methodParameters == null) {
            return new string[0];
        }
        var len= methodParameters.Length;
        var parameters= new string[len];
        for(int i= 0; i < len; ++i) {
            var p= methodParameters[i];
            parameters[i]= TypeName(p.type)+" "+p.name;
        }
        return parameters;           
    }

	// ----------------------------------------------------------------------
    // Returns the messages associated with the behaviour node.  For now
    // all possible messages installed in the library are returned.
    public static iCS_MessageInfo[] GetMessagesForBehaviour() {
        return iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
    }

    // ======================================================================
    // Behaviour JSON manifest creation
	// ----------------------------------------------------------------------
    public static JArray BehaviourMessagesInJSON() {
        var behaviourMessages= GetMessagesForBehaviour();
        var jMessageObjects= new JObject[behaviourMessages.Length];
        for(var x= 0; x < behaviourMessages.Length; ++x) {
            jMessageObjects[x]= MethodBaseInJSON(behaviourMessages[x]);
        }
        return new JArray(jMessageObjects);       
    }
	// ----------------------------------------------------------------------
    public static JObject MethodBaseInJSON(iCS_MethodBaseInfo method) {
        var jParamObjects= ParametersInJSON(method.Parameters);
        var jParams= new JNameValuePair("Parameters", jParamObjects);
        return new JObject(new JNameValuePair("Name", method.DisplayName), jParams);        
    }
	// ----------------------------------------------------------------------
    public static JArray ParametersInJSON(iCS_Parameter[] parameters) {
        var pLen= parameters.Length;
        var jParamObjects= new JObject[pLen];
        for(int i= 0; i < pLen; ++i) {
            var p= parameters[i];
            var pName= new JNameValuePair("Name", p.name);
            var pType= new JNameValuePair("Type", p.type.Name);
            jParamObjects[i]= new JObject(pName, pType);                
        }        
        return new JArray(jParamObjects);
    }

    // ======================================================================
    // Utilities
    // ----------------------------------------------------------------------
    public static string TypeName(Type type) {
        if(type == null) return "void";
        string name= null;
        if(type.HasElementType) {
            name= TypeName(type.GetElementType());
            if(type.IsArray)   name= name+"[]";
            if(type.IsByRef)   name= "out "+name;
            if(type.IsPointer) name= "*"+name;
            return name;
        }
        if(type == typeof(float)) return "float";
        if(type == typeof(bool))  return "bool";
        if(type == typeof(void))  return "void";
        if(type == typeof(int))   return "int";
        return type.Name;
    }
    
}
