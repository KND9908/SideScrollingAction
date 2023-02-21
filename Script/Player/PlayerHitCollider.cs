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
        //�_���[�W�������s�������莞�Ԗ��G�ɂ���
        if (other.CompareTag("enemy"))
        {
            //�m�b�N�o�b�N
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
