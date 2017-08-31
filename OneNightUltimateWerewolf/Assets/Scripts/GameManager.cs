using UnityEngine;
using Random = System.Random;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

public class GameManager : MonoBehaviour {

	public Sprite[] cardSprites = new Sprite[16];
	public Sprite[] tokenSprites = new Sprite[16];

	public AudioClip[] sounds = new AudioClip[2];                             //holds all audio clips
	public AudioSource audioPlayer;

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

	private List<string> rolesInPlay = new List<string> ();

	//orginal position of the doppleganger, insomniac, drunk, and robber
	private Vector3 dopplegangerOrginal;
	private Vector3 insomniacOriginal;
	private Vector3 drunkOriginal;
	private Vector3 robberOriginal;

	//keeps track of which roles with night actions (that deal with cards) are in player pool
	//two werewolves is false in the array, if 1 werewolf it will be true
	//Order of array: Doppleganger, Werewolf, Seer, Robber, Troublemaker, Drunk, Insomniac
	private bool[] isPlayer= new bool[7]{false,false,false,false,false,false,false};

	//keeps track of doppleganger role if in play
	private string doppleRole;
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
		card.GetComponent<Card> ().token.sprite = tokenSprites [tokenCount];

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
					rolesInPlay.Add (token.role);
					selectedCount++;
					//Debug.Log(selectedCount);
				} else {
					tokenObject.SetActive (false);
				}
			}
			deck.GetComponent<Deck> ().cards = cardsInPlay;        //have deck contain only roles chosen

			deck.GetComponent<Deck> ().playerArray = selectedCount - 6;
			int size = cardsInPlay.Count;
			Random r = new Random ();

//			Deck myDeck = deck.GetComponent<Deck> ();
			for (int i = 0; i < size; i++) {
				//Deck myDeck = deck.GetComponent<Deck> ();
				//myDeck.deal(myDeck.cards [r.Next (0, myDeck.cards.Count)]);

				deck.GetComponent<Deck> ().deal (deck.GetComponent<Deck> ().cards [r.Next (0, deck.GetComponent<Deck> ().cards.Count)]);
			}
			deck.GetComponent<Deck> ().cards = deck.GetComponent<Deck> ().tmpCards;
			//btnAdv.SetActive (false);
			deck.SetActive (false);
			gamePhase = GamePhase.dealPhase;
			break;
		case GamePhase.dealPhase:
			//deal cards
			//Deck myDeck2 = deck.GetComponent<Deck> ();
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				//myDeck2.handout(myDeck2.cards [l.Next (0, myDeck2.cards.Count)]);
				//myDeck2.handout (myDeck2.cards [0]);
				deck.GetComponent<Deck> ().handout (deck.GetComponent<Deck> ().cards [0]);
			}
			deck.GetComponent<Deck> ().cards = deck.GetComponent<Deck> ().tmpCards;

			//get orginal postions of insomniac, drunk, doppleganger, and robber
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (deck.GetComponent<Deck> ().cards [i].role == "Robber") {
					robberOriginal = deck.GetComponent<Deck> ().cards [i].transform.position;	
				} else if (deck.GetComponent<Deck> ().cards [i].role == "Insomniac") {
					insomniacOriginal = deck.GetComponent<Deck> ().cards [i].transform.position;
				} else if (deck.GetComponent<Deck> ().cards [i].role == "Drunk") {
					drunkOriginal = deck.GetComponent<Deck> ().cards [i].transform.position;	
				} else if (deck.GetComponent<Deck> ().cards [i].role == "Doppelganger") {
					dopplegangerOrginal = deck.GetComponent<Deck> ().cards [i].transform.position;	
				}
			}

		//			//following code for initial token placement ~code used for a narration mode; tokens can be used to mark players to roles they are believed to be
