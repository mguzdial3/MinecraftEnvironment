namespace Environment{
public class SubmarineGenerator {
	private Map map;


	private const int floorBlock = 0;
	private const int wallBlock1 = 2;
	private const int wallBlock2 = 2;
	private const int radioactive = 4;
	private const int radioactivity = 5;

	private int radioactiveRadius = 2;
	public SubmarineGenerator(Map map) {
		this.map = map;
		
	}
	
	public void GenerateFloor(int cx, int cz) {
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				int worldX = cx*Chunk.SIZE_X+x;
				int worldZ = cz*Chunk.SIZE_Z+z;
					map.SetBlock (floorBlock, new Vector3i (worldX, 1, worldZ));
			}		
		}
	}

	public void GenerateWall(int cx, int cz) {
		map.SetBlock (wallBlock1, new Vector3i (cx, 2, cz));
		map.SetBlock (wallBlock2, new Vector3i (cx, 3, cz));
		map.SetBlock (wallBlock2, new Vector3i (cx, 4, cz));
	}




	public void GenerateRadioactive(int x, int y, int z, int deep=0) {
		map.SetBlock(radioactive, x, y, z);
		
		for(int i = x-radioactiveRadius; i<x+1+radioactiveRadius; i++){
			for(int j = z-radioactiveRadius; j<z+1+radioactiveRadius; j++){
				if((i!=x || j!=z) && ((!map.CheckEquivalentBlocks(radioactive,i,y,j)))){
					map.SetBlock(radioactivity, i, y, j);
				}
			}
		}

	}

	
}
}

