
namespace Environment{
public class TreeGenerator {
	
	private Map map;
	
	private Block trunkTree;
	private Block leaves;
	
	public TreeGenerator(Map map) {
		this.map = map;
		BlockSet blockSet = map.GetBlockSet();
		trunkTree = blockSet.GetBlock("Trunk Tree");
		leaves = blockSet.GetBlock("Leaves");

		//Generate (0, 0);
	}
	
	
	public void Generate(int x, int z) {
		int y = GetMaxY(x, z);
		Generate(x, y+18, z, 0);
		for(int i=0; i<20; i++) {
			map.SetBlock( new BlockData(trunkTree), x, y+i, z );
		}
	}
	
	private void Generate(int x, int y, int z, int deep) {
		if(deep > 6) return;
		deep++;
		if(map.GetBlock(x, y, z).IsEmpty()) {
			map.SetBlock(new BlockData(leaves), x, y, z);
			Generate(x-1, y, z, deep);
			Generate(x+1, y, z, deep);
			

			Generate(x, y+1, z, deep);
			
			Generate(x, y, z-1, deep);
			Generate(x, y, z+1, deep);
		}
	}
	
	private int GetMaxY(int x, int z) {
		int maxY = Map.maxChunkY * Chunk.SIZE_Y;
		for(int y=maxY; y>=0; y--) {
			if(!map.GetBlock(x, y, z).IsEmpty()) return y;
		}
		return 0;
	}
	
}
}
