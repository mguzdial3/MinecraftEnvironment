using UnityEngine;
using UnityEditor;
using System.Collections;

public class AtlasViewer {
	
	public static Rect DrawAtlas(Atlas atlas, Rect texCoords) {
		return DrawTexture(atlas.GetMaterial().mainTexture, texCoords);
	}
	
	private static Rect DrawTexture(Texture texture, Rect texCoords) {
		GUILayout.BeginVertical("box");
		Rect rect = GUILayoutUtility.GetAspectRect((float)texture.width/texture.height);
		GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
		GUILayout.EndVertical();
		return rect;
	}
	
	
	
	
	public static void DrawCursor(Rect cursorRect, Rect viewRect, Rect texCoords) {
		cursorRect = FromWindowCoord(cursorRect, texCoords);
		cursorRect.y = 1-cursorRect.y;
		cursorRect.height *= -1;
		cursorRect = Scale(cursorRect, viewRect.width, viewRect.height);
		
		GUI.BeginGroup(viewRect);
		DrawRect(cursorRect, Color.green);
		GUI.EndGroup();
		
		cursorRect = Scale(cursorRect, 1.0f/viewRect.width, 1.0f/viewRect.height);
		cursorRect.height *= -1;
		cursorRect.y = 1-cursorRect.y;
		cursorRect = ToWindowCoord(cursorRect, texCoords);
	}
	
	public static void DrawRect(Rect rect, Color color) {
		Vector3 a = new Vector3(rect.xMin, rect.yMin, 0);
		Vector3 b = new Vector3(rect.xMax, rect.yMin, 0);
		Vector3 c = new Vector3(rect.xMax, rect.yMax, 0);
		Vector3 d = new Vector3(rect.xMin, rect.yMax, 0);
		
		Handles.color = color;
		Handles.DrawLine(a, b);
		Handles.DrawLine(b, c);
		Handles.DrawLine(c, d);
		Handles.DrawLine(d, a);
	}
	
	private static Rect ToWindowCoord(Rect rect, Rect window) {
		rect = Scale(rect, window.width, window.height);
		rect.x += window.x;
		rect.y += window.y;
		return rect;
	}
	
	private static Rect FromWindowCoord(Rect rect, Rect window) {
		rect.x -= window.x;
		rect.y -= window.y;
		return Scale(rect, 1.0f/window.width, 1.0f/window.height);
	}
	
	private static Rect Scale(Rect rect, float x, float y) {
		rect.xMin *= x;
		rect.xMax *= x;
		rect.yMin *= y;
		rect.yMax *= y;
		return rect;
	}
	
	public static Vector2 ToWindowCoord(Vector2 v, Rect window) {
		v.x *= window.width;
		v.y *= window.height;
		v.x += window.x;
		v.y += window.y;
		return v;
	}
	
	public static Vector2 FromWindowCoord(Vector2 v, Rect window) {
		v.x -= window.x;
		v.y -= window.y;
		v.x /= window.width;
		v.y /= window.height;
		return v;
	}
	
	
	public static void DrawAtlasEditor(Atlas atlas, ref Rect texCoords, ref Rect position) {
		Rect viewRect = DrawAtlas(atlas, texCoords);
		
		DrawCursor(position, viewRect, texCoords);
		
		if(!viewRect.Contains(Event.current.mousePosition)) return;
		
		Vector2 mouse = FromWindowCoord(Event.current.mousePosition, viewRect);
		mouse.y = 1-mouse.y;
			
		if(Event.current.type == EventType.ScrollWheel) {
			float scale = 0.95f;
			if(Event.current.delta.y > 0) scale = 1.0f/scale;
			Rect oldTexCoords = texCoords;
				
			texCoords.width = Mathf.Clamp01(texCoords.width*scale);
			texCoords.height = Mathf.Clamp01(texCoords.height*scale);
			texCoords.x -= (texCoords.width-oldTexCoords.width)*mouse.x;
			texCoords.y -= (texCoords.height-oldTexCoords.height)*mouse.y;
				
			texCoords = FixTexCoords(texCoords);
			Event.current.Use();
			GUI.changed = true;
		}
		if(Event.current.type == EventType.MouseDrag && Event.current.button == 1) {
			texCoords.x -= Event.current.delta.x/viewRect.width*texCoords.width;
			texCoords.y += Event.current.delta.y/viewRect.height*texCoords.height;
				
			texCoords = FixTexCoords(texCoords);
			GUI.changed = true;
		}
			
			
		if(Event.current.type == EventType.MouseDown && Event.current.button == 0) {
			Vector3 pos = ToWindowCoord(mouse, texCoords);
			position.x = pos.x - pos.x%atlas.GetTileSizeX();
			position.y = pos.y - pos.y%atlas.GetTileSizeY();
			position.width = atlas.GetTileSizeX();
			position.height = atlas.GetTileSizeY();
			GUI.changed = true;
		}
	}
	
	private static Rect FixTexCoords(Rect rect) {
		rect.x = Mathf.Clamp01(rect.x);
		rect.y = Mathf.Clamp01(rect.y);
		if(rect.xMax > 1) rect.x -= rect.xMax%1;
		if(rect.yMax > 1) rect.y -= rect.yMax%1;
		return rect;
	}
	
	
}
