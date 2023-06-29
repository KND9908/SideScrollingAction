using System.Collections;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEditor;
/// <summary>
/// プレイヤー全般の処理を管理するクラス
/// </summary>
public class Player : CreatureBase
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CreatureBase))]
#endif

    /// <summary>
    /// 現在のプレイヤーキャラクターの種類を定義する列挙型
    /// </summary>
    private enum _PlayerCode : int
    {
        kit,
        rail
    }

    /// <summary>
    /// プレイヤーキャラクターのモーションを定義する列挙型
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
    [Tooltip("現在のプレイヤー（複数のキャラクターがプレイヤーになることを想定）")]
    private _PlayerCode currentPlayer = _PlayerCode.kit;


    [SerializeField]
    [Tooltip("攻撃した時のアニメーションをグラフィックする時間")]
    private int _WaitFrame = 1;

    [SerializeField]
    [Tooltip("プレイヤーの装着する武器　デフォルトはガントレットを設定する")]
    private GameObject _Weapon;

    [SerializeField]
    [Tooltip("プレイヤーの着地判定を行うセンサー用のオブジェクト")]
    private GameObject _GroundCensor;

    [SerializeField]
    [Tooltip("攻撃力")]
    public int AttackPower = 1;

    //以下プレイヤーのアニメーションを描画するのゲームオブジェクト　
    [SerializeField]
    [Tooltip("プレイヤーの通常時のグラフィック")]
    private GameObject _ObjNormal;
    public GameObject SetObjNormal { set => _ObjNormal = value; }

    [SerializeField]
    [Tooltip("プレイヤーが放つレーザーのオブジェクト")]
    private GameObject _ObjLaser;

    //アニメーションの反転判定
    private bool _VisualInversion = false;
    public bool GetSetVisualInversion { get => _VisualInversion; set => _VisualInversion = value; }
    //移動している方向
    private float _Movevec = 0;

    //ダメージ受けたときの点滅する際の透明不透明を切り替える時間
    [SerializeField]
    private GameObject _ObjInvisible;

    //2段ジャンプをしたかの判定
    private bool _TwiceJump = false;
    //攻撃中か否か
    private bool _IsAttack = false;

    // ダメージをくらっているか否か
    private bool _IsDamage = false;

    //プレイヤーの行動時にかかるキーロック
    private bool _KeyLockbyAct = false;

    //ジャンプ出来る高さのMAX
    [SerializeField]
    private float _MAXJump = 0;

    //ジャンプするときの上昇値
    [SerializeField]
    private float _JumpPower = 20;

    private float _JumpCount = 0;

    private int _HaveOpenKey = 0;

    private bool _GM_KeyLock => _CompGamenager.KeyLock;

    //以下コンポーネントの取得
    private Guntret _CompGuntret;
    private Rigidbody _CompRigidbody;
    private Animator AnimaorNormal;
    //プレイヤーキャラクターの全体の表示非表示を管理するアニメーター
    private Animator _CompAnimatorAll;

    [SerializeField]
    private AnimationClip ClipNormal;
    //プレイヤーが倒された時のアニメーションクリップ
    [SerializeField]
    private AnimationClip _ClipDead;


    private PlayerCensorGround _CompPlayerCensorGround => _GroundCensor.GetComponent<PlayerCensorGround>();
    //プレイヤーが地面に接地しているか否か
    public override bool GetSetOnGround
    {
        get => _CompPlayerCensorGround.OnGround;
        set => _OnGround = _CompPlayerCensorGround.OnGround;
    }
    //RigidBodyに付加するVelocity
    private Vector3 _SetRbVelocity;

    //HPに変動があったときに発生させるイベント
    public event System.Action<int> UpdateHP;

    private void Start()
    {
        //初期値の代入処理
        _NowHP = _MaxHP;
        _SetRbVelocity = Vector3.zero;
        _CompRigidbody = GetComponent<Rigidbody>();
        AnimaorNormal = _ObjNormal.GetComponent<Animator>();
        _CompAnimatorAll = _ObjInvisible.GetComponent<Animator>();
    }
    private void Update()
    {
        //イベント中はキー入力の操作を無効にする
        if (_GM_KeyLock)
        {
            // キー入力が無効の場合は横移動ができなくなり、ジャンプ中でない場合はノーマルモーションに戻す
            _SetRbVelocity.x = 0;
            if (!_JumpNow) _CompAnimatorAll.SetBool("isJump", false);
        }
        else
        {
            // キー入力が有効の場合はコマンドが実行可能
            HandleInput();
        }
    }
    //RigidBodyに関与した処理はFixedUpdateにて行う
    private void FixedUpdate()
    {
        Gravity();
        // ゲームマネージャーによってキー入力のロックがされていない場合、プレイヤーは動作が可能とする
        if (!_GM_KeyLock)
        {
            Move();
        }

        if (_JumpNow)
        {
            // ジャンプ中の場合はジャンプ処理を実行し、垂直速度をジャンプ力に設定する
            Jump();
            _SetRbVelocity.y = _JumpPower;
        }
        else
        {
            //着地した場合はisjumpをfalseにする
            if (GetSetOnGround)
            {
                _CompAnimatorAll.SetBool("isJump", false);
            }
            // ジャンプ中でない場合は、地面にいない場合重力を設定する
            _SetRbVelocity.y = GetSetOnGround ? 0 : _GravityVelocity;
            AnimaorNormal.SetBool("isFall", !GetSetOnGround);

        }

        // もし地面に着地していてかつ2回目のジャンプを実行していた場合は、判定をリセットする
        if (GetSetOnGround && _TwiceJump)
        {
            _TwiceJump = false;
        }
        _CompRigidbody.velocity = new Vector3(_SetRbVelocity.x, _SetRbVelocity.y, 0);
    }
    //レーザー発射の処理
    private void LaserShot()
    {
        //レーザーの発射
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
        //ダメージ処理を行った後一定時間無敵にする
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
    // 他のコライダーがトリガーに入った際の処理
    private void OnTriggerEnter(Collider other)
    {
        // オブジェクトがアイテムだった場合
        if (other.gameObject.GetComponent<Item>())
        {
            // アイテムの種類を取得してアイテム取得イベントを実行する。
            ItemGetEvent(other.gameObject.GetComponent<Item>().thisitem);
            Destroy(other.gameObject);
        }
        // トリガーに入ったオブジェクトが穴のセンサーだった場合
        else if (other.gameObject.CompareTag("Hole"))
        {
            // 無敵モードでない場合はダメージ処理を開始
            if (!GetSetGodMode)
            {
                StartCoroutine("Damaged");
            }
        }
        // トリガーに入ったオブジェクトがステージクリア用のフラグだった場合
        else if (other.gameObject.CompareTag("ClearFlag"))
        {
            _CompGamenager.Clearflag = true;
        }
        // トリガーに入ったオブジェクトが敵のスポナーだった場合
        else if (other.gameObject.CompareTag("Spawner"))
        {
            // Spawnerオブジェクトをアクティブ化する
            other.gameObject.SetActive(true);
        }
    }

    //必殺技の処理
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

    [SerializeField] private float _dashDuration = 0.5f; // ダッシュの持続時間
    [SerializeField] private float _dashSpeed = 10f; // ダッシュ時の移動速度
    private bool _IsDashing = false; // ダッシュ中かどうかのフラグ
    private float _DashTimer = 0f; // ダッシュの継続時間計測用のタイマー

    // プレイヤーの横移動を行う関数
    protected override void Move()
    {
        // 操作する方向を取得する
        _Movevec = Input.GetAxisRaw("Horizontal");
        // プレイヤーの向きを確認する
        VisualInversionCheck();

        // ジャンプ、ダメージ、攻撃中でない場合は以下の処理を実行する
        if (GetSetOnGround && !_JumpNow && !_IsDamage && !_IsAttack)
        {
            // 移動方向が0でない場合は、プレイヤーのモーションを「Run」に変更する
            if (_Movevec != 0)
            {
                //全体管理のアニメーターのモーションを変更する
                _CompAnimatorAll.SetBool("isRun", true);
            }
            // 移動方向が0である場合は、プレイヤーのモーションを「Normal」に変更する
            else if (_Movevec == 0)
            {
                //全体管理のアニメーターのモーションを変更する
                _CompAnimatorAll.SetBool("isRun", false);
            }
        }

        if (_IsDashing)
        {
            // ダッシュ中は移動速度を上げる
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
            // ゲームマネージャーによってキー入力のロックがされていない場合、プレイヤーは動作が可能とする
            _SetRbVelocity.x = _GM_KeyLock || _IsDamage ? 0 : _Movevec * _MoveSpeed;
            //ダッシュ中に空中にいた場合は着地するまで速度をダッシュ中のものにする

        }
    }
    //キー入力処理
    private void HandleInput()
    {
        // ジャンプの処理
        if (Input.GetButtonDown("Jump"))
        {
            if (GetSetOnGround && !_JumpNow)
            {
                // ジャンプが可能であればジャンプする
                _JumpNow = true;
                //SetVisible(_PlayerMotion.Jump);
                _CompAnimatorAll.SetBool("isJump", true);

            }
            else if (!_TwiceJump)
            {
                // ジャンプ中であれば2段ジャンプをする
                _JumpNow = true;
                _TwiceJump = true;
                _JumpCount = 0;
                // SetVisible(_PlayerMotion.Jump);
                _CompAnimatorAll.SetBool("isJump", true);
            }
        }

        if (Input.GetButtonDown("Fire1") && !_IsAttack)
        {
            // 強攻撃の処理
            StartCoroutine(HeavyAttack());
        }
        if (Input.GetButtonDown("Fire3") && !_IsAttack)
        {
            //もしゲームマネージャーの必殺技ゲージがたまっていたら発動できる
            if (_CompGamenager.CurrentSpecialAttackGuide >= 100)
            {
                // 特殊攻撃の処理
                StartCoroutine(SpecialMove());
            }
        }

        // ダッシュ処理　接地していてかつダッシュ中でない場合に行える
        if (Input.GetButtonDown("Dash") && !_IsDashing && GetSetOnGround)
        {
            _IsDashing = true;
            _DashTimer = _dashDuration;
        }
    }

    //オブジェクトの描画の反転処理
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
        //プレイヤーの向きを変える
        if (_VisualInversion)
        {
            _ObjInvisible.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            _ObjInvisible.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //ジャンプ処理
    private void Jump()
    {
        // ジャンプのカウントが90以下のとき
        if (_JumpCount < 90)
        {
            //ジャンプのカウントを増やす
            _JumpCount += 3;
            //ジャンプ力を計算する
            _JumpPower = _MAXJump * Mathf.Sin(Mathf.Deg2Rad * (90 - _JumpCount));
            //アニメーションをジャンプに変更する
            AnimaorNormal.SetBool("isJump", true);
            _CompAnimatorAll.SetBool("isJump", true);
        }
        //ジャンプが終了したとき
        else
        {
            //カウントを初期化する
            _JumpCount = 0;
            //ジャンプ状態を解除する
            _JumpNow = false;
            //アニメーションを通常状態に変更する
            AnimaorNormal.SetBool("isJump", false);
        }
    }


    private new void Gravity()
    {
        //重力加速度は正弦波で表現したいため実装
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
    // 強攻撃処理
    private IEnumerator HeavyAttack()
    {
        // アクション中は入力を受け付けないようにする
        _KeyLockbyAct = true;
        // 攻撃中フラグをオンにする
        _IsAttack = true;

        // 攻撃用エフェクトを表示する
        GameObject instantinateweapon = Instantiate(_Weapon);

        if (currentPlayer == _PlayerCode.kit)
        {

            // キットの場合
            _CompAnimatorAll.SetBool("isHeavyAttack", true);

            // 子オブジェクトを生成する
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
                // キットが逆向きの場合は反転する
                _CompGuntret.GetSetInversion = this._VisualInversion;
                guntoffset *= -1;
            }
            instantinateweapon.transform.position = new Vector3(this.transform.position.x + offsetx * guntoffset, this.transform.position.y + offsety * guntoffset, this.transform.position.z);

        }

        _KeyLockbyAct = false;
        // 攻撃中フラグをオフにする
        _IsAttack = false;

        //1秒待つ
        yield return new WaitForSeconds(1.0f);

        _CompAnimatorAll.SetBool("isHeavyAttack", false);

    }

    //無敵時間を管理する処理
    IEnumerator ChgGodMode()
    {
        GetSetGodMode = true;

        //2秒間無敵にする
        yield return new WaitForSeconds(2.0f);

        GetSetGodMode = false;
    }

    //死んだときの処理
    private IEnumerator Dead()
    {
        float minusframe = 2.0f;
        //HPが0になった時の処理
        _DeathFlag = true;

        //やられた時のアニメーションクリップを再生
        _CompAnimatorAll.Play(_ClipDead.name);

        //現在のアニメーション再生が終わるまで待機
        yield return new WaitForSeconds(_ClipDead.length - minusframe);

        //シーンチェンジ
        _KeyLockbyAct = false;
    }
    //ダメージ受けたときの処理
    public IEnumerator Damaged()
    {
        //ダメージをくらった時のエフェクトを表示
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

            //全体管理のアニメーターのisDamageをtrueにする
            _CompAnimatorAll.SetBool("isDamage", true);

            UpdateHP.Invoke(GetSetNowLife);

            //1秒ほど待機
            yield return new WaitForSeconds(1.0f);

            //全体管理のアニメーターのisDamageをfalseにする
            _CompAnimatorAll.SetBool("isDamage", false);
            _IsDamage = false;
        }
        yield break;
    }
    //アイテム取得時の処理
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
            Debug.LogError("想定していないアイテムを取得");
        }
    }
}


