using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    public enum InventoryPieces {
        pawn,
        knight,
        bishop,
        rook,
        queen
    }

    private RectTransform rectTrans;
    [SerializeField] private TextMeshProUGUI hideShowText;
    [SerializeField] private int[] maxHeldPieces = new int[5];//the maximum number of each piece the player can have
    private int[] heldPieces = new int[5];//the amount of each piece the player has

    public void ShowInventoryPanel()
    {
        gameObject.SetActive(true);
    }

    public void HideInventoryPanel()
    {
        gameObject.SetActive(false);
    }

    private bool isShowingPanel = false;
    private bool isMoving = false;
    [SerializeField] private float travelSpeed = 1f;

    [SerializeField] private Vector3 showLocation;
    [SerializeField] private Vector3 hideLocation;

    [SerializeField] private GameObject[] PiecePrefabs;
    private Camera cam;

    [SerializeField] private int storedPiece = 1;//pawn - 0, knight - 1, bishop - 2, rook - 3, queen - 4, remove - -1

    public void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    private void Update()
    {
        //if in deploy mode
        if (isShowingPanel)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);//shoot ray using mouse from camera

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.transform.gameObject.CompareTag("Chess Board"))//if you hit a board tile
                {

                    //remove other visuals
                    foreach (GameObject obj in PiecePrefabs)
                    {
                        obj.transform.position = new Vector3(-200, 0, 0);
                    }

                    //show desired visual
                    if (storedPiece >= 0)
                    {
                        PiecePrefabs[storedPiece].transform.localPosition = hit.transform.position;
                    }



                    //if button press, place piece if can
                }
                else
                {
                    //remove other visuals
                    foreach (GameObject obj in PiecePrefabs)
                    {
                        obj.transform.position = new Vector3(-200, 0, 0);
                    }
                }
            }
            else
            {
                //remove other visuals
                foreach (GameObject obj in PiecePrefabs)
                {
                    obj.transform.position = new Vector3(-200, 0, 0);
                }
            }
        }
        
    }

    //changes the designated number of held pieces by amount
    public void AlterPiece(InventoryPieces type, int amount)
    {
        heldPieces[(int)type] += amount;
        if (heldPieces[(int)type] > maxHeldPieces[(int)type])
        {
            heldPieces[(int)type] = maxHeldPieces[(int)type];
        }
    }

    //sets the designated held amount of pieces to amount
    public void SetPiece(InventoryPieces type, int amount)
    {
        heldPieces[(int)type] = amount;
        if (heldPieces[(int)type] > maxHeldPieces[(int)type])
        {
            heldPieces[(int)type] = maxHeldPieces[(int)type];
        }
    }

    //returns how many pieces the player has of a designated type
    public int GetNumberOfPieces(InventoryPieces type)
    {
        return heldPieces[(int)type];
    }

    public void PawnButtonClicked()
    {
        storedPiece = 0;
    }

    public void KnightButtonClicked()
    {
        storedPiece = 1;
    }

    public void BishopButtonClicked()
    {
        storedPiece = 2;
    }

    public void RookButtonClicked()
    {
        storedPiece = 3;
    }

    public void QueenButtonClicked()
    {
        storedPiece = 4;
    }

    public void RemoveButtonClicked()
    {
        storedPiece = -1;
    }

    public void HideShowButtonClicked()
    {
        if (isMoving)
        {
            StopAllCoroutines();
        }
        StartCoroutine(ShowHideInventoryPanel());
    }

    private IEnumerator ShowHideInventoryPanel()
    {
        isMoving = true;

        if (isShowingPanel)
        {
            hideShowText.text = "Hide";
        }
        else
        {
            hideShowText.text = "Show";
        }

        isShowingPanel = !isShowingPanel;

        if (!isShowingPanel)
        {
            while (Vector3.Distance(rectTrans.anchoredPosition3D, hideLocation) >= 0.15)
            {
                rectTrans.anchoredPosition3D = Vector3.Lerp(rectTrans.anchoredPosition3D, hideLocation, Time.deltaTime * travelSpeed);
                yield return null;
            }
            
        }
        else
        {
            while (Vector3.Distance(rectTrans.anchoredPosition3D, showLocation) >= 0.15)
            {
                rectTrans.anchoredPosition3D = Vector3.Lerp(rectTrans.anchoredPosition3D, showLocation, Time.deltaTime * travelSpeed);
                yield return null;
            }
            
        }

        isMoving = false;
    }

    
}
