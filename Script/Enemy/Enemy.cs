using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Enemy : EnemyBase
{
#if UNITY_EDITOR
    //�ݒ肳�ꂽ�p�����[�^���C���X�y�N�^�[�ォ�猩���悤�ɂ���
    [CustomEditor(typeof(EnemyBase))]
#endif
    [Tooltip("�G�I�u�W�F�N�g�̒��̉摜�I�u�W�F�N�g")]
    [SerializeField]
    private GameObject _EnemyImage;
    public GameObject SetEnemyImage { set => _EnemyImage = value; }

    [Tooltip("����I�u�W�F�N�g�͈̔͂��������铮����������ꍇ�A�Y���̏��I�u�W�F�N�g���w��")]
    [SerializeField]
    protected GameObject _OnObject;
    public GameObject SetOnObject { set => _OnObject = value; }

    [Tooltip("�������U���p�̒e�@�������U��������G�̂ݎ���")]
    [SerializeField]
    protected GameObject _Bullet;

    public GameObject SetBullet { set => _Bullet = value; }

    [Tooltip("�G�̓������w�肷��R�[�h")]
    [SerializeField]
    protected int _ActionCode = 0;
    public int SetActionCode { set => _ActionCode = value; }

    [Tooltip("�G���h���b�v����A�C�e��")]
    [SerializeField]
    protected GameObject _DropItem;
    public GameObject SetDropItem { set => _DropItem = value; }

    [Tooltip("�A�C�e���̃h���b�v�m��")]
    [SerializeField]
    protected int _DropItemProbability;
    public int SetDropItemProbability { set => _DropItemProbability = value; }

    private float _BulletShotTime = 0;

    private BoxCollider _BoxCollider => GetComponent<BoxCollider>();
    private Animator animator;
    private Bullet _ScrBullet;

    //�|���ꂽ����ʒm����C�x���g
    public event Action OnDead;
    private void Start()
    {
        if (_ActionCode == 3)
        {
            animator = GetComponent<Animator>();
        }
    }
    private void Update()
    {
        if (!_DeathFlag)
        {
            //�d�͂������邩�̔���
            if (_GravityPower > 0 && _ActionCode != 3)
            {
                Gravity();
                float y_velocity = _OnGround ? 0 : _GravityVelocity;
                _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, y_velocity, 0);
            }

            //���S����
            if (_MaxHP <= 0)
            {
                _DeathFlag = true;
                StartCoroutine(Dead());
            }
        }
        //���G��Ԃ̉����`�F�b�N
        GodModeCheck();
    }
    private void FixedUpdate()
    {
        MovePattern();
    }

    private void MovePattern()
    {
        if (_ActionCode == 1)
            MovePattern1();
        else if (_ActionCode == 2)
            StartCoroutine(MovePattern2());
        else if (_ActionCode == 3)
            MovePattern3();
        else if (_ActionCode == 4)
            MovePattern4();
    }

    //�p�^�[���P:�Ђ�����v���C���[�̂ق��֓ːi���Ă���G
    private void MovePattern1()
    {
        float gotoposi = _Player.transform.position.x;
        float movespeedx = _MoveSpeed;
        if (this.transform.position.x >= gotoposi)
        {
            movespeedx *= -1;
        }
        int dezchg = movespeedx < 0 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, dezchg, 0);
        _Rigidbody.MovePosition(transform.position + new Vector3(movespeedx, 0, 0));
    }
    //�p�^�[���Q:����̏��̏�݂̂𔽕��ړ����鏈���i���V����j
    private IEnumerator MovePattern2()
    {
        float gotoposi = _Player.transform.position.x;
        float movespeedX = _MoveSpeed;
        float cnt = 0;
        Vector3 basepos = transform.position;
        GameObject subrazer = _Bullet;

        while (_MaxHP > 0)
        {
            if (this.transform.position.x < _OnObject.transform.position.x + _OnObject.transform.lossyScale.x / 2
    && transform.position.x > _OnObject.transform.position.x - _OnObject.transform.lossyScale.x / 2)
            {

            }
            else
            {
                movespeedX *= -1;
            }
            cnt = cnt + (1.0f / 10.0f);
            if (cnt > 360)
                cnt = 0;
            int dezchg = movespeedX < 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, dezchg, 0);
            _Rigidbody.MovePosition(new Vector3(transform.position.x + movespeedX, basepos.y + Mathf.Sin(cnt), 0));
            yield return null;
        }
        yield break;
    }
    //�p�^�[��3:����̏��̏�݂̂𔽕��ړ����鏈��(��ڂ̓G�p)
    private void MovePattern3()
    {
        float gotoposi = _Player.transform.position.x;
        Vector3 basepos = transform.position;
        GameObject bullet = _Bullet;
        int dezchg = 0;

        if (_MaxHP > 0)
        {
            //�v���C���[�������i�U���Ԑ��j
            if (Mathf.Abs(_Player.transform.position.x - transform.position.x) < 10 && Mathf.Abs(_Player.transform.position.y - transform.position.y) < 10)
            {
                //�\��ω�
                if (!animator.GetBool("isAttack"))
                {
                    animator.SetBool("isAttack", true);
                }
                _BulletShotTime += Time.deltaTime;
                if (_BulletShotTime > 1.5f)
                {
                    _BulletShotTime = 0;
                    bullet = Instantiate(_Bullet);
                    _ScrBullet = bullet.GetComponent<Bullet>();
                    _ScrBullet.Player = _Player.GetComponent<Player>();
                    bullet.transform.position = transform.position;
                }

                dezchg = _Player.transform.position.x - transform.position.x < 0 ? 0 : 180;
                transform.rotation = Quaternion.Euler(0, dezchg, 0);
            }
            else
            {
                _BulletShotTime = 0;
                if (animator.GetBool("isAttack"))
                {
                    animator.SetBool("isAttack", false);
                }
                if (this.transform.position.x < _OnObject.transform.position.x + _OnObject.transform.lossyScale.x / 2
&& transform.position.x > _OnObject.transform.position.x - _OnObject.transform.lossyScale.x / 2)
                {

                }
                else
                {
                    _MoveSpeed *= -1;
                }
                dezchg = _MoveSpeed < 0 ? 0 : 180;
                transform.rotation = Quaternion.Euler(0, dezchg, 0);
                _Rigidbody.MovePosition(transform.position + new Vector3(_MoveSpeed, 0, 0));
            }
        }
    }
    //�p�^�[��4:����̏��̏�݂̂𔽕��ړ����鏈���i���V���Ȃ��j
    private void MovePattern4()
    {
        Vector3 basepos = transform.position;
        GameObject subrazer = _Bullet;

        if (_MaxHP > 0)
        {   //Dotween�g���������y����
            if (this.transform.position.x + transform.lossyScale.x / 2 <= _OnObject.transform.position.x + _OnObject.transform.lossyScale.x / 2
    && transform.position.x - transform.lossyScale.x / 2 >= _OnObject.transform.position.x - _OnObject.transform.lossyScale.x / 2)
            {

            }
            else
            {
                _MoveSpeed *= -1;
            }
            int dezchg = _MoveSpeed < 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, dezchg, 0);
            _Rigidbody.MovePosition(transform.position + new Vector3(_MoveSpeed, 0, 0));
        }
    }

    private IEnumerator Dead()
    {
        //���ꂽ���̓����`��
        OnDead?.Invoke();
        _Rigidbody.useGravity = true;
        _BoxCollider.enabled = false;//�����蔻������ł�����
        _Rigidbody.constraints = RigidbodyConstraints.None;
        _Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        ItemDropJudge();//�A�C�e�����h���b�v���邩����
        float x = UnityEngine.Random.Range(-5f, 10f);
        float y = UnityEngine.Random.Range(5f, 10f);
        float z = UnityEngine.Random.Range(-5f, 5f);
        _Rigidbody.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
        for (int i = 0; i < 180; i++)//�̂���TimeDeltatime�Ń��[�v���Ԃ�ݒ�o����悤�C������\��
        {
            Quaternion rot = Quaternion.Euler(0, 0, 30);
            // ���݂̎��g�̉�]�̏����擾����B
            Quaternion q = _EnemyImage.transform.rotation;
            // �������āA���g�ɐݒ�
            _EnemyImage.transform.rotation = q * rot;

            yield return null;
        }
        //��莞�Ԃ�������I�u�W�F�N�g�폜
        Destroy(gameObject);
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
    private void ItemDropJudge()
    {
        int rnd = UnityEngine.Random.Range(1, _DropItemProbability + 1);
        if (_DropItemProbability == rnd)
        {
            GameObject dropitem;
            dropitem = (GameObject)Instantiate(_DropItem, this.transform.position, Quaternion.identity);
        }
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _Rigidbody.velocity = Vector3.zero;
        }
    }
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _OnGround = false;
        }
        if (col.gameObject.CompareTag("Player"))
        {
            _Rigidbody.isKinematic = false;
        }
    }
    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _OnGround = true;
        }
    }
}
