using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public static class iCS_CEGenerator {
	// ----------------------------------------------------------------------
    public static void GenerateBehaviour(iCS_EditorObject behaviour, GameObject go, string objectId, iCS_Storage storage) {
        var behaviourMessages= iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour));
        var messages= new List<iCS_MessageInfo>();
        behaviour.ForEachChildNode(
            n => {
                if(n.IsMessage) {
                    for(int i= 0; i < behaviourMessages.Length; ++i) {
                        if(behaviourMessages[i].displayName == n.Name) {
                            messages.Add(behaviourMessages[i]);
                        }
                    }
                }
            }
        );
        
        var behaviourClassName= iCS_PreferencesEditor.CodeGenerationFilePrefix+go.name+"Behaviour_"+objectId;
        var code= BehaviourMessageProxy(behaviourClassName, messages.ToArray());
        var fileName= behaviourClassName+".cs";
        var filePath= iCS_PreferencesEditor.BehaviourGenerationSubfolder;
//        iCS_CETextFile.WriteFile(filePath+"/"+fileName, code);
        if(storage.FileName != fileName) {
            if(!string.IsNullOrEmpty(storage.FileName)) {
                AssetDatabase.DeleteAsset("Assets/"+iCS_PreferencesEditor.CodeGenerationFolder+"/"+filePath+"/"+storage.FileName);
            }
            storage.FileName= fileName;
            EditorUtility.SetDirty(storage);
        }
//		iCS_CETextFile.EditFile(fileName);            
//        var newPath= AssetDatabase.GenerateUniqueAssetPath("Assets/"+fileName);
//        Debug.Log("Please enable behaviour code generation... Path= "+newPath);
    }
    

    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    public static string BehaviourMessageProxy(string className, iCS_MessageInfo[] messages) {
        var fileHeader= iCS_CETemplate.FileHeader(className+".cs", className);
        var imports= "\nusing UnityEngine;\n";
        var inspector= "\n[CustomEditor (typeof ("+className+"))]\n"+
                       "public sealed class "+className+"Inspector : iCS_Inspector {}\n\n";
        var classHeader= "\npublic sealed class "+className+" : iCS_BehaviourImp {\n";
        var classTrailer= "\n}\n";
        var messageImpls= "";
        foreach(var msg in messages) {
            messageImpls+= MessageReceiverImp(msg)+"\n";
        }
//        return "#if FRED\n"+fileHeader+imports+classHeader+messageImpls+classTrailer+"\n#endif";
        return fileHeader+imports/*+inspector*/+classHeader+messageImpls+classTrailer;
    }

    // ======================================================================
    // Messsage Receiver code generation
	// ----------------------------------------------------------------------
    public static string MessageReceiverImp(iCS_MessageInfo message) {
        return "\n\t"+MessageSignature(message)+MessageBody(message);
    }
    public static string MessageSignature(iCS_MessageInfo message) {
        return MethodSignature(message);
    }
    public static string MessageBody(iCS_MessageInfo message) {
        var parameters= message.parameters;
        var len= parameters.Length;
        var msgParamStr= "";
        for(int i= 0; i < len; ++i) {
            msgParamStr+= parameters[i].name;
            if(i < len-1) {
                msgParamStr+= ", ";
            }
        }
        var paramStr= "\""+message.displayName+"\""+(msgParamStr.Length != 0 ? ", ":"")+msgParamStr;
        return "\n\t{\n\t\tRunMessage("+paramStr+");\n\t}";
    }

    // ======================================================================
    // Method/Function code generation
	// ----------------------------------------------------------------------
    public static string MethodSignature(iCS_MethodBaseInfo method) {
        var storageClass= "";
        if(method.isClassMember) storageClass= "static ";
        var returnType= "void";
        if(method.functionReturn != null) {
            returnType= TypeName(method.functionReturn.type);
        }
        var name= method.displayName;
        var parameters="";
        if(method.parameters != null) {
            foreach(var p in method.parameters) {
				parameters+= TypeName(p.type)+" "+p.name+", ";
            }            
        }
        if(parameters.Length != 0) parameters= parameters.Substring(0, parameters.Length-2);
        return storageClass+returnType+" "+name+"("+parameters+")";
    }
	// ----------------------------------------------------------------------
    public static string MethodStorageClass(iCS_MethodBaseInfo method) {
        return method.isClassMember ? "static" : "";
    }
	// ----------------------------------------------------------------------
    public static string[] MethodParameters(iCS_MethodBaseInfo method) {
        var methodParameters= method.parameters;
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
        return member.displayName;
    }
    public static string Description(iCS_MemberInfo member) {
        return member.description;
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
