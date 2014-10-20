using UnityEngine;
using System.Collections.Generic;

public class BlockSet : MonoBehaviour {
	
	[SerializeField] private Atlas[] atlases = new Atlas[0];
	
	[SerializeField] private List<Cube> cubes = new List<Cube>();
	[SerializeField] private List<Cross> crosses = new List<Cross>();
	
	private Material[] materials;
	
	public void OnEnable() {
		//Debug.Log("BlockSet "+name+": Materials="+atlases.Length+" Blocks="+GetCount());
		
		materials = new Material[atlases.Length];
		for(int i=0; i<materials.Length; i++) {
			materials[i] = atlases[i].GetMaterial();
		}
		
		for(int i=0; i<GetCount(); i++) {
			GetBlock(i).Init(this, i);
		}
	}
	
	public void SetAtlases(Atlas[] atlases) {
		this.atlases = atlases;
	}
	public Atlas[] GetAtlases() {
		return atlases;
	}
	
	
	public int Add(Block block) {
		if(block is Cube) {
			cubes.Add( (Cube)block );
			return cubes.Count-1;
		}
		if(block is Cross) {
			crosses.Add( (Cross)block );
			return cubes.Count + crosses.Count-1;
		}
		return -1;
	}
	
	public void Remove(int index) {
		if(index >= 0 && index < cubes.Count) {
			cubes.RemoveAt(index);
			return;
		}
		index -= cubes.Count;
		if(index >= 0 && index < crosses.Count) {
			crosses.RemoveAt(index);
			return;
		}
	}
	
	public Block GetBlock(int index) {
		if(index >= 0 && index<cubes.Count) return cubes[index];
		index -= cubes.Count;
		if(index >= 0 && index < crosses.Count) return crosses[index];
		return null;
	}
	
	public Block GetBlock(string name) {
		foreach(Block block in cubes) {
			if(block.GetName() == name) return block;
		}
		foreach(Block block in crosses) {
			if(block.GetName() == name) return block;
		}
		return null;
	}

	public string[] GetStringArray(){
		string[] toReturn = new string[cubes.Count+crosses.Count];
		int index = 0;
		foreach(Block block in cubes) {
			toReturn[index] = block.GetName();
			index++;
		}
		foreach(Block block in crosses) {
			toReturn[index] = block.GetName();
			index++;
		}

		return toReturn;
	}
	
	public int GetCount() {
		return cubes.Count + crosses.Count;
	}
	
	public Material[] GetMaterials() {
		return materials;
	}
	
}
