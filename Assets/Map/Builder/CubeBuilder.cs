using UnityEngine;
using System.Collections.Generic;
using Environment;

public class CubeBuilder {
	
	private static CubeFace[] faces = new CubeFace[] {
		CubeFace.Left,
		CubeFace.Right,
		
		CubeFace.Bottom,
		CubeFace.Top,
		
		CubeFace.Back,
		CubeFace.Front
	};
	
	private static Vector3i[] directions = new Vector3i[] {
		new Vector3i(-1, 0, 0), //left
		new Vector3i( 1, 0, 0), //right
		
		new Vector3i(0, -1, 0), //bottom
		new Vector3i(0,  1, 0), //top
		
		new Vector3i(0, 0, -1), //back
		new Vector3i(0, 0,  1)  //front
	};
	
	private static Vector3[][] vertices = new Vector3[][] {
		//Front
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f,  0.5f, 0.5f),
			new Vector3( 0.5f,  0.5f, 0.5f),
			new Vector3( 0.5f, -0.5f, 0.5f),
		},
		//Back
		new Vector3[] {
			new Vector3( 0.5f, -0.5f, -0.5f),
			new Vector3( 0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f),
		},
		//Right
		new Vector3[] {
			new Vector3(0.5f, -0.5f,  0.5f),
			new Vector3(0.5f,  0.5f,  0.5f),
			new Vector3(0.5f,  0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
		},
		//Left
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f, -0.5f),
			new Vector3(-0.5f,  0.5f,  0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			
		},
		//Top
		new Vector3[] {
			new Vector3( 0.5f, 0.5f, -0.5f),
			new Vector3( 0.5f, 0.5f,  0.5f),
			new Vector3(-0.5f, 0.5f,  0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
		},
		//Bottom
		new Vector3[] {
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f,  0.5f),
			new Vector3( 0.5f, -0.5f, -0.5f),
		},
	};
	
	private static Vector3[][] normals = new Vector3[][] {
		new Vector3[] {
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
		},
		new Vector3[] {
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
		},
		new Vector3[] {
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
		},
		new Vector3[] {
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
		},
		new Vector3[] {
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
		},
		new Vector3[] {
			Vector3.down, Vector3.down, Vector3.down, Vector3.down,
		},
	};
	
	
	public static void BuildCube(Vector3i localPos, Vector3i worldPos, MeshData mesh, bool onlyLight) {
		BlockData block = Map.Instance.GetBlock(worldPos);
		Cube cube = (Cube) block.block;
		BlockDirection direction = block.GetDirection();
		
		for(int i=0; i<6; i++) {
			CubeFace face = faces[i];
			Vector3i dir = directions[i];
			Vector3i nearPos = worldPos + dir;
			if( IsFaceVisible(nearPos, cube) ) {
				if(!onlyLight) BuildFace(face, cube, direction, new Vector3(localPos.x,localPos.y,localPos.z), mesh);
				BuildFaceLight(face, worldPos, mesh);
			}
		}
	}
	
	private static bool IsFaceVisible(Vector3i pos, Cube currentCube) {
		Block block = Map.Instance.GetBlock(pos).block;
		if(!(block is Cube)) return true;
		return block.IsAlpha() && block != currentCube;
	}
	
	private static void BuildFace(CubeFace face, Cube cube, BlockDirection direction, Vector3 position, MeshData mesh) {
		List<int> indices = mesh.GetIndices(cube.GetAtlasID());
		Rect texCoord = cube.GetFace(face, direction);
		int iFace = (int)face;
		
		BuildUtils.AddFaceIndices(mesh.vertices.Count, indices);
		BuildUtils.AddVertices(mesh.vertices, vertices[iFace], position);
		mesh.normals.AddRange( normals[iFace] );
		BuildUtils.AddFaceUV(texCoord, mesh.uv);
	}
	
	private static void BuildFaceLight(CubeFace face, Vector3i pos, MeshData mesh) {
		BlockData block = Map.Instance.GetBlock(pos);
		float light = (float) block.GetLight() / LightComputer.MAX_LIGHT;
		foreach(Vector3 ver in vertices[(int)face]) {
			float sun = GetVertexSunLight( pos, ver, face);
			Color color = new Color(light, light, light, sun);
			mesh.colors.Add( color );
		}
	}
	
	private static float GetVertexSunLight(Vector3i pos, Vector3 vertex, CubeFace face) {
		int dx = (int)Mathf.Sign( vertex.x );
		int dy = (int)Mathf.Sign( vertex.y );
		int dz = (int)Mathf.Sign( vertex.z );
		
		Vector3i a, b, c, d;
		if(face == CubeFace.Left || face == CubeFace.Right) { // X
			a = pos + new Vector3i(dx, 0,  0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(dx, 0,  dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else 
		if(face == CubeFace.Bottom || face == CubeFace.Top) { // Y
			a = pos + new Vector3i(0,  dy, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else { // Z
			a = pos + new Vector3i(0,  0,  dz);
			b = pos + new Vector3i(dx, 0,  dz);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		}
		
		Lightmap lightmap = Map.Instance.GetLightmap();
		int sun = 0;
		if(Map.Instance.GetBlock(b).IsAlpha() || Map.Instance.GetBlock(c).IsAlpha()) {
			sun = lightmap.GetLight(a) + lightmap.GetLight(b) + lightmap.GetLight(c) + lightmap.GetLight(d);
			sun /= 4;
		} else {
			sun = lightmap.GetLight(a) + lightmap.GetLight(b) + lightmap.GetLight(c);
			sun /= 3;
		}
		return (float) sun / LightComputer.MAX_LIGHT;
	}
	
	
}