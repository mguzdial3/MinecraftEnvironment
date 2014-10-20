using System.Collections;


namespace Environment{
public class TerrainGenerator {
	
	private const int WATER_LEVEL = 1;
	private const int ICE_LEVEL = 22;
	
	private PerlinNoise2D noise1 = new PerlinNoise2D(1/150f).SetOctaves(5);
	private PerlinNoise2D noise2 = new PerlinNoise2D(1/150f).SetOctaves(2);
	private PerlinNoise3D noise3d = new PerlinNoise3D(1/30f);
	
	private Map map;

	//Name
	public static string[] blockNames;
	public static Block[] blocks;


	public TerrainGenerator(Map map) {
		this.map = map;
		BlockSet blockSet = map.GetBlockSet();
		blockNames = blockSet.GetStringArray ();
		blocks = new Block[blockNames.Length];

		for (int i = 0; i<blockNames.Length; i++) {
			blocks[i] = blockSet.GetBlock(i);		
		}

	}
	
	public void Generate(int cx, int cz) {
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				int worldX = cx*Chunk.SIZE_X+x;
				int worldZ = cz*Chunk.SIZE_Z+z;

				int h1 = (int) (noise1.Noise(worldX, worldZ)*70);//Change this to constant to make a constant map
				if(h1<0){
					h1*=-1;	
				}

				h1 = (h1)<5? 5: (h1);
				h1 = (h1)>300? 300: h1;
				
				int h2 = (int) (noise2.Noise(worldX, worldZ)*40);//Change this to constant to make a constant map
				h2 = h2<0 ? 0: h2;
				h2 = 200>h2 ? 200: h2;
				h2 += h1;
				
				int deep = 0;
				int worldY = h2;
				for(; worldY>h1; worldY--) {
					if(noise3d.Noise(worldX, worldY, worldZ) < 0) {
						GenerateBlock(worldX, worldY, worldZ, deep);
						deep++;
					} else {
						deep = 0;
					}
				}
				
				//int down = h1 - Chunk.SIZE_Y;
				for(; worldY>=0; worldY--) {
					GenerateBlock(worldX, worldY, worldZ, deep);
					deep++;
				}
				
			}
		}
	}
	
	private void GenerateBlock(int worldX, int worldY, int worldZ, int deep) {
		Block block = GetBlock(worldX, worldY, worldZ, deep);
		if(block != null) map.SetBlock(new BlockData(block), worldX, worldY, worldZ);
	}
	
	private Block GetBlock(int worldX, int worldY, int worldZ, int deep) {
		if(deep == 0) return blocks[0];
		if(deep <= 5) return blocks[1];
		return blocks[2];
	}

	private Block GetBlockFancy(int worldX, int worldY, int worldZ, int deep) {
		if(worldY == WATER_LEVEL+1) return blocks[3];
		if (worldY <= WATER_LEVEL) return blocks[0];
		if(worldY>ICE_LEVEL && deep==0) return blocks[5];
		if(deep == 0) return blocks[1];
		if(deep <= 5 && deep>0) return blocks[2];
		return blocks[4];
	}
	
}
}

