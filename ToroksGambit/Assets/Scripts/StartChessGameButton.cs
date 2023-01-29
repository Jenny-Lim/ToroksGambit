using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChessGameButton : MonoBehaviour
{
    public void OnStartChessGameButtonPressed()
    {
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.game);
        this.gameObject.SetActive(false);
    }
}
