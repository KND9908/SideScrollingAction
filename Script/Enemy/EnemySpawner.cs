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
    private GameObject _ObjEnemy;
    
    private Enemy _ScrEnemy;

    [Tooltip("一度にスポーンさせる敵の最大数")]
    [SerializeField]
    private int _SpawnMax = 0;

    //複数召喚する場合、1体当たりの召喚にかける時間間隔
    private float _SpawnBetween = 0;

    //スポナーの起動を判定
    private bool _Activate = false;
    [Tooltip("ゲームマネージャー　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject _GameManager;
    public GameObject GetSetGameManager { get { return _GameManager; } set { _GameManager = value; } }

    [Tooltip("プレイヤー　距離を参照するために使用　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject _Player;
    public GameObject GetSetPlayer { get { return _Player; } set { _Player = value; } }
    protected Gamemanager _ScrGamemanager => _GameManager.GetComponent<Gamemanager>();

    [Tooltip("特定オブジェクトの範囲を往復する動きをさせる場合、該当の床オブジェクトを指定")]
    [SerializeField]
    protected GameObject _OnObject;
    public GameObject GetSetOnObject { get { return _OnObject; } set { _OnObject = value; } }

    //敵をスポーンさせる時間感覚
    const float SPAWNTIME = 1.0f;
    private void Start()
    {
        _ScrEnemy = _ObjEnemy.GetComponent<Enemy>();
        Debug.Log("WakeUp" + gameObject.name);
    }
    private void Update()
    {
        if (!_ScrGamemanager.KeyLock && _Activate)
        {
            if (this.transform.childCount < _SpawnMax)
            {
                _SpawnBetween += Time.deltaTime;
                if (_SpawnBetween > SPAWNTIME)
                {
                    _SpawnBetween = 0;
                    Spawn();
                }
            }
        }
    }
    private void Spawn()
    {
        //スポーンする敵エネミーにはこのスポナーで設定した値を渡す
        _ScrEnemy.GetSetPlayer = GetSetPlayer;
        _ScrEnemy.GetSetGameManager = GetSetGameManager;
        _ScrEnemy.SetOnObject = GetSetOnObject;

        GameObject enemy;
        enemy = (GameObject)Instantiate(_ObjEnemy, this.transform.position, Quaternion.identity);
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
             _Activate = true;
            Debug.Log("Spawner ON");
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _Activate = false;
            Debug.Log("Spawner OFF");
            ChildDestroy();
        }
    }
}
