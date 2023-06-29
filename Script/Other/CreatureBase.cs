using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureBase : MonoBehaviour
{
    [Tooltip("�I�u�W�F�N�g��HP")]
    [SerializeField]
    protected int _MaxHP = 0;
    //�X�|�i�[����l��ݒ肷��p��GetSet�i��{Set�����g��Ȃ��̂�Get�͂��łɎ����j
    public int GetSetMaxHP { get => _MaxHP; set => _MaxHP = value; }

    [SerializeField]
    [Tooltip("�v���C���[�̌��݂�HP")]
    protected int _NowHP;
    public int GetSetNowLife { get => _NowHP; set => _NowHP = value; }

    [Tooltip("�ړ�����")]
    [SerializeField]
    protected float _MoveSpeed = 0.1f;
    public float GetSetMoveVec { get => _MoveSpeed; set => _MoveSpeed = value; }

    [Tooltip("���G���ԁ@�C���X�y�N�^�[����̂ݕҏW�ł���悤�ɂ��Ă܂�")]
    [SerializeField]
    protected float _GodModeTime = 0.1f;

    //���G��ԃt���O
    protected bool _GodMode = false;
    public bool GetSetGodMode { get => _GodMode; set => _GodMode = value; }

    //�n�ʂɂ��邩�̊m�F
    protected bool _OnGround = false;
    public virtual bool GetSetOnGround { get => _OnGround; set => _OnGround = value; }

    protected float _DamageWaitTime = 0;

    protected bool _DeathFlag = false;
    public bool GetSetDeathFlag { get => _DeathFlag; set => _DeathFlag = value;}

    //�d�͂𐳌��g�ł��������������邽�߂̃J�E���g
    protected int _GravityFrameCnt = 0;
    //�d�͂̑傫���iRigidBody.velocity�ɐݒ肷�邱�Ƃ�z��)
    protected float _GravityVelocity = 0;
    //�W�����v�����̔���
    protected bool _JumpNow = false;

    [Tooltip("�Q�[���}�l�[�W���[�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject _GameManager;
    public GameObject GetSetGameManager { get=> _GameManager; set => _GameManager = value;}
    protected Gamemanager _CompGamenager => _GameManager.GetComponent<Gamemanager>();

    [Tooltip("�I�u�W�F�N�g�ɂ�����d�͂̏d��")]
    [SerializeField]
    protected float _GravityPower = 9.8f;
    protected virtual void Gravity()
    {
        if (!_OnGround)
        {
            if (_GravityFrameCnt < 90)
                _GravityFrameCnt += 3;
            _GravityVelocity = _GravityPower * -1 * Mathf.Sin(_GravityFrameCnt * Mathf.Deg2Rad);
        }
        else
        {
            _GravityFrameCnt = 0;
        }
    }
    protected virtual void GodModeCheck()
    {
        //���G��Ԃ̉����`�F�b�N
        if (_GodMode)
        {
            _DamageWaitTime += UnityEngine.Time.deltaTime;
            //���G���ԉ���
            if (_DamageWaitTime > _GodModeTime)
            {
                _GodMode = false;
            }
        }
    }
    protected virtual void Move() { }
}
