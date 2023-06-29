using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taihou : MonoBehaviour
{
    //砲弾のオブジェクト
    public GameObject houdan;

    //動作を行うか否か
    public bool isAction = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //動作を行う場合
        if (isAction)
        {
            //一定周期で砲弾のオブジェクトを生成する
            if (Time.frameCount % 60 == 0)
            {
                //子オブジェクトが存在しない場合
                if (transform.childCount == 0)
                {
                    //砲弾のオブジェクトを生成する
                    GameObject houdanObj = Instantiate(houdan);
                    //生成したオブジェクトは子オブジェクトにする
                    houdanObj.transform.parent = transform;

                    //砲弾のオブジェクトの位置を設定する
                    houdanObj.transform.position = transform.position;
                    houdanObj.transform.rotation = transform.rotation;
                }
            }
        }
    }
    private void OnBecameInvisible()
    {
        isAction = false;
    }

    private void OnBecameVisible()
    {
        isAction = true;
    }
}
