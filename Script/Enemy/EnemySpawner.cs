using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
public class EnemySpawner:MonoBehaviour
{
#if UNITY_EDITOR
    //スポーンするエネミーの要素をスポナーのインスペクターから渡す
    [CustomEditor(typeof(EnemyBase))]
#endif

    [Tooltip("スポーンさせたい敵のプレハブをここに設定")]
    [SerializeField]
    private GameObject ObjEnemy;
    
    private Enemy ScrEnemy;

    [Tooltip("一度にスポーンさせる敵の最大数")]
    [SerializeField]
    private int SpawnMax = 0;

    //複数召喚する場合、1体当たりの召喚にかける時間間隔
    private float SpawnBetween = 0;

    //スポナーの起動を判定
    private bool Activate = false;
    [Tooltip("ゲームマネージャー　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject GameManager;
    public GameObject GetSetGameManager { get { return GameManager; } set { GameManager = value; } }

    [Tooltip("プレイヤー　距離を参照するために使用　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject Player;
    public GameObject GetSetPlayer { get { return Player; } set { Player = value; } }
    protected Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();

    [Tooltip("特定オブジェクトの範囲を往復する動きをさせる場合、該当の床オブジェクトを指定")]
    [SerializeField]
    protected GameObject OnObject;
    public GameObject GetSetOnObject { get { return OnObject; } set { OnObject = value; } }

    //敵をスポーンさせる時間感覚
    const float SpawnTime = 1.0f;

    private void Start()
    {
        ScrEnemy = ObjEnemy.GetComponent<Enemy>();
        //スポーンする敵エネミーにはこのスポナーで設定した値を渡す
        ScrEnemy.GetSetPlayer = GetSetPlayer;
        ScrEnemy.GetSetGameManager = GetSetGameManager;
        ScrEnemy.GetSetOnObject = GetSetOnObject;
    }
    private void Update()
    {
        if (!ScrGamemanager.KeyLock && Activate)
        {
            if (this.transform.childCount < SpawnMax)
            {
                SpawnBetween += Time.deltaTime;
                if (SpawnBetween > SpawnTime)
                {
                    SpawnBetween = 0;
                    Spawn();
                }
            }
        }
    }
    private void Spawn()
    {
        GameObject enemy;
        enemy = (GameObject)Instantiate(ObjEnemy, this.transform.position, Quaternion.identity);
        enemy.transform.parent = this.transform;
    }

    private void ChildDestroy()
    {
        if (this.transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Activate = true;
            Debug.Log("Spawner ON");
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Activate = false;
            Debug.Log("Spawner OFF");
            ChildDestroy();
        }
    }
}
