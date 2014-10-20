using UnityEngine;
using System.Collections;
using Environment;

public class PauseGUI : MonoBehaviour {
	public SnapshotHandler snapshotHandler;
	private string saveText="";
	private string numberText="1";

	void OnResume() {
		enabled = false;
		snapshotHandler.SetSaveAllowed (true);
	}
	
	void OnPause() {
		enabled = true;
		snapshotHandler.SetSaveAllowed (false);
	}

	void OnGUI() {
		GUILayout.BeginArea( new Rect(0, 0, Screen.width, Screen.height) );
		GUILayout.FlexibleSpace();
			DrawResumeButton();
			DrawTextField ();
			DrawSaveButton ();
			DrawVCRField ();
			DrawHelpText();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
	
	private void DrawResumeButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Resume", GUILayout.ExpandWidth(false))) {
			SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void DrawSunSlider() {
		const float min = (float) LightComputer.MIN_LIGHT / LightComputer.MAX_LIGHT;
		const float max = 1;
		float light = RenderSettings.ambientLight.r;
		
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label("Sun");
				light = GUILayout.HorizontalSlider( light, min, max, GUILayout.Width(Screen.width/2f) );
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		light = Mathf.Clamp(light, min, 1f);
		RenderSettings.ambientLight = new Color(light, light, light, 1f);
	}
	
	private void DrawHelpText() {
		string text = "Current Snapshot Number: "+snapshotHandler.GetCurrentSnapshotNumber()+"\n" +
						"E - Open the inventory";
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Box(text, GUILayout.ExpandWidth(false));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void DrawQuitButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Quit", GUILayout.ExpandWidth(false))) {
			Application.Quit();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void DrawSaveButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Save", GUILayout.ExpandWidth(false))) {
			snapshotHandler.SpecialSave(saveText);
		}

		if(GUILayout.Button("Load", GUILayout.ExpandWidth(false))) {
			snapshotHandler.SpecialLoad(saveText);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void DrawTextField() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		saveText = GUILayout.TextField (saveText, GUILayout.Width(90));

		if (saveText.Contains (" ")) {
			saveText=saveText.Substring(0,saveText.IndexOf(' '));		
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void DrawVCRField() {

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if(GUILayout.Button("<<", GUILayout.ExpandWidth(false))) {
			int currentSnapshotNumber = snapshotHandler.GetCurrentSnapshotNumber();
			int change = int.Parse(numberText);

			if(currentSnapshotNumber-change>0){
				int lowerBound = currentSnapshotNumber-change;
				while(currentSnapshotNumber>lowerBound){
					if(snapshotHandler.JustGetMapNumber(""+currentSnapshotNumber)){
						snapshotHandler.ReverseMapUpdate();//Reverse the map update since there were changes on this line
					}

					currentSnapshotNumber--;
				}

				snapshotHandler.SetCurrentSnapshotNumber(currentSnapshotNumber);
				snapshotHandler.SetCurrentMapNumber(snapshotHandler.GetCurrMap());
				snapshotHandler.LoadNPCS(""+currentSnapshotNumber);
			}
		}

		string newNumberText = GUILayout.TextField (numberText, GUILayout.Width(30));
		int result=0; 
		if (int.TryParse(newNumberText,out result)) {
			numberText = newNumberText;		
		}

		if(GUILayout.Button(">>", GUILayout.ExpandWidth(false))) {
			int currentSnapshotNumber = snapshotHandler.GetCurrentSnapshotNumber();
			int change = int.Parse(numberText);

			int higherBound = currentSnapshotNumber+change;
			while(currentSnapshotNumber<higherBound){
				if(snapshotHandler.JustGetMapNumber(""+currentSnapshotNumber)){
					snapshotHandler.MapUpdate();
				}
				
				
				currentSnapshotNumber++;
			}
			snapshotHandler.SetCurrentSnapshotNumber(currentSnapshotNumber);
			snapshotHandler.SetCurrentMapNumber(snapshotHandler.GetCurrMap());
			snapshotHandler.LoadNPCS(""+currentSnapshotNumber);
			
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

	}

}