//			float placementXleft, placementXright, placementYleft, placementYright;
//			place		mentXleft = -5.3f;
//			placementXright = 5.3f;
//			placementYleft = 2.7f;
//			placementYright = 2.7f;
//			for (int i = 0; i < tokens.Count; i++) {
//				if(tokens [i].activeInHierarchy){
//				if (i % 2!=0 ||i==0) {
//						iTween.MoveTo (tokens [i].gameObject, new Vector3 (placementXleft, placementYleft, 0), 0.5f);
//					placementYleft -= .5f;
//				} else {	
//						iTween.MoveTo (tokens [i].gameObject, new Vector3 (placementXright, placementYright, 0), 0.5f);
//					placementYright = placementYright - .5f;
//				}
//				}
//			}

			float placementXleft, placementXright, placementYleft, placementYright;
			placementXleft = -5.3f;
			placementXright = 5.3f;
			placementYleft = 2.7f;
			placementYright = 2.7f;
			for (int i = 0; i < tokens.Count; i++) {
				if(tokens [i].activeInHierarchy){
					if (i % 2!=0 ||i==0) {
						iTween.MoveTo (tokens [i].gameObject, new Vector3 (placementXleft, placementYleft, 0), 0.5f);
						placementYleft -= .5f;
						tokens [i].gameObject.transform.localScale = new Vector3(1F, 0, 1f);
					} else {	
						iTween.MoveTo (tokens [i].gameObject, new Vector3 (placementXright, placementYright, 0), 0.5f);
						placementYright = placementYright - .5f;
						tokens [i].gameObject.transform.localScale = new Vector3(1F, 0, 1f);
					}
				}

			}


			gamePhase = GamePhase.nightPhase;
			break;
		case GamePhase.nightPhase: //night actions


			//naaration mode code
			//find out which night roles are players
			isPlayer[0]= roleIsAPlayer("Doppelganger");
			isPlayer[1]= roleIsAPlayer("Werewolf");
			isPlayer[2]= roleIsAPlayer("Seer");
			isPlayer[3]= roleIsAPlayer("Robber");
			isPlayer[4]= roleIsAPlayer("Troublemaker");
			isPlayer[5]= roleIsAPlayer("Drunk");
			isPlayer[6]= roleIsAPlayer("Insomniac");

			//play background music
			audioPlayer.Play ();
			StartCoroutine(nightPhase());
			gamePhase = GamePhase.dayPhase;
			break;
		case GamePhase.dayPhase:
			//timer goes here
			break;
		case GamePhase.votePhase:
			//tap the card everyone voted for
			//handle vote
			break;
		default:
			break;
		}
	}
	 
	//This coroutine will play a specified set of sounds from the sounds array one after the other from var start to var stop
	IEnumerator soundPrompt(int start,int stop){
		for (int i = start; i<=stop; i++) {
			
			audioPlayer.PlayOneShot (sounds [i]);
//			if (i == stop) {
//				yield return new WaitForSeconds (sounds[i].length + pausetime);
//			} else {
				yield return new WaitForSeconds (sounds[i].length);
//			}
		}
	}

	//runs nightphase of game with sounds, narration mode
	IEnumerator nightPhase(){
		//close eyes
		yield return StartCoroutine (soundPrompt (0, 0));
		//doppleganger action
		if (rolesInPlay.Contains ("Doppelganger")) {
			//doppleganger view and take action
			yield return StartCoroutine (soundPrompt (1, 1));
			yield return StartCoroutine (doppleGangerAction(isPlayer[0]));
			//doppleganerAction();
			yield return StartCoroutine (soundPrompt (9, 9));
			yield return new WaitForSeconds (3f);
			//dopplegannger minion action
			yield return StartCoroutine (soundPrompt (10, 11));
			yield return new WaitForSeconds (1f);
		}
		//werewolf action 
		if (rolesInPlay.Contains ("Werewolf")){
			yield return StartCoroutine (soundPrompt (12, 13));
			yield return StartCoroutine (werewolfAction(isPlayer[1]));
//			findAndFlip ("Werewolf");
			yield return StartCoroutine (soundPrompt (14, 14));
			yield return new WaitForSeconds (1f);
		}
		//minion action
		if (rolesInPlay.Contains ("Minion")) {
			yield return StartCoroutine (soundPrompt (15, 15));
			yield return new WaitForSeconds (3f);//new
			yield return StartCoroutine (soundPrompt (16, 17));
			yield return new WaitForSeconds (1f);//new
		}
		//mason action
		if (rolesInPlay.Contains ("Mason")) {
			yield return StartCoroutine (soundPrompt (18, 18));
			yield return new WaitForSeconds (3f);
			yield return StartCoroutine (soundPrompt (19, 19));
			yield return new WaitForSeconds (1f);
		}
		//seer action
		if (rolesInPlay.Contains ("Seer")) {
			StartCoroutine (soundPrompt (20, 20));
			yield return StartCoroutine (seerAction(isPlayer[2]));
			yield return StartCoroutine (soundPrompt (21, 21));
			yield return new WaitForSeconds (1f);
		}
		//robber action
		if (rolesInPlay.Contains ("Robber")) {
			yield return StartCoroutine (soundPrompt (22, 22));
			yield return StartCoroutine (robberAction(isPlayer[3]));
			yield return StartCoroutine (soundPrompt (23, 23));
			yield return new WaitForSeconds (1f);
		}
		//troublemaker action
		if (rolesInPlay.Contains ("Troublemaker")) {
			yield return StartCoroutine (soundPrompt (24, 24));
			yield return StartCoroutine (troublemakerAction(isPlayer[4]));
			yield return StartCoroutine (soundPrompt (25, 25));
			yield return new WaitForSeconds (1f);
		}
		//drunk action
		if (rolesInPlay.Contains ("Drunk")) {
			yield return StartCoroutine (soundPrompt (26, 26));
			yield return StartCoroutine (drunkAction(isPlayer[5]));
			yield return StartCoroutine (soundPrompt (27, 27));
			yield return new WaitForSeconds (1f);
		}
		//insomniac action
		if (rolesInPlay.Contains ("Insomniac")) {
			yield return StartCoroutine (soundPrompt (28, 28));
			yield return StartCoroutine (insomniacAction(isPlayer[6]));
			yield return StartCoroutine (soundPrompt (29, 29));
			yield return new WaitForSeconds (1f);
		}
		//doppleganger insomniac
		if (rolesInPlay.Contains ("Doppelganger")) {
			yield return StartCoroutine (soundPrompt (30, 30));
			//doppleganger();
			yield return StartCoroutine (soundPrompt (31, 31));
		}
		yield return StartCoroutine(soundPrompt (32, 32));
	}


	//Check if role is in player pool
	private bool roleIsAPlayer(string role){ 
		bool i = false;
		for (int p = 0; p < deck.GetComponent<Deck> ().cards.Count-3; p++) { //Count.-3 because last three cards are in the middle and aren't players 
			if (deck.GetComponent<Deck> ().cards[p].role==role) {
				i=!i;
			}
		}
		return i;
	}

	//returns any tapped cards back to facedown position and marks them as untapped
	public void facedown(){ 
		for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) { 
			if (deck.GetComponent<Deck> ().cards [i].tapped == true) {
				deck.GetComponent<Deck> ().cards [i].flip ();
				deck.GetComponent<Deck> ().cards [i].tapped = false;
			}
		}
	}

	IEnumerator doppleGangerAction(bool player){
		//just flip the card if doppleganger is insomniac
		if (doppleRole == "Insomniac") { 
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
				if (deck.GetComponent<Deck> ().cards [i].transform.position == dopplegangerOrginal) {
					deck.GetComponent<Deck> ().cards [i].flip ();
					deck.GetComponent<Deck> ().cards [i].tapped = true;
				}
			}
		
			yield return new WaitForSeconds (3f);
			facedown ();
		
		//have player select another players card, proceed to do action if chose card was seer, drunk, troublemaker, or robber
		}else if (player) {
			// make player cards tappable
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (!(deck.GetComponent<Deck> ().cards [i].transform.position.y == 1) || !(deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
					deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
					//deck.GetComponent<Deck> ().cards [i].swapPhase = true;
				}
			}
			//wait for player to choose a card and set it to dopple role
			deck.GetComponent<Deck> ().tapcount = 0;
			while (deck.GetComponent<Deck> ().tapcount != 1) {
				deck.GetComponent<Deck> ().updateTapped ();
				yield return null;
			} 
			yield return new WaitForSeconds (1f);
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
				//find the player who was tapped, set it to dopple role
				//flip card
				if (deck.GetComponent<Deck> ().cards [i].tapped == true) {
					doppleRole = deck.GetComponent<Deck> ().cards [i].role;
					yield return new WaitForSeconds (1f);
					deck.GetComponent<Deck> ().cards [i].flip ();
					deck.GetComponent<Deck> ().cards [i].tapped = false;
					break;
				}
			}
			yield return StartCoroutine (soundPrompt (2, 8));
			deck.GetComponent<Deck> ().tapcount = 0;
			Debug.Log (doppleRole);
			switch(doppleRole){
				case "Robber":
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
						if (!(deck.GetComponent<Deck> ().cards [i].transform.position.y == 1) || !(deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
						     deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
							deck.GetComponent<Deck> ().cards [i].swapPhase = true;
						}
					}
					while (deck.GetComponent<Deck> ().tapcount != 1) {
						deck.GetComponent<Deck> ().updateTapped ();
						yield return null;
					}
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
						if (deck.GetComponent<Deck> ().cards [i].transform.position == dopplegangerOrginal) {
							for (int l = 0; i < deck.GetComponent<Deck> ().cards.Count; l++) {
								//flip chosen card then swap
								if (deck.GetComponent<Deck> ().cards [l].swapCard == true) {
									deck.GetComponent<Deck> ().switchPosition (deck.GetComponent<Deck> ().cards [i], deck.GetComponent<Deck> ().cards [l]);
									deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [i]);
									deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [l]);
									yield return new WaitForSeconds (1f);
									deck.GetComponent<Deck> ().cards [l].flip ();
									deck.GetComponent<Deck> ().cards [l].swapCard = false;
									//deck.GetComponent<Deck> ().cards [i].tapped = false;
									break;
								}
							}
							break;
						}
					}
					break;
				case "Troublemaker":
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
						if (!(deck.GetComponent<Deck> ().cards [i].transform.position.y == 1) || !(deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
						    deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
							deck.GetComponent<Deck> ().cards [i].swapPhase = true;
						}
					}
					while (deck.GetComponent<Deck> ().tapcount != 2) {
						deck.GetComponent<Deck> ().tapcount = 0;
						deck.GetComponent<Deck> ().updateTapped ();
						yield return null;
					}
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
						if (deck.GetComponent<Deck> ().cards [i].swapCard == true) {
							for (int l = i + 1; i < deck.GetComponent<Deck> ().cards.Count; l++) {
								if (deck.GetComponent<Deck> ().cards [l].swapCard == true) {
									deck.GetComponent<Deck> ().switchPosition (deck.GetComponent<Deck> ().cards [i], deck.GetComponent<Deck> ().cards [l]);
									deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [i]);
									deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [l]);
									yield return new WaitForSeconds (1f);
									deck.GetComponent<Deck> ().cards [l].swapCard = false;
									deck.GetComponent<Deck> ().cards [i].swapCard = false;
									deck.GetComponent<Deck> ().cards [i].tapped = false;
									deck.GetComponent<Deck> ().cards [l].tapped = false;
									break;
								}
							}
							break;
						}
					}
					break;
				case "Drunk":
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
						deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
						deck.GetComponent<Deck> ().cards [i].swapPhase = false;
					}
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
						if (deck.GetComponent<Deck> ().cards [i].transform.position.y == 1 && (deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
						    deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
							deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
							deck.GetComponent<Deck> ().cards [i].swapPhase = true;
						}
					}
					while (deck.GetComponent<Deck> ().tapcount != 1) {
						deck.GetComponent<Deck> ().updateTapped ();
						yield return null;
					}
					//swap the two cards.
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
						if (deck.GetComponent<Deck> ().cards[i].transform.position == dopplegangerOrginal) {
						Debug.Log ("inside");
							for (int l = 0; i < deck.GetComponent<Deck> ().cards.Count; l++) {
								if (deck.GetComponent<Deck> ().cards [l].swapCard == true) {
								Debug.Log ("inside2");
									deck.GetComponent<Deck> ().switchPosition (deck.GetComponent<Deck> ().cards [i], deck.GetComponent<Deck> ().cards [l]);
									deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [i]);
									deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [l]);
									deck.GetComponent<Deck> ().cards [l].swapCard = false;
									deck.GetComponent<Deck> ().cards [l].tapped = false;
									break;
								}
							}
							break;
						}
					}
					break;
			case "Seer":
				//make all cards tappable
				for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
				} 
				//wait for a card to be tapped
				yield return new WaitForSeconds (1f);
				while (deck.GetComponent<Deck> ().tapcount != 1) {
					deck.GetComponent<Deck> ().updateTapped ();
					yield return null;
				}
				//check if a middle card was tapped
				bool middle = false;
				for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
					if (deck.GetComponent<Deck> ().cards [i].transform.position.y == 1 && (deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
					    deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
						if (deck.GetComponent<Deck> ().cards [i].tapped == true) {									
							middle = true; 
							break;
						}
					}
				} 
				//middle tapped, prevent player cards from being tapped, wait for second card to be tapped
				if (middle) {
					for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
						deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
					}

					while (deck.GetComponent<Deck> ().tapcount != 2) {
						deck.GetComponent<Deck> ().tapcount = 0;
						deck.GetComponent<Deck> ().updateTapped ();
						yield return null;
					}
				}
				break;
			}
			//reset all cards and counters
			facedown ();
			deck.GetComponent<Deck> ().tapcount = 0;
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
				deck.GetComponent<Deck> ().cards [i].swapPhase = false;
				deck.GetComponent<Deck> ().cards [i].swapCard = false;
			}
			yield return new WaitForSeconds (5f);
		} else { 
			yield return StartCoroutine (soundPrompt (2, 8));
			yield return new WaitForSeconds (10f);
		}
	}

	IEnumerator werewolfAction(bool oneWolf){
		if (oneWolf) {
			//allow single wolf to look at middle cards
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (deck.GetComponent<Deck> ().cards [i].transform.position.y==1 && (deck.GetComponent<Deck> ().cards [i].transform.position.x==-3 || 
					deck.GetComponent<Deck> ().cards [i].transform.position.x==0 || deck.GetComponent<Deck> ().cards [i].transform.position.x==3)) {							
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
				}
			} 
			//Debug.LogError (deck.GetComponent<Deck> ().tapcount);
			while (deck.GetComponent<Deck> ().tapcount != 1) {
				deck.GetComponent<Deck> ().updateTapped ();
				yield return null;
			}
//				for (int i = deck.GetComponent<Deck> ().cards.Count; i > deck.GetComponent<Deck> ().cards.Count - 3; i--) {
//					deck.GetComponent<Deck> ().cards [i-1].GetComponent<TapGesture> ().enabled = false;
//				}

			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (deck.GetComponent<Deck> ().cards [i].transform.position.y==1 && (deck.GetComponent<Deck> ().cards [i].transform.position.x==-3 || 
					deck.GetComponent<Deck> ().cards [i].transform.position.x==0 || deck.GetComponent<Deck> ().cards [i].transform.position.x==3)) {
						deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
				}
			} 
			yield return new WaitForSeconds (3f);
		} else {
			yield return new WaitForSeconds (3f);
		}
		deck.GetComponent<Deck> ().tapcount = 0;
		facedown ();
	}
		

	IEnumerator seerAction(bool Player){
		if (Player) {
			//make all cards tappable
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
			} 
			//wait for a card to be tapped
			yield return new WaitForSeconds (1f);
			while (deck.GetComponent<Deck> ().tapcount != 1) {
				deck.GetComponent<Deck> ().updateTapped ();
				yield return null;
			}
			//check if a middle card was tapped
			bool middle = false;
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (deck.GetComponent<Deck> ().cards [i].transform.position.y == 1 && (deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
				    deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
					if (deck.GetComponent<Deck> ().cards [i].tapped == true) {									
						middle = true; 
						break;
					}
				}
			} 
//		for (int i = deck.GetComponent<Deck> ().cards.Count; i > deck.GetComponent<Deck> ().cards.Count - 3; i--) {
//				if(deck.GetComponent<Deck> ().cards [i-1].tapped==true){
//				
//					middle = true; 
//					break;
//				}
//				
//		}
			//middle tapped, prevent player cards from being tapped, wait for second card to be tapped
			if (middle) {
				for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
				}

				while (deck.GetComponent<Deck> ().tapcount != 2) {
					deck.GetComponent<Deck> ().tapcount = 0;
					deck.GetComponent<Deck> ().updateTapped ();
					yield return null;
				}
			}
		} else {
			yield return new WaitForSeconds (5f);
		}
		for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
			deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
			//deck.GetComponent<Deck> ().cards [i].tapped = false;
		}
		deck.GetComponent<Deck> ().tapcount = 0;
		yield return new WaitForSeconds (3f);
		facedown ();
	}

	IEnumerator robberAction(bool Player){
		if (Player) {
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (!(deck.GetComponent<Deck> ().cards [i].transform.position.y == 1) || !(deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
				   deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
					deck.GetComponent<Deck> ().cards [i].swapPhase = true;
				}
			}
			while (deck.GetComponent<Deck> ().tapcount != 1) {
				deck.GetComponent<Deck> ().updateTapped ();
				yield return null;
			}
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
				//find the player who is the robber
				if (deck.GetComponent<Deck> ().cards [i].transform.position == robberOriginal) {
					for (int l = 0; i < deck.GetComponent<Deck> ().cards.Count; l++) {
						//flip chosen card then swap
						if (deck.GetComponent<Deck> ().cards [l].swapCard == true) {
							deck.GetComponent<Deck> ().switchPosition (deck.GetComponent<Deck> ().cards [i], deck.GetComponent<Deck> ().cards [l]);
							deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [i]);
							deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [l]);
							yield return new WaitForSeconds (1f);
							deck.GetComponent<Deck> ().cards [l].flip ();
							deck.GetComponent<Deck> ().cards [l].swapCard = false;
							//deck.GetComponent<Deck> ().cards [i].tapped = false;
							break;
						}
					}
					break;
				}
			}
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
				deck.GetComponent<Deck> ().cards [i].swapPhase = false;

			}
		}
		deck.GetComponent<Deck> ().tapcount = 0;
		yield return new WaitForSeconds (3f);
		facedown ();
	}

	IEnumerator troublemakerAction (bool Player)
	{
		if (Player) {
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (!(deck.GetComponent<Deck> ().cards [i].transform.position.y == 1) || !(deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
					deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
					deck.GetComponent<Deck> ().cards [i].swapPhase = true;
				}
			}
			while (deck.GetComponent<Deck> ().tapcount != 2) {
				deck.GetComponent<Deck> ().tapcount = 0;
				deck.GetComponent<Deck> ().updateTapped ();
				yield return null;
			}
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
				if (deck.GetComponent<Deck> ().cards [i].swapCard == true) {
					for (int l = i+1; i < deck.GetComponent<Deck> ().cards.Count; l++) {
						//flip chosen card then swap
						if (deck.GetComponent<Deck> ().cards [l].swapCard == true) {
							deck.GetComponent<Deck> ().switchPosition (deck.GetComponent<Deck> ().cards [i], deck.GetComponent<Deck> ().cards [l]);
							deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [i]);
							deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [l]);
							yield return new WaitForSeconds (1f);
							deck.GetComponent<Deck> ().cards [l].swapCard = false;
							deck.GetComponent<Deck> ().cards [i].swapCard = false;
							deck.GetComponent<Deck> ().cards [i].tapped = false;
							deck.GetComponent<Deck> ().cards [l].tapped = false;
							break;
						}
					}
					break;
				}
			}
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
				deck.GetComponent<Deck> ().cards [i].swapPhase = false;
			}
		}
		deck.GetComponent<Deck> ().tapcount = 0;
		yield return new WaitForSeconds (3f);
	}
		

	IEnumerator drunkAction(bool Player){
		if (Player){
			//make middle cards interactable
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				if (deck.GetComponent<Deck> ().cards [i].transform.position.y == 1 && (deck.GetComponent<Deck> ().cards [i].transform.position.x == -3 ||
			    	deck.GetComponent<Deck> ().cards [i].transform.position.x == 0 || deck.GetComponent<Deck> ().cards [i].transform.position.x == 3)) {
					deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = true;
					deck.GetComponent<Deck> ().cards [i].swapPhase = true;
				}
			}
			while (deck.GetComponent<Deck> ().tapcount != 1) {
				deck.GetComponent<Deck> ().updateTapped ();
				yield return null;
			}
			//swap the two cards.
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
				//find the player who is the drunk
				if (deck.GetComponent<Deck> ().cards [i].transform.position == drunkOriginal) {
					for (int l = 0; i < deck.GetComponent<Deck> ().cards.Count; l++) {
						if (deck.GetComponent<Deck> ().cards[l].swapCard==true) {
							deck.GetComponent<Deck> ().switchPosition (deck.GetComponent<Deck> ().cards [i], deck.GetComponent<Deck> ().cards [l]);
							deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [i]);
							deck.GetComponent<Deck> ().Rotator (deck.GetComponent<Deck> ().cards [l]);
							deck.GetComponent<Deck> ().cards [l].swapCard =false;
							deck.GetComponent<Deck> ().cards [l].tapped =false;
							break;
						}
					}
					break;
				}
			}
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count; i++) {
				deck.GetComponent<Deck> ().cards [i].GetComponent<TapGesture> ().enabled = false;
				deck.GetComponent<Deck> ().cards [i].swapPhase = false;
			}
		}

		deck.GetComponent<Deck> ().tapcount = 0;
		yield return new WaitForSeconds (3f);
	} 

	IEnumerator insomniacAction(bool Player){
		if (Player) {
			for (int i = 0; i < deck.GetComponent<Deck> ().cards.Count - 3; i++) {
				if (deck.GetComponent<Deck> ().cards [i].transform.position == insomniacOriginal) {
					deck.GetComponent<Deck> ().cards [i].flip ();
					deck.GetComponent<Deck> ().cards [i].tapped = true;
				}
			}
		}
		yield return new WaitForSeconds (3f);
		facedown ();
	}

	 public void nightPhase2(){
		
	}
    }
