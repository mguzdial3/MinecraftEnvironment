using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;


namespace Environment{
public class MapGenerator  {
	public Map map;
	private Grid<Chunk2D> map2D = new Grid<Chunk2D>();
	private TerrainGenerator terrainGenerator;
	private SubmarineGenerator submarineGenerator;

	private const int ROOM_SIZE=2;//radius
	
	public MapGenerator(BlockSet blockset) {
		map = new Map (blockset);
		terrainGenerator = new TerrainGenerator(map);
		submarineGenerator = new SubmarineGenerator (map);

	}

	public void SpawnMap(Vector3i pos){
		Vector3i current = Chunk.ToChunkPosition( (int)pos.x, (int)pos.y, (int)pos.z );
		Vector3i? nearEmpty = FindNearestEmptyColumn(current.x, current.z, 7);
		
		if(nearEmpty.HasValue) {
			int cx = nearEmpty.Value.x;
			int cz = nearEmpty.Value.z;
			GenerateColumn(cx, cz);
			LightComputer.Smooth(map, cx, cz);
			BuildColumn(cx, cz);
		}

	}

	public void SpawnSubmarine(){

		//This does the floor
		for (int i =-1*ROOM_SIZE; i<ROOM_SIZE; i++) {
			for (int j =-1*ROOM_SIZE; j<ROOM_SIZE; j++) {
				Vector3i current = Chunk.ToChunkPosition(i, 0,j );

				int cx = current.x;
				int cz = current.z;

				GenerateSubFloor(cx,cz);
				LightComputer.Smooth(map, cx, cz);
				BuildColumn(cx,cz);

			}		
		}

		//Walls
		for (int i = -Chunk.SIZE_X; i<Chunk.SIZE_X; i++) {
			submarineGenerator.GenerateWall(i,-Chunk.SIZE_Z);
			submarineGenerator.GenerateWall(i,Chunk.SIZE_Z-1);

			if(Math.Abs(i)!=8){
				submarineGenerator.GenerateWall(i,0);
			}
		}

		for (int i = -Chunk.SIZE_Z; i<Chunk.SIZE_Z; i++) {
			submarineGenerator.GenerateWall(-Chunk.SIZE_X,i);
			submarineGenerator.GenerateWall(Chunk.SIZE_X-1,i);

			if(Math.Abs(i)!=8){
				submarineGenerator.GenerateWall(0,i);
			}
		}

		//Generate some random radioactivity
		int numRandomRadioactive = 8;

		while (numRandomRadioactive>0) {
			int x = UnityEngine.Random.Range(-Chunk.SIZE_X,Chunk.SIZE_X);
			int z = UnityEngine.Random.Range(-Chunk.SIZE_Z,Chunk.SIZE_Z);

			submarineGenerator.GenerateRadioactive(x,1,z);

			numRandomRadioactive--;	
		}


	}
	
	private Vector3i? FindNearestEmptyColumn(int cx, int cz, int rad) {
		Vector3i center = new Vector3i(cx, 0, cz);
		Vector3i? near = null;
		for(int z=cz-rad; z<=cz+rad; z++) {
			for(int x=cx-rad; x<=cx+rad; x++) {
				Vector3i current = new Vector3i(x, 0, z);
				int dis = center.DistanceSquared( current );
				if(dis > rad*rad) continue;
				if( GetChunk2D(x, z).built ) continue;
				if(!near.HasValue) {
					near = current;
				} else {
					int oldDis = center.DistanceSquared(near.Value);
					if(dis < oldDis) near = current;
				}
			}
		}
		return near;
	}
	
	
	private void GenerateColumn(int cx, int cz) {
		if( GetChunk2D(cx, cz).genereted ) return;
		terrainGenerator.Generate(cx, cz);
		LightComputer.ComputeSolarLighting(map, cx, cz);
		GetChunk2D(cx, cz).genereted = true;
	}

	private void GenerateSubFloor(int cx, int cz) {
		if( GetChunk2D(cx, cz).genereted ) return;
		submarineGenerator.GenerateFloor(cx, cz);
		LightComputer.ComputeSolarLighting(map, cx, cz);
		GetChunk2D(cx, cz).genereted = true;
	}

	public void CheckGeneration(int cx, int cz){
		if( GetChunk2D(cx, cz).genereted ) return;
		LightComputer.ComputeSolarLighting(map, cx, cz);
		GetChunk2D(cx, cz).genereted = true;

	}
	
	public void BuildColumn(int cx, int cz) {
		map.BuildColumn(cx, cz);
		GetChunk2D(cx, cz).built = true;
	}
	
	private Chunk2D GetChunk2D(int x, int z) {
		Chunk2D chunk = map2D.SafeGet(x, 0, z);
		if(chunk == null) {
			chunk = new Chunk2D();
			map2D.AddOrReplace(chunk, x, 0, z);
		}
		return chunk;
	}
	
}

public class Chunk2D {
	public bool genereted = false;
	public bool built = false;
}
}