using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    public int tickets;
    public Text ticketsTxt;
    public static Currency instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ticketsTxt = gameObject.GetComponent<Text>();
        ticketsTxt.enabled = false;
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
}
