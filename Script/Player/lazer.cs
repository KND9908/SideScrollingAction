using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lazer : MonoBehaviour
{
    //���[�U�[�̑��x
    public float speed = 0.5f;

    //�ړ�����
    public float moveAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���[�U�[��O�i������
        transform.Translate(0, 0, speed);

        //�ړ��ʂ����Z����
        moveAmount += speed;

        //�ړ��ʂ�100�𒴂����ꍇ�f���[�g
        if (moveAmount > 100)
        {
            Destroy(gameObject);
        }
    }

    //��ʊO�֏o���ꍇ���̃I�u�W�F�N�g���f���[�g����
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
