using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;

public class Enemy : EnemyBase
{
#if UNITY_EDITOR
    //設定されたパラメータをインスペクター上から見れるようにする
    [CustomEditor(typeof(EnemyBase))]
#endif
    [Tooltip("敵オブジェクトの中の画像データ")]
    [SerializeField]
    private GameObject EnemyImage;
    public GameObject GetSetEnemyImage { get { return EnemyImage; } set { EnemyImage = value; } }

    [Tooltip("特定オブジェクトの範囲を往復する動きをさせる場合、該当の床オブジェクトを指定")]
    [SerializeField]
    protected GameObject OnObject;
    public GameObject GetSetOnObject { get { return OnObject; } set { OnObject = value; } }

    [Tooltip("遠距離攻撃用の弾　遠距離攻撃をする敵のみ実装")]
    [SerializeField]
    protected GameObject Bullet;

    public GameObject GetSetBullet { get { return Bullet; } set { Bullet = value; } }

    [Tooltip("敵の動きを指定するコード")]
    [SerializeField]
    protected int ActionCode = 0;
    public int GetSetActionCode { get { return ActionCode; } set { ActionCode = value; } }

    [Tooltip("敵がドロップするアイテム")]
    [SerializeField]
    protected GameObject DropItem;
    public GameObject GetSetDropItem { get { return DropItem; } set { DropItem = value; } }

    [Tooltip("アイテムのドロップ確率")]
    [SerializeField]
    protected int DropItemProbability;
    public int GetSetDropItemProbability { get { return DropItemProbability; } set { DropItemProbability = value; } }

    //敵の表情を切り替える時に表示/非表示するオブジェクト 魔法使いの敵の画像データの構造に合わせ実装
    //今後アニメーションによる切り替えに修正する予定
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
        //無敵状態の解除チェック
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

    //パターン１:ひたすらプレイヤーのほうへ突進してくる敵
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
    //パターン２:特定の床の上のみを反復移動する処理（浮遊する）
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
    //パターン3:特定の床の上のみを反復移動する処理(一つ目の敵用)
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
    //パターン２:特定の床の上のみを反復移動する処理（浮遊しない）
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
        //やられた時の動作を描画
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
            Quaternion q = EnemyImage.transform.rotation;
            // 合成して、自身に設定
            EnemyImage.transform.rotation = q * rot;

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
