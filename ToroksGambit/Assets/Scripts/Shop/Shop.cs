using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private int numPiecesOnSale;
    [SerializeField] private GameObject prefab;
    private GameObject[] inStock;

    void OnEnable()
    {

        for (int i = 0; i < numPiecesOnSale; i++)
        {
            int num = Random.Range(0, 5); // no king
            GameObject newStock = Instantiate(prefab);
            newStock.transform.SetParent(gameObject.transform, false);
            PieceUI p = newStock.GetComponent<PieceUI>();
            //p.SetType(num);
            p.type = num;
        }

        inStock = GameObject.FindGameObjectsWithTag("StoreStock");
    }

    void OnDisable()
    {
        for (int i = 0; i < numPiecesOnSale; i++)
        {
            Destroy(inStock[i]);
        }
    }
}
