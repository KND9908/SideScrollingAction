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
    private GameObject ObjEnemy;
    
    private Enemy ScrEnemy;

    [Tooltip("��x�ɃX�|�[��������G�̍ő吔")]
    [SerializeField]
    private int SpawnMax = 0;

    //������������ꍇ�A1�̓�����̏����ɂ����鎞�ԊԊu
    private float SpawnBetween = 0;

    //�X�|�i�[�̋N���𔻒�
    private bool Activate = false;
    [Tooltip("�Q�[���}�l�[�W���[�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject GameManager;
    public GameObject GetSetGameManager { get { return GameManager; } set { GameManager = value; } }

    [Tooltip("�v���C���[�@�������Q�Ƃ��邽�߂Ɏg�p�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject Player;
    public GameObject GetSetPlayer { get { return Player; } set { Player = value; } }
    protected Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();

    [Tooltip("����I�u�W�F�N�g�͈̔͂��������铮����������ꍇ�A�Y���̏��I�u�W�F�N�g���w��")]
    [SerializeField]
    protected GameObject OnObject;
    public GameObject GetSetOnObject { get { return OnObject; } set { OnObject = value; } }

    //�G���X�|�[�������鎞�Ԋ��o
    const float SpawnTime = 1.0f;

    private void Start()
    {
        ScrEnemy = ObjEnemy.GetComponent<Enemy>();
        //�X�|�[������G�G�l�~�[�ɂ͂��̃X�|�i�[�Őݒ肵���l��n��
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
