using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChessGameButton : MonoBehaviour
{
    public void OnStartChessGameButtonPressed()
    {
        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.game);
        Inventory.instance.SlideHideInventoryPanel();
        this.gameObject.SetActive(false);
        Inventory.instance.DisableDeployUI();
        Inventory.instance.DisableModifiers();
    }
}
