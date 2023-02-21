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
        // �����ɒn�ʔ���p�R���C�_�[�����āA�n�ʂƐڐG���Ă����� isground �� true �ɂ���
         if (col.gameObject.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }
}
