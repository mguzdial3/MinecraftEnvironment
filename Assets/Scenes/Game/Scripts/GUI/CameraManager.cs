using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public Camera playerCamera, godCamera;
	private bool m_onPlayerCamera=true;

	// Use this for initialization
	void Start () {
		CameraActivation ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			m_onPlayerCamera=	!m_onPlayerCamera;

			CameraActivation();
		}
	}

	private void CameraActivation(){
		playerCamera.enabled = m_onPlayerCamera;
		godCamera.enabled = !m_onPlayerCamera;
	}
}
