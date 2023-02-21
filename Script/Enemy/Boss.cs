using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class Boss : EnemyBase
{
#if UNITY_EDITOR
    //�ݒ肳�ꂽ�p�����[�^���C���X�y�N�^�[�ォ�猩���悤�ɂ���
    [CustomEditor(typeof(EnemyBase))]
#endif
    [SerializeField]
    private GameObject NowMode;

    private bool AttackAction = false; //�U�������ۂ�
    private bool VisualInversion = false;

    private float Time = 0;
    private bool CallOnceBossFinFlag = false;
    private Animator _Animator => GetComponent<Animator>();

    private float Movevec = 0;

    [SerializeField]
    [Tooltip("�ړ���velocity�̗ʂ̑傫��")]
    private float MoveVelocity = 0;
    //RigidBody�ɕt������Velocity
    Vector3 SetVelocity;

    private void Update()
    {
        if (!JumpNow && !DeathFlag)
            Gravity();

        VisualInversionCheck();

        if (!ScrGamemanager.KeyLock)
        {
            if (!AttackAction)
                Time += UnityEngine.Time.deltaTime;
            const float waittime = 2.0f;
            //���ݍs�����łȂ���΁A�ȉ��U�����[�V�������s��
            if (!AttackAction && Time > waittime)
            {
                AttackMoveJudge();

                GodModeCheck();
            }
        }
        if (MaxHP < 0 && !CallOnceBossFinFlag)
        {
            BossDead();
        }

        SetVelocity.y = OnGround ? 0 : GravityVelocity;

        _Rigidbody.velocity = new Vector3(SetVelocity.x, SetVelocity.y, 0);
    }
    //�ǂ̍U�������邩����
    private void AttackMoveJudge()
    {
        Time = 0;
        AttackAction = true;
        int rnd = Random.Range(1, 2);//�ԍ��ɂ���ă����_���ɍU�����삷��
        if (rnd == 1)
            StartCoroutine(MoveToPosition(Player.transform.position.x));
    }
    protected void BossDead()
    {
        CallOnceBossFinFlag = true;
    }
    //�w�肵���|�C���g�܂ňړ�
    public IEnumerator MoveToPosition(float pos)
    {
        _Animator.SetBool("IsRun", true);

        bool keeploop = false;
        while (!keeploop)
        {
            Movevec = pos - transform.position.x < 0 ? -1 : 1;
            SetVelocity.x = Movevec * MoveVelocity;

            if ((pos > transform.position.x && Movevec < 0)
            || (pos <= transform.position.x && Movevec > 0))
                keeploop = true;
            yield return null;
        }
        _Animator.SetBool("IsRun", false);
        AttackAction = false;
        yield break;
    }
    private void VisualInversionCheck()
    {
        if (Movevec < 0)
        {
            VisualInversion = true;
        }
        else if (Movevec > 0)
        {
            VisualInversion = false;
        }
        //�v���C���[�̌�����ς���
        if (VisualInversion)
        {
            NowMode.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            NowMode.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    protected override void Gravity()
    {
        //�d�͉����x�͐����g�ŕ\�����������ߎ���
        if (!JumpNow)
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

    private void OnTriggerEnter(Collider other)
    {
        //�_���[�W�������s�������莞�Ԗ��G�ɂ���
        if (other.gameObject.CompareTag("guntret"))
        {
            if (!GodMode)
            {
                int Damage = other.GetComponent<Guntret>().GetSetAttackPower;
                MaxHP -= Damage;
                GodMode = true;
                VisibleDamage(Damage);
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            OnGround = false;
        }
    }
}
