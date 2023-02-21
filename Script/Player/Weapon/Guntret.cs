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
    private float TotalDistance;

    [Tooltip("ガントレットの発射速度")]
    [SerializeField]
    private float Speed = 3.0f;

    [SerializeField]
    public GameObject Player;

    [Tooltip("発射したときに表示する煙")]
    [SerializeField]
    private GameObject ParticleSmoke;

    [Tooltip("オブジェクトに触れた時に発生させる爆発")]
    [SerializeField]
    private GameObject ParticleExplosion;

    public enum Mode
    {
        Beat,
        RocketPunch
    }

    public Mode NowMode { get; set; } = Mode.Beat;

    private Vector3 DefaultPosition;

    public float VectorX = 0;
    public float VectorY = 0;

    private Vector2 MinusVect = new Vector2(1,1);

    private float Theta = 0;

    private float Deg = 0;

    private bool Inversion = false;
    public bool GetSetInversion { get { return Inversion; } set { Inversion = value; } }
    public Player _Player => Player.GetComponent<Player>();
    private int AttackPower => _Player.AttackPower;
    public int GetSetAttackPower => AttackPower;
    private void Start()
    {
        switch (NowMode){
            case Mode.Beat:
                break;
            case Mode.RocketPunch:
                RocketPunch();
                break;
        }
    }

    private void RocketPunch()
    {
        MinusVect.x = Inversion ? -1 : 1;
        MinusVect.y = VectorY < 0 ? -1 : 1;
        //入力キーの比率によって飛ばす角度を決定する
        float xhanten = Inversion ? 180 : 0;
        if (VectorY != 0)
        {
            if (VectorX == 0)
            {
                VectorX = 1;
            }
            Theta = Mathf.Abs(VectorY) / (Mathf.Abs(VectorX) + Mathf.Abs(VectorY)) * 90;
        }
        //オブジェクトの表示する角度を変更
        transform.rotation = Quaternion.Euler(0, xhanten, Theta * MinusVect.y - 90);

        Instantiate(ParticleSmoke, this.transform.position, Quaternion.identity);

        //オブジェクトの発射する角度を決定
        Deg = Mathf.Deg2Rad * Theta;

        DefaultPosition = transform.position;
        //発射
        transform.DOMove(new Vector3(DefaultPosition.x + TotalDistance * MinusVect.x, DefaultPosition.y + TotalDistance * Mathf.Sin(Deg) * MinusVect.y, 0), Speed)
        .SetEase(Ease.InQuad).OnComplete(Remove);
    }

    private void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //オブジェクトとぶつかった場合に爆炎を表示
        Instantiate(ParticleExplosion, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
