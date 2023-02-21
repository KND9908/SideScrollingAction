using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageView : MonoBehaviour
{
    [Tooltip("�\�����Ă��鎞��")]
    [SerializeField]
    private float ViewTime = 2.0f;

    [Tooltip("�I�u�W�F�N�g�̕\������")]
    [SerializeField]
    private float UpAcceleration = 0.05f;

    float time = 0;
    void Update()
    {
        if (time < ViewTime)
        {
            transform.position += new Vector3(0, UpAcceleration, 0);
            time += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
