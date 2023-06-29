using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEditor;
/// <summary>
/// �v���C���[�S�ʂ̏������Ǘ�����N���X
/// </summary>
public class Player : CreatureBase
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CreatureBase))]
#endif

    /// <summary>
    /// ���݂̃v���C���[�L�����N�^�[�̎�ނ��`����񋓌^
    /// </summary>
    private enum _PlayerCode : int
    {
        kit,
        rail
    }

    /// <summary>
    /// �v���C���[�L�����N�^�[�̃��[�V�������`����񋓌^
    /// </summary>
    private enum _PlayerMotion : int
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
    [Tooltip("���݂̃v���C���[�i�����̃L�����N�^�[���v���C���[�ɂȂ邱�Ƃ�z��j")]
    private _PlayerCode currentPlayer = _PlayerCode.kit;


    [SerializeField]
    [Tooltip("�U���������̃A�j���[�V�������O���t�B�b�N���鎞��")]
    private int _WaitFrame = 1;

    [SerializeField]
    [Tooltip("�v���C���[�̑������镐��@�f�t�H���g�̓K���g���b�g��ݒ肷��")]
    private GameObject _Weapon;

    [SerializeField]
    [Tooltip("�v���C���[�̒��n������s���Z���T�[�p�̃I�u�W�F�N�g")]
    private GameObject _GroundCensor;

    [SerializeField]
    [Tooltip("�U����")]
    public int AttackPower = 1;

    //�ȉ��v���C���[�̃A�j���[�V������`�悷��̃Q�[���I�u�W�F�N�g�@
    [SerializeField]
    [Tooltip("�v���C���[�̒ʏ펞�̃O���t�B�b�N")]
    private GameObject _ObjNormal;
    public GameObject SetObjNormal { set => _ObjNormal = value; }

    [SerializeField]
    [Tooltip("�v���C���[�������[�U�[�̃I�u�W�F�N�g")]
    private GameObject _ObjLaser;

    //�A�j���[�V�����̔��]����
    private bool _VisualInversion = false;
    public bool GetSetVisualInversion { get => _VisualInversion; set => _VisualInversion = value; }
    //�ړ����Ă������
    private float _Movevec = 0;

    //�_���[�W�󂯂��Ƃ��̓_�ł���ۂ̓����s������؂�ւ��鎞��
    [SerializeField]
    private GameObject _ObjInvisible;

    //2�i�W�����v���������̔���
    private bool _TwiceJump = false;
    //�U�������ۂ�
    private bool _IsAttack = false;

    // �_���[�W��������Ă��邩�ۂ�
    private bool _IsDamage = false;

    //�v���C���[�̍s�����ɂ�����L�[���b�N
    private bool _KeyLockbyAct = false;

    //�W�����v�o���鍂����MAX
    [SerializeField]
    private float _MAXJump = 0;

    //�W�����v����Ƃ��̏㏸�l
    [SerializeField]
    private float _JumpPower = 20;

    private float _JumpCount = 0;

    private int _HaveOpenKey = 0;

    private bool _GM_KeyLock => _CompGamenager.KeyLock;

    //�ȉ��R���|�[�l���g�̎擾
    private Guntret _CompGuntret;
    private Rigidbody _CompRigidbody;
    private Animator AnimaorNormal;
    //�v���C���[�L�����N�^�[�̑S�̂̕\����\�����Ǘ�����A�j���[�^�[
    private Animator _CompAnimatorAll;

    [SerializeField]
    private AnimationClip ClipNormal;
    //�v���C���[���|���ꂽ���̃A�j���[�V�����N���b�v
    [SerializeField]
    private AnimationClip _ClipDead;


    private PlayerCensorGround _CompPlayerCensorGround => _GroundCensor.GetComponent<PlayerCensorGround>();
    //�v���C���[���n�ʂɐڒn���Ă��邩�ۂ�
    public override bool GetSetOnGround
    {
        get => _CompPlayerCensorGround.OnGround;
        set => _OnGround = _CompPlayerCensorGround.OnGround;
    }
    //RigidBody�ɕt������Velocity
    private Vector3 _SetRbVelocity;

    //HP�ɕϓ����������Ƃ��ɔ���������C�x���g
    public event System.Action<int> UpdateHP;

    private void Start()
    {
        //�����l�̑������
        _NowHP = _MaxHP;
        _SetRbVelocity = Vector3.zero;
        _CompRigidbody = GetComponent<Rigidbody>();
        AnimaorNormal = _ObjNormal.GetComponent<Animator>();
        _CompAnimatorAll = _ObjInvisible.GetComponent<Animator>();
    }
    private void Update()
    {
        //�C�x���g���̓L�[���͂̑���𖳌��ɂ���
        if (_GM_KeyLock)
        {
            // �L�[���͂������̏ꍇ�͉��ړ����ł��Ȃ��Ȃ�A�W�����v���łȂ��ꍇ�̓m�[�}�����[�V�����ɖ߂�
            _SetRbVelocity.x = 0;
            if (!_JumpNow) _CompAnimatorAll.SetBool("isJump", false);
        }
        else
        {
            // �L�[���͂��L���̏ꍇ�̓R�}���h�����s�\
            HandleInput();
        }
    }
    //RigidBody�Ɋ֗^����������FixedUpdate�ɂčs��
    private void FixedUpdate()
    {
        Gravity();
        // �Q�[���}�l�[�W���[�ɂ���ăL�[���͂̃��b�N������Ă��Ȃ��ꍇ�A�v���C���[�͓��삪�\�Ƃ���
        if (!_GM_KeyLock)
        {
            Move();
        }

        if (_JumpNow)
        {
            // �W�����v���̏ꍇ�̓W�����v���������s���A�������x���W�����v�͂ɐݒ肷��
            Jump();
            _SetRbVelocity.y = _JumpPower;
        }
        else
        {
            //���n�����ꍇ��isjump��false�ɂ���
            if (GetSetOnGround)
            {
                _CompAnimatorAll.SetBool("isJump", false);
            }
            // �W�����v���łȂ��ꍇ�́A�n�ʂɂ��Ȃ��ꍇ�d�͂�ݒ肷��
            _SetRbVelocity.y = GetSetOnGround ? 0 : _GravityVelocity;
            AnimaorNormal.SetBool("isFall", !GetSetOnGround);

        }

        // �����n�ʂɒ��n���Ă��Ă���2��ڂ̃W�����v�����s���Ă����ꍇ�́A��������Z�b�g����
        if (GetSetOnGround && _TwiceJump)
        {
            _TwiceJump = false;
        }
        _CompRigidbody.velocity = new Vector3(_SetRbVelocity.x, _SetRbVelocity.y, 0);
    }
    //���[�U�[���˂̏���
    private void LaserShot()
    {
        //���[�U�[�̔���
        GameObject laser = Instantiate(_ObjLaser, _ObjLaser.transform.position, Quaternion.identity);
        laser.transform.parent = transform;
        laser.transform.localPosition = new Vector3(0, 0, 0);
        laser.transform.localRotation = Quaternion.identity;
        laser.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _TwiceJump = false;
        }
        //�_���[�W�������s�������莞�Ԗ��G�ɂ���
        if (other.gameObject.CompareTag("enemy"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _CompRigidbody.velocity = Vector3.zero;
            }
            if (!GetSetGodMode)
            {
                StartCoroutine("Damaged");
            }
        }
    }
    // ���̃R���C�_�[���g���K�[�ɓ������ۂ̏���
    private void OnTriggerEnter(Collider other)
    {
        // �I�u�W�F�N�g���A�C�e���������ꍇ
        if (other.gameObject.GetComponent<Item>())
        {
            // �A�C�e���̎�ނ��擾���ăA�C�e���擾�C�x���g�����s����B
            ItemGetEvent(other.gameObject.GetComponent<Item>().thisitem);
            Destroy(other.gameObject);
        }
        // �g���K�[�ɓ������I�u�W�F�N�g�����̃Z���T�[�������ꍇ
        else if (other.gameObject.CompareTag("Hole"))
        {
            // ���G���[�h�łȂ��ꍇ�̓_���[�W�������J�n
            if (!GetSetGodMode)
            {
                StartCoroutine("Damaged");
            }
        }
        // �g���K�[�ɓ������I�u�W�F�N�g���X�e�[�W�N���A�p�̃t���O�������ꍇ
        else if (other.gameObject.CompareTag("ClearFlag"))
        {
            _CompGamenager.Clearflag = true;
        }
        // �g���K�[�ɓ������I�u�W�F�N�g���G�̃X�|�i�[�������ꍇ
        else if (other.gameObject.CompareTag("Spawner"))
        {
            // Spawner�I�u�W�F�N�g���A�N�e�B�u������
            other.gameObject.SetActive(true);
        }
    }

    //�K�E�Z�̏���
    private IEnumerator SpecialMove()
    {
        const int _TIME = 50;
        for (int i = 0; i < _TIME; i++)
        {
            _CompRigidbody.velocity = new Vector3(_MoveSpeed, 0, 0);
            yield return null;
        }
        _CompRigidbody.velocity = Vector3.zero;
        yield break;
    }

    [SerializeField] private float _dashDuration = 0.5f; // �_�b�V���̎�������
    [SerializeField] private float _dashSpeed = 10f; // �_�b�V�����̈ړ����x
    private bool _IsDashing = false; // �_�b�V�������ǂ����̃t���O
    private float _DashTimer = 0f; // �_�b�V���̌p�����Ԍv���p�̃^�C�}�[

    // �v���C���[�̉��ړ����s���֐�
    protected override void Move()
    {
        // ���삷��������擾����
        _Movevec = Input.GetAxisRaw("Horizontal");
        // �v���C���[�̌������m�F����
        VisualInversionCheck();

        // �W�����v�A�_���[�W�A�U�����łȂ��ꍇ�͈ȉ��̏��������s����
        if (GetSetOnGround && !_JumpNow && !_IsDamage && !_IsAttack)
        {
            // �ړ�������0�łȂ��ꍇ�́A�v���C���[�̃��[�V�������uRun�v�ɕύX����
            if (_Movevec != 0)
            {
                //�S�̊Ǘ��̃A�j���[�^�[�̃��[�V������ύX����
                _CompAnimatorAll.SetBool("isRun", true);
            }
            // �ړ�������0�ł���ꍇ�́A�v���C���[�̃��[�V�������uNormal�v�ɕύX����
            else if (_Movevec == 0)
            {
                //�S�̊Ǘ��̃A�j���[�^�[�̃��[�V������ύX����
                _CompAnimatorAll.SetBool("isRun", false);
            }
        }

        if (_IsDashing)
        {
            // �_�b�V�����͈ړ����x���グ��
            _SetRbVelocity.x = _Movevec * _dashSpeed;

            _DashTimer -= Time.deltaTime;
            if (_DashTimer <= 0f && GetSetOnGround)
            {
                _IsDashing = false;
                //SetVisible(_PlayerMotion.Normal);
            }
        }
        else
        {
            // �Q�[���}�l�[�W���[�ɂ���ăL�[���͂̃��b�N������Ă��Ȃ��ꍇ�A�v���C���[�͓��삪�\�Ƃ���
            _SetRbVelocity.x = _GM_KeyLock || _IsDamage ? 0 : _Movevec * _MoveSpeed;
            //�_�b�V�����ɋ󒆂ɂ����ꍇ�͒��n����܂ő��x���_�b�V�����̂��̂ɂ���

        }
    }
    //�L�[���͏���
    private void HandleInput()
    {
        // �W�����v�̏���
        if (Input.GetButtonDown("Jump"))
        {
            if (GetSetOnGround && !_JumpNow)
            {
                // �W�����v���\�ł���΃W�����v����
                _JumpNow = true;
                //SetVisible(_PlayerMotion.Jump);
                _CompAnimatorAll.SetBool("isJump", true);

            }
            else if (!_TwiceJump)
            {
                // �W�����v���ł����2�i�W�����v������
                _JumpNow = true;
                _TwiceJump = true;
                _JumpCount = 0;
                // SetVisible(_PlayerMotion.Jump);
                _CompAnimatorAll.SetBool("isJump", true);
            }
        }

        if (Input.GetButtonDown("Fire1") && !_IsAttack)
        {
            // ���U���̏���
            StartCoroutine(HeavyAttack());
        }
        if (Input.GetButtonDown("Fire3") && !_IsAttack)
        {
            //�����Q�[���}�l�[�W���[�̕K�E�Z�Q�[�W�����܂��Ă����甭���ł���
            if (_CompGamenager.CurrentSpecialAttackGuide >= 100)
            {
                // ����U���̏���
                StartCoroutine(SpecialMove());
            }
        }

        // �_�b�V�������@�ڒn���Ă��Ă��_�b�V�����łȂ��ꍇ�ɍs����
        if (Input.GetButtonDown("Dash") && !_IsDashing && GetSetOnGround)
        {
            _IsDashing = true;
            _DashTimer = _dashDuration;
        }
    }

    //�I�u�W�F�N�g�̕`��̔��]����
    private void VisualInversionCheck()
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
            _ObjInvisible.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            _ObjInvisible.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //�W�����v����
    private void Jump()
    {
        // �W�����v�̃J�E���g��90�ȉ��̂Ƃ�
        if (_JumpCount < 90)
        {
            //�W�����v�̃J�E���g�𑝂₷
            _JumpCount += 3;
            //�W�����v�͂��v�Z����
            _JumpPower = _MAXJump * Mathf.Sin(Mathf.Deg2Rad * (90 - _JumpCount));
            //�A�j���[�V�������W�����v�ɕύX����
            AnimaorNormal.SetBool("isJump", true);
            _CompAnimatorAll.SetBool("isJump", true);
        }
        //�W�����v���I�������Ƃ�
        else
        {
            //�J�E���g������������
            _JumpCount = 0;
            //�W�����v��Ԃ���������
            _JumpNow = false;
            //�A�j���[�V������ʏ��ԂɕύX����
            AnimaorNormal.SetBool("isJump", false);
        }
    }


    private new void Gravity()
    {
        //�d�͉����x�͐����g�ŕ\�����������ߎ���
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
    // ���U������
    private IEnumerator HeavyAttack()
    {
        // �A�N�V�������͓��͂��󂯕t���Ȃ��悤�ɂ���
        _KeyLockbyAct = true;
        // �U�����t���O���I���ɂ���
        _IsAttack = true;

        // �U���p�G�t�F�N�g��\������
        GameObject instantinateweapon = Instantiate(_Weapon);

        if (currentPlayer == _PlayerCode.kit)
        {

            // �L�b�g�̏ꍇ
            _CompAnimatorAll.SetBool("isHeavyAttack", true);

            // �q�I�u�W�F�N�g�𐶐�����
            //Offset
            float offsetx = 1.3875f;
            float offsety = -0.014f;

            _CompGuntret = instantinateweapon.GetComponent<Guntret>();
            _CompGuntret.Player = this.gameObject;
            _CompGuntret.VectorX = Input.GetAxisRaw("Horizontal");
            _CompGuntret.VectorY = Input.GetAxisRaw("Vertical");
            _CompGuntret.NowMode = Guntret.Mode.RocketPunch;
            int guntoffset = 1;
            if (_VisualInversion)
            {
                // �L�b�g���t�����̏ꍇ�͔��]����
                _CompGuntret.GetSetInversion = this._VisualInversion;
                guntoffset *= -1;
            }
            instantinateweapon.transform.position = new Vector3(this.transform.position.x + offsetx * guntoffset, this.transform.position.y + offsety * guntoffset, this.transform.position.z);

        }

        _KeyLockbyAct = false;
        // �U�����t���O���I�t�ɂ���
        _IsAttack = false;

        //1�b�҂�
        yield return new WaitForSeconds(1.0f);

        _CompAnimatorAll.SetBool("isHeavyAttack", false);

    }

    //���G���Ԃ��Ǘ����鏈��
    IEnumerator ChgGodMode()
    {
        GetSetGodMode = true;

        //2�b�Ԗ��G�ɂ���
        yield return new WaitForSeconds(2.0f);

        GetSetGodMode = false;
    }

    //���񂾂Ƃ��̏���
    private IEnumerator Dead()
    {
        float minusframe = 2.0f;
        //HP��0�ɂȂ������̏���
        _DeathFlag = true;

        //���ꂽ���̃A�j���[�V�����N���b�v���Đ�
        _CompAnimatorAll.Play(_ClipDead.name);

        //���݂̃A�j���[�V�����Đ����I���܂őҋ@
        yield return new WaitForSeconds(_ClipDead.length - minusframe);

        //�V�[���`�F���W
        _KeyLockbyAct = false;
    }
    //�_���[�W�󂯂��Ƃ��̏���
    public IEnumerator Damaged()
    {
        //�_���[�W������������̃G�t�F�N�g��\��
        _KeyLockbyAct = true;
        _IsDamage = true;
        GetSetNowLife--;
        if (GetSetNowLife <= 0)
        {
            StartCoroutine(Dead());
            yield break;
        }
        else
        {
            StartCoroutine(ChgGodMode());

            //�S�̊Ǘ��̃A�j���[�^�[��isDamage��true�ɂ���
            _CompAnimatorAll.SetBool("isDamage", true);

            UpdateHP.Invoke(GetSetNowLife);

            //1�b�قǑҋ@
            yield return new WaitForSeconds(1.0f);

            //�S�̊Ǘ��̃A�j���[�^�[��isDamage��false�ɂ���
            _CompAnimatorAll.SetBool("isDamage", false);
            _IsDamage = false;
        }
        yield break;
    }
    //�A�C�e���擾���̏���
    private void ItemGetEvent(Item.ItemCode Code)
    {
        if (Code == Item.ItemCode.Key)
        {
            _HaveOpenKey++;
        }
        else if (Code == Item.ItemCode.Heal)
        {
            if (GetSetNowLife < _MaxHP)
            {
                GetSetNowLife++;
                UpdateHP.Invoke(GetSetNowLife);
            }
        }
        else if (Code == Item.ItemCode.LifeMaxUp)
        {
            _MaxHP++;
            GetSetNowLife = _MaxHP;
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
}


