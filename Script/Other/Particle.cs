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
        if (_Particle.isStopped) //パーティクルが終了したか判別
        {
            Destroy(this.gameObject);//パーティクル用ゲームオブジェクトを削除
        }
    }
}