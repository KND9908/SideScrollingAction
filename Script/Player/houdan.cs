using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class houdan : MonoBehaviour
{
    //���n�n�_�̍��W
    public Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���n�n�_�֌ʂ�`���悤�Ɉړ�����
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.1f);
    }

    
}
