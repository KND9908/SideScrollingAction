using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public float speed = 10.0f; // ��̑��x
    public float angle = 45.0f; // �������̊p�x
    public float gravity = 9.8f; // �d�͂̒l

    private Vector3 target; // �ڕW�ʒu
    private Vector3 initialVelocity; // �����x
    private float horizontalDistance; // �n�_�ƖڕW�_�̐�������
    private float verticalDistance; // �n�_�ƖڕW�_�̍�����
    private float verticalVelocity; // �����x��y����
    private float time; // �₪�ڕW�ʒu�ɓ��B���鎞��

    private float elapsedTime; // �o�ߎ���

    // ��𔭎˂���֐�
    public void Fire(Vector3 target)
    {
        this.target = target;

        // �����x���v�Z
        float velocity = speed / Mathf.Cos(angle * Mathf.Deg2Rad);

        // �n�_�ƖڕW�_�̍��������߂�
        Vector3 difference = target - transform.position;

        // �n�_�ƖڕW�_�̐����������v�Z
        horizontalDistance = difference.magnitude;
        time = horizontalDistance / velocity;

        // �n�_�ƖڕW�_�̍��������v�Z
        verticalDistance = difference.y;

        // �����x��y�������v�Z
        verticalVelocity = (verticalDistance - 0.5f * gravity * time * time) / time;

        // �����x���x�N�g���ɕϊ�
        initialVelocity = difference.normalized * velocity;
        initialVelocity.y = verticalVelocity;

        elapsedTime = 0.0f;
    }

    // Update�Ŋ���ړ�����
    private void Update()
    {
        if (elapsedTime < time)
        {
            Vector3 position = transform.position;
            float height = position.y;
            position += initialVelocity * Time.deltaTime;
            position.y = height - 0.5f * gravity * Time.deltaTime * Time.deltaTime;
            transform.position = position;

            // ��������`�����߂Ɋ���X����
            Vector3 direction = initialVelocity;
            direction.y -= gravity * elapsedTime;
            transform.rotation = Quaternion.LookRotation(direction);

            elapsedTime += Time.deltaTime;
        }
        else
        {
            // ����폜����
            Destroy(gameObject);
        }
    }
}
