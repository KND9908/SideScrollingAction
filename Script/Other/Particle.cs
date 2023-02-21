using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Particle : MonoBehaviour
{
    ParticleSystem _Particle;
    private void Start()
    {
        _Particle = this.GetComponent<ParticleSystem>();
        _Particle.Play();
    }

    private void Update()
    {
        if (_Particle.isStopped) //�p�[�e�B�N�����I������������
        {
            Destroy(this.gameObject);//�p�[�e�B�N���p�Q�[���I�u�W�F�N�g���폜
        }
    }
}