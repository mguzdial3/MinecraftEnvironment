
namespace Environment{
public enum BlockDirection : byte {
	Z_PLUS,
	X_PLUS,
	Z_MINUS,
	X_MINUS
}

public struct BlockData {
	
	public Block block;
	private BlockDirection rotation;
	
	public BlockData(Block block) {
		this.block = block;
		rotation = BlockDirection.Z_PLUS;
	}
	
	public void SetDirection(BlockDirection rotation) {
		this.rotation = rotation;
	}
	public BlockDirection GetDirection() {
		return rotation;
	}
	
	public byte GetLight() {
		if(block != null) return block.GetLight();
		return LightComputer.MIN_LIGHT;
	}
	
	public bool IsEmpty() {
		return block == null;
	}
	
	public bool IsAlpha() {
		return IsEmpty() || block.IsAlpha();
	}
	
}
}