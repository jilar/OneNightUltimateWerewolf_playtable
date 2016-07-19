using UnityEngine;
using System;
using System.Collections;
using TouchScript.Gestures;

public class AdvanceButton : MonoBehaviour {
	
	public GameObject gameManager;

	void OnEnable() {
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
		Debug.LogError ("Tapped adv");
		GameManager gm = gameManager.GetComponent<GameManager> ();
		gm.handlePhase ();
	}
}
