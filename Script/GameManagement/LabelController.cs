using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
/// <summary>
/// 台詞の表示を管理するクラス
/// </summary>
public class LabelController : MonoBehaviour
{
    public string[] texts;

    [SerializeField]
    private TextMeshProUGUI _ObjTxt;
    [SerializeField]
    private GameObject _Obj_BackScreen;
    [SerializeField]
    private GameObject _Obj_Icon;
    [SerializeField]
    private GameObject _Obj_IconFrame;
    [SerializeField]
    private GameObject _ObjJsonReader;
    [SerializeField]
    private GameObject _ObjGameManager;
    [SerializeField]
    private GameObject _ObjFadeOut;

    private int _TextNum;//読み込んだテキストの番号
    private string _DisplayText = "";
    private int _TextCharNum;//テキストの文字の番号

    //文字の描画速度調整用変数
    private int _CntCharDrawSpeed = 0;
    [SerializeField]
    private int _DispCharSpeed = 1;

    private bool _isClick = false;//クリックの判定

    private float _FFTime = 0;//自動でテキストを表示する時間

    public bool isAction = false;

    public bool InterruptEvent = false;//強制イベントか否か

    public bool ViewVisible = false;

    public bool CoroutineAct = false;

    public bool isEventFin = false;//イベントが終了したか否か　後々プライベートに変更する

    private bool _isTextView = false;
    private Gamemanager _CompGamemanager => _ObjGameManager.GetComponent<Gamemanager>();
    private JsonReader _CompJsonReader => _ObjJsonReader.GetComponent<JsonReader>();
    private FadeOut _CompFadeOut => _ObjFadeOut.GetComponent<FadeOut>();
    private Image _IconNowImage => _Obj_Icon.GetComponent<Image>();
    [SerializeField]
    private Texture2D _PlayerImage;
    [SerializeField]
    private Texture2D _BossImage;
    [SerializeField]
    private Texture2D _OtherImage;

    public static LabelController Singleton;

    private void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
            Debug.Log("makeSingletonInstance:LabelController");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //ラベルに台詞を表示する処理
    public IEnumerator DispText(bool Auto, bool speechbubble, int textstart, int textfin, string nextscene, bool KeyLock)
    {
        isAction = true;
        bool textstop = false;//ロードのくくりが完了した場合
        if ("Demo" != SceneManager.GetActiveScene().name)
        {
            _CompGamemanager.KeyLock = KeyLock;
        }
        _TextNum = textstart;
        int nowicon = 0;

        while (!textstop)
        {
            //強制イベントが発生した場合は全ての動作を止め、イベント用のテキストを表示する
            if (InterruptEvent)
            {
                _DisplayText = "";
                _TextCharNum = 0;
                _TextNum = 0;
                textstop = true;
                _FFTime = 0;
                InterruptEvent = false;
                //吹き出しがあるうえでの読み込み処理をしていた場合、吹き出しを隠す
                if (speechbubble)
                {
                    StartCoroutine(RemoveView());
                    _CompGamemanager.EventNow = false;
                }

                if (KeyLock)
                    _CompGamemanager.KeyLock = false;
                isAction = false;
                yield break;
            }
            else
            {
                if (!textstop)
                {
                    //イベントコードを読み込んだ場合
                    if (_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._specialflag != "")
                    {
                        if (!CoroutineAct)
                        {
                            CoroutineAct = true;
                            //対応イベントを実行
                            _CompGamemanager.EventAct(_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._specialflag);
                        }
                        else
                        {
                            if (isEventFin)
                            {
                                //イベント終わったら次の行読み込む
                                if (_TextNum < textfin)//読み込む行が最終行でない場合
                                {
                                    _TextNum++;
                                }
                                else
                                {
                                    _TextNum = 0;
                                    textstop = true;
                                }
                                CoroutineAct = false;
                                isEventFin = false;
                            }
                        }
                    }
                    //文字が終端まで行ってない場合
                    else if (_TextCharNum != _CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._words.Length)
                    {
                        if (!_isTextView)
                        {
                            StartCoroutine(CallView());
                            while (!ViewVisible)
                            {
                                yield return null;
                            }
                        }
                        //アイコンの表示位置変更
                        SetIconPosition(int.Parse(_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._iconpos));

                        nowicon = ChangeIconContent();


                        _CntCharDrawSpeed++;
                        if (_CntCharDrawSpeed % _DispCharSpeed == 0)//文字の描画速度の調整
                        {
                            //表示する文字列に1文字だけ読み込んだテキスト文字を追加する
                            char addserihu = _CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._words[_TextCharNum];
                            _DisplayText = _DisplayText + addserihu;
                            _TextCharNum = _TextCharNum + 1;
                        }
                    }
                    else
                      if (_TextNum < textfin)
                    {
                        _FFTime += Time.deltaTime;
                        if (_isClick)
                        {//追加する文字をクリアにし文章の番号を最初に戻す
                            _DisplayText = "";
                            _TextCharNum = 0;
                            _TextNum++;
                            _FFTime = 0;
                        }
                    }
                    else
                    {
                        _FFTime += Time.deltaTime;
                        //全部読んだうえでクリックされたらもろもろを初期化
                        if (_isClick)
                        {
                            _DisplayText = "";
                            _TextCharNum = 0;
                            _TextNum = 0;
                            textstop = true;
                            _FFTime = 0;
                            //吹き出しがあるうえでの読み込み処理をしているかで処理を?行うか否かがわかれる
                            if (speechbubble)
                            {
                                StartCoroutine(RemoveView());
                                _CompGamemanager.EventNow = false;
                            }
                        }
                    }
                }
                //UIに表示
                _ObjTxt.text = _DisplayText;
                _isClick = false;
                //マウスが押されたら次の文章を読み込む
                if (!Auto && Input.GetButtonDown("Hint") || (Auto && _FFTime > 2.0f))//またはAutoModeがTrueでかつタイムデルタタイムが２秒経過したとき(現在仮でジャンプと同じボタンにしている)
                {
                    _isClick = true;
                }



                yield return null;
            }
        }
        //Eventmanagerのcallsceneが今のsceneと合致している時、かつ、EventManagerのタイミングが特定の値の時、EventManagerに描いてある処理を行う
        if (Auto || nextscene != "")
            //次のシーンへフェードアウトする処理
            _CompFadeOut.FadeOutCall(nextscene);

        if (KeyLock)
            _CompGamemanager.KeyLock = false;
        isAction = false;
        yield break;
    }

