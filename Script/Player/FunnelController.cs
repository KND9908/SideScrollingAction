using UnityEngine;
using DG.Tweening;
using Mono.CompilerServices.SymbolWriter;

public class FunnelController : MonoBehaviour
{
    public Transform player; // プレイヤーのTransformを参照するための変数
    public float floatingSpeed = 1f; // ファンネルの浮遊速度
    public float rotationSpeed = 1f; // ファンネルの回転速度

    private Vector3 originalPosition; // 初期位置

    private Tweener tweener; //オブジェクトの移動が行われているか否か
    private Tweener floatingTweener;

    public float floatingHeight = 0.5f;
    public float floatingDuration = 2f;

    //敵を発見しているか否か
    private bool isFindEnemy = false;

    //敵のTransform
    private GameObject enemy;
    private Enemy scrEnemy;

    //レーザービームのオブジェクト
    public GameObject laserBeam;

    //プレイヤーとのオフセット位置
    public Vector3 offset = new Vector3(5, 0, 0);
    public float xoffset = 5;
    private void Start()
    {
        originalPosition = transform.position; // 初期位置を保存
                                               // 上下にふわふわと移動するTweenの設定
        floatingTweener = transform.DOMoveY(transform.position.y + floatingHeight, floatingDuration)
            .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).SetAutoKill(false).Pause();

    }

    private void Update()
    {
        // ファンネルを浮遊させる
        float newY = originalPosition.y + Mathf.Sin(Time.time * floatingSpeed);

        // ファンネルの位置のY軸は常にプレイヤーと一定の位置にあるものとする
        transform.position += new Vector3(0, newY, 0);

        if (isFindEnemy)
        {
            //enemy.transformが消滅している場合
            if (enemy == null)
            {
                isFindEnemy = false;
                return;
            }
            //敵の方へ1秒かけて移動する
            //transform.DOMove(enemy.transform.position, 1f);
            //敵の周囲のランダムな位置に移動する
            if (tweener == null || !tweener.IsActive())
            {
                tweener = transform.DOMove(enemy.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), 1f);
                //レーザー発射
                FireLaser();
            }
        }
        else
        {
            //プレイヤー座標の少し後ろを追いかけるように移動する　基本transform移動
            //transform.position = player.position;
            //移動しながら1秒かけてプレイヤーの後ろへ少し動かす
            if (tweener == null || !tweener.IsActive() && player.transform.position.x - player.transform.position.x + offset.x <= 0)
            {
                tweener = transform.DOMove(player.position + offset, 0.5f);
            }
            // 上下にふわふわと移動するTweenが再生中でなければ再生する
            if (!floatingTweener.IsActive())
            {
                floatingTweener.Play();
            }
        }
    }

    //ファンネルのスフィアコライダー内にenemyオブジェクトが入った場合の処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            //敵が画面内にいる場合
            if (other.gameObject.GetComponent<Enemy>().GetIsInsideCamera() && !isFindEnemy)
            {
                //敵を取得
                enemy = other.gameObject;
                scrEnemy = enemy.GetComponent<Enemy>();
                scrEnemy.OnDead += OnDead;
                isFindEnemy = true;
            }
        }
    }

    //敵が倒された時の検知処理
    private void OnDead()
    {
        isFindEnemy = false;
    }

    //ファンネルのスフィアコライダー内にenemyオブジェクトがいなくなった場合の処理
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            //プレイヤーの方へ1秒かけて移動する
            transform.DOMove(player.position, 1f);
            isFindEnemy = false;
        }
    }

    //ファンネルからレーザーを出す
    public void FireLaser()
    {
        //レーザーを出す
        GameObject laser = Instantiate(laserBeam, transform.position, Quaternion.identity);
        //レーザーの向きを敵の方向に向ける
        laser.transform.LookAt(enemy.transform);
    }
}
