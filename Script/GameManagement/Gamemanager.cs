using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Gamemanager : MonoBehaviour
{
    [Tooltip("操作するプレイヤー")]
    [SerializeField]
    private GameObject _ObjPlayer;

    [Tooltip("フェードアウトの黒幕")]
    [SerializeField]
    private GameObject _FadeOut;

    [Tooltip("クリアした時に表示するロゴ")]
    [SerializeField]
    private GameObject _Clear;

    [Tooltip("プレイ開始時に表示するロゴ")]
    [SerializeField]
    private GameObject _Ready;

    [Tooltip("ボス戦開始前に表示するロゴ")]
    [SerializeField]
    private GameObject _Warning;

    [Tooltip("ゲーム内でプレイヤーを追従するカメラ")]
    [SerializeField]
    private GameObject _ObjCinemaChine;

    [Tooltip("シネマシーンで追従するステージ内の範囲")]
    [SerializeField]
    private GameObject _ObjStageCamera;

    [Tooltip("ボス戦のエリア内を映すステージ内の範囲")]
    [SerializeField]
    private GameObject _ObjBossCamera;

    [Tooltip("音楽を再生するAudio")]
    [SerializeField]
    private GameObject _ObjAudioBGM;

    [Tooltip("Jsonからファイルを読み書きするオブジェクトJsonReaderをここに設定")]
    [SerializeField]
    private GameObject _ObjJsonReader;

    [Tooltip("ボスオブジェクトのGameObjectをここに設定")]
    [SerializeField]
    private GameObject _ObjBoss;

    [Tooltip("ヒントを表示するアイコン")]
    [SerializeField]
    private GameObject _ObjCallIcon;

    [Tooltip("台詞を管理するコントローラー")]
    [SerializeField]
    private GameObject _ObjLabelController;

    public bool KeyLock = true;

    [Tooltip("通常時BGM")]
    [SerializeField]
    private AudioClip _ObjNormalSong;

    [Tooltip("ボス戦時BGM")]
    [SerializeField]
    private AudioClip _ObjBossSong;

    [Tooltip("ステージクリアの時のジングル")]
    [SerializeField]
    private AudioClip _ObjStageClearzingle;

    public bool EventNow = true;

    //イニシャライズ判定
    private bool _init = false;

    public int StackWords = 0;

    public bool BossDeadFlag = false;
        
    //カメラのデフォルト位置
    private Vector3 _Cam_DefaultOffset;

    private Image _NoticeImage;

    private FadeOut _CompFadeOut;
    private CinemachineConfiner2D _CompCinemachineConfiner2D;
    private PolygonCollider2D _CompBossPolygonCollider2D;
    private CinemachineVirtualCamera _CinemachineVirtualCamera;
    private CinemachineFramingTransposer _CompCinemachineFramingTransposer;
    private JsonReader _CompJsonReader;
    private LabelController _CompLabelController;
    private AudioSource _CompAudioSource;
    private Player _CompPlayer;
    private Boss _CompBoss;

    public bool Clearflag = false;
    private void Start()
    {
        _NoticeImage = _ObjCallIcon.GetComponent<Image>();

        _CinemachineVirtualCamera = _ObjCinemaChine.GetComponent<CinemachineVirtualCamera>();
        _CompCinemachineFramingTransposer = _CinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _Cam_DefaultOffset = _CompCinemachineFramingTransposer.m_TrackedObjectOffset;
        _CompFadeOut = _FadeOut.GetComponent<FadeOut>();
        _CompCinemachineConfiner2D = _ObjCinemaChine.GetComponent<CinemachineConfiner2D>();
        _CompBossPolygonCollider2D = _ObjBossCamera.GetComponent<PolygonCollider2D>();
        _CompJsonReader = _ObjJsonReader.GetComponent<JsonReader>();
        _CompLabelController = _ObjLabelController.GetComponent<LabelController>();
        _CompAudioSource = _ObjAudioBGM.GetComponent<AudioSource>();
        _CompPlayer = _ObjPlayer.GetComponent<Player>();
        _CompBoss = _ObjBoss.GetComponent<Boss>();
    }

    // Update is called once per frame
    private void Update()
    {
        //プレイヤーの死亡フラグがたったらゲームにロックをかける
        if (_CompPlayer.GetSetDeathFlag)
            KeyLock = true;

        if ("Demo" != SceneManager.GetActiveScene().name)
        {   //デモシーン以外での処理
            //ゲーム開始時の処理
            if (!_init && !EventNow && !KeyLock)
            {
                _init = true;

                _CompAudioSource.Play();
                StartCoroutine(_GameStart());
            }
            //言葉のスタックがある場合、ヒントを表示する
            if (StackWords != 0)
            {
                //ヒントを表示できる通知をもらった時、アイコンを表示
                if (!_NoticeImage.enabled)
                {
                    _NoticeImage.enabled = true;
                }
                //Fボタンを押下することでヒントを表示
                if (Input.GetButtonDown("Hint"))
                {
                    _CallHint();
                }
            }
            //表示する文字のスタックがなくなった場合アイコンを非表示
            else if (_NoticeImage.enabled)
            {
                _NoticeImage.enabled = false;
            }

            //イベント発生時もアイコンを非表示
            if (KeyLock)
            {
                if (_NoticeImage.enabled)
                {
                    _NoticeImage.enabled = false;
                }
            }

            //プレイヤーの方向に合わせてカメラを移動させる
            if (_CompPlayer.GetSetVisualInversion)
            {
                _CompCinemachineFramingTransposer.m_TrackedObjectOffset = new Vector3(_Cam_DefaultOffset.x * -1, _Cam_DefaultOffset.y, _Cam_DefaultOffset.z);
            }
            else
            {
                _CompCinemachineFramingTransposer.m_TrackedObjectOffset = _Cam_DefaultOffset;
            }

            //プレイヤーが死んだときにゲームオーバー画面へ遷移する
            if (_CompPlayer.GetSetDeathFlag)
            {
                _GameOverCall();
            }

            //Clearflagがたったらステージクリアした処理をする
            if (Clearflag)
            {
                Clearflag = false;
                StartCoroutine(_StageClear());
            }

            //ボスのライフが0になったらイベントをコール　後ほどコールバックの形式にする
            if (_CompBoss.GetSetMaxHP <= 0 && !BossDeadFlag)
            {
                _BossDeathCall();
            }
        }
    }
    //ボスがやられた時の処理
    private void _BossDeathCall()
    {
        BossDeadFlag = true;
        _CompJsonReader.Callevent(21, "", true);
    }


    private void _CallHint()
    {
        if (!_CompLabelController.isAction)
        {
            //ここにたまる台詞は自由に表示できるものになるため、強制イベントのものはないので第3引数はfalse、画面遷移もないので第2引数もfalse
            _CompJsonReader.Callevent(StackWords, "", false);
            StackWords = 0;
        }
    }
    private IEnumerator _GameStart()
    {
        //Ready?を表示してゲーム開始
        KeyLock = true;
        float NowTime = 0;

        _Ready.SetActive(true);

        const float WAITTIME = 2.0f;
        while (NowTime < WAITTIME)//WaitTime
        {
            NowTime += Time.deltaTime;
            yield return null;
        }
        _Ready.SetActive(false);
        KeyLock = false;

        yield break;
    }
    //ゲームオーバー画面遷移用
    private void _GameOverCall()
    {
        _CompFadeOut.FadeOutCall("Scene/GameOver");
    }
    //暗転処理
    private void _NormalFadeOut()
    {
        _CompFadeOut.FadeOutCall("");
    }
    //ステージクリア処理
    private IEnumerator _StageClear()
    {
        // StageClearを表示するためのアニメーション開始
        StartCoroutine(_CompLabelController.RemoveView());

        // StageClear用のジングルを再生する
        _CompAudioSource.clip = _ObjStageClearzingle;
        _CompAudioSource.PlayOneShot(_CompAudioSource.clip);

        // 操作キーのロックを有効化する
        KeyLock = true;
        float NowTime = 0;

        // StageClear表示をアクティブ化する
        _Clear.SetActive(true);

        // StageClear時のジングルの再生中は処理を待機する
        while (_CompAudioSource.isPlaying)
        {
            NowTime += Time.deltaTime;
            yield return null;
        }

        // StageClear表示を非アクティブ化する
        _Clear.SetActive(false);

        // 操作キーのロックを無効化する
        KeyLock = false;

        //画面を暗転させる
        _CompFadeOut.FadeOutCall("Title");

        // LabelControllerのイベント終了フラグを有効化する
        _CompLabelController.isEventFin = true;

        // コルーチンの終了
        yield break;
    }
    //CSVに記述されているイベントコードを実行する処理
    public void EventAct(string str)
    {
        if (str == "bossbattlestart")
        {
            StartCoroutine(_BossBattleStart());
        }
        if (str == "anten")
        {
            StartCoroutine(_BlackOut());
        }
        if (str == "StageClear")
        {
            StartCoroutine(_StageClear());
        }
        if (str == "bgmstop")
        {
            StartCoroutine(_BgmStop());
        }
        if (str == "BossAppear")
        {
            //ボスのオブジェクトをアクティブ化する
            _ObjBoss.SetActive(true);
            _CompLabelController.isEventFin = true;
        }
    }

    /// <summary>
    /// ボス戦開始時に実行されるコルーチン
    /// </summary>
    private IEnumerator _BossBattleStart()
    {
        // 言葉のラベルを非表示にする
        StartCoroutine(_CompLabelController.RemoveView());

        // ボス戦用のオーディオを設定する
        _AudioChg(_ObjBossSong);

        // ボス戦用オーディオを再生する
        _CompAudioSource.Play();

        // ワーニングメッセージの表示を開始する
        _Warning.SetActive(true);

        // 2秒間待機する
        float time = 0;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // ワーニングメッセージの非表示化を行う
        _Warning.SetActive(false);

        // LabelControllerのイベント終了フラグを有効化する
        _CompLabelController.isEventFin = true;

        // コルーチンの終了
        yield break;
    }

    private IEnumerator _BgmStopAndBossFlag()
    {
        //BGMを止める
        StartCoroutine(_BgmStop());
        //カメラの描写範囲をボス戦時のものに変更する
        _CompCinemachineConfiner2D.m_BoundingShape2D = _CompBossPolygonCollider2D;
        //ボスのオブジェクトをアクティブ化する
        _ObjBoss.SetActive(true);
        //イベントの終了判定フラグ
        _CompLabelController.isEventFin = true;
        yield break;
    }

    private IEnumerator _BgmStop()
    {
        float AudioVolume = _CompAudioSource.volume;
        //徐々に音量を小さくする
        while (_CompAudioSource.volume != 0)
        {
            _CompAudioSource.volume = 0;
            yield return null;
        }
        _CompAudioSource.Stop();
        _CompAudioSource.volume = AudioVolume;

        // LabelControllerのイベント終了フラグを有効化する
        _CompLabelController.isEventFin = true;
    }
    /// <summary>
    /// 流す音楽の変更
    /// </summary>
    /// <param name="clip">流す音楽のclip</param>
    private void _AudioChg(AudioClip clip)
    {
        _CompAudioSource.clip = clip;
    }

    private IEnumerator _BlackOut()
    {
        _NormalFadeOut();
        _CompLabelController.isEventFin = true;
        yield break;
    }

    public float maxSpecialAttackGauge = 100f;  // 必殺技ゲージの最大値
    public float specialAttackIncrement = 10f;  // 敵を倒した時に増加する必殺技ゲージの量

    private float _currentSpecialAttackGauge = 0f;  // 現在の必殺技ゲージの量

    //現在の必殺技ゲージの量のゲッターセッター
    public float CurrentSpecialAttackGuide { get => _currentSpecialAttackGauge; set => _currentSpecialAttackGauge = value; }

    /// <summary>
    /// 敵を倒した時に呼び出される関数 
    /// </summary>
    public void EnemyDefeated()
    {
        _currentSpecialAttackGauge += specialAttackIncrement;  // 必殺技ゲージを増加させる

        if (_currentSpecialAttackGauge > maxSpecialAttackGauge)
        {
            _currentSpecialAttackGauge = maxSpecialAttackGauge;  // 必殺技ゲージが最大値を超えた場合は最大値に設定する
        }
    }

    /// <summary>
    /// 必殺技ゲージが満タンになったときに行う処理  
    /// <summary>
    public void SpecialAttack()
    {
        _currentSpecialAttackGauge = 0f;  // 必殺技ゲージを0にする
        //プレイヤーの必殺技処理を行う
    }
}
