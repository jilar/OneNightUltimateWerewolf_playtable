using UnityEngine;
using System.Collections;
using TouchScript;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	public List<Card> cards = new List<Card>();
	public List<Card> tmpCards = new List<Card>();
	public Vector3[][] cardPosition;                  //jagged array containing card positions depending on # of players
	public float zShift = 0.05f;
	public float initialZ = -0.4f;
	public float currentZ = 0f;
	public int playerArray;                           //stores the index of the needed array (based on # of players) in cardPosition
	private int handoutCounter = 0;

	public Vector3 dealPosition = new Vector3(0, 3, 0);

	void Awake(){
		Vector3[] pl3 = new Vector3[6] {new Vector3 (0,-2.9f, 0),new Vector3 (-7.4f, 1, 0),new Vector3 (7.5f, 1, 0),
			new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl4 = new Vector3[7] {new Vector3 (0,-2.9f, 0),new Vector3 (-7.4f, 1, 0),new Vector3 (7.5f, 1, 0), 
			new Vector3 (0, 4.9f, 0), new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl5 = new Vector3[8] {new Vector3 (2,-2.9f, 0), new Vector3 (-2,-2.9f, 0),new Vector3 (-7.4f, 1, 0),new Vector3 (7.5f, 1, 0), 
			new Vector3 (0, 4.9f, 0), new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl6 = new Vector3[9] {new Vector3 (2,-2.9f, 0), new Vector3 (-2,-2.9f, 0),new Vector3 (-7.4f, 1, 0),new Vector3 (7.5f, 1, 0), 
			new Vector3 (2, 4.9f, 0),new Vector3 (-2, 4.9f, 0), new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl7 = new Vector3[10] {new Vector3 (2,-2.9f, 0),  new Vector3 (-2,-2.9f, 0),new Vector3 (-7.4f, 3, 0),new Vector3 (-7.4f, -1, 0),new Vector3 (7.5f, 3, 0), 
			new Vector3 (7.5f,-1, 0),new Vector3 (0, 4.9f, 0), new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl8 = new Vector3[11] {new Vector3 (2,-2.9f, 0),  new Vector3 (-2,-2.9f, 0),new Vector3 (-7.4f, 3, 0),
			new Vector3 (-7.4f, -1, 0),new Vector3 (7.5f, 3, 0), new Vector3 (7.5f,-1, 0),new Vector3 (-2, 4.9f, 0), new Vector3 (2, 4.9f, 0), 
			new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl9 = new Vector3[12] {new Vector3 (-4,-2.9f, 0),  new Vector3 (-0,-2.9f, 0), new Vector3 (-4,-2.9f, 0), new Vector3 (-7.4f, 3, 0),
			new Vector3 (-7.4f, -1, 0),new Vector3 (7.5f, 3, 0), new Vector3 (7.5f,-1, 0),new Vector3 (-2, 4.9f, 0), new Vector3 (2, 4.9f, 0), 
			new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		Vector3[] pl10 = new Vector3[13] {new Vector3 (-4,-2.9f, 0),  new Vector3 (-0,-2.9f, 0), new Vector3 (4,-2.9f, 0), new Vector3 (-7.4f, 3, 0),
			new Vector3 (-7.4f, -1, 0),new Vector3 (7.5f, 3, 0), new Vector3 (7.5f,-1, 0),new Vector3 (-4, 4.9f, 0), new Vector3 (4, 4.9f, 0), new Vector3 (0, 4.9f, 0), 
			new Vector3 (-3, 1, 0),new Vector3 (0, 1, 0), new Vector3 (3, 1, 0)};
		cardPosition = new Vector3[8][]{pl3,pl4,pl5,pl6,pl7,pl8,pl9,pl10};
	}


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
		tmpCards.Add(card);                              
	}

	public void handout(Card card) {
		Debug.LogError ("DEALING");
		card.gameObject.transform.parent = null;
		iTween.MoveTo (card.gameObject, cardPosition[playerArray][handoutCounter], 0.5f);
		handoutCounter++;
		cards.Remove (card);
		tmpCards.Add(card);
	}
}
