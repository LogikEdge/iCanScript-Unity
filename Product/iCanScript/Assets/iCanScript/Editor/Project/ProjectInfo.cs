using UnityEngine;
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
		public string   myProjectName        = null;
        public string   myRootFolder         = null;
        public bool     myCreateProjectFolder= true;
		public string   myNamespace          = null;
		public string   myEditorNamespace    = null;
		
		// ========================================================================
		// Properties
		// ------------------------------------------------------------------------
		public string ProjectName {
			get { return myProjectName; }
			set { UpdateProjectName(value); }
		}
        public string RootFolder {
            get { return myRootFolder; }
            set {
                var baseFolder= Application.dataPath;
                if(value.StartsWith(baseFolder)) {
                    myRootFolder= value.Substring(baseFolder.Length+1);
                }
            }
        }
        public bool CreateProjectFolder {
            get { return myCreateProjectFolder; }
            set { myCreateProjectFolder= value; }
        }
		public string Namespace {
			get { return GetNamespace(); }
		}
		public string EditorNamespace {
			get { return GetEditorNamespace(); }
		}
		
		// ========================================================================
		// Creation/Destruction
		// ------------------------------------------------------------------------
		public ProjectInfo(string projectName= null) {
			if(projectName == null) projectName= "MyProject";
			UpdateProjectName(projectName);
		}

		// ========================================================================
		/// Extracts the relative project folder path.
		public string GetRelativeProjectFolder() {
            var projectFolder= myRootFolder;
            if(myCreateProjectFolder) {
                projectFolder+= "/"+myProjectName;
            }
            return projectFolder;
		}
		
		// ========================================================================
		/// Extracts the absolute project folder path.
		public string GetProjectFolder() {
            return Application.dataPath+"/"+myRootFolder;
		}
		
		// ========================================================================
		/// Extracts the project file name from the project name.
		public string GetFileName() {
			return myProjectName+".icsproject";
		}
		
		// ========================================================================
		/// Extracts the engine namespace from the project name.
		public string GetNamespace() {
			var splitName= SplitProjectName(myRootFolder);
			var baseNamespace= iCS_TextUtility.CombineWith(splitName, ".");
            return baseNamespace+"."+myProjectName;
		}
		
		// ========================================================================
		/// Extracts the editor namespace from the project name.
		public string GetEditorNamespace() {
			return GetNamespace()+".Editor";
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
			if(myProjectName == projectName) return;
			myProjectName= projectName;
			var splitName= SplitProjectName(projectName);
			UpdateNamespace(iCS_TextUtility.CombineWith(splitName, "."));
		}
		
		// ========================================================================
		/// Update base namespace.
		///
		/// @param projectName The new project name.
		///
		void UpdateNamespace(string newNamespace) {
			myNamespace= newNamespace;
			myEditorNamespace= myNamespace+".Editor";			
		}
		
		// ========================================================================
		/// Resets the namespaces to their default values.
		public void ResetNamespaces() {
			var splitName= SplitProjectName(myProjectName);
			UpdateNamespace(iCS_TextUtility.CombineWith(splitName, "."));			
		}
		
		// ========================================================================
		/// Save and Update the project information.
		public void Save() {
            // -- Create the project folders (if not existing). --
			var projectPath= GetProjectFolder();
            FileUtils.CreateAssetFolder(projectPath);
            FileUtils.CreateAssetFolder(projectPath+"/Visual Scripts");
            FileUtils.CreateAssetFolder(projectPath+"/Generated Code");
            FileUtils.CreateAssetFolder(projectPath+"/Editor/Visual Scripts");
            FileUtils.CreateAssetFolder(projectPath+"/Editor/Generated Code");
            // -- Update version information. --
            myVersion= Version.Current.ToString();
            // -- Save the project information. --
            var fileName= GetFileName();
            var filePath= Folder.AssetToAbsolutePath(projectPath+"/"+fileName);
            JSONFile.PrettyWrite(filePath, this);
		}
		
		// ========================================================================
		/// Creates a new Project info from the given JSON root object.
        ///
        /// @param jsonRoot The JSON root object from which to extract the project
        ///                 information.
        ///
        public static ProjectInfo Create(JObject jsonRoot) {
            var newProject= new ProjectInfo();
            JString version        = jsonRoot.GetValueFor("myVersion") as JString;
            JString projectName    = jsonRoot.GetValueFor("myProjectName") as JString;
            JString nmspace        = jsonRoot.GetValueFor("myNamespace") as JString;
            JString editorNamespace= jsonRoot.GetValueFor("myEditorNamespace") as JString;
            newProject.myVersion        = version.value;
            newProject.myProjectName    = projectName.value;
            newProject.myNamespace      = nmspace.value;
            newProject.myEditorNamespace= editorNamespace.value;
            return newProject;
        }
    }
}