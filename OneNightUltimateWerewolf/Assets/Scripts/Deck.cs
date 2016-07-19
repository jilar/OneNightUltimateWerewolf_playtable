using UnityEngine;
using System.Collections;
using TouchScript;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	public List<Card> cards = new List<Card>();
	public float zShift = 0.05f;
	public float initialZ = -0.4f;
	public float currentZ = 0f;

	public Vector3 dealPosition = new Vector3(0, 3, 0);

	public void add(Card card) {
		card.gameObject.transform.SetParent (this.gameObject.transform);
		
		//fix later
		card.gameObject.transform.localPosition = new Vector3(0, 0, initialZ + currentZ);
		currentZ += zShift;
		card.gameObject.transform.localRotation = new Quaternion (0, 180, 0, 0);
		cards.Add (card);
	}

	public void deal(Card card) {
		card.gameObject.transform.parent = null;
		iTween.MoveTo (card.gameObject, dealPosition, 0.5f);
		cards.Remove (card);
	}
}
