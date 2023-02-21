using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Enemy : EnemyBase
{
#if UNITY_EDITOR
    //�ݒ肳�ꂽ�p�����[�^���C���X�y�N�^�[�ォ�猩���悤�ɂ���
    [CustomEditor(typeof(EnemyBase))]
#endif
    [Tooltip("�G�I�u�W�F�N�g�̒��̉摜�f�[�^")]
    [SerializeField]
    private GameObject EnemyImage;
    public GameObject GetSetEnemyImage { get { return EnemyImage; } set { EnemyImage = value; } }

    [Tooltip("����I�u�W�F�N�g�͈̔͂��������铮����������ꍇ�A�Y���̏��I�u�W�F�N�g���w��")]
    [SerializeField]
    protected GameObject OnObject;
    public GameObject GetSetOnObject { get { return OnObject; } set { OnObject = value; } }

    [Tooltip("�������U���p�̒e�@�������U��������G�̂ݎ���")]
    [SerializeField]
    protected GameObject Bullet;

    public GameObject GetSetBullet { get { return Bullet; } set { Bullet = value; } }

    [Tooltip("�G�̓������w�肷��R�[�h")]
    [SerializeField]
    protected int ActionCode = 0;
    public int GetSetActionCode { get { return ActionCode; } set { ActionCode = value; } }

    [Tooltip("�G���h���b�v����A�C�e��")]
    [SerializeField]
    protected GameObject DropItem;
    public GameObject GetSetDropItem { get { return DropItem; } set { DropItem = value; } }

    [Tooltip("�A�C�e���̃h���b�v�m��")]
    [SerializeField]
    protected int DropItemProbability;
    public int GetSetDropItemProbability { get { return DropItemProbability; } set { DropItemProbability = value; } }

    //�G�̕\���؂�ւ��鎞�ɕ\��/��\������I�u�W�F�N�g ���@�g���̓G�̉摜�f�[�^�̍\���ɍ��킹����
    //����A�j���[�V�����ɂ��؂�ւ��ɏC������\��
    [SerializeField]
    protected GameObject Magician_Eye_Normal1;
    [SerializeField]
    protected GameObject Magician_Eye_Normal2;
    [SerializeField]
    protected GameObject Magician_Eye_Normal3;
    [SerializeField]
    protected GameObject Magician_Eye_Attack;
    [SerializeField]
    protected GameObject Magician_Hand_Normal;
    [SerializeField]
    protected GameObject Magician_Hand_Attack;
    public void SetMagicianData(GameObject eye1, GameObject eye2,
    GameObject eye3, GameObject eyeatk, GameObject handnormal, GameObject Handattack)
    {
        Magician_Eye_Normal1 = eye1;
        Magician_Eye_Normal2 = eye2;
        Magician_Eye_Normal3 = eye3;
        Magician_Eye_Attack = eyeatk;
        Magician_Hand_Attack = Handattack;
        Magician_Hand_Normal = handnormal;
    }
    private BoxCollider _BoxCollider =>GetComponent<BoxCollider>();
    private Bullet ScrBullet;

    private bool ActionNow = false;
    private void Update()
    {
        if (!DeathFlag)
        {
            if (GravityPower > 0)
            {
                Gravity();
                float y_velocity = OnGround ? 0 : GravityVelocity;
                _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, y_velocity, 0);
            }
            if (!ScrGamemanager.KeyLock && !ActionNow) { ActionNow = true; MovePattern(); }

            if (MaxHP <= 0)
            {
                DeathFlag = true;
                StartCoroutine(Dead());
            }
        }
        //���G��Ԃ̉����`�F�b�N
        GodModeCheck();
    }

    private void MovePattern()
    {
        if (ActionCode == 1)

            StartCoroutine(MovePattern1());
        else if (ActionCode == 2)
            StartCoroutine(MovePattern2());
        else if (ActionCode == 3)
            StartCoroutine(MovePattern3());
        else if (ActionCode == 4)
            StartCoroutine(MovePattern4());
    }

    //�p�^�[���P:�Ђ�����v���C���[�̂ق��֓ːi���Ă���G
    private IEnumerator MovePattern1()
    {
        bool goal = false;
        float gotoposi = Player.transform.position.x;
        float movespeedX = MoveSpeed;

        if (this.transform.position.x >= gotoposi)
        {
            movespeedX *= -1;
        }
        int dezchg = movespeedX < 0 ? 0 : 180;
        transform.rotation = Quaternion.Euler(0, dezchg, 0);
        while (!goal)
        {
            _Rigidbody.MovePosition(transform.position + new Vector3(movespeedX, 0, 0));
            if ((movespeedX < 0 && gotoposi > transform.position.x) || (movespeedX >= 0 && gotoposi <= transform.position.x))
            {
                goal = true;
            }
            yield return null;
        }
        yield break;
    }
    //�p�^�[���Q:����̏��̏�݂̂𔽕��ړ����鏈���i���V����j
    private IEnumerator MovePattern2()
    {
        float gotoposi = Player.transform.position.x;
        float movespeedX = MoveSpeed;
        float cnt = 0;
        Vector3 basepos = transform.position;
        GameObject subrazer = Bullet;

        while (MaxHP > 0)
        {
            if (this.transform.position.x < OnObject.transform.position.x + OnObject.transform.lossyScale.x / 2
    && transform.position.x > OnObject.transform.position.x - OnObject.transform.lossyScale.x / 2)
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
    private IEnumerator MovePattern3()
    {
        float gotoposi = Player.transform.position.x;
        float movespeedX = MoveSpeed;
        Vector3 basepos = transform.position;
        float time = 0;
        GameObject bullet = Bullet;
        int dezchg = 0;

        bool isAttack = false;
        bool isNormal = true;
        while (MaxHP > 0)
        {
            if (Mathf.Abs(Player.transform.position.x - transform.position.x) < 10 && Mathf.Abs(Player.transform.position.y - transform.position.y) < 10)
            {
                if (!isAttack)
                {
                    isAttack = true;
                    if (isNormal)
                    {
                        isNormal = false;
                        Magician_Eye_Normal1.SetActive(false);
                        Magician_Eye_Normal2.SetActive(false);
                        Magician_Eye_Normal3.SetActive(false);
                        Magician_Hand_Normal.SetActive(false);
                    }
                    Magician_Eye_Attack.SetActive(true);
                    Magician_Hand_Attack.SetActive(true);
                }
                time += Time.deltaTime;
                if (time > 1.5f)
                {
                    time = 0;
                    bullet = Instantiate(Bullet);
                    ScrBullet = bullet.GetComponent<Bullet>();
                    ScrBullet.Player = Player.GetComponent<Player>();
                    bullet.transform.position = transform.position;
                }

                dezchg = Player.transform.position.x - transform.position.x < 0 ? 0 : 180;
                transform.rotation = Quaternion.Euler(0, dezchg, 0);
                yield return null;
            }
            else
            {
                time = 0;
                if (!isNormal)
                {
                    isNormal = true;
                    if (isAttack)
                    {
                        isAttack = false;
                        Magician_Eye_Attack.SetActive(false);
                        Magician_Hand_Attack.SetActive(false);
                    }
                    Magician_Eye_Normal1.SetActive(true);
                    Magician_Eye_Normal2.SetActive(true);
                    Magician_Eye_Normal3.SetActive(true);
                    Magician_Hand_Normal.SetActive(true);


                }
                if (this.transform.position.x < OnObject.transform.position.x + OnObject.transform.lossyScale.x / 2
&& transform.position.x > OnObject.transform.position.x - OnObject.transform.lossyScale.x / 2)
                {

                }
                else
                {
                    movespeedX *= -1;
                }
                //
                dezchg = movespeedX < 0 ? 0 : 180;
                transform.rotation = Quaternion.Euler(0, dezchg, 0);
                _Rigidbody.MovePosition(transform.position + new Vector3(movespeedX, 0, 0));
                yield return null;
            }
        }
        yield break;
    }
    //�p�^�[���Q:����̏��̏�݂̂𔽕��ړ����鏈���i���V���Ȃ��j
    private IEnumerator MovePattern4()
    {
        float movespeedX = MoveSpeed;
        float cnt = 0;
        Vector3 basepos = transform.position;
        GameObject subrazer = Bullet;

        while (MaxHP > 0)
        {
            if (this.transform.position.x + transform.lossyScale.x / 2 < OnObject.transform.position.x + OnObject.transform.lossyScale.x / 2
    && transform.position.x - transform.lossyScale.x / 2 > OnObject.transform.position.x - OnObject.transform.lossyScale.x / 2)
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
            _Rigidbody.MovePosition(transform.position + new Vector3(movespeedX, 0, 0));
            yield return null;
        }
        yield break;
    }

    private IEnumerator Dead()
    {
        //���ꂽ���̓����`��
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
            Quaternion q = EnemyImage.transform.rotation;
            // �������āA���g�ɐݒ�
            EnemyImage.transform.rotation = q * rot;

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
            if (!GodMode)
            {
                int Damage = other.GetComponent<Guntret>().GetSetAttackPower;
                MaxHP -= Damage;
                GodMode = true;
                VisibleDamage(Damage);
            }
        }
    }
    private void ItemDropJudge()
    {
        int rnd = UnityEngine.Random.Range(1, GetSetDropItemProbability + 1);
        if (GetSetDropItemProbability == rnd)
        {
            GameObject dropitem;
            dropitem = (GameObject)Instantiate(GetSetDropItem, this.transform.position, Quaternion.identity);
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
            OnGround = false;
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
            OnGround = true;
        }
    }
}
