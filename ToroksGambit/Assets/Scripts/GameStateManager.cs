using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public enum GameState
    {
        deployment,
        game,
        shop
    }
    [SerializeField] private GameState currentState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.deployment:
                Inventory.instance.InventoryUpdate();
                break;

            case GameState.game:
                Board.instance.BoardUpdate();
                break;
            case GameState.shop:
                //ShopUpdate();
                break;
        }
    }
}
