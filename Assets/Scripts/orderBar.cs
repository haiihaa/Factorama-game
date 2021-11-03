using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class orderBar : MonoBehaviour
{
    private const int maxOrderOnBar = 5;
    // Maximum order placed on the orderBar
    private Vector3 firstPos = new Vector3(0, 90, 0);
    private static Vector3 nextPosScale = new Vector3(0, -50, 0);
    public GameObject cardInPrefab; // Card template in prefab
    public GameObject parent; // Set parent to easily initiate position of cards
    private Vector3 nextPos;
    // Position of the next order card displayed on the orderBar
    private float randomTimeForNextCard;
    // Random time for the next card to appear on the orderBar

    private class Card
    {
        public GameObject card;
        public string name;
        public Card(GameObject card, string name)
        {
            this.card = card;
            this.name = name;
        }
    }
    private static List<Card> cards = new List<Card>();
    // List of cards currently displayed on bar

    // STORE ITEMS THAT ARE ON SHELVES
    private class ItemOnShelf
    {
        public Item item;
        public bool orderMade;
        public ItemOnShelf(Item item)
        {
            this.item = item;
            this.orderMade = false;
        }
    }
    private static List<ItemOnShelf> onShelves = new List<ItemOnShelf>();

    // Start is called before the first frame update
    void Start()
    {
        nextPos = firstPos;
        randomTimeForNextCard = Random.Range(5, 6);
    }

    // Update is called once per frame
    void Update()
    {
        // If the randomTimeForNextCard == 0, update next time
        // Implement next order card if orderBar isn't full yet
        if (randomTimeForNextCard <= 0)
        {
            randomTimeForNextCard = Random.Range(1, 3);
            //Debug.Log(randomTimeForNextCard);
            if (cards.Count < maxOrderOnBar)
            {
                // Get new card on bar
                newCard();
            }
        }

        // Update the time
        randomTimeForNextCard -= Time.deltaTime;
    }

    // Add card to orderBar
    private void newCard()
    {
        // Loop the items on shelf to make order
        foreach (var item in onShelves)
        {
            // Only make order on those that havent been made
            if (!item.orderMade)
            {
                // Get item and price from the storage
                //string itemName
                //float price
                string name = item.item.getName();
                float reward = item.item.getReward();

                // Update nextPos
                nextPos = firstPos + cards.Count * nextPosScale;

                cards.Add(new Card(Instantiate(cardInPrefab, nextPos, cardInPrefab.transform.rotation) as GameObject, name));
                int index = cards.Count - 1;
                cards[index].card.transform.SetParent(parent.transform, false);

                Text[] texts = cards[index].card.GetComponentsInChildren<Text>();
                // Assigning name and price
                texts[0].text = name;
                texts[1].text = string.Format("{0:c}", reward);
                // Set status to notify orderBar
                item.orderMade = true;
                break;
            }
        }
    }

    // Record data of new item to shelves
    public static void recordItemOnShelves(GameObject item)
    {
        Item newItem = item.GetComponent<Item>();
        onShelves.Add(new ItemOnShelf(newItem));
    }

    // Remove data of item from shelves
    public static void removeCard(Item item)
    {
        //Item toRemove = item.GetComponent<Item>();
        string toRemove = item.getName();

        for (int i = 0; i < onShelves.Count; i++)
        {
            if (onShelves[i].orderMade && onShelves[i].item.getName() == toRemove)
            {
                onShelves.RemoveAt(i);
                break;
            }
        }

        // Get the index of card to remove
        int index = 0;
        for (int c = 0; c < cards.Count; c++)
        {
            if (cards[c].name == toRemove)
            {
                // Remove from card list and destroy on screen
                Destroy(cards[c].card);
                cards.RemoveAt(c);
                index = c;
                break;
            }
        }
        // Update the positions of the remaining cards
        if (cards.Count != 0 && index < cards.Count)
        {
            for (int j = index; j < cards.Count; j++)
            {
                cards[j].card.transform.localPosition -= nextPosScale;
            }
        }
    }

    public static void clearOrderBar()
    {
        for (int c = 0; c < cards.Count; c++)
        {
            // Remove from card list and destroy on screen
            Destroy(cards[c].card);
            //cards.RemoveAt(c);
        }
        cards.Clear();
    }

    // Method to check whether clicked-item is on the orderBar
    // If not - fire the panel to notify
    public static bool checkOrder(string checkName)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].name == checkName)
            {
                return true;
            }
        }
        return false;
    }
}
