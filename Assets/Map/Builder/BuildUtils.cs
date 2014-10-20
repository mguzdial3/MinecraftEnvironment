using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Environment;

class BuildUtils {
	
	public static void AddIndices(List<int> list, int[] indices, int offset) {
		foreach(int index in indices) {
			list.Add( index + offset );
		}
	}
	
	public static void AddVertices(List<Vector3> list, Vector3[] vertices, Vector3 offset) {
		foreach(Vector3 v in vertices) {
			list.Add( v + offset );
		}
	}

	
	public static void AddFaceNormals(Vector3 normal, List<Vector3> normals) {
		normals.Add(normal);
		normals.Add(normal);
		normals.Add(normal);
		normals.Add(normal);
	}
	
	public static void AddFaceLight(byte light, List<Color> colors) {
		float _light = (float) light / LightComputer.MAX_LIGHT;
		Color color = new Color(1, 1, 1, _light);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
	
	public static void AddFaceUV(Rect texCoord, List<Vector2> uv) {
		uv.Add( new Vector2(texCoord.xMax, texCoord.yMin) );
		uv.Add( new Vector2(texCoord.xMax, texCoord.yMax) );
		uv.Add( new Vector2(texCoord.xMin, texCoord.yMax) );
		uv.Add( new Vector2(texCoord.xMin, texCoord.yMin) );
	}
	
	public static void AddFaceIndices(int start, List<int> indices) {
		indices.Add( start+2 );
		indices.Add( start+1 );
		indices.Add( start+0 );
		
		indices.Add( start+3 );
		indices.Add( start+2 );
		indices.Add( start+0 );
	}
	
	
	
	
}

public class MeshData {
	
	public readonly List<Vector3> vertices = new List<Vector3>();
	public readonly List<Vector2> uv = new List<Vector2>();
	public readonly List<Vector3> normals = new List<Vector3>();
	public readonly List<Color> colors = new List<Color>();
	private List<int>[] indices = new List<int>[0];
	
	public MeshData(int subMeshCount) {
		indices = new List<int>[subMeshCount];
		for(int i=0; i<subMeshCount; i++) {
			indices[i] = new List<int>();
		}
	}
	
	public List<int> GetIndices(int index) {
		/*if(index >= indices.Length) {
			List<int>[] oldIndices = indices;
			indices = new List<int>[index+1];
			for(int i=0; i<indices.Length; i++) {
				if(i < oldIndices.Length) {
					indices[i] = oldIndices[i];
				} else {
					indices[i] = new List<int>();
				}
			}
		}*/
		return indices[index];
	}
	
	public void Clear() {
		vertices.Clear();
		uv.Clear();
		normals.Clear();
		colors.Clear();
		foreach(List<int> list in indices) {
			list.Clear();
		}
	}
	
	public Mesh ToMesh(Mesh mesh) {
		if(vertices.Count == 0) {
			GameObject.Destroy(mesh);
			return null;
		}
		
		if(mesh == null) mesh = new Mesh();
		
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();
		
		mesh.subMeshCount = indices.Length;
		for(int i=0; i<indices.Length; i++) {
			mesh.SetTriangles( indices[i].ToArray(), i );
		}
		
		return mesh;
	}
	
}