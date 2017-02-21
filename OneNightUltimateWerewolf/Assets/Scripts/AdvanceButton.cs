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
		GameManager gm = gameManager.GetComponent<GameManager> ();
		if (GameManager.gamePhase == GameManager.GamePhase.idlePhase) {
			gm.handlePhase ();
		} else if (GameManager.gamePhase == GameManager.GamePhase.dealPhase) {
			gm.handlePhase ();
		} else if (GameManager.gamePhase == GameManager.GamePhase.nightPhase) {
			gm.handlePhase ();
		}
	}
}
