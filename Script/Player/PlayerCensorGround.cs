using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �v���C���[���n�ʂƒ��n�����Ă��邩�𔻒肷��X�N���v�g�@�v���C���[���ɐڒn����p�R���C�_�[��p�ӂ��A���̃R���|�[�l���g�ɒǉ����Ċ��p����
/// </summary>
public class PlayerCensorGround : MonoBehaviour
{
    public bool OnGround = false;

    GameObject OnObject;

    private void Update()
    {
        //����Ă��鏰���j������Ă��邩�ۂ���OnGround�𔻒肷��
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
        // �����ɒn�ʔ���p�R���C�_�[�����āA�n�ʂƐڐG���Ă����� isground �� true �ɂ���
         if (col.gameObject.CompareTag("Ground"))
        {
            OnObject = col.gameObject;
        }
    }
}
