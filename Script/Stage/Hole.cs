using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("穴に落ちたときのリスポーン地点")]
    private GameObject _RespawnPos;

    //プレイヤーのオブジェクト
    private GameObject _Player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = new Vector3(_RespawnPos.transform.position.x,_RespawnPos.transform.position.y,0);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
