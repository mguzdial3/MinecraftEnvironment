using UnityEngine;
using System.Collections;
using Environment;

public class Builder : MonoBehaviour {
	
	private Transform cameraTrans;
	private Block selectedBlock;

	private float _distanceOfRaycast=30f;
	
	void Awake() {
		cameraTrans = transform.GetComponentInChildren<Camera>().transform;
	}
	
	public void SetSelectedBlock(Block block) {
		selectedBlock = block;
	}

	public Block GetSelectedBlock() {
		return selectedBlock;
	}

	public NPCMovementController TryGetSelectedNPC(){
		RaycastHit hit;
		NPCMovementController npcMovement = null;
		if(Physics.Raycast(cameraTrans.position,cameraTrans.forward,out hit, _distanceOfRaycast)){//TODO; change number to variable
			if(hit.collider!=null){
				npcMovement = hit.collider.GetComponent<NPCMovementController>();
			}
		}

		return npcMovement;
	}

	public Vector3 GetGoalPositionFromLook(){
		RaycastHit hit;

		if(Physics.Raycast(cameraTrans.position,cameraTrans.forward,out hit,_distanceOfRaycast)){//TODO; change number to variable
			return hit.point;
		}

		return default(Vector3);
	}
	
	// Update is called once per frame
	void Update () {
		if(Screen.showCursor) return;
		
		if( Input.GetKeyDown(KeyCode.LeftControl) ) {
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				byte light = Map.Instance.GetLightmap().GetLight(point.Value.x, point.Value.y, point.Value.z);
			}
		}
		
		if( Input.GetKeyDown(KeyCode.RightControl) ) {
			Vector3i? point = GetCursor(true);
			if(point.HasValue) {
				byte light = Map.Instance.GetLightmap().GetLight(point.Value.x, point.Value.y, point.Value.z);
			}
		}
		
		if( Input.GetMouseButtonDown(0) ) {
			Vector3i? point = GetCursor(true);
			if(point.HasValue) 
				Map.Instance.SetBlockAndRecompute(new BlockData(), point.Value);
		}
		
		if( Input.GetMouseButtonDown(1) ) {
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				BlockData block = new BlockData( selectedBlock );
				block.SetDirection( GetDirection(-transform.forward) );
				Map.Instance.SetBlockAndRecompute(block, point.Value);
			}
		}
		
	}
	
	void OnDrawGizmos() {
		if(!Application.isPlaying) return;
		Vector3i? cursor = GetCursor(true);
		if(cursor.HasValue) {
			Gizmos.DrawWireCube( new Vector3(cursor.Value.x,cursor.Value.y,cursor.Value.z), Vector3.one*1.05f );
		}
	}
	
	private Vector3i? GetCursor(bool inside) {
		Ray ray = new Ray(cameraTrans.position, cameraTrans.forward);
		Vector3? point =  RayBoxCollision.Intersection(Map.Instance, ray, 10);
		if( point.HasValue ) {
			Vector3 pos = point.Value;
			if(inside) pos += ray.direction*0.01f;
			if(!inside) pos -= ray.direction*0.01f;
			int posX = Mathf.RoundToInt(pos.x);
			int posY = Mathf.RoundToInt(pos.y);
			int posZ = Mathf.RoundToInt(pos.z);
			return new Vector3i(posX, posY, posZ);
		}
		return null;
	}

	private static BlockDirection GetDirection(Vector3 dir) {
		if( Mathf.Abs(dir.z) >= Mathf.Abs(dir.x) ) {
			// 0 или 180
			if(dir.z >= 0) return BlockDirection.Z_PLUS;
			return BlockDirection.Z_MINUS;
		} else {
			// 90 или 270
			if(dir.x >= 0) return BlockDirection.X_PLUS;
			return BlockDirection.X_MINUS;
		}
	}

}
