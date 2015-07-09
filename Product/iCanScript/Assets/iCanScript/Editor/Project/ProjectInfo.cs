using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using iCanScript.Internal.JSON;

namespace iCanScript.Internal.Editor {
	using CodeParsing;
	
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class contains all the project related information.
    ///
    /// The content is saved inside the project file.
    ///
    public class ProjectInfo {
		// ========================================================================
		// Fields
		// ------------------------------------------------------------------------
        public string   myVersion            = null;
		public string   myProjectName        = "";
        public string   myParentFolder       = "";
        public bool     myCreateProjectFolder= true;
		
		// ========================================================================
		// Properties
		// ------------------------------------------------------------------------
		public string ProjectName {
			get { return myProjectName ?? ""; }
			set { UpdateProjectName(value); }
		}
        public string RootFolder {
            get { return myParentFolder ?? ""; }
            set {
                var baseFolder= Application.dataPath;
                if(value.StartsWith(baseFolder)) {
                    if(baseFolder == value) {
                        myParentFolder= "";
                    }
                    else {
                        myParentFolder= value.Substring(baseFolder.Length+1);                        
                    }
                }
            }
        }
        public bool CreateProjectFolder {
            get { return myCreateProjectFolder; }
            set { myCreateProjectFolder= value; }
        }
		public string EngineNamespace {
			get { return GetEngineNamespace(); }
		}
		public string EditorNamespace {
			get { return GetEditorNamespace(); }
		}
		// ========================================================================
        /// Determines if the project already exists.
        public bool AlreadyExists {
            get {
                var filePath= GetAbsoluteFileNamePath();
                return File.Exists(filePath);                
            }
        }
        
		// ========================================================================
		// Creation/Destruction
		// ------------------------------------------------------------------------
		public ProjectInfo(string projectName= null) {
			if(projectName == null) {
                projectName= UnityUtility.GetProjectName();
    			UpdateProjectName(projectName);
                CreateProjectFolder= false;
                return;
            }
			UpdateProjectName(projectName);
		}

		// ========================================================================
		/// Computes the project file name.
        ///
        /// @return The iCanScript file name including its extension.
        ///
		public string GetFileName() {
			return myProjectName+".icsproject";
		}
		
		// ========================================================================
		/// Extracts the relative project folder path.
		public string GetRelativeProjectFolder() {
            if(string.IsNullOrEmpty(myParentFolder)) {
                return myCreateProjectFolder ? myProjectName : "";
            }
            var projectFolder= myParentFolder;
            if(myCreateProjectFolder) {
                projectFolder+= "/"+myProjectName;
            }
            return projectFolder;
		}
		
		// ========================================================================
		/// Computes the absolute project folder path.
		public string GetProjectFolder() {
            return Application.dataPath+"/"+GetRelativeProjectFolder();
		}
		
		// ========================================================================
		/// Computes the relative path of the project file.
		public string GetRelativeFileNamePath() {
            var relativePath= GetRelativeProjectFolder();
            if(!string.IsNullOrEmpty(relativePath)) {
                relativePath+= "/";
            }
			return relativePath + GetFileName();
		}
		
		// ========================================================================
        /// Computes the absolute path of the project file.
        public string GetAbsoluteFileNamePath() {
			var projectPath= GetRelativeProjectFolder();
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var fileName= GetFileName();
            return Folder.AssetToAbsolutePath(projectPath+separator+fileName);            
        }
        
		// ========================================================================
		/// Extracts the engine namespace from the project name.
		public string GetEngineNamespace() {
            // -- Translate '-' to '_' for the namespace. --
            var formattedProjectName= NameUtility.ToTypeName(myProjectName.Replace('-','_'));
            if(string.IsNullOrEmpty(myParentFolder)) return formattedProjectName;
			var splitName= SplitProjectName(myParentFolder);
			var baseNamespace= iCS_TextUtility.CombineWith(splitName, ".");
            return baseNamespace+"."+formattedProjectName;
		}
		
		// ========================================================================
		/// Extracts the editor namespace from the project name.
		public string GetEditorNamespace() {
			return GetEngineNamespace()+".Editor";
		}
		
		// ========================================================================
		/// Returns the absolute path of the generated engine code.
		///
		/// @return The absolute path of the generated engine code.
		///
		public string GetEngineCodeGenerationFolder() {
			var relativePath= GetRelativeEngineCodeGenerationFolder();
			return Folder.AssetToAbsolutePath(relativePath);
		}
		
		// ========================================================================
		/// Returns the relative path of the generated engine code.
		///
		/// @return The relative path of the generated engine code.
		///
		public string GetRelativeEngineCodeGenerationFolder() {
			var projectPath= GetRelativeProjectFolder();
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            FileUtils.CreateAssetFolder(projectPath);
            return projectPath+separator+"Generated Code";
		}
		
		// ========================================================================
		/// Returns the absolute path of the generated editor code.
		///
		/// @return The absolute path of the generated editor code.
		///
		public string GetEditorCodeGenerationFolder() {
			var relativePath= GetRelativeEditorCodeGenerationFolder();
			return Folder.AssetToAbsolutePath(relativePath);
		}
		
		// ========================================================================
		/// Returns the relative path of the generated editor code.
		///
		/// @return The relative path of the generated editor code.
		///
		public string GetRelativeEditorCodeGenerationFolder() {
			var projectPath= GetRelativeProjectFolder();
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            FileUtils.CreateAssetFolder(projectPath);
            return projectPath+separator+"Editor/Generated Code";
		}
		
