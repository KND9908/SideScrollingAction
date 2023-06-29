using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// プレイヤーが地面と着地をしているかを判定するスクリプト　プレイヤー内に接地判定用コライダーを用意し、そのコンポーネントに追加して活用する
/// </summary>
public class PlayerCensorGround : MonoBehaviour
{
    public bool OnGround = false;

    GameObject OnObject;

    private void Update()
    {
        //乗っている床が破棄されているか否かでOnGroundを判定する
        if (OnObject == null)
        {
            OnGround = false;
        }
        else
        {
            OnGround = true;
        }

    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            OnObject = null;
        }
    }
    private void OnTriggerStay(Collider col)
    {
        // 足元に地面判定用コライダーをつけて、地面と接触していたら isground を true にする
         if (col.gameObject.CompareTag("Ground"))
        {
            OnObject = col.gameObject;
        }
    }
}
