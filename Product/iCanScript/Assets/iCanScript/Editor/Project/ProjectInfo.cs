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
		public string ProjectVersion {
			get { return myVersion; }
			set { myVersion= value; }
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
		/// Creates and return the relative project folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The project folder path relative to the Assets folder.
        ///
        public string GetRelativeProjectFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
            string projectFolder;
            if(string.IsNullOrEmpty(myParentFolder)) {
                projectFolder= myCreateProjectFolder ? myProjectName : "";
            }
            else {
                projectFolder= myParentFolder;
                if(myCreateProjectFolder) {
                    projectFolder+= "/"+myProjectName;
                }                
            }
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(projectFolder);                
            }
            return projectFolder;
		}
		
		// ========================================================================
		/// Creates and returns the absolute project folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript project folder.
        ///
		public string GetProjectFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeProjectFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative engine visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The engine visual script folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEngineVisualScriptFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativeProjectFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var visualScriptPath= projectPath+separator+"Visual Scripts";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(visualScriptPath);                
            }
            return visualScriptPath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute engine visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript engine visual script
        ///         folder.
        ///
		public string GetEngineVisualScriptFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEngineVisualScriptFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative engine generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The engine generated code folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEngineGeneratedCodeFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativeProjectFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var generatedCodePath= projectPath+separator+"Generated Code";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(generatedCodePath);                
            }
            return generatedCodePath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute engine generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript engine generated code
        ///         folder.
        ///
		public string GetEngineGeneratedCodeFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEngineGeneratedCodeFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative editor visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The editor visual script folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEditorVisualScriptFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativeProjectFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var visualScriptPath= projectPath+separator+"Editor/Visual Scripts";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(visualScriptPath);                
            }
            return visualScriptPath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute editor visual script folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript editor visual script
        ///         folder.
        ///
		public string GetEditorVisualScriptFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEditorVisualScriptFolder(doCreate);
		}
		
		// ========================================================================
		/// Creates and return the relative editor generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return The editor generated code folder path relative to the _Assets_
        ///         folder.
        ///
        public string GetRelativeEditorGeneratedCodeFolder(bool doCreate= false) {
            // -- Determine project folder from the configuration. --
			var projectPath= GetRelativeProjectFolder(doCreate);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
            var generatedCodePath= projectPath+separator+"Editor/Generated Code";
            // -- Create project folder if it does not exists. --
            if(doCreate) {
                FileUtils.CreateAssetFolder(generatedCodePath);                
            }
            return generatedCodePath;
		}
		
		// ========================================================================
		/// Creates and returns the absolute editor generated code folder path.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        /// @return Returns the full path to the iCanScript editor generated code
        ///         folder.
        ///
		public string GetEditorGeneratedCodeFolder(bool doCreate= false) {
            return Application.dataPath+"/"+GetRelativeEditorGeneratedCodeFolder(doCreate);
		}
		
		// ========================================================================
		/// Computes the relative path of the project file.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        ///
		public string GetRelativeFileNamePath(bool doCreate= false) {
            var relativePath= GetRelativeProjectFolder(doCreate);
            if(!string.IsNullOrEmpty(relativePath)) {
                relativePath+= "/";
            }
			return relativePath + GetFileName();
		}
		
		// ========================================================================
        /// Computes the absolute path of the project file.
        ///
        /// @param doCreate Set to _true_ to create the folder.  Default is _false_.
        ///
        public string GetAbsoluteFileNamePath(bool doCreate= false) {
			var projectPath= GetRelativeProjectFolder(doCreate);
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
			var projectPath= GetRelativeProjectFolder(true);
            var separator= string.IsNullOrEmpty(projectPath) ? "" : "/";
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