using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(BlockSet))]
public class BlockSetEditor : Editor {

	private BlockSet blockSet;
	private static Vector2 blockSetScrollPosition;
	private static int selectedBlock = -1;
	
	private static Rect atlasRect = new Rect(0, 0, 1, 1);
	private static CubeFace selectedFace = CubeFace.Front;
	
	[MenuItem ("Map/Create BlockSet")]
	private static void CreateCube() {
		GameObject go = new GameObject("TileSet", typeof(BlockSet));
		PrefabUtility.CreatePrefab("Assets/NewBlockSet.prefab", go);
		GameObject.DestroyImmediate(go);
	}
	
	void OnEnable() {
		blockSet = (BlockSet)target;
	}
	
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeControls();
		
		Atlas[] list = DrawAtlasesEditor( blockSet.GetAtlases() );
		blockSet.SetAtlases( list );
		EditorGUILayout.Separator();
		
		DrawBlockSetEditor( blockSet );
		EditorGUILayout.Separator();
		
		if( blockSet.GetBlock(selectedBlock) != null ) {
			DrawBlockEditor( blockSet.GetBlock(selectedBlock), blockSet );
		}
		
		if(GUI.changed) {
			EditorUtility.SetDirty(blockSet);
			Repaint();
		}
	}
	
	
	private static Atlas[] DrawAtlasesEditor( Atlas[] list ) {
		GUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
		for(int i=0; i<list.Length;) {
			GUILayout.BeginHorizontal();
			if( GUILayout.Button("Remove") ) {
				ArrayUtility.RemoveAt<Atlas>(ref list, i);
			} else {
				list[i] = (Atlas)EditorGUILayout.ObjectField(list[i], typeof(Atlas), false);
				i++;
			}
			GUILayout.EndHorizontal();
		}
		
		if(GUILayout.Button("Add Atlas")) {
			ArrayUtility.Add<Atlas>(ref list, null);
		}
		GUILayout.EndVertical();
		return list;
	}
	
	
	private static Atlas AtlasField(Atlas atlas) {
		return (Atlas)EditorGUILayout.ObjectField(atlas, typeof(Atlas), false);
	}
	private static Atlas AtlasField(string label, Atlas atlas) {
		return (Atlas)EditorGUILayout.ObjectField(label, atlas, typeof(Atlas), false);
	}
	
	private static void DrawBlockSetEditor(BlockSet blockSet) {
		GUILayout.BeginVertical("box");
		selectedBlock = DrawBlockList( blockSet, selectedBlock, ref blockSetScrollPosition );
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("Add Cube")) {
				selectedBlock = blockSet.Add( new Cube("new Cube") );
			}
			if(GUILayout.Button("Add Cross")) {
				selectedBlock = blockSet.Add( new Cross("new Cross") );
			}
		GUILayout.EndHorizontal();
		if( GUILayout.Button("Remove") && blockSet.GetBlock(selectedBlock) != null ) {
			Undo.RecordObject( blockSet, "Remove block" );
			blockSet.Remove( selectedBlock );
			selectedBlock = Mathf.Clamp(selectedBlock, 0, blockSet.GetCount()-1);
		}
		GUILayout.EndVertical();
	}
	
	private static int DrawBlockList(BlockSet blocks, int selected, ref Vector2 scrollPosition) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
		
		GUIStyle normal = GUI.skin.button;
		GUIStyle active = new GUIStyle(normal);
		active.normal = active.active;
		
		for(int i=0; i<blocks.GetCount(); i++) {
			Block block = blocks.GetBlock(i);
			if(i == selected) GUI.skin.button = active;
			if( GUILayout.Button(block.GetName()) ) {
				if(selected != i) GUIUtility.keyboardControl = 0;
				selected = i;
			}
			GUI.skin.button = normal;
		}
		
		GUILayout.EndScrollView();
		return selected;
	}
	
	
	private static void DrawBlockEditor(Block block, BlockSet blockSet) {
		string name = EditorGUILayout.TextField("Name", block.GetName());
		block.SetName(name);
		
		Atlas[] atlases = blockSet.GetAtlases();
		string[] atlasNames = new string[atlases.Length];
		for(int i=0; i<atlasNames.Length; i++) {
			Atlas tAtlas = atlases[i];
			atlasNames[i] = tAtlas != null ? tAtlas.name : "null";
		}
		int index = ArrayUtility.IndexOf(atlases, block.GetAtlas());
		index = EditorGUILayout.Popup("Atlas", index, atlasNames);
		block.SetAtlas( SafeGet(atlases, index) );
		
		int light = EditorGUILayout.IntField("Light", block.GetLight());
		light = Mathf.Clamp(light, 0, 15);
		block.SetLight((byte)light);
		
		Atlas atlas = block.GetAtlas();
		if(atlas == null || atlas.GetMaterial() == null || atlas.GetMaterial().mainTexture == null) return;
		if(block is Cube) DrawCubeBlockEditor( (Cube)block, atlas );
		if(block is Cross) DrawCrossBlockEditor( (Cross)block, atlas );
	}
	
	private static Atlas SafeGet(Atlas[] list, int index) {
		if(index < 0 || index >= list.Length) return null;
		return list[index];
	}
	
	private static void DrawCubeBlockEditor(Cube block, Atlas atlas) {
		selectedFace = DrawCubeFacesEditor(selectedFace, block, atlas);
		Rect facePosition = block.GetFace(selectedFace);
		AtlasViewer.DrawAtlasEditor(atlas, ref atlasRect, ref facePosition);
		block.SetFace(facePosition, selectedFace);
	}
	
	private static void DrawCrossBlockEditor(Cross block, Atlas atlas) {
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(GUIContent.none, GUILayout.Width(64), GUILayout.Height(64));
			Rect rect = GUILayoutUtility.GetLastRect();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUI.DrawTextureWithTexCoords(rect, atlas.GetMaterial().mainTexture, block.GetFace());
		
		Rect position = block.GetFace();
		AtlasViewer.DrawAtlasEditor(atlas, ref atlasRect, ref position);
		block.SetFace(position);
	}
	
	private static CubeFace DrawCubeFacesEditor(CubeFace face, Cube cube, Atlas atlas) {
		string[] items = new string[6];
		for(int i=0; i<6; i++) {
			items[i] = ((CubeFace)i).ToString();
		}
		Texture texture = atlas.GetMaterial().mainTexture;
		
		GUILayout.BeginVertical("box");
		face = (CubeFace)GUILayout.Toolbar((int)face, items);
		Rect bigRect = GUILayoutUtility.GetAspectRect(items.Length);
		for(int i=0; i<items.Length; i++) {
			Rect position = bigRect;
			position.width /= items.Length;
			position.x += i*position.width;
			Rect face_rect = cube.GetFace( (CubeFace) i );
			GUI.DrawTextureWithTexCoords(position, texture, face_rect);
		}
		GUILayout.EndVertical();
		
		return face;
	}
	
}
