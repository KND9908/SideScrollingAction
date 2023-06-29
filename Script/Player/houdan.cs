using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class houdan : MonoBehaviour
{
    //着地地点の座標
    public Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //着地地点へ弧を描くように移動する
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.1f);
    }

    
}
