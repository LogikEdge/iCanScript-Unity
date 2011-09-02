using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AP_Graphics {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public bool     IsInitialized= false;
    
    // ----------------------------------------------------------------------
	Texture2D   lineTexture;
	Texture2D   nodeIcon;
    Vector2     drawOffset= Vector2.zero;

    // ----------------------------------------------------------------------
    bool lineTextureErrorSeen= false;
    bool nodeIconErrorSeen   = false;
    
    // ======================================================================
	// CONSTANTS
    // ----------------------------------------------------------------------
    public static readonly Vector2 UpDirection   = new Vector2(0,-1);
    public static readonly Vector2 DownDirection = new Vector2(0,1);
    public static readonly Vector2 RightDirection= new Vector2(1,0);
    public static readonly Vector2 LeftDirection = new Vector2(-1,0);
    public static readonly Vector3 FacingNormal  = new Vector3(0,0,-1);
        

    // ======================================================================
    //  INITIALIZATION
	// ----------------------------------------------------------------------
    ~AP_Graphics() {
        lineTexture = null;
        nodeIcon    = null;
    }

	// ----------------------------------------------------------------------
    public bool Init() {
        // Create an AA line standard texture.
        string lineTexturePath= AP_EditorConfig.GuiAssetPath + "/AP_LineTexture.psd";
        lineTexture= AssetDatabase.LoadAssetAtPath(lineTexturePath, typeof(Texture2D)) as Texture2D;
        if(lineTexture == null) {
            ResourceMissingError(lineTexturePath, ref lineTextureErrorSeen);
            IsInitialized= false;
            return IsInitialized;
        }
        
        // A temporary node icon.
        // TODO: Support for node icons.
        string nodeBackgroundPath= AP_EditorConfig.GuiAssetPath + "/AP_NodeIcon.psd";
        nodeIcon= AssetDatabase.LoadAssetAtPath(nodeBackgroundPath, typeof(Texture2D)) as Texture2D;
        if(nodeIcon == null) {
            ResourceMissingError(nodeBackgroundPath, ref nodeIconErrorSeen);
            IsInitialized= false;
            return IsInitialized;
        }
        
        // Graphic resources properly initialized.
        IsInitialized= true;
        return IsInitialized;
    }


    // ======================================================================
    //  TOOL TIP
    // ----------------------------------------------------------------------
    public void ShowToolTip(AP_Object _object) {
        AP_Port port= _object as AP_Port;
        if(port != null) {
            Vector2 pos= port.Position;
            string name= port.Name + ":" + port.TypeName;
            Vector2 labelSize= AP_EditorConfig.GetLabelSize(name);
            GUI.Label(new Rect(pos.x, pos.y, labelSize.x, labelSize.y), name);            
        }
    }
    
    
    // ======================================================================
    //  NODE
    // ----------------------------------------------------------------------
    public void DrawNode(AP_Node _node) {
        // Don't show hiden nodes.
        if(_node.IsVisible == false) return;
        
        // Draw node box.
        Rect position= _node.Position;
        string title= ObjectNames.NicifyVariableName(_node.NameOrTypeName);
        if(_node is AP_State) {
            GUI.backgroundColor= Color.blue;
        }
        else if(_node is AP_Module) {
            GUI.backgroundColor= Color.yellow;            
        }
        else if(_node is AP_Function) {
            GUI.backgroundColor= Color.green;
        }
        else {
            GUI.backgroundColor= Color.grey;
        }
        GUIStyle guiStyle= null;
        if(_node.IsCompactNode()) {
            guiStyle= AP_EditorConfig.CompactNodeStyle;            
        }
        else {
            guiStyle= AP_EditorConfig.NodeStyle;            
        }
        GUI.Box(position, title, guiStyle);            
        GUI.backgroundColor= Color.grey;
        EditorGUIUtility.AddCursorRect (new Rect(position.x,  position.y, position.width, AP_EditorConfig.NodeTitleHeight), MouseCursor.MoveArrow);   

//        // Draw back drop
//        if(_node.Icon != null) {
//            Rect bdRect= new Rect(position.x+2, position.y+12, position.width-4, position.height-14);
//            GUIStyle backgroundStyle= new GUIStyle();
//            backgroundStyle.normal.background= _node.Icon;
//            GUI.Label(bdRect, "", backgroundStyle);            
//        }
    }

    
    // ======================================================================
    //  PORT
    // ----------------------------------------------------------------------
    public void DrawPort(AP_Port port) {
        // Only data ports are drawn.
        if(!(port is AP_DataPort)) return;
        AP_DataPort dataPort= port as AP_DataPort;

        // Don't draw port if port is not visible.
        if(dataPort.IsVisible == false) return;
        
        Vector2 pos= dataPort.Position;
        string name= dataPort.Name;
        Color color= dataPort.DisplayColor;
        if(dataPort.IsInput) {
            Color invColor= new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b, 1.0f);
            DrawPort(AP_Graphics.PortShape.UpTriangle, pos, color, invColor);
        }
        else if(dataPort.IsOutput) {
            DrawPort(AP_Graphics.PortShape.Circular, port.Position, color);                                        
        }
        // Show name if requested.
        if(dataPort.IsNameVisible) {
            Vector2 labelSize= AP_EditorConfig.GetPortLabelSize(name);
            if(dataPort.IsOnLeftEdge) {                
                pos.x+= AP_EditorConfig.PortSize;
                pos.y-= 0.4f * labelSize.y;
            }
            if(dataPort.IsOnRightEdge) {
                pos.x-= labelSize.x + AP_EditorConfig.PortSize;
                pos.y-= 0.4f * labelSize.y;        
            }
            GUI.Label(new Rect(pos.x, pos.y, labelSize.x, labelSize.y), name);
        }
    }
    public enum PortShape { Circular, Square, Diamond, UpTriangle, DownTriangle, LeftTriangle, RightTriangle };
    public void DrawPort(PortShape _shape, Vector3 _center, Color _fillColor, Color _borderColor) {
        // Readjust to screen coordinates.
        Vector3 center= new Vector3(_center.x-drawOffset.x, _center.y-drawOffset.y, _center.z);

        // Configure move cursor for port.
        EditorGUIUtility.AddCursorRect (new Rect(center.x-AP_EditorConfig.PortRadius,
                                                 center.y-AP_EditorConfig.PortRadius,
                                                 AP_EditorConfig.PortSize,
                                                 AP_EditorConfig.PortSize),
                                        MouseCursor.MoveArrow);   

        // Activate fill color.
        Handles.color= _fillColor;
        
        // Draw specific shapes.
        switch(_shape) {
            case PortShape.Circular:       DrawCircularPort(center, _borderColor); break;
            case PortShape.Square:         DrawSquarePort(center, _borderColor); break;
            case PortShape.Diamond:        DrawDiamondPort(center, _borderColor); break;
            case PortShape.UpTriangle:     DrawUpTrianglePort(center, _borderColor); break;
            case PortShape.DownTriangle:   DrawDownTrianglePort(center, _borderColor); break;
            case PortShape.LeftTriangle:   DrawLeftTrianglePort(center, _borderColor); break;
            case PortShape.RightTriangle:  DrawRightTrianglePort(center, _borderColor); break;
        }        
    }
    public void DrawPort(PortShape _shape, Vector3 _center, Color _color) {
        DrawPort(_shape, _center, _color, _color);
    }

	// ----------------------------------------------------------------------
    void DrawCircularPort(Vector3 _center, Color _borderColor) {
        Handles.DrawSolidDisc(_center, FacingNormal, AP_EditorConfig.PortRadius);
        Handles.color= _borderColor;
        Handles.DrawWireDisc(_center, FacingNormal, AP_EditorConfig.PortRadius);
    }

	// ----------------------------------------------------------------------
    void DrawSquarePort(Vector3 _center, Color _borderColor) {
        // Draw connector.
        Vector3[] vectors= new Vector3[5];
        Handles.DrawSolidDisc(_center, FacingNormal, AP_EditorConfig.PortRadius);
        float delta= AP_EditorConfig.PortRadius-1;
        float minSize= 0.707f * AP_EditorConfig.PortRadius;
        for(; delta > minSize; --delta) {
            vectors[0]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[3]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[4]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= AP_EditorConfig.PortRadius;
        vectors[0]= new Vector3(_center.x-delta, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[3]= new Vector3(_center.x+delta, _center.y-delta, 0);
        vectors[4]= vectors[0];
        Handles.DrawPolyLine(vectors);
    }
    
	// ----------------------------------------------------------------------
    void DrawDiamondPort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[5];
        float radius= AP_EditorConfig.PortRadius+1;
        Handles.DrawSolidDisc(_center, FacingNormal, 0.707f * radius);
        float delta= radius-1;
        float minSize= 0.707f * radius;
        for(; delta > minSize; --delta) {
            vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y, 0);
            vectors[2]= new Vector3(_center.x, _center.y+delta, 0);
            vectors[3]= new Vector3(_center.x+delta, _center.y, 0);
            vectors[4]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x-delta, _center.y, 0);
        vectors[2]= new Vector3(_center.x, _center.y+delta, 0);
        vectors[3]= new Vector3(_center.x+delta, _center.y, 0);
        vectors[4]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }
    
	// ----------------------------------------------------------------------
    void DrawDownTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= AP_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x, _center.y+delta, 0);
            vectors[1]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x, _center.y+delta, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y-delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    void DrawUpTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= AP_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
            vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x, _center.y-delta, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    void DrawRightTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= AP_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x+delta, _center.y, 0);
            vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x+delta, _center.y, 0);
        vectors[1]= new Vector3(_center.x-delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x-delta, _center.y-delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }

	// ----------------------------------------------------------------------
    void DrawLeftTrianglePort(Vector3 _center, Color _borderColor) {
        Vector3[] vectors= new Vector3[4];
        float radius= AP_EditorConfig.PortRadius+1;
        Handles.DrawLine(new Vector3(_center.x+1, _center.y+1, 0), new Vector3(_center.x-1, _center.y-1, 0));
        float delta= 1;
        for(; delta < radius; ++delta) {
            vectors[0]= new Vector3(_center.x-delta, _center.y, 0);
            vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
            vectors[2]= new Vector3(_center.x+delta, _center.y-delta, 0);
            vectors[3]= vectors[0];            
            Handles.DrawPolyLine(vectors);
        }
		Handles.color= _borderColor;
        delta= radius;
        vectors[0]= new Vector3(_center.x-delta, _center.y, 0);
        vectors[1]= new Vector3(_center.x+delta, _center.y+delta, 0);
        vectors[2]= new Vector3(_center.x+delta, _center.y-delta, 0);
        vectors[3]= vectors[0];
        Handles.DrawPolyLine(vectors);        
    }


    // ======================================================================
    //  CONNECTION
    // ----------------------------------------------------------------------
    public void DrawConnection(AP_Port port) {
        // Only data connection are drawn.
        if(!(port is AP_DataPort)) return;
        AP_DataPort dataPort= port as AP_DataPort;
        
        if(dataPort.Parent.IsVisible) {
            AP_DataPort source= dataPort.Source;
            if(source != null && source.Parent.IsVisible) {
                Vector2 start= source.Position;
                Vector2 end= dataPort.Position;
                Vector2 startDirection= source.IsOnHorizontalEdge ? DownDirection : RightDirection;
                Vector2 endDirection= dataPort.IsOnHorizontalEdge ? UpDirection : LeftDirection;
                Vector2 diff= end-start;
                if(Vector2.Dot(diff, startDirection) < 0) {
                    startDirection= -startDirection;
                }
                if(Vector2.Dot(diff, endDirection) > 0) {
                    endDirection  = - endDirection;
                }
                Color color= dataPort.DisplayColor;
                DrawBezierCurve(start, end, startDirection, endDirection, color);
            }                    
        }        
    }
    // ----------------------------------------------------------------------
    public void DrawBezierCurve(Vector3 _start, Vector3 _end, Vector3 _startDir, Vector3 _endDir, Color _color) {
        // Readjust to screen coordinates.
        Vector3 start= new Vector3(_start.x-drawOffset.x, _start.y-drawOffset.y, _start.z);
        Vector3 end  = new Vector3(_end.x-drawOffset.x  , _end.y-drawOffset.y  , _end.z);
        
		// Determine weight of the tangents.
		Vector3 vertex= end-start;

        // Compute Bezier tangents.
        Vector3 startTangent= start + _startDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector3.Dot(_startDir, vertex)));
        Vector3 endTangent  = end + _endDir * 0.25f * (vertex.magnitude + Mathf.Abs(Vector3.Dot(_endDir, vertex)));

        // Use a Bezier for the connections.
        Color color= new Color(_color.r, _color.g, _color.b, 0.5f*_color.a);
		Handles.DrawBezier(start, end, startTangent, endTangent, color, lineTexture, 1.5f);
    }


    // ======================================================================
    //  ERROR MANAGEMENT
    // ----------------------------------------------------------------------
	public static void ResourceMissingError(string _name, ref bool _alreadySeen) {
        string errorMsg= "Unable to locate editor resource: " + _name;
        if(!_alreadySeen) {
    		EditorUtility.DisplayDialog ("State Chart Abort Message", errorMsg, "Ok");            
        }
		else {
		    Debug.LogError(errorMsg);
	    }
		_alreadySeen= true;
	}


}
