using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEditor;

public class Player : CreatureBase
{
#if UNITY_EDITOR
    //�ݒ肳�ꂽ�p�����[�^���C���X�y�N�^�[�ォ�猩���悤�ɂ���
    [CustomEditor(typeof(CreatureBase))]
#endif

    //���݂̃v���C�A�u���L�����N�^�[�̃R�[�h
    [SerializeField]
    private PlayerCode NowPlayerCode = PlayerCode.kit;

    //���݂̃v���C���[�L�����N�^�[
    private enum PlayerCode : int
    {
        kit,
        rail
    }

    //�v���C���[�L�����N�^�[�̏��
    private enum PlayerMode : int
    {
        Normal,
        Run,
        Beat,
        Jump,
        Fall,
        RocketPunch,
        Damage,
        SpecialMove
    }

    [SerializeField]
    [Tooltip("�v���C���[�̒ʏ펞�̃O���t�B�b�N")]
    private GameObject ObjNormal;

    [SerializeField]
    [Tooltip("�v���C���[�̑����Ă鎞�̃O���t�B�b�N")]
    private GameObject ObjRun;

    [SerializeField]
    [Tooltip("�v���C���[�̍U�����̃O���t�B�b�N")]
    private GameObject ObjBeat;

    [SerializeField]
    [Tooltip("�v���C���[�̃��P�b�g�p���`���ˎ��̃O���t�B�b�N")]
    private GameObject ObjRocketPunch;

    [SerializeField]
    [Tooltip("�v���C���[�̃_���[�W�󂯂����̃O���t�B�b�N")]
    private GameObject ObjDamage;

    [SerializeField]
    [Tooltip("�v���C���[�̕K�E�Z�������̃O���t�B�b�N")]
    private GameObject ObjSpecialMove;


    [Tooltip("�U���������̃A�j���[�V�����̃O���t�B�b�N����")]
    [SerializeField]
    private int WaitFrame;

    [SerializeField]
    [Tooltip("�v���C���[�̑������镐��@�f�t�H���g�̓K���g���b�g��ݒ肷��")]
    private GameObject Weapon;

    [SerializeField]
    [Tooltip("�v���C���[�̒��n������s���Z���T�[")]
    private GameObject GroundCensor;

    [SerializeField]
    [Tooltip("�U����")]
    public int AttackPower = 1;

    [SerializeField]
    [Tooltip("�_���[�W�󂯂��Ƃ��̓����s������؂�ւ��邽�߂̕ϐ�")]
    private GameObject InvisiObj;

    //����Ǘ��p�N���X
    private GameObject Instantitate_Weapon;

    //�v���C���[�̌��݂̃��[�h
    GameObject NowMode;

    //2�i�W�����v���������̔���
    private bool TwiceJump = false;
    //�U�������ۂ�
    private bool NowAttackAction = false;
    //�i�s�����Ɍ������߂̔��]
    private bool VisualInversion = false;
    public bool GetSetVisualInversion { get { return VisualInversion; } set { VisualInversion = value; } }

    private float JumpVelocity = 0;
    // ���݂̃v���C���[�̃��[�h
    private PlayerMode NowPlayerMode;

    // �_���[�W��������Ă��邩�ۂ�
    private bool DamageNow = false;

    //�v���C���[�̍s�����ɂ�����L�[���b�N
    private bool ActionKeyLock = false;

    //�W�����v�����ʒu
    private float JumpPos = 0;

    //�W�����v�o���鍂����MAX
    [SerializeField]
    private float MAXJump = 0;

    private float JumpCount = 0;
    public int HaveOpenKey { get; set; } = 0;

    // �Q�[���}�l�[�W���ɂ���ăL�[���b�N���������Ă��邩�ۂ�
    private bool GM_KeyLock => ScrGamemanager.KeyLock;
    private Guntret _Guntret;
    private Rigidbody _Rigidbody;
    PlayerCensorGround _PlayerCensorGround => GroundCensor.GetComponent<PlayerCensorGround>();
    private new bool OnGround => _PlayerCensorGround.OnGround;

    //RigidBody�ɕt������Velocity
    Vector3 SetVelocity;

    //�ړ����Ă������
    private float Movevec = 0;

    //HP�ɕϓ����������Ƃ��ɔ���������C�x���g
    public event System.Action<int> UpdateHP;

