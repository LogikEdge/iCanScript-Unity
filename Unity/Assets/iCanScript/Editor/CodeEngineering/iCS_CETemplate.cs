using UnityEngine;
using System;
using System.Collections;

public static class iCS_CETemplate {
    // ----------------------------------------------------------------------
    public static string FileHeader(string fileName, string className, string author= null, string elemType= null) {
        var bar= "/////////////////////////////////////////////////////////////////\n";
        var h1=  "//  "+fileName+"\n"+
                 "//\n";
        var h2=  elemType == null ?
                 "//  Implementation of "+className+"\n" :
                 "//  Implementation of the "+elemType+" for "+className+"\n";
        var h3=  "//  Generated by iCanScript v"+iCS_EditorConfig.VersionId+"\n"+
                 "//  Created on: "+DateTime.Now+"\n";
        var h4=  author == null ?
                 "" :
                 "//  Original author: "+author+"\n";
        return bar+h1+h2+h3+h4+bar;
    } 

}