		// ========================================================================
		/// Parse project name.
		///
		/// @param projectName The full name of the project.
		/// @return An array of the project name constituents.
		///
		static string[] SplitProjectName(string projectName) {
			// -- Convert file path to namespace format. --
			projectName= projectName.Replace('/', '.'); 
			// -- Remove all illegal characters. --
			var cleanName= new StringBuilder(64);
			var len= projectName.Length;
			for(int i= 0; i < len; ++i) {
				char c= projectName[i];
				if(cleanName.Length == 0) {
					if(ParsingUtility.IsFirstIdent(c)) {
						cleanName.Append(c);
					}
				}
				else if(Char.IsWhiteSpace(c) || c == '.' || ParsingUtility.IsIdent(c)) {
					cleanName.Append(c);
				}
				else {
					cleanName.Append(' ');
				}
			}
			// -- Split the name into its parts. --
			var splitName= cleanName.ToString().Split(new Char[]{'.'});
			// -- Convert each part to a type format. --
			for(int i= 0; i < splitName.Length; ++i) {
				splitName[i]= NameUtility.ToTypeName(splitName[i]);
			}
			return splitName;
		}
		
		// ========================================================================
		/// Update project name.
		///
		/// @param projectName The new project name.
		///
		void UpdateProjectName(string projectName) {
			myProjectName= projectName;
            // -- Force first character ident rules for project name. --
            while(myProjectName.Length > 0 && !iCS_TextUtil.IsIdent1(myProjectName[0])) {
                myProjectName= myProjectName.Substring(1);
            }
            // -- Accept ident rule or '-' for remaining of the project name. --
            for(int i= 1; i < myProjectName.Length; ++i) {
                var c= myProjectName[i];
                if(!iCS_TextUtil.IsIdent(c) && c != '-' && c != ' ') {
                    UpdateProjectName(myProjectName.Substring(0,i)+myProjectName.Substring(i+1));
                    return;
                }
            }
		}
		
		// ========================================================================
		/// Save and Update the project information.
		public void Save() {
            // -- Create the project folders (if not existing). --
			var projectPath= GetRelativeProjectFolder();
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            FileUtils.CreateAssetFolder(projectPath);
            FileUtils.CreateAssetFolder(projectPath+separator+"Visual Scripts");
            FileUtils.CreateAssetFolder(projectPath+separator+"Generated Code");
            FileUtils.CreateAssetFolder(projectPath+separator+"Editor/Visual Scripts");
            FileUtils.CreateAssetFolder(projectPath+separator+"Editor/Generated Code");
            // -- Update version information. --
            myVersion= Version.Current.ToString();
            // -- Save the project information. --
            var fileName= GetFileName();
            var filePath= Folder.AssetToAbsolutePath(projectPath+separator+fileName);
            JSONFile.PrettyWrite(filePath, this);
		}
		
		// ========================================================================
		/// Removes all files associated with the iCanScript project.
        public void RemoveProject() {
            // -- Create the project folders (if not existing). --
			var projectPath= GetRelativeProjectFolder();
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
			AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+"Visual Scripts");
            AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+"Generated Code");
           	AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+"Editor");
            var fileName= GetFileName();
            AssetDatabase.DeleteAsset("Assets/"+projectPath+separator+fileName);
            if(Folder.IsEmpty(GetProjectFolder())) {
				if(!string.IsNullOrEmpty(projectPath)) {
	                AssetDatabase.DeleteAsset("Assets/"+projectPath);
				}
            }
        }
        
		// ========================================================================
		/// Removes all files associated with the iCanScript project.
        ///
        /// @param absolutePath The absolute path of the project file.
        ///
        public static void RemoveProject(string absolutePath) {
			var project= ProjectInfo.Load(absolutePath);
			project.RemoveProject();
        }
        
		// ========================================================================
		/// Creates a new Project info from the given JSON root object.
        ///
        /// @param jsonRoot The JSON root object from which to extract the project
        ///                 information.
        ///
        public static ProjectInfo Load(JObject jsonRoot) {
            var newProject= new ProjectInfo();
            JString version            = jsonRoot.GetValueFor("myVersion") as JString;
            JString projectName        = jsonRoot.GetValueFor("myProjectName") as JString;
            JString parentFolder       = jsonRoot.GetValueFor("myParentFolder") as JString;
            JBool   createProjectFolder= jsonRoot.GetValueFor("myCreateProjectFolder") as JBool;
            newProject.myVersion            = version.value;
            newProject.myProjectName        = projectName.value;
            newProject.myParentFolder       = parentFolder.value;
            newProject.myCreateProjectFolder= createProjectFolder.value;
            return newProject;
        }

        // =================================================================================
        /// Load the project file from disk.
        ///
        /// @param absolutePath The absolute path of the project file.
        /// @info The active project set to the newly loaded project.
        /// 
        public static ProjectInfo Load(string absolutePath, bool declareError= false) {
            var jsonRoot= JSONFile.Read(absolutePath);
            if(jsonRoot == null || jsonRoot.isNull) {
                if(declareError) {
                    Debug.LogError("iCanScript: Unable to load project at=> "+absolutePath);                    
                }
                return null;
            }
            return Load(jsonRoot);
        }
    }
}