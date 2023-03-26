using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    public int tickets;
    public Text ticketsTxt;
    public GameObject ticketTextObject;
    public GameObject ticketBackgroundObject;
    public static Currency instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ticketTextObject = gameObject;
        ticketsTxt = gameObject.GetComponent<Text>();
        ticketTextObject.SetActive(false);
        ticketBackgroundObject.SetActive(false);
    }

    void Update()
    {
        ticketsTxt.text = "Tickets Owned: " + tickets.ToString();
    }

    public void AddToCurrency(int amount)
    {
        tickets += amount;
    }

    public void SubtractFromCurrency(int amount)
    {
        tickets -= amount;
    }

    public void GetReward(int level) // idk im just thinking, might not be in here
    {
        int amount = level * 6;
        Debug.Log("Amount rewarded: " + amount);
        instance.AddToCurrency(amount);
    }

    public void SetCurrency(int amount)
    {
        tickets = amount;
    }

    //add by jordan so that when a ticket touches this thing it will add to the counter and get rid of the ticket
    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("got collision");
        if (collision.gameObject.CompareTag("Ticket"))
        {
            AddToCurrency(1);
            UIParticleSystem.instance.DeleteParticle(collision.gameObject);
        }
    }
}
