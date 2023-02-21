using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageView : MonoBehaviour
{
    [Tooltip("表示している時間")]
    [SerializeField]
    private float ViewTime = 2.0f;

    [Tooltip("オブジェクトの表示時間")]
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
