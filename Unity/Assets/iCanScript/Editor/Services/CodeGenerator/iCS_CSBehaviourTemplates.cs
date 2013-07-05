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

public class iCS_CSBehaviourTemplates {
	// ----------------------------------------------------------------------
	public static string GetBehaviourClassName(iCS_VisualScript visualScript) {
		var className= visualScript.BehaviourClassName;
		if(!string.IsNullOrEmpty(className)) {
			return className;
		}
		var path= "Assets/"+iCS_PreferencesEditor.CodeGenerationFolder+"/"+iCS_PreferencesEditor.BehaviourGenerationSubfolder;
		var fileName= iCS_PreferencesEditor.CodeGenerationFilePrefix+visualScript.gameObject.name+"Behaviour.cs";
		var filePathAndName= path+"/"+fileName;
		var uniqueFilePathAndName= AssetDatabase.GenerateUniqueAssetPath(filePathAndName);
		className= Path.GetFileNameWithoutExtension(uniqueFilePathAndName);
		return className;
	}
	// ----------------------------------------------------------------------
    public static void GenerateBehaviourCode(iCS_EditorObject behaviour) {
        var storage= behaviour.Storage;
        var go= storage.gameObject;
        var objectId= go.GetInstanceID().ToString();
        
        var behaviourMessages= iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
        var jMessageObjects= new JObject[behaviourMessages.Length];
        for(var x= 0; x < behaviourMessages.Length; ++x) {
            var msg= behaviourMessages[x];
            var parameters= msg.Parameters;
            var plen= parameters.Length;
            var jps= new JObject[plen];
            for(int i= 0; i < plen; ++i) {
                var p= parameters[i];
                var pname= new JNameValuePair("Name", p.name);
                var ptype= new JNameValuePair("Type", p.type.Name);
                jps[i]= new JObject(new JNameValuePair[2]{pname, ptype});                
            }
            var jParams= new JNameValuePair("Parameters", jps);
            jMessageObjects[x]= new JObject(new JNameValuePair("name", msg.DisplayName), jParams);
        }
        var jMessages= new JNameValuePair("Messages", jMessageObjects);
        var jVersion= new JNameValuePair("Version", iCS_EditorConfig.VersionId);
        var jProductType= new JNameValuePair("ProductType", "Standard");
        var manifestInJSON= new JObject(jProductType, jVersion, jMessages);
        var s= manifestInJSON.Encode();
        var pretty= JSONPrettyPrint.Beautify(s);
        iCS_TextFileUtility.WriteFile(iCS_CodeGeneratorUtility.ToGeneratedCodePath("JSON_Test.txt"), pretty);
        Debug.Log(s);
        Debug.Log("MD5: "+iCS_TextUtility.CalculateMD5Hash(s));
        return;
        
        var messages= new List<iCS_MessageInfo>();
        behaviour.ForEachChildNode(
            n => {
                if(n.IsMessage) {
                    for(int i= 0; i < behaviourMessages.Length; ++i) {
                        if(behaviourMessages[i].DisplayName == n.Name) {
                            messages.Add(behaviourMessages[i]);
                        }
                    }
                }
            }
        );
        
        // Generate behaviour source code.
		var fred= GetBehaviourClassName(storage as iCS_VisualScript);
		Debug.Log("Proposed class name: "+fred);
		
		
        var behaviourClassName= iCS_TextUtility.ToClassName(go.name+"Behaviour_"+objectId);
        var code= BehaviourMessageProxy(behaviourClassName, messages.ToArray());
        var fileName= ClassNameToFileName(behaviourClassName);
        var filePath= iCS_PreferencesEditor.BehaviourGenerationSubfolder;
        var filePathAndName= iCS_CodeGeneratorUtility.ToGeneratedCodePath(Path.Combine(filePath, fileName));
        iCS_TextFileUtility.WriteFile(filePathAndName, code);

        // Remove previous file if fileName has changed.
        if(storage.BehaviourClassName != behaviourClassName) {
            RemoveBehaviourCode(behaviour);
            storage.BehaviourClassName= behaviourClassName; 
            EditorUtility.SetDirty(storage);
        }
        
        // Install on game object if not already present.
        var gameObject= behaviour.Storage.gameObject;
        var proxy= gameObject.GetComponent(behaviourClassName);
        if(proxy == null) {
            gameObject.AddComponent(behaviourClassName);
        }
    }
	// ----------------------------------------------------------------------
    public static void RemoveBehaviourCode(iCS_EditorObject behaviour) {
        var storage= behaviour.Storage;
        var behaviourClassName= storage.BehaviourClassName;
        if(string.IsNullOrEmpty(behaviourClassName)) return;
        var path= "Assets/"+iCS_PreferencesEditor.CodeGenerationFolder+"/"+iCS_PreferencesEditor.BehaviourGenerationSubfolder+"/";
        AssetDatabase.DeleteAsset(path+ClassNameToFileName(behaviourClassName));
    }
	// ----------------------------------------------------------------------
    public static string ClassNameToFileName(string className) {
        return className+".cs";
    }
    
    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    public static string BehaviourMessageProxy(string className, iCS_MessageInfo[] messages) {
        var fileHeader= iCS_CSFileTemplates.FileHeader(className+".cs", className);
        var imports= "\nusing UnityEngine;\n";
        var classHeader= "\npublic sealed class "+className+" : MonoBehaviour {\n"+
                         "\tiCS_VisualScript   myVisualScript= null;\n\n"+
                         "\tvoid Start()\n"+
                         "\t{\n"+
                         "\t\tmyVisualScript= GetComponent(typeof(iCS_VisualScript)) as iCS_VisualScript;\n"+
                         "\t}\n";
        var classTrailer= "\n}\n";
        var messageImpls= "";
        foreach(var msg in messages) {
            messageImpls+= MessageReceiverImp(msg)+"\n";
        }
        return fileHeader+imports/*+inspector*/+classHeader+messageImpls+classTrailer;
    }

    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    public static string MessageReceiverImp(iCS_MessageInfo message) {
        // Start message is already handle...
        if("Start" == message.DisplayName) return "";
        return "\n\t"+MessageSignature(message)+MessageBody(message);
    }
    public static string MessageSignature(iCS_MessageInfo message) {
        return MethodSignature(message);
    }
    public static string MessageBody(iCS_MessageInfo message) {
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
               "\t\tif(myVisualScript != null) {\n"+
               "\t\t\tmyVisualScript.RunMessage("+paramStr+");\n"+
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

    // ======================================================================
    // Member code generation
	// ----------------------------------------------------------------------
    public static string Name(iCS_MemberInfo member) {
        return member.DisplayName;
    }
    public static string Description(iCS_MemberInfo member) {
        return member.Description;
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
