using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TouchScript.Gestures;
using System;

public class Token : MonoBehaviour {

	public int id;
	public string role;
	public Text roleText;
	public Image image;
	public bool selected = false;
	public GameObject card;

	void Awake() {
		GetComponent<Renderer> ().material.color = Color.clear;
	}

	void OnEnable() {
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
		/*
		GameManager gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		Debug.LogError (gm.gamePhase);
		if (gm.gamePhase == GameManager.GamePhase.idlePhase) {
			selected = true;
		}
		*/
		if (GameManager.gamePhase == GameManager.GamePhase.idlePhase) {
			selected = true;
			gameObject.GetComponent<Renderer> ().material.color = Color.yellow;
		} else {
			selected = false;
			gameObject.GetComponent<Renderer> ().material.color = Color.clear;
		}
		
	}

	public void setText(string s) {
		roleText.text = s;
	}
}
