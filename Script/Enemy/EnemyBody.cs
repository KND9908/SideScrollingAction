using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    [SerializeField]
    private GameObject ObjEnemy;
    private Enemy ScrEnemy => ObjEnemy.GetComponent<Enemy>();
    void Update()
    {
        if (ScrEnemy.GetSetDeathFlag)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
