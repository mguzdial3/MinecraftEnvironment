using UnityEngine;
using System.Collections;

public class NPCUnit : MonoBehaviour {
	public static string SAVE_STRING = "NPC";
	public NPCMovementController movementController;
	public NPCAppearanceHandler appearanceController;


	public string GetSaveString(bool hasControl=false, Vector3 playerPos=default(Vector3)){
		string saveString = SAVE_STRING+" " + gameObject.name+" ";
		saveString += movementController.GetSaveString (hasControl,playerPos) + " ";
		saveString += appearanceController.GetSaveString () + "\n";
		return saveString;
	}

	public Color GetColor(){
		return appearanceController.GetColor ();
	}

	public void SetInvisible(){
		appearanceController.SetInvisible ();
		movementController.SetPause (true);
	}
	
	public void SetVisible(){
		appearanceController.SetVisible ();
		movementController.SetPause (false);
	}

	public Vector3 GetGoal(){
		return movementController.GetGoal ();
	}

	public void SetCurrGoal(Vector3 goal){
		movementController.SetCurrGoal (goal);
	}

	void OnTriggerEnter(Collider other){
		transform.localScale = new Vector3(0.1f,0.1f,0.1f);
	}

}