    private void SetIconPosition(int iconPos)
    {
        if (iconPos == 1)
        { _Obj_Icon.transform.localPosition = new Vector3(-267, 130, 0); }
        else if (iconPos == 2)
        { _Obj_Icon.transform.localPosition = new Vector3(282, 130, 0); }
    }

    private int ChangeIconContent()
    {
        int nowicon = 0;
        if (_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._character == "キット")
        {
            if (nowicon != 1)
            {
                nowicon = 1;
                _IconNowImage.sprite = Sprite.Create(_PlayerImage, new Rect(0, 0, _PlayerImage.width, _PlayerImage.height), Vector2.zero);
            }
        }
        else if (_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._character == "レール")
        {
            if (nowicon != 2)
            {
                nowicon = 2;
                _IconNowImage.sprite = Sprite.Create(_BossImage, new Rect(0, 0, _BossImage.width, _BossImage.height), Vector2.zero);
            }
        }
        else
        {
            if (nowicon != 0)
            {
                nowicon = 0;
                _IconNowImage.sprite = Sprite.Create(_OtherImage, new Rect(0, 0, _OtherImage.width, _OtherImage.height), Vector2.zero);
            }
        }
        return nowicon;
    }

    public IEnumerator CallView()
    {
        _Obj_BackScreen.GetComponent<Image>().enabled = true;
        _Obj_Icon.GetComponent<Image>().enabled = true;
        _Obj_IconFrame.GetComponent<Image>().enabled = true;
        ViewVisible = true;
        _isTextView = true;
        yield break;
    }
    //吹き出しを閉じる
    public IEnumerator RemoveView()
    {
        _Obj_BackScreen.GetComponent<Image>().enabled = false;
        _Obj_Icon.GetComponent<Image>().enabled = false;
        _Obj_IconFrame.GetComponent<Image>().enabled = false;
        for (int i = 0; i < 50; i++)
        {
            yield return null;
        }
        ViewVisible = false;
        _isTextView = false;
        yield break;
    }
}
