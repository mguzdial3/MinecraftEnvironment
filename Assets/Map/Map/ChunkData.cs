using System.Collections;
using System;

namespace Environment{
public class ChunkData {
	private BlockData[][][] blocks = new BlockData[Chunk.SIZE_Z][][];
	private Vector3i position;
	private Chunk chunk;
	
	public ChunkData( Vector3i position) {
		this.position = position;
	}
	
	public Chunk GetChunkInstance() {
		if(chunk == null) chunk = Chunk.CreateChunk(position, this);
		return chunk;
	}

	public Chunk GetChunk() {
		return chunk;
	}
	
	public void SetBlock(BlockData block, Vector3i pos) {
		SetBlock(block, pos.x, pos.y, pos.z);
	}
	
	public void SetBlock(BlockData block, int x, int y, int z) {
		if (blocks [z] == null) {
			blocks[z] = new BlockData[Chunk.SIZE_Y][];
		}
		if (blocks [z][y] == null) {
			blocks[z][y] = new BlockData[Chunk.SIZE_X];		
		}

		blocks[z] [y] [x] = block;

		Map.Instance.GetLightmap().SetLight(LightComputer.MAX_LIGHT, position, new Vector3i(x, y, z));
	}

	public BlockData GetBlock(Vector3i pos) {
		return GetBlock(pos.x, pos.y, pos.z);
	}

	public BlockData GetBlock(int x, int y, int z) {
		if (blocks [z] == null || blocks [z][y] == null ) {
			return default(BlockData);
		}
		return blocks[z] [y] [x];
	}

	public Vector3i GetPosition() {
		return position;
	}

	public override string ToString (){
		string singleString = "";

		for(int x = 0; x<Chunk.SIZE_X; x++){
			for(int y = 0; y<Chunk.SIZE_Y; y++){
				for(int z = 0; z<Chunk.SIZE_Z; z++){
					BlockData blockData = GetBlock(x,y,z);

					if(blockData.block!=null){
						singleString+= Array.IndexOf(TerrainGenerator.blockNames,blockData.block.GetName())+" ";
					}
					else{
						singleString+= "-1 ";
					}

				}
			}
		}


		return singleString;
	}
}
}
