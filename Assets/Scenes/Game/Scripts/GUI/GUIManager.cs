using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	private static GUIManager _guiManager;
	public static GUIManager guimanager {
		get {
			if(_guiManager == null) _guiManager = (GUIManager) GameObject.FindObjectOfType( typeof(GUIManager) );
			return _guiManager;
		}
	}
	
	public static bool IsPause {
		get {
			return Time.timeScale <= 0.1f;
		}
	}
	public static bool IsPlaying {
		get {
			return !IsPause;
		}
	}

	// Use this for initialization
	void Start () {
		//Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			if(IsPause) {
				SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
			} else {
				SendMessage("OnPause", SendMessageOptions.DontRequireReceiver);
			}
		}
		if(!Screen.showCursor) Screen.lockCursor = true;
	}
	
	void OnResume() {
		//Screen.showCursor = false;
		//Screen.lockCursor = true;
		Time.timeScale = 1f;
	}
	
	void OnPause() {
		//Screen.showCursor = true;
		//Screen.lockCursor = false;
		Time.timeScale = 1f/10000f;
	}
	
}
