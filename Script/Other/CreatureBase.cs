using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureBase : MonoBehaviour
{
    [Tooltip("�I�u�W�F�N�g��HP")]
    [SerializeField]
    protected int MaxHP = 0;
    //�X�|�i�[����l��ݒ肷��p��GetSet�i��{Set�����g��Ȃ��̂�Get�͂��łɎ����j
    public int GetSetMaxHP { get { return MaxHP; } set { MaxHP = value; } }

    [SerializeField]
    [Tooltip("�v���C���[�̌��݂�HP")]
    protected int NowHP;
    public int GetSetNowLife { get { return NowHP; } set { NowHP = value; } }

    [Tooltip("�ړ�����")]
    [SerializeField]
    protected float MoveSpeed = 0.1f;
    public float GetSetMoveVec { get { return MoveSpeed; } set { MoveSpeed = value; } }

    [Tooltip("���G���ԁ@�C���X�y�N�^�[����̂ݕҏW�ł���悤�ɂ��Ă܂�")]
    [SerializeField]
    protected float GodModeTime = 0.1f;

    //���G��ԃt���O
    protected bool GodMode = false;
    public bool GetSetGodMode { get { return GodMode; } set { GodMode = value; } }

    //�n�ʂɂ��邩�̊m�F
    protected bool OnGround = false;
    public bool GetSetOnGround { get { return OnGround; } set { OnGround = value; } }

    protected float DamageWaitTime = 0;

    protected bool DeathFlag = false;
    public bool GetSetDeathFlag { get { return DeathFlag; } set { DeathFlag = value; } }

    //�d�͂𐳌��g�ł��������������邽�߂̃J�E���g
    protected int GravityFrameCnt = 0;
    //�d�͂̑傫���iRigidBody.velocity�ɐݒ肷�邱�Ƃ�z��)
    protected float GravityVelocity = 0;
    //�W�����v�����̔���
    protected bool JumpNow = false;

    [Tooltip("�Q�[���}�l�[�W���[�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject GameManager;
    public GameObject GetSetGameManager { get { return GameManager; } set { GameManager = value; } }
    protected Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();

    [Tooltip("�I�u�W�F�N�g�ɂ�����d�͂̏d��")]
    [SerializeField]
    protected float GravityPower = 9.8f;
    protected virtual void Gravity()
    {
        if (!OnGround)
        {
            if (GravityFrameCnt < 90)
                GravityFrameCnt += 3;
            GravityVelocity = GravityPower * -1 * Mathf.Sin(GravityFrameCnt * Mathf.Deg2Rad);
        }
        else
        {
            GravityFrameCnt = 0;
        }
    }
    protected virtual void GodModeCheck()
    {
        //���G��Ԃ̉����`�F�b�N
        if (GodMode)
        {
            DamageWaitTime += UnityEngine.Time.deltaTime;
            //���G���ԉ���
            if (DamageWaitTime > GodModeTime)
            {
                GodMode = false;
            }
        }
    }
    protected virtual void Move() { }
}
