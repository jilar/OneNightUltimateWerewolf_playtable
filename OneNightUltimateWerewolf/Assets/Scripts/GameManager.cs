using UnityEngine;
using Random = System.Random;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

public class GameManager : MonoBehaviour {

	public Sprite[] cardSprites = new Sprite[16];
	public Sprite[] tokenSprites = new Sprite[16];

	public enum GamePhase {idlePhase, dealPhase, nightPhase, dayPhase, votePhase};

	public static GamePhase gamePhase;
	public GameObject tokenPrefab;
	public GameObject cardPrefab;
	public GameObject deck;

	public GameObject btnAdv;

	public List<GameObject> tokens = new List<GameObject> ();
	public List<GameObject> cards = new List<GameObject> ();

	public Vector3 firstTokenPosition = new Vector3 (-5, 0, 0);
	public Vector3 currentTokenPosition = new Vector3 (-5, 0, 0);
	public int row = 0;
	public float horizontalShift = 1.5f;
	public float verticalShift = -1.5f;
	public int tokenCount = 0;
	public int selectedCount=0;

	void Awake() {
		gamePhase = GamePhase.idlePhase;
		createToken ("Doppelganger");
		createToken ("Mason");
		createToken ("Mason");
		createToken ("Minion");
		createToken ("Drunk");
		createToken ("Insomniac");
		createToken ("Hunter");
		createToken ("Tanner");
		createToken ("Werewolf");
		createToken ("Werewolf");
		createToken ("Robber");
		createToken ("Troublemaker");
		createToken ("Villager");
		createToken ("Villager");
		createToken ("Villager");
		createToken ("Seer");
	}

	public void createToken(string role) {
		GameObject token = (GameObject)Instantiate (tokenPrefab, currentTokenPosition, tokenPrefab.transform.rotation);
		currentTokenPosition.x += horizontalShift;


		token.GetComponent<Token> ().role = role;
		token.GetComponent<Token> ().setText (role);
		token.GetComponent<Token> ().image.sprite = tokenSprites [tokenCount];
		token.GetComponent<Token> ().image.rectTransform.sizeDelta = new Vector2(120,120);  //rezise images to fit token

		GameObject card = (GameObject)Instantiate (cardPrefab, Vector3.zero, cardPrefab.transform.rotation);
		card.GetComponent<Card> ().role = role;
		card.GetComponent<Card> ().setText (role);
		card.GetComponent<Card> ().image.sprite = cardSprites [tokenCount];

		deck.GetComponent<Deck> ().add (card.GetComponent<Card>());
		token.GetComponent<Token> ().card = card;

		tokens.Add (token);
		cards.Add (card);

		tokenCount += 1;
		row += 1;
		if (row >= 8) {
			row = 0;
			currentTokenPosition.y += verticalShift;
			currentTokenPosition.x = firstTokenPosition.x;
		}
	}

	public void handlePhase() {
		switch (gamePhase) {
		case GamePhase.idlePhase:
			List<Card> cardsInPlay = new List<Card> ();
			foreach (GameObject tokenObject in tokens) {
				Token token = tokenObject.GetComponent<Token> ();
				if (token.selected) {
					tokenObject.GetComponent<Renderer> ().material.color = Color.clear;
					tokenObject.GetComponent<TransformGesture> ().enabled = true;
					token.selected = false;
					//deck.GetComponent<Deck>().deal(token.card.GetComponent<Card>());
					cardsInPlay.Add (token.card.GetComponent<Card> ());
					selectedCount++;
					Debug.Log(selectedCount);
				} else {
					tokenObject.SetActive (false);
				}
			}
			deck.GetComponent<Deck> ().cards = cardsInPlay;        //have deck contain only roles chosen
			Debug.Log(selectedCount);
			deck.GetComponent<Deck> ().playerArray = selectedCount-6;
			int size = cardsInPlay.Count;
			Random r = new Random ();

//			Deck myDeck = deck.GetComponent<Deck> ();
			for (int i = 0; i < size; i++) {
				Deck myDeck = deck.GetComponent<Deck> ();
				myDeck.deal(myDeck.cards [r.Next (0, myDeck.cards.Count)]);
//				deck.GetComponent<Deck> ().deal (deck.GetComponent<Deck> ().cards [r.Next (0, deck.GetComponent<Deck> ().cards.Count)]);
			}
			deck.GetComponent<Deck> ().cards = deck.GetComponent<Deck> ().tmpCards;
			btnAdv.SetActive (false);
			deck.SetActive (false);
			gamePhase = GamePhase.dealPhase;
			break;
		case GamePhase.dealPhase:
			//deal cards
			Debug.LogError ("made it to deal phase");
			//Random l = new Random ();
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				Deck myDeck2 = deck.GetComponent<Deck> ();
				//myDeck2.handout(myDeck2.cards [l.Next (0, myDeck2.cards.Count)]);
				myDeck2.handout (myDeck2.cards [0]);
			}
			//following code for initial token placement
			float placementXleft, placementXright, placementYleft, placementYright;
			placementXleft = -5.3f;
			placementXright = 5.3f;
			placementYleft = 2.7f;
			placementYright = 2.7f;
			for (int i = 0; i < tokens.Count; i++) {
				if(tokens [i].active==true){
				if (i % 2!=0 ||i==0) {
						iTween.MoveTo (tokens [i].gameObject, new Vector3 (placementXleft, placementYleft, 0), 0.5f);
					placementYleft -= .5f;
				} else {	
						iTween.MoveTo (tokens [i].gameObject, new Vector3 (placementXright, placementYright, 0), 0.5f);
					placementYright = placementYright - .5f;
				}
				}
			}

			break;
		case GamePhase.nightPhase:
			//handle events
			break;
		case GamePhase.dayPhase:
			break;
		case GamePhase.votePhase:
			//handle vote
			break;
		default:
			break;
		}
	}

	public void reset() {
		// TODO: reset the board
	}
}
