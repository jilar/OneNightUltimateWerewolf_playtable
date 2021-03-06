﻿using UnityEngine;
using System;
using System.Collections;
using TouchScript.Gestures;

public class DealButton : MonoBehaviour {

	public GameObject deck;
	public GameManager gm;

	void OnEnable() {
		GetComponent<TapGesture> ().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	public void TapHandler(object sender, EventArgs e) {
//		Debug.LogError ("Tapped");
//		Deck deckObject = deck.GetComponent<Deck>();
//		if (deckObject.cards.Count > 0) {
//			deckObject.deal (deckObject.cards [0]);
//		}
		if (GameManager.gamePhase == GameManager.GamePhase.dealPhase) {
			//Debug.LogError ("beep beep");
			gm.handlePhase();
		}
	}
}
