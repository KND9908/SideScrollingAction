using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using DG.Tweening;

public class Boss : EnemyBase
{
#if UNITY_EDITOR
    //設定されたパラメータをインスペクター上から見れるようにする
    [CustomEditor(typeof(EnemyBase))]
#endif
    // エネミーのGameObject
    [SerializeField]
    [Tooltip("エネミーのGameObject")]
    private GameObject _NowMode;

    // 攻撃中か否か
    private bool _AttackAction = false;

    // キャラクターの移動方向が右か否か
    private bool _VisualInversion = false;

    // 攻撃開始からの時間
    private float _Time = 0;

    // _BossDeadメソッドを一回だけ呼ぶためのフラグ
    private bool _CallOnceBossFinFlag = false;

    // コンポーネント
    private Animator _Animator => GetComponent<Animator>();

    // キャラクターの移動量
    private float _Movevec = 0;

    [SerializeField]
    [Tooltip("移動のvelocityの量の大きさ")]
    private float _MoveVelocity = 0;
    //RigidBodyに付加するVelocity
    private Vector3 _SetVelocity;

    //現在のアクション
    Tween _NowAction;

    private void Update()
    {
        //死亡フラグが立っているなら処理を行わない
        if (!_JumpNow && !_DeathFlag)
            //Gravity();

        //反転処理
        _VisualInversionCheck();

        //現在ロック中でなければ、以下攻撃モーションを行う
        if (!_CompGamenager.KeyLock)
        {
            //現在行動中でなければ、以下攻撃モーションを行う
            if (!_AttackAction)
                _Time += UnityEngine.Time.deltaTime;
            const float WAITTIME = 2.0f;
            //現在行動中でなければ、以下攻撃モーションを行う
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
    //どの攻撃をするか決定
    private void _AttackMoveJudge()
    {
        _Time = 0;
        _AttackAction = true;
        int rnd = Random.Range(1, 3);//番号によってランダムに攻撃動作する
        if (rnd == 1)
            StartCoroutine(MoveToPosition(_Player.transform.position.x));
        else if (rnd == 2)
            StartCoroutine(JumpToPosition(_Player.transform.position.x));
    }
    private void _BossDead()
    {
        //何かアクション中ならキャンセルする
        if (_NowAction != null)
            _NowAction.Kill();
        _CallOnceBossFinFlag = true;

    }

    /// <summary>
    /// 指定した位置まで移動するコルーチン
    /// </summary>
    /// <param name="pos">移動先の位置</param>
    public IEnumerator MoveToPosition(float pos)
    {
        // プレイヤーが敵の左側にいるか右側にいるかを判定し、移動方向を決定する
        Vector3 moveDir = (pos - transform.position.x > 0) ? Vector3.right : Vector3.left;

        // 指定された位置へ移動する前に少し後ろに下がる
        _NowAction = transform.DOMoveX(transform.position.x + ((-1) * moveDir.x * transform.localScale.x * 0.3f), 0.5f).OnComplete(() =>
        {
            // 指定された位置まで移動する
            _NowAction = transform.DOMoveX(pos, Mathf.Abs(pos - transform.position.x) / _MoveVelocity).OnComplete(() =>
            {
                // 攻撃が完了したことを示すフラグを立てる
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
        //プレイヤーの向きを変える
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
        //重力加速度はRigidBodyの既定のものではなく正弦波で表現したいため実装
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

    //指定位置までジャンプするコルーチン
    public IEnumerator JumpToPosition(float pos)
    {
        //ジャンプ中フラグを立てる
        _JumpNow = true;

        //ジャンプの高さを決定
        float jumpHeight = 0.5f;

        //ジャンプの時間を決定
        float jumpTime = 0.5f;

        //ジャンプの初速を決定
        float jumpVelocity = jumpHeight / jumpTime;

        //ジャンプの初速を与える
        _SetVelocity.y = jumpVelocity;

        //ジャンプの初速を与えた後、重力を与える
        //yield return new WaitForSeconds(jumpTime);
        _SetVelocity.y = _GravityVelocity;

        //ジャンプ中フラグを折る
        _JumpNow = false;

        // 攻撃が完了したことを示すフラグを立てる
        _AttackAction = false;

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
