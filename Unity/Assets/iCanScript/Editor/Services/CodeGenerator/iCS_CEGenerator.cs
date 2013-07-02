//#define USE_INSPECTOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_CEGenerator {
	// ----------------------------------------------------------------------
    public static void GenerateBehaviourCode(iCS_EditorObject behaviour) {
        var storage= behaviour.Storage;
        var go= storage.gameObject;
        var objectId= go.GetInstanceID().ToString();
        
        var behaviourMessages= iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
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
        var behaviourClassName= iCS_TextFile.ToClassName(iCS_PreferencesEditor.CodeGenerationFilePrefix+go.name+"BehaviourProxy_"+objectId);
        var code= BehaviourMessageProxy(behaviourClassName, messages.ToArray());
        var fileName= ClassNameToFileName(behaviourClassName);
        var filePath= iCS_PreferencesEditor.BehaviourGenerationSubfolder;
        iCS_CETextFile.WriteFile(filePath+"/"+fileName, code);

        // Remove previous file if fileName has changed.
        if(storage.FileName != fileName) {
            RemoveBehaviourCode(behaviour);
            storage.FileName= fileName; 
            EditorUtility.SetDirty(storage);
        }
        
        // Install on game object if not already present.
        var gameObject= behaviour.Storage.gameObject;
        var proxy= gameObject.GetComponent(behaviourClassName);
        if(proxy == null) {
//            while(EditorApplication.isCompiling) {}
            gameObject.AddComponent(behaviourClassName);
        }
    }
	// ----------------------------------------------------------------------
    public static void RemoveBehaviourCode(iCS_EditorObject behaviour) {
        var storage= behaviour.Storage;
        var fileName= storage.FileName;
        if(string.IsNullOrEmpty(fileName)) return;
        var path= "Assets/"+iCS_PreferencesEditor.CodeGenerationFolder+"/"+iCS_PreferencesEditor.BehaviourGenerationSubfolder+"/";
        AssetDatabase.DeleteAsset(path+fileName);
    }
	// ----------------------------------------------------------------------
    public static string ClassNameToFileName(string className) {
        return className+".cs";
    }
	// ----------------------------------------------------------------------
    public static string FileNameToClassName(string fileName) {
        return fileName.Substring(0, fileName.Length-3);
    }
    
    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    public static string BehaviourMessageProxy(string className, iCS_MessageInfo[] messages) {
        var fileHeader= iCS_CETemplate.FileHeader(className+".cs", className);
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
