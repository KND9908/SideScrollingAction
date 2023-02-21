using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("穴に落ちたときのリスポーン地点")]
    private GameObject RespawnPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = RespawnPos.transform.position;
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
