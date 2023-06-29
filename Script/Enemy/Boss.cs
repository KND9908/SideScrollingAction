using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using DG.Tweening;

public class Boss : EnemyBase
{
#if UNITY_EDITOR
    //�ݒ肳�ꂽ�p�����[�^���C���X�y�N�^�[�ォ�猩���悤�ɂ���
    [CustomEditor(typeof(EnemyBase))]
#endif
    // �G�l�~�[��GameObject
    [SerializeField]
    [Tooltip("�G�l�~�[��GameObject")]
    private GameObject _NowMode;

    // �U�������ۂ�
    private bool _AttackAction = false;

    // �L�����N�^�[�̈ړ��������E���ۂ�
    private bool _VisualInversion = false;

    // �U���J�n����̎���
    private float _Time = 0;

    // _BossDead���\�b�h����񂾂��ĂԂ��߂̃t���O
    private bool _CallOnceBossFinFlag = false;

    // �R���|�[�l���g
    private Animator _Animator => GetComponent<Animator>();

    // �L�����N�^�[�̈ړ���
    private float _Movevec = 0;

    [SerializeField]
    [Tooltip("�ړ���velocity�̗ʂ̑傫��")]
    private float _MoveVelocity = 0;
    //RigidBody�ɕt������Velocity
    private Vector3 _SetVelocity;

    //���݂̃A�N�V����
    Tween _NowAction;

    private void Update()
    {
        //���S�t���O�������Ă���Ȃ珈�����s��Ȃ�
        if (!_JumpNow && !_DeathFlag)
            //Gravity();

        //���]����
        _VisualInversionCheck();

        //���݃��b�N���łȂ���΁A�ȉ��U�����[�V�������s��
        if (!_CompGamenager.KeyLock)
        {
            //���ݍs�����łȂ���΁A�ȉ��U�����[�V�������s��
            if (!_AttackAction)
                _Time += UnityEngine.Time.deltaTime;
            const float WAITTIME = 2.0f;
            //���ݍs�����łȂ���΁A�ȉ��U�����[�V�������s��
            if (!_AttackAction && _Time > WAITTIME)
            {
                _AttackMoveJudge();

                GodModeCheck();
            }
        }
        if (_MaxHP < 0 && !_CallOnceBossFinFlag)
        {
            _BossDead();
        }

        //_SetVelocity.y = _OnGround ? 0 : _GravityVelocity;

        //_Rigidbody.velocity = new Vector3(_SetVelocity.x, _SetVelocity.y, 0);
    }
    //�ǂ̍U�������邩����
    private void _AttackMoveJudge()
    {
        _Time = 0;
        _AttackAction = true;
        int rnd = Random.Range(1, 3);//�ԍ��ɂ���ă����_���ɍU�����삷��
        if (rnd == 1)
            StartCoroutine(MoveToPosition(_Player.transform.position.x));
        else if (rnd == 2)
            StartCoroutine(JumpToPosition(_Player.transform.position.x));
    }
    private void _BossDead()
    {
        //�����A�N�V�������Ȃ�L�����Z������
        if (_NowAction != null)
            _NowAction.Kill();
        _CallOnceBossFinFlag = true;

    }

    /// <summary>
    /// �w�肵���ʒu�܂ňړ�����R���[�`��
    /// </summary>
    /// <param name="pos">�ړ���̈ʒu</param>
    public IEnumerator MoveToPosition(float pos)
    {
        // �v���C���[���G�̍����ɂ��邩�E���ɂ��邩�𔻒肵�A�ړ����������肷��
        Vector3 moveDir = (pos - transform.position.x > 0) ? Vector3.right : Vector3.left;

        // �w�肳�ꂽ�ʒu�ֈړ�����O�ɏ������ɉ�����
        _NowAction = transform.DOMoveX(transform.position.x + ((-1) * moveDir.x * transform.localScale.x * 0.3f), 0.5f).OnComplete(() =>
        {
            // �w�肳�ꂽ�ʒu�܂ňړ�����
            _NowAction = transform.DOMoveX(pos, Mathf.Abs(pos - transform.position.x) / _MoveVelocity).OnComplete(() =>
            {
                // �U���������������Ƃ������t���O�𗧂Ă�
                _AttackAction = false;
            });
        });

        yield break;
    }



    private void _VisualInversionCheck()
    {
        if (_Movevec < 0)
        {
            _VisualInversion = true;
        }
        else if (_Movevec > 0)
        {
            _VisualInversion = false;
        }
        //�v���C���[�̌�����ς���
        if (_VisualInversion)
        {
            _NowMode.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            _NowMode.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    protected override void Gravity()
    {
        //�d�͉����x��RigidBody�̊���̂��̂ł͂Ȃ������g�ŕ\�����������ߎ���
        if (!_JumpNow)
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

    //�w��ʒu�܂ŃW�����v����R���[�`��
    public IEnumerator JumpToPosition(float pos)
    {
        //�W�����v���t���O�𗧂Ă�
        _JumpNow = true;

        //�W�����v�̍���������
        float jumpHeight = 0.5f;

        //�W�����v�̎��Ԃ�����
        float jumpTime = 0.5f;

        //�W�����v�̏���������
        float jumpVelocity = jumpHeight / jumpTime;

        //�W�����v�̏�����^����
        _SetVelocity.y = jumpVelocity;

        //�W�����v�̏�����^������A�d�͂�^����
        //yield return new WaitForSeconds(jumpTime);
        _SetVelocity.y = _GravityVelocity;

        //�W�����v���t���O��܂�
        _JumpNow = false;

        // �U���������������Ƃ������t���O�𗧂Ă�
        _AttackAction = false;

        yield break;
    }


    private void OnTriggerEnter(Collider other)
    {
        //�_���[�W�������s�������莞�Ԗ��G�ɂ���
        if (other.gameObject.CompareTag("guntret"))
        {
            if (!_GodMode)
            {
                int Damage = other.GetComponent<Guntret>().GetSetAttackPower;
                _MaxHP -= Damage;
                _GodMode = true;
                VisibleDamage(Damage);
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _OnGround = true;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _OnGround = false;
        }
    }
}
