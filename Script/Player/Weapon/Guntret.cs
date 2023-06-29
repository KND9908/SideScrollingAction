using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//オブジェクトの移動にはDoTweenを使用

public class Guntret : MonoBehaviour
{
    [SerializeField]
    public bool SpecialMove = false;

    [Tooltip("ガントレットが飛ばせるMAXの距離")]
    [SerializeField]
    private float _TotalDistance;

    [Tooltip("ガントレットの発射速度")]
    [SerializeField]
    private float _Speed = 3.0f;

    [SerializeField]
    public GameObject Player;

    [Tooltip("発射したときに表示する煙")]
    [SerializeField]
    private GameObject _ParticleSmoke;

    [Tooltip("オブジェクトに触れた時に発生させる爆発")]
    [SerializeField]
    private GameObject _ParticleExplosion;

    public enum Mode
    {
        Beat,
        RocketPunch
    }

    public Mode NowMode { get; set; } = Mode.Beat;

    private Vector3 _DefaultPosition;

    public float VectorX = 0;
    public float VectorY = 0;

    private Vector2 _MinusVect = new Vector2(1,1);

    private float _Theta = 0;

    private float _Deg = 0;

    private bool _Inversion = false;
    public bool GetSetInversion { get { return _Inversion; } set { _Inversion = value; } }
    public Player _Player => Player.GetComponent<Player>();
    private int _AttackPower => _Player.AttackPower;
    public int GetSetAttackPower => _AttackPower;
    private void Start()
    {
        switch (NowMode){
            case Mode.Beat:
                break;
            case Mode.RocketPunch:
                _RocketPunch();
                break;
        }
    }

    private void _RocketPunch()
    {
        _MinusVect.x = _Inversion ? -1 : 1;
        _MinusVect.y = VectorY < 0 ? -1 : 1;
        //入力キーの比率によって飛ばす角度を決定する
        float xhanten = _Inversion ? 180 : 0;
        if (VectorY != 0)
        {
            if (VectorX == 0)
            {
                VectorX = 1;
            }
            _Theta = Mathf.Abs(VectorY) / (Mathf.Abs(VectorX) + Mathf.Abs(VectorY)) * 90;
        }
        //オブジェクトの表示する角度を変更
        transform.rotation = Quaternion.Euler(0, xhanten, _Theta * _MinusVect.y - 90);

        Instantiate(_ParticleSmoke, this.transform.position, Quaternion.identity);

        //オブジェクトの発射する角度を決定
        _Deg = Mathf.Deg2Rad * _Theta;

        _DefaultPosition = transform.position;
        //発射
        transform.DOMove(new Vector3(_DefaultPosition.x + _TotalDistance * _MinusVect.x, _DefaultPosition.y + _TotalDistance * Mathf.Sin(_Deg) * _MinusVect.y, 0), _Speed)
        .SetEase(Ease.InQuad).OnComplete(_Remove);
    }

    private void _Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //オブジェクトとぶつかった場合に爆炎を表示
        Instantiate(_ParticleExplosion, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
