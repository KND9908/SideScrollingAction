using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    [Tooltip("操作するプレイヤー")]
    [SerializeField]
    private GameObject ObjPlayer;

    [Tooltip("フェードアウトの時の黒い幕")]
    [SerializeField]
    private GameObject FadeOut;

    [Tooltip("クリアした時に表示するロゴ")]
    [SerializeField]
    private GameObject Clear;

    [Tooltip("プレイ開始時に表示するロゴ　Ready")]
    [SerializeField]
    private GameObject Ready;

    [Tooltip("ボス戦開始前に表示するロゴ")]
    [SerializeField]
    private GameObject warning;

    [Tooltip("ゲーム内でプレイヤーを追従するカメラ")]
    [SerializeField]
    private GameObject CinemaChine;

    [Tooltip("シネマシーンで追従するステージ内の範囲")]
    [SerializeField]
    private GameObject StageCamera;

    [Tooltip("ボス戦のエリア内を映すステージ内の範囲")]
    [SerializeField]
    private GameObject BossCamera;

    [Tooltip("音楽を再生するAudio")]
    [SerializeField]
    private GameObject AudioBGM;

    [Tooltip("Jsonからファイルを読み書きするオブジェクトJsonReaderをここに設定")]
    [SerializeField]
    private GameObject JsonReader;

    [Tooltip("ボスオブジェクトのGameObjectをここに設定")]
    [SerializeField]
    private GameObject ObjBoss;

    [Tooltip("ヒントを表示するアイコン")]
    [SerializeField]
    private GameObject CallIcon;

    [Tooltip("台詞を管理するコントローラー")]
    [SerializeField]
    private GameObject Labelcontroller;

    public bool KeyLock = true;

    [Tooltip("通常時BGM")]
    [SerializeField]
    private AudioClip NormalSong;

    [Tooltip("ボス戦時BGM")]
    [SerializeField]
    private AudioClip BossSong;

    [Tooltip("ステージクリアの時のジングル")]
    [SerializeField]
    private AudioClip StageClearzingle;
    
    public bool EventNow = true;
    
    //イニシャライズ判定
    private bool init = false;

    public int StackWords = 0;

    //カメラのデフォルト位置
    private Vector3 Cam_DefaultOffset;

    private Image NoticeImage;

    private FadeOut ScrFadeOut => FadeOut.GetComponent<FadeOut>();
    private CinemachineConfiner2D CNM2D => CinemaChine.GetComponent<CinemachineConfiner2D>();
    private PolygonCollider2D BOSSPC2D => BossCamera.GetComponent<PolygonCollider2D>();
    private CinemachineVirtualCamera CMV => CinemaChine.GetComponent<CinemachineVirtualCamera>();
    private CinemachineFramingTransposer CFT => CMV.GetCinemachineComponent<CinemachineFramingTransposer>();
    private JsonReader _JsonReader => JsonReader.GetComponent<JsonReader>();
    private LabelController ScrLabelController => Labelcontroller.GetComponent<LabelController>();
    private AudioSource _AudioSource => AudioBGM.GetComponent<AudioSource>();
    private Player ScrPlayer => ObjPlayer.GetComponent<Player>();
    private Boss ScrBoss => ObjBoss.GetComponent<Boss>();

    public bool Clearflag = false;
    private void Start()
    {
        NoticeImage = CallIcon.GetComponent<Image>();

        Cam_DefaultOffset = CFT.m_TrackedObjectOffset;
    }

    // Update is called once per frame
    private void Update()
    {
        //プレイヤーの死亡フラグがたったらゲームにロックをかける
        if (ScrPlayer.GetSetDeathFlag)
            KeyLock = true;

        if ("Demo" != SceneManager.GetActiveScene().name)
        {
            if (!init && !EventNow && !KeyLock)
            {
                init = true;

                _AudioSource.Play();
                StartCoroutine(GameStart());
            }

            if (StackWords != 0)
            {
                //ヒントを表示できる通知をもらった時、アイコンを表示
                if (!NoticeImage.enabled)
                {
                    NoticeImage.enabled = true;
                }
                //Fボタンを押下することでヒントを表示
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CallHint();
                }
            }
            //表示する文字のスタックがなくなった場合アイコンを非表示
            else if (NoticeImage.enabled)
            {
                NoticeImage.enabled = false;
            }

            //イベント発生時もアイコンを非表示
            if (KeyLock)
            {
                if (NoticeImage.enabled)
                {
                    NoticeImage.enabled = false;
                }
            }

            //プレイヤーの方向に合わせてカメラを移動させる
            if (ScrPlayer.GetSetVisualInversion)
            {
                CFT.m_TrackedObjectOffset = new Vector3(Cam_DefaultOffset.x * -1, Cam_DefaultOffset.y, Cam_DefaultOffset.z);
            }
            else
            {
                CFT.m_TrackedObjectOffset = Cam_DefaultOffset;
            }

            //プレイヤーが死んだときにゲームオーバー画面へ遷移する
            if (ScrPlayer.GetSetDeathFlag)
            {
                GameOverCall();
            }

            //Clearflagがたったらステージクリアした処理をする
            if (Clearflag)
            {
                Clearflag = false;
                StartCoroutine(StageClear());
            }
        }
    }

    private void CallHint()
    {
        if (!ScrLabelController.ActionNow)
        {
            //ここにたまる台詞は自由に表示できるものになるため、強制イベントのものはないので第3引数はfalse、画面遷移もないので第2引数もfalse
            _JsonReader.CensorCallevent(StackWords, "", false);
            StackWords = 0;
        }
    }
    private IEnumerator GameStart()
    {
        //Ready?を表示してゲーム開始
        KeyLock = true;
        float NowTime = 0;

        Ready.SetActive(true);

        const float WaitTime = 2.0f;
        while (NowTime < WaitTime)//WaitTime
        {
            NowTime += Time.deltaTime;
            yield return null;
        }
        Ready.SetActive(false);
        KeyLock = false;

        yield break;
    }
    //ゲームオーバー画面遷移用
    private void GameOverCall()
    {
        ScrFadeOut.FadeOutCall("Scene/GameOver");
    }

    private void NormalFadeOut()
    {
        ScrFadeOut.FadeOutCall("");
    }
    private IEnumerator StageClear()
    {
        //StageClearを表示
        StartCoroutine(ScrLabelController.RemoveView());
        _AudioSource.clip = StageClearzingle;
        _AudioSource.PlayOneShot(_AudioSource.clip);
        KeyLock = true;
        float NowTime = 0;

        Clear.SetActive(true);
        while (_AudioSource.isPlaying)//WaitTime
        {
            NowTime += Time.deltaTime;
            yield return null;
        }
        Clear.SetActive(false);
        KeyLock = false;

        ScrLabelController.isEventFin = true;
        yield break;
    }
    //ステージの箇所によって台詞を喋れるか否かもCSVにて確認する
    //CSVから読み込んでたデータを処理する際に関数をCallする記述が描かれていた場合にはここにて処理をする
    public void EventAct(string str)
    {
        if (str == "bossbattlestart")
        {
            StartCoroutine(BossBattleStart());
        }
        if (str == "anten")
        {
            StartCoroutine(BlackOut());
        }
        if (str == "StageClear")
        {
            StartCoroutine(StageClear());
        }
    }

    private IEnumerator BossBattleStart()
    {
        StartCoroutine(ScrLabelController.RemoveView());
        float time = 0;
        AudioChg(BossSong);
        _AudioSource.Play();
        //WARNING的な奴を表示する処理
        warning.SetActive(true);
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;

        }
        warning.SetActive(false);
        ScrLabelController.isEventFin = true;
        yield break;
    }

    private IEnumerator BgmStopAndBossFlag()
    {
        StartCoroutine(BgmStop());
        CNM2D.m_BoundingShape2D = BOSSPC2D;
        ObjBoss.SetActive(true);
        ScrLabelController.isEventFin = true;
        yield break;
    }

    private IEnumerator BgmStop()
    {
        float AudioVolume = _AudioSource.volume;
        //徐々に音量を小さくする
        while (_AudioSource.volume != 0)
        {
            _AudioSource.volume = 0;
            yield return null;
        }
        _AudioSource.Stop();
        _AudioSource.volume = AudioVolume;
    }
    /// <summary>
    /// 流す音楽の変更
    /// </summary>
    /// <param name="clip">流す音楽のclip</param>
    private void AudioChg(AudioClip clip)
    {
        _AudioSource.clip = clip;
    }

    private IEnumerator BlackOut()
    {
        NormalFadeOut();
        ScrLabelController.isEventFin = true;
        yield break;
    }
}
