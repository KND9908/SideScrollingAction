using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    [SerializeField]
    private GameObject _ObjEnemy;
    private Enemy _ScrEnemy => _ObjEnemy.GetComponent<Enemy>();
    void Update()
    {
        if (_ScrEnemy.GetSetDeathFlag)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
