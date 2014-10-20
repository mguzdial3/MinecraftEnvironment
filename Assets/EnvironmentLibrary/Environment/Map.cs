using System.Collections;
using System.Collections.Generic;
using System;

namespace Environment{

	public class Map {
		
		public const int maxChunkY = 16;
		public const int maxBlockY = maxChunkY * Chunk.SIZE_Y;
		public static string SAVE_STRING = "Map";
		public static string CHUNK_SAVE_STRING = "Chunk";
		
		private BlockSet blockSet;
		private Grid<ChunkData> chunks = new Grid<ChunkData>();
		private Lightmap lightmap = new Lightmap();

		private List<string> changeList;
		public static Map Instance;

		private int[,,] intMap;
		public readonly int xSize = 40;
		public readonly int ySize = 10;
		public readonly int zSize = 40;
		public readonly int xMin = -20;
		public readonly int yMin = 0;
		public readonly int zMin = -20;
		



		public Map(BlockSet _blockSet) {
			ChunkBuilder.Init( _blockSet.GetMaterials().Length );
			this.blockSet = _blockSet;

			if (Instance == null) {
				Instance = this;		
			}
			changeList = new List<string> ();
			
			SetUpIntMap ();

		}

		public bool CanSave(){
			return chunks != null;
		}

		public bool HasReachedBounds(){
			return chunks.HasReachedEdge ();
		}

		public bool HasChangeString(){
			return changeList.Count != 0;
		}

		public string GetChangeString(){
			string changeString = "";

			foreach (string change in changeList) {
				changeString+= change+"\n";
			}

			changeList.Clear ();

			return changeString;
		}

		public void ClearList(){
			changeList.Clear ();
		}

		public int GetMinX(){
			return chunks.GetMinX ();
		}

		public int GetMinZ(){
			return chunks.GetMinZ ();
		}

		public int GetMaxX(){
			return chunks.GetMaxX ();
		}

		public int GetMaxZ(){
			return chunks.GetMaxZ ();
		}

		public string GetSaveString(){
			string saveString = SAVE_STRING+"\n";//Let's reader know we started the map section
			saveString += chunks.GetMinX () + " " + chunks.GetMinY () + " " + chunks.GetMinZ ()+" ";
			saveString += chunks.GetMaxX () + " " + chunks.GetMaxY () + " " + chunks.GetMaxZ (); //Get size of chunk grid

			saveString += "\n";
			for(int x = chunks.GetMinX(); x<=chunks.GetMaxX(); x++){
				for(int y = chunks.GetMinY(); y<=chunks.GetMaxY(); y++){
					for(int z = chunks.GetMinZ(); z<=chunks.GetMaxZ(); z++){

						ChunkData chunkData = GetChunkData(new Vector3i(x,y,z));
						saveString+=CHUNK_SAVE_STRING+" "+x+" "+y+" "+z+"\n"; //We are on a new chunk at this location

						if(chunkData!=default(ChunkData)){
							saveString+=chunkData.ToString();
						}
					}
				}
			}
		
			return saveString;
		}
		
		public void SetBlockAndRecompute(BlockData block, Vector3i pos) {
			SetBlock( block, pos );
			
			Build( Chunk.ToChunkPosition(pos) );
			foreach( Vector3i dir in Vector3i.directions ) {
				Build( Chunk.ToChunkPosition(pos + dir) );
			}
			LightComputer.RecomputeLightAtPosition(this, pos);
		}
		
		public void BuildColumn(int cx, int cz) {
			for(int cy=chunks.GetMinY(); cy<chunks.GetMaxY(); cy++) {
				Build( new Vector3i(cx, cy, cz) );
			}
		}

		private void Build(Vector3i pos) {
			ChunkData chunk = GetChunkData( pos );
			if(chunk != null) chunk.GetChunkInstance().SetDirty();
		}
		
		private ChunkData GetChunkDataInstance(Vector3i pos) {
			if(pos.y < 0) return null;
			ChunkData chunk = GetChunkData(pos);
			if(chunk == null) {
				chunk = new ChunkData(pos);
				chunks.AddOrReplace(chunk, pos);
			}
			return chunk;
		}

		private bool HasBlockSet(){
			return blockSet!=null;
		}

		
		//Int Map Stuff
		private void SetBlockIntMap(int blockId, int x, int y, int z){
			int xIndex = x - xMin;
			int yIndex = y - yMin;
			int zIndex = z - zMin;

			intMap [xIndex,yIndex,zIndex] = blockId;
		}

		private void SetUpIntMap(){
			intMap = new int[xSize,ySize,zSize];
			for (int i = 0; i<xSize; i++) {
				for (int j = 0; j<ySize; j++) {		
					for(int k = 0; k<zSize; k++){
						intMap[i,j,k] = -1;
					}
				}
			}
		}

