using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // ジャンプ力
    public float jumpPower = 10f;

    // プレイヤーとの接触判定
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 接触したオブジェクトがプレイヤーの場合はジャンプ力を与える
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            playerRb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        }
    }
}