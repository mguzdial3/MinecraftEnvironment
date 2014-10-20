using UnityEngine;
using System.Collections;
using Environment;

public class Chunk : MonoBehaviour {
	
	public const int SIZE_X_BITS = 4;
	public const int SIZE_Y_BITS = 4;
	public const int SIZE_Z_BITS = 4;
	
	public const int SIZE_X = 1 << SIZE_X_BITS;
	public const int SIZE_Y = 1 << SIZE_Y_BITS;
	public const int SIZE_Z = 1 << SIZE_Z_BITS;
	
	private BlockSet blockSet;
	private ChunkData chunkData;
	
	private MeshFilter filter;
	private MeshCollider meshCollider;
	
	private bool dirty = false, lightDirty = false;
	
	
	public static Chunk CreateChunk(Vector3i pos, ChunkData chunkData) {
		GameObject go = new GameObject("("+pos.x+" "+pos.y+" "+pos.z+")  "+SnapshotHandler.Instance.transform.childCount);
		go.transform.parent = SnapshotHandler.Instance.transform;
		go.transform.localPosition = new Vector3(pos.x*Chunk.SIZE_X, pos.y*Chunk.SIZE_Y, pos.z*Chunk.SIZE_Z);
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		
		Chunk chunk = go.AddComponent<Chunk>();
		chunk.blockSet = Map.Instance.GetBlockSet();
		chunk.chunkData = chunkData;
		return chunk;
	}
	
	
	void Update() {
		if(dirty) {
			Build();
			dirty = lightDirty = false;
		}
		if(lightDirty) {
			BuildLighting();
			lightDirty = false;
		}
	}
	
	private void Build() {
		if(filter == null) {
			gameObject.AddComponent<MeshRenderer>().sharedMaterials = blockSet.GetMaterials();
			gameObject.renderer.castShadows = false;
			gameObject.renderer.receiveShadows = false;
			filter = gameObject.AddComponent<MeshFilter>();
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
		
		filter.sharedMesh = ChunkBuilder.BuildChunk(filter.sharedMesh, chunkData);
		
		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = filter.sharedMesh;
		
		if(gameObject.activeSelf && filter.sharedMesh == null) {
			gameObject.SetActive(false);
		}
		if(!gameObject.activeSelf && filter.sharedMesh != null) {
			gameObject.SetActive(true);
		}
	}
	
	private void BuildLighting() {
		if(filter != null && filter.sharedMesh != null) {
			ChunkBuilder.BuildChunkLighting(filter.sharedMesh, chunkData);
		}
	}
	
	public void SetDirty() {
		dirty = true;
	}
	public void SetLightDirty() {
		lightDirty = true;
	}
	
	public static bool FixCoords(ref Vector3i chunk, ref Vector3i local) {
		bool changed = false;
		if(local.x < 0) {
			chunk.x--;
			local.x += Chunk.SIZE_X;
			changed = true;
		}
		if(local.y < 0) {
			chunk.y--;
			local.y += Chunk.SIZE_Y;
			changed = true;
		}
		if(local.z < 0) {
			chunk.z--;
			local.z += Chunk.SIZE_Z;
			changed = true;
		}
		
		if(local.x >= Chunk.SIZE_X) {
			chunk.x++;
			local.x -= Chunk.SIZE_X;
			changed = true;
		}
		if(local.y >= Chunk.SIZE_Y) {
			chunk.y++;
			local.y -= Chunk.SIZE_Y;
			changed = true;
		}
		if(local.z >= Chunk.SIZE_Z) {
			chunk.z++;
			local.z -= Chunk.SIZE_Z;
			changed = true;
		}
		return changed;
	}
	
	public static bool IsCorrectLocalPosition(Vector3i local) {
		return IsCorrectLocalPosition(local.x, local.y, local.z);
	}
	public static bool IsCorrectLocalPosition(int x, int y, int z) {
		return (x & SIZE_X-1) == x &&
			   (y & SIZE_Y-1) == y &&
			   (z & SIZE_Z-1) == z;
	}
	
	public static Vector3i ToChunkPosition(Vector3i point) {
		return ToChunkPosition( point.x, point.y, point.z );
	}
	public static Vector3i ToChunkPosition(int pointX, int pointY, int pointZ) {
		int chunkX = pointX >> SIZE_X_BITS;
		int chunkY = pointY >> SIZE_Y_BITS;
		int chunkZ = pointZ >> SIZE_Z_BITS;
		return new Vector3i(chunkX, chunkY, chunkZ);
	}
	
	public static Vector3i ToLocalPosition(Vector3i point) {
		return ToLocalPosition(point.x, point.y, point.z);
	}
	public static Vector3i ToLocalPosition(int pointX, int pointY, int pointZ) {
		int localX = pointX & (SIZE_X-1);
		int localY = pointY & (SIZE_Y-1);
		int localZ = pointZ & (SIZE_Z-1);
		return new Vector3i(localX, localY, localZ);
	}
	
	public static Vector3i ToWorldPosition(Vector3i chunkPosition, Vector3i localPosition) {
		int worldX = (chunkPosition.x << SIZE_X_BITS) + localPosition.x;
		int worldY = (chunkPosition.y << SIZE_Y_BITS) + localPosition.y;
		int worldZ = (chunkPosition.z << SIZE_Z_BITS) + localPosition.z;
		return new Vector3i(worldX, worldY, worldZ);
	}
	
}

