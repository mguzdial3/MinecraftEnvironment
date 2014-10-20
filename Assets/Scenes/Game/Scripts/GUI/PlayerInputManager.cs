using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour {
	private const string START_OF_TEXT ="/Users/matthewguzdial/MinecraftClone/PlayerPolicyFeedback/policy";
	private const string END_OF_TEXT =".txt";
	private string changeList;

	public string feedbackDescriptor = "Test";

	public PolicyFollower policyFollower;

	void Start(){
		if(policyFollower==null){
			Debug.LogError("Policy Follower was NULL. FIX IT.");
		}
		changeList ="";
	}


	// Use this for GUI display because its easy
	void OnGUI () {
		GUI.skin.button.fontSize = 30;
		if(GUI.Button(new Rect(Screen.width-100,0,100,100),"+")){
			changeList+="CurrState: "+policyFollower.GetCurrentIndex()+". CurrAction: "+policyFollower.GetCurrentAction()+". Feedback: Yes\n";
		}

		if(GUI.Button(new Rect(Screen.width-100,100,100,100),"-")){
			changeList+="CurrState: "+policyFollower.GetCurrentIndex()+". CurrAction: "+policyFollower.GetCurrentAction()+". Feedback: No\n";
		}


	}

	void OnDestroy(){
		System.IO.File.WriteAllText(START_OF_TEXT+feedbackDescriptor+END_OF_TEXT, changeList);
	}
}
