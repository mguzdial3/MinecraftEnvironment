using System.Collections;
using System.Collections.Generic;

namespace Environment{
public class LightComputer {
	
	public const byte MIN_LIGHT = 1;
	public const byte MAX_LIGHT = 15;
	public const byte STEP_LIGHT = 1;
	
	
	public static void ComputeSolarLighting(Map map, int cx, int cz) {
		int x1 = cx*Chunk.SIZE_X-1;
		int z1 = cz*Chunk.SIZE_Z-1;
		int x2 = (cx+1)*Chunk.SIZE_X+1;
		int z2 = (cz+1)*Chunk.SIZE_Z+1;
		for(int z=z1; z<z2; z++) {
			for(int x=x1; x<x2; x++) {
				ComputeSolarLightingAtPosition(map, x, z);
			}
		}
	}
	
	private static void ComputeSolarLightingAtPosition(Map map, int x, int z) {
		for(int y=Map.maxBlockY; y>=0; y--) {
			BlockData block = map.GetBlock( x, y, z );
			if( !block.IsAlpha() ) {
				map.GetLightmap().SetSunHeight(y+1, x, z);
				break;
			}
		}
	}
	
	
	public static void Smooth(Map map, int cx, int cz) {
		int x1 = cx*Chunk.SIZE_X;
		int z1 = cz*Chunk.SIZE_Z;
		
		int x2 = x1+Chunk.SIZE_X;
		int z2 = z1+Chunk.SIZE_Z;
		int y2 = Map.maxBlockY;
		
		Lightmap lightmap = map.GetLightmap();
		for(int x=x1; x<x2; x++) {
			for(int z=z1; z<z2; z++) {
				int y = lightmap.GetSunHeight(x, z);
				for(; y<y2; y++) {
					if(IsTerrainEnd(lightmap, x, y, z)) break;
					Smooth(map, x, y, z, MAX_LIGHT);
				}
			}
		}
	}
	
	private static bool IsTerrainEnd(Lightmap lightmap, int x, int y, int z) { // верхняя точка террейна
		return  lightmap.IsSunLight(x-1, y, z) &&
				lightmap.IsSunLight(x+1, y, z) &&
				lightmap.IsSunLight(x,   y, z-1) &&
				lightmap.IsSunLight(x,   y, z+1);
	}
	
	private static void Smooth(Map map, int x, int y, int z, byte light) {
		light -= STEP_LIGHT;
		if(light <= MIN_LIGHT) return;
		if(y<0 || y>=Map.maxBlockY) return;
		
		Lightmap lightmap = map.GetLightmap();
		foreach(Vector3i dir in Vector3i.directions) {
			int nx = x + dir.x;
			int ny = y + dir.y;
			int nz = z + dir.z;
			
			BlockData block = map.GetBlock(nx, ny, nz);
			if(block.IsAlpha()) {
				if( lightmap.SetMaxLight(light, nx, ny, nz) ) {
					Smooth(map, nx, ny, nz, light);
				}
			}
			if(!block.IsEmpty()) {
				SetLightDirty(map, nx, ny, nz);
			}
		}
	}
	
	public static void SetLightDirty(Map map, int blockX, int blockY, int blockZ) {
		ChunkData chunk = map.GetChunkData( Chunk.ToChunkPosition(blockX, blockY, blockZ) );
		if( chunk != default(ChunkData) && chunk.GetChunk() != default(Chunk) ) {
			chunk.GetChunk().SetLightDirty();
		}
	}
	
	
	
	
	public static void RecomputeLightAtPosition(Map map, Vector3i pos) {
		int oldSunHeight = map.GetLightmap().GetSunHeight(pos.x, pos.z);
		ComputeSolarLightingAtPosition(map, pos.x, pos.z);
		int newSunHeight = map.GetLightmap().GetSunHeight(pos.x, pos.z);
		
		if(newSunHeight > oldSunHeight) { // brighter
			for(int y=oldSunHeight; y<newSunHeight; y++) {
				RecomputeNearerLights(map, new Vector3i(pos.x, y, pos.z));
			}
		}
		if(newSunHeight < oldSunHeight) { // darker
			for(int y=newSunHeight; y<oldSunHeight; y++) {
				Smooth(map, pos.x, y, pos.z, MAX_LIGHT);
				RecomputeLight(map, new Vector3i(pos.x, y, pos.z));
			}
		}
		
		if(newSunHeight == oldSunHeight) {
			if( map.GetBlock(pos).IsAlpha() ) {
				RecomputeLight(map, pos);
			} else {
				RecomputeNearerLights(map, pos);
			}
		}
	}
	
	private static void RecomputeNearerLights(Map map, Vector3i pos) {
		foreach(Vector3i dir in Vector3i.directions) {
			RecomputeLight(map, pos+dir);
		}
	}
	
	private static void RecomputeLight(Map map, Vector3i pos) {
		BlockData block = map.GetBlock(pos);
		if(!block.IsEmpty()) SetLightDirty(map, pos.x, pos.y, pos.z);
		if(!block.IsAlpha()) return;
		
		Lightmap lightmap = map.GetLightmap();
		byte oldLight = lightmap.GetLight(pos);
		byte newLight = ComputeLight(map, pos);
		if(oldLight != newLight) {
			lightmap.SetLight(newLight, pos);
			foreach(Vector3i dir in Vector3i.directions) {
				RecomputeLight(map, pos+dir);
			}
		}
	}
	
	private static byte ComputeLight(Map map, Vector3i pos) {
		Lightmap lightmap = map.GetLightmap();
		BlockData block = map.GetBlock(pos);
		if( !block.IsAlpha() ) return MIN_LIGHT;
		if( lightmap.IsSunLight(pos.x, pos.y, pos.z) ) return MAX_LIGHT;
		
		int light = MIN_LIGHT;
		foreach(Vector3i dir in Vector3i.directions) {
			int newLight = lightmap.GetLight( pos+dir ) - STEP_LIGHT;
			light = light>newLight ?light : newLight;
			if(light == MAX_LIGHT-STEP_LIGHT) return (byte) light;
		}
		return (byte) light;
	}
}
}
