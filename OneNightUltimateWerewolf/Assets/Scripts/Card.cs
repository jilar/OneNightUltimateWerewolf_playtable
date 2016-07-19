using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TouchScript.Gestures;

public class Card : MonoBehaviour {

	public int deckLayer = 8;
	public int id;
	public string role;
	public Text roleText;
	public Image image;

	void Awake() {
		GetComponent<Renderer> ().material.color = Color.clear;
	}

	void OnCollisionEnter(Collision col) {
		Debug.LogError ("Collided");
		if (col.gameObject.layer == deckLayer) {
			Deck deck = col.gameObject.GetComponent<Deck> ();
			deck.add (this);
		}
	}

	public void setText(string s) {
		roleText.text = s;
	}

	public void flip() {
		//iTween.RotateTo (gameObject, iTween.Hash ("x", 0, "y", 0, "z", 0));
		//iTween.RotateBy (gameObject, iTween.Hash ("x", 0, "y", 180, "z", 0));
		iTween.RotateAdd (gameObject, iTween.Hash ("x", 0, "y", 180, "z", 0));
	}

	void OnEnable() {
		GetComponent<TapGesture>().Tapped += TapHandler;
	}

	void OnDisable() {
		GetComponent<TapGesture> ().Tapped -= TapHandler;
	}

	void TapHandler (object sender, System.EventArgs e) {
		Debug.LogError ("Tapped card");
		flip ();
	}
}
