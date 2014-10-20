using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(Atlas))]
public class AtlasEditor : Editor {
	
	private Atlas atlas;
	
	
	[MenuItem ("Map/Create Atlas")]
	private static void CreateAtlas() {
		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Atlas>(), "Assets/NewAtlas.asset");
		
		//Atlas atlas = ScriptableObject.CreateInstance<Atlas>();
		//atlas.name = "Atlas";
		
		//Material material = new Material(Shader.Find("Diffuse"));
		//material.name = "Material";
		
		//string path = "Assets/NewAtlas.asset";
		//AssetDatabase.CreateAsset(atlas, path);
		//AssetDatabase.AddObjectToAsset(material, path);
		//AssetDatabase.ImportAsset(path);
	}
	
	
	void OnEnable() {
		atlas = (Atlas) target;
	}
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeControls();
		
		int w = EditorGUILayout.IntField("Width", atlas.GetWidth());
		atlas.SetWidth(w);
		
		int h = EditorGUILayout.IntField("Height", atlas.GetHeight());
		atlas.SetHeight(h);
		
		Material material = (Material) EditorGUILayout.ObjectField("Material", atlas.GetMaterial(), typeof(Material), true);
		atlas.SetMaterial(material);
		
		bool alpha = EditorGUILayout.Toggle("Alpha", atlas.IsAlpha());
		atlas.SetAlpha( alpha );
		
		if(atlas.GetMaterial() != null && atlas.GetMaterial().mainTexture != null) {
			AtlasViewer.DrawAtlas(atlas, new Rect(0, 0, 1, 1));
		}
		
		if(GUI.changed) EditorUtility.SetDirty(atlas);
	}
	
	
}
