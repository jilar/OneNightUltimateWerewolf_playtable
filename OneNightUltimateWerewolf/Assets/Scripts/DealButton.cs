using UnityEngine;
using System;
using System.Collections;
using TouchScript.Gestures;

public class DealButton : MonoBehaviour {

	public GameObject deck;

	void OnEnable() {
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
		Debug.LogError ("Tapped");
		Deck deckObject = deck.GetComponent<Deck>();
		if (deckObject.cards.Count > 0) {
			deckObject.deal (deckObject.cards [0]);
		}
	
	}
}
