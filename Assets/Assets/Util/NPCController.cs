using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {
	public static string SAVE_STRING = "NPC Enemies List ";

	public GameObject npcPrefab;
	public int numToSpawn=30;

	private NPCUnit[] npcs;
	private int controlledUnit=-1;

	public Builder player;

	private bool takeOver;

	public GameObject goalIndicator;

	void Start(){
		goalIndicator.SetActive (false);
	}

	void OnGUI(){
		if (controlledUnit != -1) {
			GUI.color = npcs[controlledUnit].GetColor();
			GUI.Label(new Rect (0,30,30,30),""+controlledUnit);	
			GUI.Label(new Rect(0,50,100,30), "Praise: R");
			GUI.Label(new Rect(0,70,100,30), "Critique: F");
			GUI.Label(new Rect(0,90,100,30), "Take Over: V");
			GUI.Label(new Rect(0,110,100,30), "Place Goal: G");
			GUI.Label(new Rect(0,130,100,30), "Leave: Z");
			GUI.color = Color.white;
		}
	}

	void Update(){
		if (controlledUnit != -1) {
			goalIndicator.SetActive(true);
			goalIndicator.transform.position = npcs[controlledUnit].GetGoal();

			if(Input.GetKey(KeyCode.Z)){
				if(takeOver){
					npcs[controlledUnit].transform.position=player.transform.position+Vector3.up*2;
					npcs[controlledUnit].transform.rotation=player.transform.rotation;
					npcs[controlledUnit].SetVisible();
					takeOver=false;
				}
				goalIndicator.SetActive(false);
				controlledUnit=-1;
			}

			if(Input.GetKey(KeyCode.R)){
				//GOOD SIGNAL
				npcs[controlledUnit].transform.position+=Vector3.up; //Placeholder for now
			}

			if(Input.GetKey(KeyCode.F)){
				//Bad SIGNAL
				npcs[controlledUnit].transform.position+=Vector3.down; //Placeholder for now
			}

			if(Input.GetKey(KeyCode.G)){
				//Change Goal Position
				Vector3 goalPos = player.GetGoalPositionFromLook();
				if(goalPos!=default(Vector3)){
					goalIndicator.transform.position = goalPos;
					npcs[controlledUnit].SetCurrGoal(goalPos);
				}

			}

			if(Input.GetKey(KeyCode.V) && controlledUnit!=-1){
				//Take Over
				takeOver=true;

				player.transform.position = npcs[controlledUnit].transform.position;
				player.transform.rotation = npcs[controlledUnit].transform.rotation;
				npcs[controlledUnit].SetInvisible();
			}

		}
		else{
			//if(Input.GetMouseButtonDown(0)){
			//	SetControlledUnit(player.TryGetSelectedNPC());
			//}
		}
	}

	// Use this for initialization
	public void Init () {
		npcs = null;

		npcs= new NPCUnit[numToSpawn];
		for (int i = 0; i<numToSpawn; i++) {
			GameObject go = (GameObject)Instantiate(npcPrefab);
			
			NPCMovementController npc = go.GetComponent<NPCMovementController>();
			NPCAppearanceHandler npcA = go.GetComponent<NPCAppearanceHandler>();
			
			if(npcA!=null){
				npcA.Init();
			}
			
			if(npc!=null){
				npc.Init();
				
				//Naming for ease
				npc.gameObject.name = i.ToString("0000"); //assuming we have less than 10,000
				npcs[i] = npc.GetComponent<NPCUnit>();
			}
		}
	}

	//Load Info Stuff//
	public void CreateNPCArray(int length){
		npcs = new NPCUnit[length];
		numToSpawn = length;
	}

	public void AddNPC(int npcIndex, Vector3 position, Vector3[] path, int colorIndex){
		GameObject go = (GameObject)Instantiate(npcPrefab);
		NPCMovementController npc = go.GetComponent<NPCMovementController>();

		if(npc!=null){
			npc.InitFromSave(position,path);

			//Naming for ease
			npc.gameObject.name = npcIndex.ToString("0000"); //assuming we have less than 10,000
			npcs[npcIndex] = npc.GetComponent<NPCUnit>();
		}

		NPCAppearanceHandler npcA = go.GetComponent<NPCAppearanceHandler>();

		if (npcA != null) {
			npcA.InitFromSave(colorIndex);		
		}
	}

	public void DestroyAllNPCs(){
		for (int i = 0; i<npcs.Length; i++) {
			if(npcs[i]!=null){
				Destroy (npcs[i].gameObject);
			}
		}
	}

	public void SetControlledUnit(NPCMovementController go){

		if (go != null) {
			int index = 0;

			while(index<npcs.Length && controlledUnit==-1){
				if(npcs[index] !=null && go.gameObject==npcs[index].gameObject){
					controlledUnit = index;
				}

				index++;
			}
		}
	}

	public bool CanSetControlledUnit(){
		return controlledUnit == -1;
	}

	//Public Save String Stuff//
	public bool CanSave(){
		return npcs != null;
	}

	public string GetSaveString(){
		string saveString = "";
		if(npcs!=null){
			saveString+=SAVE_STRING+ numToSpawn+"\n";

			for (int i = 0; i<numToSpawn; i++) {
				saveString+=npcs[i].GetSaveString(takeOver && i==controlledUnit,player.transform.position);		
			
			}
		}
		return saveString;
	}


}