		public int[,] Get2DArray(){
			int [,] twoDArray = new int[xSize, zSize];

			for (int i = 0; i<xSize; i++) {
				for (int j = 0; j<zSize; j++) {		
					int yIndex = ySize-yMin-1;

					bool notFound = true;
					while(yIndex>=0 && notFound){
						if(intMap[i,yIndex,j]!=-1){
							twoDArray[i,j] = intMap[i,yIndex,j];
							notFound=false;
						}
						else{
							yIndex--;
						}
					}

					if(yIndex<0){
						twoDArray[i,j] =-1;
					}
				}
			}
			return twoDArray;
		}

		//Public methods
		public ChunkData GetChunkData(Vector3i pos) {
			return chunks.SafeGet(pos);
		}

		public int GetBlockInt(int x, int y, int z){
			int xIndex = x - xMin;
			int yIndex = y - yMin;
			int zIndex = z - zMin;

			return intMap[xIndex,yIndex,zIndex];
		}

		public void SetBlock(BlockData block, Vector3i pos) {
			SetBlock(block, pos.x, pos.y, pos.z);
		}

		public void SetBlock(int blockIndex, int x, int y, int z){
			if(HasBlockSet()){
				SetBlock(new BlockData(blockSet.GetBlock(blockIndex)),x,y,z);
			}
			else{
				SetBlockIntMap(blockIndex,x,y,z);
			}
		}

		public void SetBlock(int blockIndex, Vector3i pos){
			SetBlock(blockIndex,pos.x,pos.y,pos.z);
		}

		public void SetBlock(BlockData block, int x, int y, int z) {
			//Debug.Log ("SetBlock : " + x + ", " + y + ", " + z);
			ChunkData chunk = GetChunkDataInstance( Chunk.ToChunkPosition(x, y, z) );
			if (chunk != null) {
				if(block.block!=null){
					SetBlockIntMap(Array.IndexOf(TerrainGenerator.blockNames,block.block.GetName()),x,y,z);
					changeList.Add(""+x+" "+y+" "+z+" "+Array.IndexOf(TerrainGenerator.blockNames,block.block.GetName()));
				}
				else{
					SetBlockIntMap(-1,x,y,z);
					BlockData data = GetBlock(x,y,z);

					if(data.block!=null){
						changeList.Add(""+x+" "+y+" "+z+" -"+(Array.IndexOf(TerrainGenerator.blockNames,data.block.GetName())));//So we can have negative zeroes
					}
					else{
						changeList.Add(""+x+" "+y+" "+z+" ERROR");
					}
				}
				chunk.SetBlock (block, Chunk.ToLocalPosition (x, y, z));
			}

		}

		public void SetBlockNoSave(Block block, int x, int y, int z) {
			ChunkData chunk = GetChunkDataInstance( Chunk.ToChunkPosition(x, y, z) );
			if (chunk != null) {
				chunk.SetBlock (new BlockData(block), Chunk.ToLocalPosition (x, y, z));

				Chunk chunkObj =chunk.GetChunkInstance();
				chunkObj.SetDirty();
				//chunkObj.SetLightDirty();

			}
		}

		public void SetBlockAndRecomputeNoSave(int block, int x, int y, int z) {
			SetBlockNoSave( block, x,y,z);

			Vector3i pos = new Vector3i (x, y, z);

			Build( Chunk.ToChunkPosition(pos) );
			foreach( Vector3i dir in Vector3i.directions ) {
				Build( Chunk.ToChunkPosition(pos + dir) );
			}
			LightComputer.RecomputeLightAtPosition(this, pos);
		}

		public void SetBlockNoSave(int block, int x, int y, int z) {
			SetBlockNoSave (GetBlockByIndex (block), x, y, z);
			SetBlockIntMap(block,x,y,z);

		}



		public BlockData GetBlock(Vector3i pos) {
			return GetBlock(pos.x, pos.y, pos.z);
		}


		public Block GetBlockByIndex(int index){
			return (blockSet!=null ?blockSet.GetBlock(index): null);
		}

		 public BlockData GetBlock(int x, int y, int z) {
			ChunkData chunk = GetChunkData( Chunk.ToChunkPosition(x, y, z) );
			if(chunk == null) return default(BlockData);
			return chunk.GetBlock( Chunk.ToLocalPosition(x, y, z) );
		}

		public bool CheckEquivalentBlocks(int blockIndex, int x, int y, int z){
			BlockData blockData = GetBlock(x,y,z);
			return (blockData.block!=null && HasBlockSet() &&  blockData.block.GetName().Equals(blockSet.GetBlock(blockIndex).GetName()));
		}

		public bool IsPositionOpen(Vector3i pos){
			return (GetBlock (pos)).IsEmpty () || GetBlock(pos).IsAlpha();
		}

		public Vector3i getEmptyLOC(Vector3i location){
			while (!IsPositionOpen(location) || ! IsPositionOpen(location-Vector3i.up)) {
				location+=Vector3i.up;
			}

			return location;
		}
		
		public Lightmap GetLightmap() {
			return lightmap;
		}
		
		public BlockSet GetBlockSet() {
			return blockSet;
		}
		
	}
}

