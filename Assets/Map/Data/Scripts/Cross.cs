using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cross : Block {
	
	[SerializeField] private Rect face;
	
	public Cross(string name) {
		SetName(name);
	}
	
	public override Rect GetPreviewFace() {
		return face;
	}
	
	public void SetFace(Rect face) {
		this.face = face;
	}
	public Rect GetFace() {
		return face;
	}
	
	public override void Build(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		CrossBuilder.BuildCross(localPos, worldPos, mesh, onlyLight);
	}
	
}