using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MemoryGameController : MonoBehaviour {

    // Public Attributes
    public Text levelText;
    public Text movesText;
	public Sprite[] spriteCardsBack;
	public Sprite[] spriteCardsFront;
	public Sprite spriteCardShadow;

	public AudioClip soundDealCard;
	public AudioClip soundButtonClick;
	public AudioClip soundUncoverCard;
	public AudioClip soundFoundPair;
	public AudioClip soundNoPair;
    public AudioClip soundLose;

    // Private Attributes
    private float uncoverTime = 10.0f;      // How fast to uncover a card
    private float dealTime = 8.0f;          // how fast to deal one card
    private float checkPairTime = 0.5f;
    private int cardsPadding = 20;          // The Padding between 2 Cards
    private int pairCount = 8;

    private bool shadow = true;             // Indicates whether there is a shadow or not
    private float shadowOffsetX = 32;
    private float shadowOffsetY = -32;
    private float shadOffsetX;
    private float shadOffsetY;

    private int chosenCardsBack = 0;
    private int[] chosenCardsFront;
    private Vector3 dealingStartPosition = new Vector3(-12800, -12800, -8);

    private int totalMoves = 0;
    private int maxMoves = 25;
    private int uncoveredCards = 0;
    private Transform[] selectedCards = new Transform[2];

    private int oldPairCount;

    private bool isTouching = false;
    private bool isUncovering = false;
    private bool isDealing = false;
    private bool isGameOver = true;
    private bool readyToPlay = false;


    void Start() {
        if (GameManager.instance.Medals >= 5) {
            pairCount = 10;
            maxMoves = 25;

            levelText.text = "Advanced";
            movesText.text = "Moves: " + totalMoves + "/" + maxMoves;
        }
    }

    public void PrepareNewGame() {
        // clear the game field and reset variables
        DestroyImmediate(GameObject.Find("DeckParent"));
        DestroyImmediate(GameObject.Find("Temp"));
        selectedCards = new Transform[2];
        totalMoves = 0;
        uncoveredCards = 0;
        
        movesText.text = "Moves: " + totalMoves + "/" + maxMoves;

        readyToPlay = true;
    }

	void CreateDeck() {
		isDealing = true;

		// randomly choose the back graphic of the cards
		chosenCardsBack = Random.Range(0, spriteCardsBack.Length);

		// randomly choose the card motives to play with
		List<int> tmp = new List<int>();
		for(int i = 0; i < spriteCardsFront.Length; i++) {
			tmp.Add(i);
		}
		tmp.Shuffle();
		chosenCardsFront = tmp.GetRange(0, pairCount).ToArray();

		GameObject deckParent = new GameObject("DeckParent"); // this holds all the cards
		GameObject temp = new GameObject("Temp");

		int cur = 0;

		float minX = Mathf.Infinity;
		float maxX = Mathf.NegativeInfinity;
		float minY = Mathf.Infinity;
		float maxY = Mathf.NegativeInfinity;

		// calculate columns and rows needed for the selected pair count
		int cCards = pairCount * 2;
		int cols = (int)Mathf.Sqrt(cCards);
		int rows = (int)Mathf.Ceil(cCards / (float)cols);

		List<int> deck = new List<int>();
		for(int i = 0; i < pairCount; i++) {
			deck.AddRange(new int[] {i, i});
		}
		deck.Shuffle();

		int cardWidth = 0;
		int cardHeight = 0;

		for(int x = 0; x < rows; x++) {
			for(int y = 0; y < cols; y++) {
				if(cur > cCards-1)
					break;

				// Create the Card
				GameObject card = new GameObject("Card"); // parent object
				GameObject cardFront = new GameObject("CardFront");
				GameObject cardBack = new GameObject("CardBack");
				GameObject destination = new GameObject("Destination");

				cardFront.transform.parent = card.transform; // make front child of card
				cardBack.transform.parent = card.transform; // make back child of card

				// front (motive)
				cardFront.AddComponent<SpriteRenderer>();
				cardFront.GetComponent<SpriteRenderer>().sprite = spriteCardsFront[chosenCardsFront[deck[cur]]];
				cardFront.GetComponent<SpriteRenderer>().sortingOrder = -1;

				// back
				cardBack.AddComponent<SpriteRenderer>();
				cardBack.GetComponent<SpriteRenderer>().sprite = spriteCardsBack[chosenCardsBack];
				cardBack.GetComponent<SpriteRenderer>().sortingOrder = 1;

				cardWidth = (int)spriteCardsBack[chosenCardsBack].rect.width;
				cardHeight = (int)spriteCardsBack[chosenCardsBack].rect.height;

				// now add the Card GameObject to the Deck GameObject "deckParent"
				card.tag = "Card";
				card.transform.parent = deckParent.transform;
				card.transform.position = dealingStartPosition;
				card.AddComponent<BoxCollider2D>();
				card.GetComponent<BoxCollider2D>().size = new Vector2(cardWidth, cardHeight);
				card.AddComponent<CardProperties>().Pair = deck[cur];

				destination.transform.parent = temp.transform;
				destination.tag = "Destination";
				destination.transform.position = new Vector3(x * (cardWidth + cardsPadding), y * (cardHeight + cardsPadding));

				if(shadow) {
					GameObject cardShadow = new GameObject("CardShadow");

					cardShadow.tag = "CardShadow";
					cardShadow.transform.parent = deckParent.transform;
					cardShadow.transform.position = dealingStartPosition;
					cardShadow.AddComponent<SpriteRenderer>();
					cardShadow.GetComponent<SpriteRenderer>().sprite = spriteCardShadow;
					cardShadow.GetComponent<SpriteRenderer>().sortingOrder = -2;
				}
				cur++;

				// determine positions for the camera helper objects
				Vector3 pos = destination.transform.position;
				minX = Mathf.Min(minX, pos.x - cardWidth);
				minY = Mathf.Min(minY, pos.y - cardHeight);
				maxX = Mathf.Max(maxX, pos.x + cardWidth + shadowOffsetX);
				maxY = Mathf.Max(maxY, pos.y + cardHeight + shadowOffsetY);
			}
		}

		// scale to fit onto the "table"
		float tableScale = (GameObject.Find("Table") == null) ? 1f : GameObject.Find("Table").transform.localScale.x;
		float scale = tableScale / (maxX + cardsPadding);

		Vector2 point = LineIntersectionPoint(
			new Vector2(minX, maxY),
			new Vector2(maxX, minY),
			new Vector2(minX, minY),
			new Vector2(maxX, maxY)
			);

		temp.transform.position -= new Vector3(point.x * scale, point.y * scale);

		shadOffsetX = shadowOffsetX * scale;
		shadOffsetY = shadowOffsetY * scale;

		deckParent.transform.localScale = new Vector3(scale, scale, scale);
		temp.transform.localScale = new Vector3(scale, scale, scale);

        StartCoroutine(dealCards());
	}


	IEnumerator dealCards() {
		GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
		GameObject[] cardsShadow = GameObject.FindGameObjectsWithTag("CardShadow");
		GameObject[] destinations = GameObject.FindGameObjectsWithTag("Destination");

		for(int i = 0; i < cards.Length; i++) {
			float t = 0;

            AudioSource.PlayClipAtPoint(soundDealCard, Camera.main.transform.position);

			while(t < 1f) {
				t += Time.deltaTime * dealTime;

				cards[i].transform.position = Vector3.Lerp(
					dealingStartPosition, destinations[i].transform.position, t);

				if(cardsShadow.Length > 0) {
					cardsShadow[i].transform.position = Vector3.Lerp(
						dealingStartPosition, 
						destinations[i].transform.position + new Vector3(shadOffsetX, shadOffsetY), t);
				}

				yield return null;
			}
			yield return null;
		}

		isDealing = false;

		yield return 0;
	}


	IEnumerator uncoverCard(Transform card, bool uncover) {
		isUncovering = true;

		float minAngle = uncover ? 0 : 180;
		float maxAngle = uncover ? 180 : 0; 

		float t = 0;
		bool uncovered = false;

        AudioSource.PlayClipAtPoint(soundUncoverCard, Camera.main.transform.position);

		// find the shadow for the selected card
		var shadow = GameObject.FindGameObjectsWithTag("CardShadow").Where(
			g => (g.transform.position == card.position + new Vector3(shadOffsetX, shadOffsetY))).FirstOrDefault();

		while(t < 1f) {
			t += Time.deltaTime * uncoverTime;

			float angle = Mathf.LerpAngle(minAngle, maxAngle, t);
			card.eulerAngles = new Vector3(0, angle, 0);

			if(shadow != null)
				shadow.transform.eulerAngles = new Vector3(0, angle, 0);

			if( ( (angle >= 90 && angle < 180) || 
			      (angle >= 270 && angle < 360) ) && !uncovered) {

				uncovered = true;
				for(int i = 0; i < card.childCount; i++) {
					// reverse sorting order to show the otherside of the card
					// otherwise you would still see the same sprite because they are sorted 
					// by order not distance (by default)
					Transform c = card.GetChild(i);
					c.GetComponent<SpriteRenderer>().sortingOrder *= -1;

					yield return null;
				}
			}

			yield return null;
		}

		// check if we uncovered 2 cards
		if(uncoveredCards == 2) {
			// if so compare the cards
			if(selectedCards[0].GetComponent<CardProperties>().Pair !=
			   selectedCards[1].GetComponent<CardProperties>().Pair) {

				// if they are not equal cover back
				yield return new WaitForSeconds(checkPairTime);
                AudioSource.PlayClipAtPoint(soundNoPair, Camera.main.transform.position, 0.5f);
				StartCoroutine (uncoverCard(selectedCards[0], false));
				StartCoroutine (uncoverCard(selectedCards[1], false));
			}
			else {
                yield return new WaitForSeconds(checkPairTime/2);
                AudioSource.PlayClipAtPoint(soundFoundPair, Camera.main.transform.position, 0.3f);

				// set as solved
				selectedCards[0].GetComponent<CardProperties>().Solved = true;
				selectedCards[1].GetComponent<CardProperties>().Solved = true;
			}
			selectedCards[0].GetComponent<CardProperties>().Selected = false;
			selectedCards[1].GetComponent<CardProperties>().Selected = false;
			uncoveredCards = 0;
			totalMoves++;
            movesText.text = "Moves: " + totalMoves + "/" + maxMoves;

            if (IsSolved()) {
                readyToPlay = false;

                if (GameManager.instance.Medals == 0) {
                    GameManager.instance.Medals++;
                    GameManager.instance.DisplayEndPanel();
                }
                else {
                    GameManager.instance.AdvancedMedalOne = true;
                    GameManager.instance.DisplayAdvancedPanel();
                }
            }
            else if (totalMoves == maxMoves) {
                AudioSource.PlayClipAtPoint(soundLose, Camera.main.transform.position);
                isGameOver = true;
                readyToPlay = false;
                GameManager.instance.DisplayLosePanel();
            }

			yield return new WaitForSeconds(0.1f);
		}

		isUncovering = false;
		yield return 0;
	}


	bool IsSolved() {
		foreach(GameObject g in GameObject.FindGameObjectsWithTag("Card")) {
			if(!g.GetComponent<CardProperties>().Solved)
				return false;
		}
		return true;
	}
	

	void Update () {
		if (isDealing || isGameOver)
			return;

		if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isTouching && !isUncovering && uncoveredCards < 2) {
			isTouching = true;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

			// we hit a card
			if (hit.collider != null) {
				if(!hit.collider.GetComponent<CardProperties>().Selected) {
					// and its not one of the already solved ones
					if(!hit.collider.GetComponent<CardProperties>().Solved) {
						// uncover it
                        StartCoroutine(uncoverCard(hit.collider.transform, true));
						selectedCards[uncoveredCards] = hit.collider.transform;
						selectedCards[uncoveredCards].GetComponent<CardProperties>().Selected = true;
						uncoveredCards += 1;
					}
				}
			}
		}
		else {
			isTouching = false;
		}
	}


	Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
		// Get A,B,C of first line - points : ps1 to pe1
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;
		float C1 = A1*ps1.x+B1*ps1.y;
		
		// Get A,B,C of second line - points : ps2 to pe2
		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;
		float C2 = A2*ps2.x+B2*ps2.y;
		
		// Get delta and check if the lines are parallel
		float delta = A1*B2 - A2*B1;
		if(delta == 0)
			return new Vector2();
		
		// now return the Vector2 intersection point
		return new Vector2(
			(B2*C1 - B1*C2)/delta,
			(A1*C2 - A2*C1)/delta
			);
	}


    public void Play() {
        if (readyToPlay) {
            if (!(isDealing || isUncovering)) {
                AudioSource.PlayClipAtPoint(soundButtonClick, Camera.main.transform.position);
                
                isGameOver = false;
                PrepareNewGame();
                CreateDeck();
            }
        }
    }
}
