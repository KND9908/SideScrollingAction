using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField]
    [Tooltip("���ɗ������Ƃ��̃��X�|�[���n�_")]
    private GameObject _RespawnPos;

    //�v���C���[�̃I�u�W�F�N�g
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
