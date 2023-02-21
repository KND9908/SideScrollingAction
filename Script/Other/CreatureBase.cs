using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureBase : MonoBehaviour
{
    [Tooltip("オブジェクトのHP")]
    [SerializeField]
    protected int MaxHP = 0;
    //スポナーから値を設定する用のGetSet（基本Setしか使わないのでGetはついでに実装）
    public int GetSetMaxHP { get { return MaxHP; } set { MaxHP = value; } }

    [SerializeField]
    [Tooltip("プレイヤーの現在のHP")]
    protected int NowHP;
    public int GetSetNowLife { get { return NowHP; } set { NowHP = value; } }

    [Tooltip("移動方向")]
    [SerializeField]
    protected float MoveSpeed = 0.1f;
    public float GetSetMoveVec { get { return MoveSpeed; } set { MoveSpeed = value; } }

    [Tooltip("無敵時間　インスペクターからのみ編集できるようにしてます")]
    [SerializeField]
    protected float GodModeTime = 0.1f;

    //無敵状態フラグ
    protected bool GodMode = false;
    public bool GetSetGodMode { get { return GodMode; } set { GodMode = value; } }

    //地面にいるかの確認
    protected bool OnGround = false;
    public bool GetSetOnGround { get { return OnGround; } set { OnGround = value; } }

    protected float DamageWaitTime = 0;

    protected bool DeathFlag = false;
    public bool GetSetDeathFlag { get { return DeathFlag; } set { DeathFlag = value; } }

    //重力を正弦波でゆっくり加速させるためのカウント
    protected int GravityFrameCnt = 0;
    //重力の大きさ（RigidBody.velocityに設定することを想定)
    protected float GravityVelocity = 0;
    //ジャンプ中かの判定
    protected bool JumpNow = false;

    [Tooltip("ゲームマネージャー　インスペクターからの操作は基本デバッグでしか使わない想定")]
    [SerializeField]
    protected GameObject GameManager;
    public GameObject GetSetGameManager { get { return GameManager; } set { GameManager = value; } }
    protected Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();

    [Tooltip("オブジェクトにかかる重力の重さ")]
    [SerializeField]
    protected float GravityPower = 9.8f;
    protected virtual void Gravity()
    {
        if (!OnGround)
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
    protected virtual void GodModeCheck()
    {
        //無敵状態の解除チェック
        if (GodMode)
        {
            DamageWaitTime += UnityEngine.Time.deltaTime;
            //無敵時間解除
            if (DamageWaitTime > GodModeTime)
            {
                GodMode = false;
            }
        }
    }
    protected virtual void Move() { }
}
