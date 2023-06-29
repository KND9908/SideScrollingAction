using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // �W�����v��
    public float jumpPower = 10f;

    // �v���C���[�Ƃ̐ڐG����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �ڐG�����I�u�W�F�N�g���v���C���[�̏ꍇ�̓W�����v�͂�^����
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            playerRb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        }
    }
}