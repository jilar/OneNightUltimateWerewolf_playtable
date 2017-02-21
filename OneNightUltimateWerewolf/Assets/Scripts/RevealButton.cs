using UnityEngine;
using System;
using System.Collections;
using TouchScript.Gestures;

public class RevealButton : MonoBehaviour {

	public GameObject deck;
	public GameManager gm;

	void OnEnable() {
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
		for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) { 
			deck.GetComponent<Deck> ().cards [i].flip ();
		}
	}
}