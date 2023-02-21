using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemCode
    {
        None,
        Heal,
        Key,
        PowerUp,
        LifeMaxUp
    }

    [SerializeField]
    public ItemCode thisitem = ItemCode.None;
}
