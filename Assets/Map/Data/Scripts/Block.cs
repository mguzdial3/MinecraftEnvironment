using UnityEngine;
using System.Collections;
using System;
using Environment;


[System.Serializable]
public abstract class Block {
	
	[SerializeField] private string name;
	[SerializeField] private Atlas atlas;
	[SerializeField] private byte light = LightComputer.MIN_LIGHT;
	
	private bool alpha = false;
	private int atlasId;
	private int blockId;
	
	public void Init(BlockSet blockSet, int blockId) {
		if(atlas != null) alpha = atlas.IsAlpha();
		atlasId = Array.IndexOf(blockSet.GetAtlases(), atlas);
		this.blockId = blockId;
		light = (byte) Mathf.Clamp(light, 0, 15);
	}
	
	public bool DrawPreview(Rect rect) {
		Rect face = GetPreviewFace();
		if(atlas != null && atlas.GetMaterial() != null && atlas.GetMaterial().mainTexture != null) {
			GUI.DrawTextureWithTexCoords(rect, atlas.GetMaterial().mainTexture, face);
		}
		return Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition);
	}
	public abstract Rect GetPreviewFace();
	
	public abstract void Build(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight);
	
	public void SetName(string name) {
		this.name = name;
	}
	public string GetName() {
		return name;
	}

	public int GetBlockId(){
		return blockId;
	}
	
	public void SetAtlas(Atlas atlas) {
		this.atlas = atlas;
	}
	public Atlas GetAtlas() {
		return atlas;
	}
	public int GetAtlasID() {
		return atlasId;
	}
	
	public void SetLight(byte light) {
		this.light = light;
	}
	public byte GetLight() {
		return light;
	}
	
	public bool IsAlpha() {
		return alpha;
	}
	
}