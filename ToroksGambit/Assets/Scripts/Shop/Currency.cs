using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
    public int tickets;
    private Text ticketsTxt;
    public static Currency instance;
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        ticketsTxt = gameObject.GetComponent<Text>();
    }

    void Update()
    {
        ticketsTxt.text = tickets.ToString();
    }

    public void AddToCurrency(int amount)
    {
        tickets += amount;
    }

    public void SubtractFromCurrency(int amount)
    {
        tickets -= amount;
    }
}
