using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Enemy : EnemyBase
{
#if UNITY_EDITOR
    //設定されたパラメータをインスペクター上から見れるようにする
    [CustomEditor(typeof(EnemyBase))]
#endif
    [Tooltip("敵オブジェクトの中の画像オブジェクト")]
    [SerializeField]
    private GameObject _EnemyImage;
    public GameObject SetEnemyImage { set => _EnemyImage = value; }

    [Tooltip("特定オブジェクトの範囲を往復する動きをさせる場合、該当の床オブジェクトを指定")]
    [SerializeField]
    protected GameObject _OnObject;
    public GameObject SetOnObject { set => _OnObject = value; }

    [Tooltip("遠距離攻撃用の弾　遠距離攻撃をする敵のみ実装")]
    [SerializeField]
    protected GameObject _Bullet;

    public GameObject SetBullet { set => _Bullet = value; }

    [Tooltip("敵の動きを指定するコード")]
    [SerializeField]
    protected int _ActionCode = 0;
    public int SetActionCode { set => _ActionCode = value; }

    [Tooltip("敵がドロップするアイテム")]
    [SerializeField]
    protected GameObject _DropItem;
    public GameObject SetDropItem { set => _DropItem = value; }

    [Tooltip("アイテムのドロップ確率")]
    [SerializeField]
    protected int _DropItemProbability;
    public int SetDropItemProbability { set => _DropItemProbability = value; }

    private float _BulletShotTime = 0;

    private BoxCollider _BoxCollider => GetComponent<BoxCollider>();
    private Animator animator;
    private Bullet _ScrBullet;

    //倒された事を通知するイベント
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
            //重力を加えるかの判定
            if (_GravityPower > 0 && _ActionCode != 3)
            {
                Gravity();
                float y_velocity = _OnGround ? 0 : _GravityVelocity;
                _Rigidbody.velocity = new Vector3(_Rigidbody.velocity.x, y_velocity, 0);
            }

            //死亡判定
            if (_MaxHP <= 0)
            {
                _DeathFlag = true;
                StartCoroutine(Dead());
            }
        }
        //無敵状態の解除チェック
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

    //パターン１:ひたすらプレイヤーのほうへ突進してくる敵
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
    //パターン２:特定の床の上のみを反復移動する処理（浮遊する）
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
    //パターン3:特定の床の上のみを反復移動する処理(一つ目の敵用)
    private void MovePattern3()
    {
        float gotoposi = _Player.transform.position.x;
        Vector3 basepos = transform.position;
        GameObject bullet = _Bullet;
        int dezchg = 0;

        if (_MaxHP > 0)
        {
            //プレイヤー発見時（攻撃態勢）
            if (Mathf.Abs(_Player.transform.position.x - transform.position.x) < 10 && Mathf.Abs(_Player.transform.position.y - transform.position.y) < 10)
            {
                //表情変化
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
    //パターン4:特定の床の上のみを反復移動する処理（浮遊しない）
    private void MovePattern4()
    {
        Vector3 basepos = transform.position;
        GameObject subrazer = _Bullet;

        if (_MaxHP > 0)
        {   //Dotween使った方が楽かも
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
        //やられた時の動作を描画
        OnDead?.Invoke();
        _Rigidbody.useGravity = true;
        _BoxCollider.enabled = false;//当たり判定を消滅させる
        _Rigidbody.constraints = RigidbodyConstraints.None;
        _Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        ItemDropJudge();//アイテムをドロップするか判定
        float x = UnityEngine.Random.Range(-5f, 10f);
        float y = UnityEngine.Random.Range(5f, 10f);
        float z = UnityEngine.Random.Range(-5f, 5f);
        _Rigidbody.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
        for (int i = 0; i < 180; i++)//のちにTimeDeltatimeでループ時間を設定出来るよう修正する予定
        {
            Quaternion rot = Quaternion.Euler(0, 0, 30);
            // 現在の自身の回転の情報を取得する。
            Quaternion q = _EnemyImage.transform.rotation;
            // 合成して、自身に設定
            _EnemyImage.transform.rotation = q * rot;

            yield return null;
        }
        //一定時間たったらオブジェクト削除
        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        //ダメージ処理を行った後一定時間無敵にする
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
