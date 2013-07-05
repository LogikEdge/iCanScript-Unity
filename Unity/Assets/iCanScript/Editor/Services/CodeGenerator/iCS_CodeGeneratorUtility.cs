using UnityEngine;
using System.IO;
using System.Collections;

public static class iCS_CodeGeneratorUtility {
    // ----------------------------------------------------------------------
    public static string ToGeneratedCodePath(string fileNameAndPath) {
        var fileName= Path.GetFileName(fileNameAndPath);
        var filePrefix= iCS_PreferencesEditor.CodeGenerationFilePrefix;
        if(!string.IsNullOrEmpty(filePrefix) && !fileName.StartsWith(filePrefix)) {
            fileName= filePrefix+fileName;
        }
        fileNameAndPath= Path.Combine(Path.GetDirectoryName(fileNameAndPath), fileName);
        return Path.Combine(iCS_PreferencesEditor.CodeGenerationFolder, fileNameAndPath);
    }
}
