using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    public void PieceBoughtEnd() // animation event
    {
        PhysicalShop.instance.anim.SetBool("PieceSold", false);
    }
}
