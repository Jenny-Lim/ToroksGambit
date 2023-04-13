using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChessGameButton : MonoBehaviour
{

    [SerializeField] private AudioClip clip;
    public void OnStartChessGameButtonPressed()
    {

        GameStateManager.instance.ChangeGameState(GameStateManager.GameState.game);
        Inventory.instance.SlideHideInventoryPanel();
        Inventory.instance.DisableDeployUI();
        Inventory.instance.DisableModifiers();
        SoundObjectPool.instance.GetPoolObject().Play(clip);
        PhysicalShop.instance.pieceDescriptionObject.SetActive(false);
        this.gameObject.SetActive(false);

    }
}
