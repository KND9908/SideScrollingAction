using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCensorGround : MonoBehaviour
{
    public bool OnGround = false;
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            OnGround = false;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        // 足元に地面判定用コライダーをつけて、地面と接触していたら isground を true にする
         if (col.gameObject.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }
}
