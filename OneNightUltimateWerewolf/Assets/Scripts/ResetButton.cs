using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using TouchScript.Gestures;

public class QuitButton : MonoBehaviour {

	void OnEnable() {
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
		SceneManager.LoadScene ("Main");
	}
}
