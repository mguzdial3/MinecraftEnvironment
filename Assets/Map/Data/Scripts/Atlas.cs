using UnityEngine;
using System.Collections;

public class Atlas : ScriptableObject {
	
	[SerializeField] private int width = 1, height = 1;
	[SerializeField] private Material material;
	[SerializeField] private bool alpha = false;
	
	
	public void SetMaterial(Material material) {
		this.material = material;
	}
	public Material GetMaterial() {
		return material;
	}
	
	public void SetWidth(int width) {
		this.width = width;
	}
	public int GetWidth() {
		return width;
	}
	
	public void SetHeight(int height) {
		this.height = height;
	}
	public int GetHeight() {
		return height;
	}
	
	public void SetAlpha(bool alpha) {
		this.alpha = alpha;
	}
	public bool IsAlpha() {
		return alpha;
	}
	
	public float GetTileSizeX() {
		return 1.0f/width;
	}
	public float GetTileSizeY() {
		return 1.0f/height;
	}
	
}