    private void Start()
    {
        //�����l�̑������
        NowHP = MaxHP;
        NowPlayerMode = PlayerMode.Normal;
        NowMode = ObjNormal;
        SetVelocity = Vector3.zero;

        _Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //�C�x���g���̓L�[���͂̑���𖳌��ɂ���
        if (!GM_KeyLock)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (OnGround && !JumpNow)
                {
                    JumpNow = true;
                    SetVisible(PlayerMode.Jump);
                }
                else if (!TwiceJump)
                {
                    JumpNow = true;
                    TwiceJump = true;
                    JumpCount = 0;
                    SetVisible(PlayerMode.Jump);
                }
            }
            if (Input.GetKeyDown(KeyCode.Z) && !NowAttackAction)
            {
                StartCoroutine(WeakAttack());
            }
            if (Input.GetKeyDown(KeyCode.X) && !NowAttackAction)
            {
                StartCoroutine(HeavyAttack());
            }
            if (Input.GetKeyDown(KeyCode.S) && !NowAttackAction)
            {
                StartCoroutine(SpecialMove());
            }
        }
        else
        {
            SetVelocity.x = 0;
            if (!JumpNow) SetVisible(PlayerMode.Normal);//�L�[���b�N�������Ă��鎞�͓���͂Ȃɂ��ł��Ȃ��̂Ńm�[�}����
        }

        if (JumpNow)
        {
            Jump();
            SetVelocity.y = JumpVelocity;
        }
        else
        {
            SetVelocity.y = OnGround ? 0 : GravityVelocity;
        }

        if (OnGround && TwiceJump)
        {
            TwiceJump = false;
        }
        _Rigidbody.velocity = new Vector3(SetVelocity.x, SetVelocity.y, 0);
    }

    private void FixedUpdate()
    {
        if (NowPlayerMode != PlayerMode.SpecialMove)
        {
            Gravity();
        }
        if (!GM_KeyLock)
        {
            Move();
        }
    }

    private IEnumerator SpecialMove()
    {
        NowPlayerMode = PlayerMode.SpecialMove;
        for (int i = 0; i < 50; i++)
        {
            _Rigidbody.velocity = new Vector3(MoveSpeed, 0, 0);
            yield return null;
        }
        _Rigidbody.velocity = Vector3.zero;
        yield break;
    }
    protected override void Move()
    {
        Movevec = Input.GetAxisRaw("Horizontal");
        VisualInversionCheck();
        if (OnGround && !JumpNow && !DamageNow && !NowAttackAction)
        {
            if (Movevec != 0 && NowPlayerMode != PlayerMode.Run)
            {
                SetVisible(PlayerMode.Run);
            }
            else if (Movevec == 0 && NowPlayerMode != PlayerMode.Normal)
            {
                SetVisible(PlayerMode.Normal);
            }
        }
        //SetVelocity.x = GM_KeyLock || ActionKeyLock ? 0 : Movevec * MoveVelocity;
        SetVelocity.x = GM_KeyLock ? 0 : Movevec * MoveSpeed;
        Debug.Log("now velocity x =" + SetVelocity.x);
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

    //�v���C���[�̑{���󋵂ɂ���āA�\������I�u�W�F�N�g��ύX����
    private void SetVisible(PlayerMode mode)
    {
        if (!DeathFlag)
        {
            GameObject obj = NowMode;
            //�_���[�W�󂯂Ă���ⓚ���p�Ń_���[�W�̃G�t�F�N�g�ɕω�������
            if (mode == PlayerMode.Damage)
            {
                obj = ObjDamage;
                //�_���[�W�󂯂��Ƃ��̕`�ʕύX
            }
            //�W�����v���Ȃ�ⓚ���p�ŃW�����v�ɐ؂�ւ���
            if (mode == PlayerMode.Jump)
            {
                obj = ObjNormal;
                //�W�����v���͂��̕`�悪�D�悳���i���ꂾ���A�j���[�V�����̊Ǘ��ɂȂ�Ǝv����j
            }

            if (mode == PlayerMode.Fall)
            {
                obj = ObjNormal;
                //�W�����v���͂��̕`�悪�D�悳���i���ꂾ���A�j���[�V�����̊Ǘ��ɂȂ�Ǝv����j
            }

            if (mode == PlayerMode.Normal)
            {
                obj = ObjNormal;
            }

            if (mode == PlayerMode.Run)
            {
                obj = ObjRun;
            }

            if (mode == PlayerMode.RocketPunch)
            {
                obj = ObjRocketPunch;
            }

            if (mode == PlayerMode.Beat)
            {
                obj = ObjBeat;
            }

            if (mode == PlayerMode.SpecialMove)
            {
                obj = ObjSpecialMove;
            }
            NowPlayerMode = mode;
            NowMode.SetActive(false);
            NowMode = obj;
            NowMode.SetActive(true);
            Debug.Log(NowMode);
        }
    }
    private void Jump()
    {
        JumpPos = transform.position.y;
        {
            if (JumpCount < 90)
            {
                JumpCount += 3;
                JumpVelocity = MAXJump * Mathf.Sin((90 - JumpCount) * Mathf.Deg2Rad);
            }
            else
            {
                JumpCount = 0;
                JumpNow = false;
            }
        }
    }

    private new void Gravity()
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
    private IEnumerator WeakAttack()
    {
        ActionKeyLock = true;
        NowAttackAction = true;
        //�U���p�̃G�t�F�N�g��\��
        SetVisible(PlayerMode.Beat);
        for (int i = 0; i < WaitFrame; i++)
        {
            yield return null;
        }
        GetSetGodMode = false;
        NowAttackAction = false;
        ActionKeyLock = false;
        //�\�����[�V�������U���ɕω�
        yield break;
    }

    private IEnumerator HeavyAttack()
    {
        ActionKeyLock = true;
        NowAttackAction = true;
        //�U���p�̃G�t�F�N�g��\��
        Instantitate_Weapon = Instantiate(Weapon);

        if (NowPlayerCode == PlayerCode.kit)
        {
            SetVisible(PlayerMode.RocketPunch);
            //�q�I�u�W�F�N�g����
            _Guntret = Instantitate_Weapon.GetComponent<Guntret>();
            _Guntret.Player = this.gameObject;
            _Guntret.VectorX = Input.GetAxisRaw("Horizontal");
            _Guntret.VectorY = Input.GetAxisRaw("Vertical");
            _Guntret.NowMode = Guntret.Mode.RocketPunch;
            int guntoffset = 1;
            if (VisualInversion)
            {
                _Guntret.GetSetInversion = this.VisualInversion;
                guntoffset *= -1;
            }
            Instantitate_Weapon.transform.position = new Vector3(this.transform.position.x + 1.3875f * guntoffset, this.transform.position.y + -0.014f * guntoffset, this.transform.position.z);

        }
        ActionKeyLock = false;
        while (Instantitate_Weapon != null)
        {
            yield return null;
        }

        NowAttackAction = false;
        //�\�����[�V�������U���ɕω�
        yield break;
    }

    //���G���Ԃ��Ǘ����鏈��
    //���G���Ԓ��͓_�ł�����
    IEnumerator ChgGodMode()
    {
        GetSetGodMode = true;
        bool flashing = false;

        const float flashingtime = 2.0f;
        const float flashingchgtime = 0.3f;
        float flashingchgtime_count = 0;
        for (float i = 0; i < flashingtime; i += Time.deltaTime)
        {
            flashingchgtime_count += Time.deltaTime;
            if (flashingchgtime_count >= flashingchgtime)//�_���[�W�H��������̃E�F�C�g
            {
                flashing = !flashing;
                InvisiObj.SetActive(flashing);
            }
            yield return null;
        }
        GetSetGodMode = false;
        InvisiObj.SetActive(true);
    }

    private IEnumerator Dead()
    {
        float minusframe = 2.0f;
        //HP��0�ɂȂ������̏���
        DeathFlag = true;

        while (NowMode.GetComponent<SpriteRenderer>().color.r > 0)
        {
            NowMode.GetComponent<SpriteRenderer>().color -= new Color(1 / minusframe * Time.deltaTime,
                1 / minusframe * Time.deltaTime, 1 / minusframe * Time.deltaTime, 0);
            yield return null;
        }
        //�V�[���`�F���W
        NowMode.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 1);
        ActionKeyLock = false;
        yield break;
    }
    public IEnumerator Damaged()
    {
        //�_���[�W������������̃G�t�F�N�g��\��
        ActionKeyLock = true;
        DamageNow = true;
        GetSetNowLife--;
        SetVisible(PlayerMode.Damage);
        if (GetSetNowLife <= 0)
        {
            StartCoroutine(Dead());
            yield break;
        }
        else
        {
            StartCoroutine(ChgGodMode());
            UpdateHP.Invoke(GetSetNowLife);
            const float waittime = 1.0f;
            for (float f = 0; f < waittime; f += Time.deltaTime)
            {
                yield return null;
            }
            ActionKeyLock = false;

            if (OnGround)
            {
                SetVisible(PlayerMode.Normal);
            }
            else if (JumpNow)
            {
                SetVisible(PlayerMode.Jump);
            }
            else
            {
                SetVisible(PlayerMode.Fall);
            }
            DamageNow = false;
        }
        yield break;
    }
    private void ItemGetEvent(Item.ItemCode Code)
    {
        if (Code == Item.ItemCode.Key)
        {
            HaveOpenKey++;
        }
        else if (Code == Item.ItemCode.Heal)
        {
            if (GetSetNowLife < MaxHP)
            {
                GetSetNowLife++;
                UpdateHP.Invoke(GetSetNowLife);
            }
        }
        else if (Code == Item.ItemCode.LifeMaxUp)
        {
            MaxHP++;
            GetSetNowLife = MaxHP;
            UpdateHP.Invoke(GetSetNowLife);
        }
        else if (Code == Item.ItemCode.PowerUp)
        {
            AttackPower++;
        }
        else
        {
            Debug.LogError("�z�肵�Ă��Ȃ��A�C�e�����擾");
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            TwiceJump = false;
        }
        //�_���[�W�������s�������莞�Ԗ��G�ɂ���
        if (other.gameObject.CompareTag("enemy"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _Rigidbody.velocity = Vector3.zero;
            }
            if (!GetSetGodMode)
            {
                StartCoroutine("Damaged");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Item>())
        {
            ItemGetEvent(other.gameObject.GetComponent<Item>().thisitem);
            Destroy(other.gameObject);
        }else if (other.gameObject.CompareTag("Hole"))
        {
            if (!GetSetGodMode)
            {
                StartCoroutine("Damaged");
            }
        }else if (other.gameObject.CompareTag("ClearFlag"))
        {
            ScrGamemanager.Clearflag = true;
        }
    }
}


