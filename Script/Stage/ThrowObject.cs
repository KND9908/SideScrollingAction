using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public float speed = 10.0f; // 岩の速度
    public float angle = 45.0f; // 放物線の角度
    public float gravity = 9.8f; // 重力の値

    private Vector3 target; // 目標位置
    private Vector3 initialVelocity; // 初速度
    private float horizontalDistance; // 始点と目標点の水平距離
    private float verticalDistance; // 始点と目標点の高さ差
    private float verticalVelocity; // 初速度のy成分
    private float time; // 岩が目標位置に到達する時間

    private float elapsedTime; // 経過時間

    // 岩を発射する関数
    public void Fire(Vector3 target)
    {
        this.target = target;

        // 初速度を計算
        float velocity = speed / Mathf.Cos(angle * Mathf.Deg2Rad);

        // 始点と目標点の差分を求める
        Vector3 difference = target - transform.position;

        // 始点と目標点の水平距離を計算
        horizontalDistance = difference.magnitude;
        time = horizontalDistance / velocity;

        // 始点と目標点の高さ差を計算
        verticalDistance = difference.y;

        // 初速度のy成分を計算
        verticalVelocity = (verticalDistance - 0.5f * gravity * time * time) / time;

        // 初速度をベクトルに変換
        initialVelocity = difference.normalized * velocity;
        initialVelocity.y = verticalVelocity;

        elapsedTime = 0.0f;
    }

    // Updateで岩を移動する
    private void Update()
    {
        if (elapsedTime < time)
        {
            Vector3 position = transform.position;
            float height = position.y;
            position += initialVelocity * Time.deltaTime;
            position.y = height - 0.5f * gravity * Time.deltaTime * Time.deltaTime;
            transform.position = position;

            // 放物線を描くために岩を傾ける
            Vector3 direction = initialVelocity;
            direction.y -= gravity * elapsedTime;
            transform.rotation = Quaternion.LookRotation(direction);

            elapsedTime += Time.deltaTime;
        }
        else
        {
            // 岩を削除する
            Destroy(gameObject);
        }
    }
}
