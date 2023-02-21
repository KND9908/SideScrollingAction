using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEditor;

public class Player : CreatureBase
{
#if UNITY_EDITOR
    //設定されたパラメータをインスペクター上から見れるようにする
    [CustomEditor(typeof(CreatureBase))]
#endif

    //現在のプレイアブルキャラクターのコード
    [SerializeField]
    private PlayerCode NowPlayerCode = PlayerCode.kit;

    //現在のプレイヤーキャラクター
    private enum PlayerCode : int
    {
        kit,
        rail
    }

    //プレイヤーキャラクターの状態
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
    [Tooltip("プレイヤーの通常時のグラフィック")]
    private GameObject ObjNormal;

    [SerializeField]
    [Tooltip("プレイヤーの走ってる時のグラフィック")]
    private GameObject ObjRun;

    [SerializeField]
    [Tooltip("プレイヤーの攻撃時のグラフィック")]
    private GameObject ObjBeat;

    [SerializeField]
    [Tooltip("プレイヤーのロケットパンチ発射時のグラフィック")]
    private GameObject ObjRocketPunch;

    [SerializeField]
    [Tooltip("プレイヤーのダメージ受けた時のグラフィック")]
    private GameObject ObjDamage;

    [SerializeField]
    [Tooltip("プレイヤーの必殺技発動時のグラフィック")]
    private GameObject ObjSpecialMove;


    [Tooltip("攻撃した時のアニメーションのグラフィック時間")]
    [SerializeField]
    private int WaitFrame;

    [SerializeField]
    [Tooltip("プレイヤーの装着する武器　デフォルトはガントレットを設定する")]
    private GameObject Weapon;

    [SerializeField]
    [Tooltip("プレイヤーの着地判定を行うセンサー")]
    private GameObject GroundCensor;

    [SerializeField]
    [Tooltip("攻撃力")]
    public int AttackPower = 1;

    [SerializeField]
    [Tooltip("ダメージ受けたときの透明不透明を切り替えるための変数")]
    private GameObject InvisiObj;

    //武器管理用クラス
    private GameObject Instantitate_Weapon;

    //プレイヤーの現在のモード
    GameObject NowMode;

    //2段ジャンプをしたかの判定
    private bool TwiceJump = false;
    //攻撃中か否か
    private bool NowAttackAction = false;
    //進行方向に向くための反転
    private bool VisualInversion = false;
    public bool GetSetVisualInversion { get { return VisualInversion; } set { VisualInversion = value; } }

    private float JumpVelocity = 0;
    // 現在のプレイヤーのモード
    private PlayerMode NowPlayerMode;

    // ダメージをくらっているか否か
    private bool DamageNow = false;

    //プレイヤーの行動時にかかるキーロック
    private bool ActionKeyLock = false;

    //ジャンプした位置
    private float JumpPos = 0;

    //ジャンプ出来る高さのMAX
    [SerializeField]
    private float MAXJump = 0;

    private float JumpCount = 0;
    public int HaveOpenKey { get; set; } = 0;

    // ゲームマネージャによってキーロックがかかっているか否か
    private bool GM_KeyLock => ScrGamemanager.KeyLock;
    private Guntret _Guntret;
    private Rigidbody _Rigidbody;
    PlayerCensorGround _PlayerCensorGround => GroundCensor.GetComponent<PlayerCensorGround>();
    private new bool OnGround => _PlayerCensorGround.OnGround;

    //RigidBodyに付加するVelocity
    Vector3 SetVelocity;

    //移動している方向
    private float Movevec = 0;

    //HPに変動があったときに発生させるイベント
    public event System.Action<int> UpdateHP;

    private void Start()
    {
        //初期値の代入処理
        NowHP = MaxHP;
        NowPlayerMode = PlayerMode.Normal;
        NowMode = ObjNormal;
        SetVelocity = Vector3.zero;

        _Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //イベント中はキー入力の操作を無効にする
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
            if (!JumpNow) SetVisible(PlayerMode.Normal);//キーロックかかっている時は動作はなにもできないのでノーマルに
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
        //プレイヤーの向きを変える
        if (VisualInversion)
        {
            NowMode.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            NowMode.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //プレイヤーの捜査状況によって、表示するオブジェクトを変更する
    private void SetVisible(PlayerMode mode)
    {
        if (!DeathFlag)
        {
            GameObject obj = NowMode;
            //ダメージ受けてたら問答無用でダメージのエフェクトに変化させる
            if (mode == PlayerMode.Damage)
            {
                obj = ObjDamage;
                //ダメージ受けたときの描写変更
            }
            //ジャンプ中なら問答無用でジャンプに切り替える
            if (mode == PlayerMode.Jump)
            {
                obj = ObjNormal;
                //ジャンプ中はこの描画が優先される（これだけアニメーションの管理になると思われる）
            }

            if (mode == PlayerMode.Fall)
            {
                obj = ObjNormal;
                //ジャンプ中はこの描画が優先される（これだけアニメーションの管理になると思われる）
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
        //重力加速度は正弦波で表現したいため実装
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
        //攻撃用のエフェクトを表示
        SetVisible(PlayerMode.Beat);
        for (int i = 0; i < WaitFrame; i++)
        {
            yield return null;
        }
        GetSetGodMode = false;
        NowAttackAction = false;
        ActionKeyLock = false;
        //表示モーションを攻撃に変化
        yield break;
    }

    private IEnumerator HeavyAttack()
    {
        ActionKeyLock = true;
        NowAttackAction = true;
        //攻撃用のエフェクトを表示
        Instantitate_Weapon = Instantiate(Weapon);

        if (NowPlayerCode == PlayerCode.kit)
        {
            SetVisible(PlayerMode.RocketPunch);
            //子オブジェクト生成
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
        //表示モーションを攻撃に変化
        yield break;
    }

    //無敵時間を管理する処理
    //無敵時間中は点滅をする
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
            if (flashingchgtime_count >= flashingchgtime)//ダメージ食らった時のウェイト
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
        //HPが0になった時の処理
        DeathFlag = true;

        while (NowMode.GetComponent<SpriteRenderer>().color.r > 0)
        {
            NowMode.GetComponent<SpriteRenderer>().color -= new Color(1 / minusframe * Time.deltaTime,
                1 / minusframe * Time.deltaTime, 1 / minusframe * Time.deltaTime, 0);
            yield return null;
        }
        //シーンチェンジ
        NowMode.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 1);
        ActionKeyLock = false;
        yield break;
    }
    public IEnumerator Damaged()
    {
        //ダメージをくらった時のエフェクトを表示
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
            Debug.LogError("想定していないアイテムを取得");
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            TwiceJump = false;
        }
        //ダメージ処理を行った後一定時間無敵にする
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


