using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public class ProjectSelectionWindow : EditorWindow {
        // =================================================================================
        // Types
        // ---------------------------------------------------------------------------------
		enum RowSelection { None, Project, Settings, Remove };
		
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
		const  float	   kSpacer            = 16f;
        const  float       kWidth             = 1000f;
        const  float       kHeaderHeight      = 100f;
        const  float       kListAreaHeight    = 450f;
		const  int  	   kHeaderFontSize    = 30;
		const  int  	   kTitleFontSize     = 18;
		const  int  	   kFolderFontSize    = 14;
		const  int         kButtonFontSize    = kTitleFontSize;
		const  float	   kButtonHeight      = kTitleFontSize+kFolderFontSize;
		const  float	   kTileToFolderSpacer= 0.25f*kSpacer;
		const  float       kRowHeight         = 2f*kSpacer+kTitleFontSize+kFolderFontSize+kTileToFolderSpacer;
		static Color	   ourHeaderBackgroundColor  = Color.white;
		static Color	   ourListAreaBackgroundColor= new Color(0.9f, 0.9f, 0.9f);
		static Color	   ourSelectedColor  		 = new Color(0.25f, 0.5f, 1f);
        static Vector3[]   ourHeaderShape  = null;
        static Vector3[]   ourListAreaShape= null;
        static Rect        ourHeaderRect   = new Rect(0, 0, kWidth, kHeaderHeight);
        static Rect        ourListAreaRect = new Rect(0, kHeaderHeight, kWidth, kListAreaHeight);
        static Texture2D   ourLogo         = null;
		static Rect		   ourLogoPosition;
		static GUIStyle	   ourHeaderTextStyle= null;
		static GUIContent  ourProjectsText;
		static Rect		   ourProjectsTextRect;
		static GUIStyle	   ourProjectTitleStyle = null;
		static GUIStyle	   ourProjectFolderStyle= null;
		static GUIStyle	   ourButtonStyle       = null;
        static Vector2     ourScrollPosition    = Vector2.zero;
		
		int selectedProjectId= 0;
		
        // =================================================================================
        /// Creates a project selection window.
        public static ProjectSelectionWindow Init() {
            // -- Create window. --
            var editor= EditorWindow.CreateInstance<ProjectSelectionWindow>();
			editor.wantsMouseMove= true;
			
            // -- Set window title --
            Texture2D iCanScriptLogo= null;
            TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
            editor.titleContent= new GUIContent("iCanScript Project Selection", iCanScriptLogo);
    
            // -- Fix window size. --
            editor.minSize= new Vector2(kWidth, kHeaderHeight+kListAreaHeight);
            editor.maxSize= new Vector2(kWidth, kHeaderHeight+kListAreaHeight);

            // -- Show the window. --
            editor.ShowUtility();
            return editor;
        }

        // =================================================================================
		/// Initialize all class variables.
		static void Initialize() {
            // -- Build the window area shapes. --
			ourHeaderShape  = Shapes.Rectangle2D(0, 0, kWidth, kHeaderHeight);
			ourListAreaShape= Shapes.Rectangle2D(0, kHeaderHeight, kWidth, kListAreaHeight);
                            
			// -- Load adornments. --
            TextureCache.GetTexture(iCS_EditorStrings.LogoIcon, out ourLogo);
			ourLogoPosition= new Rect(kWidth-80f, 16f, 64f, 64f);
			
			// -- Setup GUI styles. --
			ourHeaderTextStyle= new GUIStyle(EditorStyles.largeLabel);
			ourHeaderTextStyle.fontSize= kHeaderFontSize;
			ourHeaderTextStyle.fontStyle= FontStyle.Normal;
			ourProjectsText= new GUIContent("Projects");
			var projectsTextSize= ourHeaderTextStyle.CalcSize(ourProjectsText);
			ourProjectsTextRect= new Rect(kSpacer,kHeaderHeight-kSpacer-projectsTextSize.y, projectsTextSize.x, projectsTextSize.y);

			ourProjectTitleStyle= new GUIStyle(EditorStyles.largeLabel);
			ourProjectTitleStyle.fontSize= kTitleFontSize;
			ourProjectFolderStyle= new GUIStyle(EditorStyles.largeLabel);
			ourProjectFolderStyle.fontSize= kFolderFontSize;			
		}
		
        // =================================================================================
        /// Draw window
        public void OnGUI() {
			// -- Assure the GUI is properly initialized. --
			Initialize();
			
			// -- Set background colors. --
			Handles.DrawSolidRectangleWithOutline(ourHeaderShape, ourHeaderBackgroundColor, ourHeaderBackgroundColor);
			Handles.DrawSolidRectangleWithOutline(ourListAreaShape, ourListAreaBackgroundColor, ourListAreaBackgroundColor);

			// -- Draw fix adornments. --
			GUI.DrawTexture(ourLogoPosition, ourLogo);

			// -- Header. --
			GUI.Label(ourProjectsTextRect, ourProjectsText, ourHeaderTextStyle);

			// -- Project list. --
			var projects= ProjectController.Projects;
            var viewRect= new Rect(0,0, ourListAreaRect.width-16f, kRowHeight*projects.Length);
            ourScrollPosition= GUI.BeginScrollView(ourListAreaRect, ourScrollPosition, viewRect);
			for(int i= 0; i < projects.Length; ++i) {
				var p= projects[i];
				var name= p.ProjectName;
				var folder= p.GetRelativeProjectFolder();
                var separator= string.IsNullOrEmpty(folder) ? "" : "/";
                folder= "Assets"+separator+folder;
				var version= p.ProjectVersion;
				switch(DisplayRow(i, name, folder, version, i == selectedProjectId)) {
					case RowSelection.Project: {
						selectedProjectId= i;
						break;
					}
					case RowSelection.Remove: {
						selectedProjectId= 0;
						p.RemoveProject();
						ProjectController.UpdateProjectDatabase();
						break;
					}
					case RowSelection.Settings: {
						var editor= ProjectSettingsEditor.Init();
						editor.ChangeSelection("Update");
						break;
					}
				}
			}
            GUI.EndScrollView();

			Event.current.Use();
        }
		
		RowSelection DisplayRow(int rowId, string title, string folder, string version, bool isSelected= false) {
			// -- Determine if mouse is hovering. --
			float y= rowId*kRowHeight;
			var mousePosition= Event.current.mousePosition+Event.current.delta;
			bool isMouseOver= false;
			if(mouseOverWindow) {
				if(mousePosition.y >= y && mousePosition.y < y+kRowHeight) {
					isMouseOver= true;
				}
			}
			
			// -- Assume project is selected if mouse button is pressed. --
			var rowSelection= RowSelection.None;
			var e= Event.current;
			if(isMouseOver && e.type == EventType.MouseDown && e.button == 0) {
				rowSelection= RowSelection.Project;
			}
			
			// -- Add separator line between project rows. --
			if(rowId != 0) {
				Handles.color= Color.grey;
				Handles.DrawLine(new Vector3(kSpacer, y), new Vector3(kWidth-kSpacer, y));
			}
			ourProjectTitleStyle.normal.textColor= isSelected ? ourSelectedColor : Color.black;

			// -- Show project title and version. --
			y+= kSpacer;
			var titleContent= new GUIContent(title);
			var titleSize= ourProjectTitleStyle.CalcSize(titleContent);
			var titleRect= new Rect(kSpacer, y, kWidth, kRowHeight);
			GUI.Label(titleRect, titleContent, ourProjectTitleStyle);
			if(isMouseOver) {
				Handles.color= ourProjectTitleStyle.normal.textColor;
				var lineY= titleRect.y+0.85f*titleSize.y;
				Handles.DrawLine(new Vector3(kSpacer, lineY), new Vector3(kSpacer+titleSize.x, lineY));
			}
			var versionContent= new GUIContent(version);
			var versionSize= ourProjectTitleStyle.CalcSize(versionContent);
			var versionRect= new Rect(kWidth-kSpacer-versionSize.x, y, versionSize.x, versionSize.y);
			GUI.Label(versionRect, versionContent, ourProjectTitleStyle);

			// -- Show option buttons. --
			if(ourButtonStyle == null) {
				ourButtonStyle= new GUIStyle(GUI.skin.button);
				ourButtonStyle.fontSize= ourProjectTitleStyle.fontSize;				
			}			
			var settingRect= new Rect(kWidth-kSpacer-300f, y, 100f-0.5f*kSpacer, 34f);
			if(GUI.Button(settingRect, "Settings", ourButtonStyle)) {
				rowSelection= RowSelection.Settings;
			}
			var removeRect= new Rect(kWidth-kSpacer-200f, y, 100f-0.5f*kSpacer, 34f);
			if(GUI.Button(removeRect, "Remove", ourButtonStyle)) {
				rowSelection= RowSelection.Remove;
			}

			// -- Show project folder. --
			titleRect.y+= ourProjectTitleStyle.fontSize+0.25f*kSpacer;
			GUI.Label(titleRect, folder, ourProjectFolderStyle);			

			return rowSelection;
		}
    }

}
