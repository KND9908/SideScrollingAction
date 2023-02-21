using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitCollider : MonoBehaviour
{
    [SerializeField]
    GameObject GO_Player;
    Player _Player => GO_Player.GetComponent<Player>();

    private void OnTriggerEnter(Collider other)
    {
        //ダメージ処理を行った後一定時間無敵にする
        if (other.CompareTag("enemy"))
        {
            //ノックバック
            if (!_Player.GetSetGodMode)
            {
                StartCoroutine(_Player.Damaged());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
