using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taihou : MonoBehaviour
{
    //�C�e�̃I�u�W�F�N�g
    public GameObject houdan;

    //������s�����ۂ�
    public bool isAction = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //������s���ꍇ
        if (isAction)
        {
            //�������ŖC�e�̃I�u�W�F�N�g�𐶐�����
            if (Time.frameCount % 60 == 0)
            {
                //�q�I�u�W�F�N�g�����݂��Ȃ��ꍇ
                if (transform.childCount == 0)
                {
                    //�C�e�̃I�u�W�F�N�g�𐶐�����
                    GameObject houdanObj = Instantiate(houdan);
                    //���������I�u�W�F�N�g�͎q�I�u�W�F�N�g�ɂ���
                    houdanObj.transform.parent = transform;

                    //�C�e�̃I�u�W�F�N�g�̈ʒu��ݒ肷��
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
