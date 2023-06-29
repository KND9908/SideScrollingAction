using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lazer : MonoBehaviour
{
    //レーザーの速度
    public float speed = 0.5f;

    //移動総量
    public float moveAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //レーザーを前進させる
        transform.Translate(0, 0, speed);

        //移動量を加算する
        moveAmount += speed;

        //移動量が100を超えた場合デリート
        if (moveAmount > 100)
        {
            Destroy(gameObject);
        }
    }

    //画面外へ出た場合このオブジェクトをデリートする
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
