using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
public class ActionGround : MonoBehaviour
{
    //床の挙動内容を管理するenum
    public enum GroundType
    {
        //上下に動く
        UpDown,
        //左右に動く
        LeftRight,
        //回転する
        Rotation,
        //拡大縮小する
        Scale,
        //乗ったら特定位置まで移動する
        Move,
        //一定カウントで爆発する
        Explosion,
        //一定カウントで消える
        Vanish,
    }

    //この床の挙動内容
    public GroundType MoveType;

    [SerializeField]
    [Tooltip("床の速度")]
    private float _MoveSpeed;

    public float Amount_of_movement = 2.0f;

    private int _Movevec = 1;

    [Tooltip("初期位置")]
    private Vector3 _DefTransform;

    //移動前後のポジション
    private Vector3 _BeforePosition;

    private Vector3 _AfterPosition;

    Transform _PlayerTransform;
    //プレイヤーが乗っているかの判定
    private bool _PlayerOn = false;

    //動作させる内容

    void Start()
    {
        _DefTransform = transform.position;
    }

    private void FixedUpdate()
    {
        //動きのアクションを設定  
        SetAction(MoveType);
    }

    //動きのアクションを設定
    public void SetAction(GroundType type)
    {
        switch (type)
        {
            case GroundType.UpDown:
                //上下に動く
                //移動前のポジションを取得
                _BeforePosition = transform.position;
                //一定範囲を上下移動させる
                transform.position = new Vector3(_DefTransform.x, _DefTransform.y + Mathf.PingPong(Time.time * _MoveSpeed, Amount_of_movement), _DefTransform.z);
                //移動後のポジションを取得
                _AfterPosition = transform.position;
                //それをノーマライズ
                Vector3 _TotalDistance = _AfterPosition - _BeforePosition;
                //プレイヤーが乗っている場合
                if (_PlayerOn)
                {
                    //プレイヤーのポジションを移動分だけ移動
                    _PlayerTransform.position += _TotalDistance;

                }
                break;
            case GroundType.LeftRight:
                //左右に動く
                //移動前のポジションを取得
                _BeforePosition = transform.position;
                //一定範囲を左右移動させる
                transform.position = new Vector3(_DefTransform.x + Mathf.PingPong(Time.time * _MoveSpeed, Amount_of_movement), _DefTransform.y, _DefTransform.z);
                //移動後のポジションを取得
                _AfterPosition = transform.position;
                //それをノーマライズ
                _TotalDistance = _AfterPosition - _BeforePosition;
                //プレイヤーが乗っている場合
                if (_PlayerOn)
                {
                    //プレイヤーのポジションを移動分だけ移動
                    _PlayerTransform.position += _TotalDistance;
                }
                break;
            case GroundType.Rotation:
                //回転する
                break;
            case GroundType.Scale:
                //拡大縮小する
                break;
            case GroundType.Move:
                //乗ったら特定位置まで移動する
                break;
            case GroundType.Explosion:
                break;
            case GroundType.Vanish:
                //一定カウントで消える
                break;
        }
    }

    //爆発する床の処理
    private IEnumerator Explosion()
    {
        //カウント
        int _Count = 3;
        //一定カウントで爆発する
        while (_Count != 0)
        {
            //一秒待つ
            yield return new WaitForSeconds(1.0f);
            _Count--;
        }

        //爆発する
        Destroy(gameObject);

    }

    //プレイヤーがオブジェクトの上に乗った場合の処理
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _PlayerOn = true;
            //爆発する床の場合、爆発する床の処理を行う
            if (MoveType == GroundType.Explosion)
            {
                StartCoroutine(Explosion());
            }
            //プレイヤーのトランスフォームを取得
            _PlayerTransform = collision.gameObject.transform;
        }
    }

    //プレイヤーがオブジェクトから離れた場合の処理
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _PlayerOn = false;
            _PlayerTransform = null;
        }
    }
}
