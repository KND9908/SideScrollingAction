using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureBase : MonoBehaviour
{
    [Tooltip("オブジェクトのHP")]
    [SerializeField]
    protected int _MaxHP = 0;
    //スポナーから値を設定する用のGetSet（基本Setしか使わないのでGetはついでに実装）
    public int GetSetMaxHP { get => _MaxHP; set => _MaxHP = value; }

    [SerializeField]
    [Tooltip("プレイヤーの現在のHP")]
    protected int _NowHP;
    public int GetSetNowLife { get => _NowHP; set => _NowHP = value; }

    [Tooltip("移動方向")]
    [SerializeField]
    protected float _MoveSpeed = 0.1f;
    public float GetSetMoveVec { get => _MoveSpeed; set => _MoveSpeed = value; }

    [Tooltip("無敵時間　インスペクターからのみ編集できるようにしてます")]
    [SerializeField]
    protected float _GodModeTime = 0.1f;

    //無敵状態フラグ
    protected bool _GodMode = false;
    public bool GetSetGodMode { get => _GodMode; set => _GodMode = value; }

    //地面にいるかの確認
    protected bool _OnGround = false;
    public virtual bool GetSetOnGround { get => _OnGround; set => _OnGround = value; }

    protected float _DamageWaitTime = 0;

    protected bool _DeathFlag = false;
    public bool GetSetDeathFlag { get => _DeathFlag; set => _DeathFlag = value;}

    //重力を正弦波でゆっくり加速させるためのカウント
    protected int _GravityFrameCnt = 0;
    //重力の大きさ（RigidBody.velocityに設定することを想定)
    protected float _GravityVelocity = 0;
    //ジャンプ中かの判定
    protected bool _JumpNow = false;

    [Tooltip("ゲームマネージャー　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject _GameManager;
    public GameObject GetSetGameManager { get=> _GameManager; set => _GameManager = value;}
    protected Gamemanager _CompGamenager => _GameManager.GetComponent<Gamemanager>();

    [Tooltip("オブジェクトにかかる重力の重さ")]
    [SerializeField]
    protected float _GravityPower = 9.8f;
    protected virtual void Gravity()
    {
        if (!_OnGround)
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
    protected virtual void GodModeCheck()
    {
        //無敵状態の解除チェック
        if (_GodMode)
        {
            _DamageWaitTime += UnityEngine.Time.deltaTime;
            //無敵時間解除
            if (_DamageWaitTime > _GodModeTime)
            {
                _GodMode = false;
            }
        }
    }
    protected virtual void Move() { }
}
