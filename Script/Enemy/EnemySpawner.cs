using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
public class EnemySpawner:MonoBehaviour
{
#if UNITY_EDITOR
    //�X�|�[������G�l�~�[�̗v�f���X�|�i�[�̃C���X�y�N�^�[����n��
    [CustomEditor(typeof(EnemyBase))]
#endif

    [Tooltip("�X�|�[�����������G�̃v���n�u�������ɐݒ�")]
    [SerializeField]
    private GameObject _ObjEnemy;
    
    private Enemy _ScrEnemy;

    [Tooltip("��x�ɃX�|�[��������G�̍ő吔")]
    [SerializeField]
    private int _SpawnMax = 0;

    //������������ꍇ�A1�̓�����̏����ɂ����鎞�ԊԊu
    private float _SpawnBetween = 0;

    //�X�|�i�[�̋N���𔻒�
    private bool _Activate = false;
    [Tooltip("�Q�[���}�l�[�W���[�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject _GameManager;
    public GameObject GetSetGameManager { get { return _GameManager; } set { _GameManager = value; } }

    [Tooltip("�v���C���[�@�������Q�Ƃ��邽�߂Ɏg�p�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject _Player;
    public GameObject GetSetPlayer { get { return _Player; } set { _Player = value; } }
    protected Gamemanager _ScrGamemanager => _GameManager.GetComponent<Gamemanager>();

    [Tooltip("����I�u�W�F�N�g�͈̔͂��������铮����������ꍇ�A�Y���̏��I�u�W�F�N�g���w��")]
    [SerializeField]
    protected GameObject _OnObject;
    public GameObject GetSetOnObject { get { return _OnObject; } set { _OnObject = value; } }

    //�G���X�|�[�������鎞�Ԋ��o
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
        //�X�|�[������G�G�l�~�[�ɂ͂��̃X�|�i�[�Őݒ肵���l��n��
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
