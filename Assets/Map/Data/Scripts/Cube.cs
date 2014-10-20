using UnityEngine;
using System.Collections.Generic;
using Environment;

public enum CubeFace {
	Front  = 0,
	Back   = 1,
	Right  = 2,
	Left   = 3,
	Top    = 4,
	Bottom = 5,
}

[System.Serializable]
public class Cube : Block {
	
	[SerializeField] private Rect forward;
	[SerializeField] private Rect back;
	[SerializeField] private Rect right;
	[SerializeField] private Rect left;
	[SerializeField] private Rect up;
	[SerializeField] private Rect down;
	
	
	public Cube(string name) {
		SetName(name);
	}
	
	public override Rect GetPreviewFace() {
		return forward;
	}
	
	public void SetFace(Rect coord, CubeFace face) {
		switch (face) {
			case CubeFace.Front: forward = coord; return;
			case CubeFace.Back: back = coord; return;
			
			case CubeFace.Right: right = coord; return;
			case CubeFace.Left: left = coord; return;
			
			case CubeFace.Top: up = coord; return;
			case CubeFace.Bottom: down = coord; return;
		}
	}
	
	public Rect GetFace(CubeFace face) {
		switch (face) {
			case CubeFace.Front: return forward;
			case CubeFace.Back: return back;
			
			case CubeFace.Right: return right;
			case CubeFace.Left: return left;
			
			case CubeFace.Top: return up;
			case CubeFace.Bottom: return down;
		}
		return new Rect(0,0,0,0);
	}
	
	public Rect GetFace(CubeFace face, BlockDirection dir) {
		if(face != CubeFace.Top && face != CubeFace.Bottom) {
			face = TransformFace(face, dir);
		}
		
		switch (face) {
			case CubeFace.Front: return forward;
			case CubeFace.Back: return back;
			
			case CubeFace.Right: return right;
			case CubeFace.Left: return left;
			
			case CubeFace.Top: return up;
			case CubeFace.Bottom: return down;
		}
		return default(Rect);
	}
	
	private static CubeFace TransformFace(CubeFace face, BlockDirection dir) {
		//Front, Right, Back, Left
		//0      90     180   270
		
		int angle = 0;
		if(face == CubeFace.Right) angle = 90;
		if(face == CubeFace.Back)  angle = 180;
		if(face == CubeFace.Left)  angle = 270;
		
		if(dir == BlockDirection.X_MINUS) angle += 90;
		if(dir == BlockDirection.Z_MINUS) angle += 180;
		if(dir == BlockDirection.X_PLUS) angle += 270;
		
		angle %= 360;
		
		if(angle == 0) return CubeFace.Front;
		if(angle == 90) return CubeFace.Right;
		if(angle == 180) return CubeFace.Back;
		if(angle == 270) return CubeFace.Left;
		
		return CubeFace.Front;
	}
	
	
	public override void Build(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		CubeBuilder.BuildCube(localPos, worldPos, mesh, onlyLight);
	}
	
	
}