using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LabelController : MonoBehaviour
{
    public string[] texts;

    [SerializeField]
    private TextMeshProUGUI TxtWords_Label;
    [SerializeField]
    private GameObject TxtWords_BackScreen;
    [SerializeField]
    private GameObject TxtWords_Icon;
    [SerializeField]
    private GameObject TxtWords_Frame;
    [SerializeField]
    private GameObject JsonReader;
    [SerializeField]
    private GameObject GameManager;
    [SerializeField]
    private GameObject FadeOut;

    private int TextNum;//読み込んだテキストの番号
    private string DisplayText = "";
    private int TextCharNum;//テキストの文字の番号
    
    //文字の描画速度調整用変数
    private int CntCharDrawSpeed = 0;
    [SerializeField]
    private int dispcharspeed = 1;

    private bool isClick = false;//クリックの判定

    private float FFTime = 0;

    public bool ActionNow = false;

    public bool InterruptEvent = false;//強制イベントか否か

    public bool ViewVisible = false;

    public bool CoroutineAct = false;

    public bool isEventFin = false;

    private bool isTextView = false;
    private Gamemanager ScrGamemanager => GameManager.GetComponent<Gamemanager>();
    private JsonReader ScrJsonReader => JsonReader.GetComponent<JsonReader>();
    private FadeOut ScrFadeOut => FadeOut.GetComponent<FadeOut>();
    private Image IconNowImage => TxtWords_Icon.GetComponent<Image>();
    [SerializeField]
    private Texture2D PlayerImage;
    [SerializeField]
    private Texture2D BossImage;
    [SerializeField]
    private Texture2D OtherImage;

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
    public IEnumerator DispText(bool Auto, bool speechbubble, int textstart, int textfin, string nextscene, bool KeyLock)
    {
        ActionNow = true;
        bool textstop = false;//ロードのくくりが完了した場合
        if ("Demo" != SceneManager.GetActiveScene().name)
        {
            ScrGamemanager.KeyLock = KeyLock;
        }
        TextNum = textstart;
        int nowicon = 0;

        while (!textstop)
        {
            //強制イベントが発生した場合は全ての動作を止め、イベント用のテキストを表示する
            if (InterruptEvent)
            {
                DisplayText = "";
                TextCharNum = 0;
                TextNum = 0;
                textstop = true;
                FFTime = 0;
                InterruptEvent = false;
                //吹き出しがあるうえでの読み込み処理をしていた場合、吹き出しを隠す
                if (speechbubble)
                {
                    StartCoroutine(RemoveView());
                    ScrGamemanager.EventNow = false;
                }

                if (KeyLock)
                    ScrGamemanager.KeyLock = false;
                ActionNow = false;
                yield break;
            }
            else
            {
                if (!textstop)
                {
                    //イベントコードを読み込んだ場合
                    if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._specialflag != "")
                    {
                        if (!CoroutineAct)
                        {
                            CoroutineAct = true;
                            //とりあえずその内容で処理を変えてみるのを直訳した関数で書く
                            ScrGamemanager.EventAct(ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._specialflag);
                        }
                        else
                        {
                            if (isEventFin)
                            {
                                //イベント終わったら次の行読み込む
                                if (TextNum < textfin)
                                {
                                    TextNum++;
                                }
                                else
                                {
                                    TextNum = 0;
                                    textstop = true;
                                }
                                CoroutineAct = false;
                                isEventFin = false;
                            }
                        }
                    }
                    //文字が終端まで行ってない場合
                    else if (TextCharNum != ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._words.Length)
                    {
                        if (!isTextView)
                        {
                            StartCoroutine(CallView());
                            while (!ViewVisible)
                            {
                                yield return null;
                            }
                        }
                        //アイコンの表示位置変更
                        if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._iconpos == "1")
                        {
                            TxtWords_Icon.transform.localPosition = new Vector3(-267,130,0);
                        }
                        else if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._iconpos == "2")
                        {
                            TxtWords_Icon.transform.localPosition = new Vector3(282, 130, 0);
                        }
                        //アイコンの内容変更
                        if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._character == "キット")
                        {
                            if (nowicon != 1)
                            {
                                nowicon = 1;
                                IconNowImage.sprite = Sprite.Create(PlayerImage, new Rect(0, 0, PlayerImage.width, PlayerImage.height), Vector2.zero);
                            }
                        }
                        else if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._character == "レール")
                        {
                            if (nowicon != 2)
                            {
                                nowicon = 2;
                                IconNowImage.sprite = Sprite.Create(BossImage, new Rect(0, 0, BossImage.width, BossImage.height), Vector2.zero);
                            }
                        }
                        else
                        {
                            if (nowicon != 0)
                            {
                                nowicon = 0;
                                IconNowImage.sprite = Sprite.Create(OtherImage, new Rect(0, 0, OtherImage.width, OtherImage.height), Vector2.zero);
                            }
                        }


                        CntCharDrawSpeed++;
                        if (CntCharDrawSpeed % dispcharspeed == 0)//文字の描画速度の調整
                        {
                            //表示する文字列に1文字だけ読み込んだテキスト文字を追加する
                            char addserihu = ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._words[TextCharNum];
                            DisplayText = DisplayText + addserihu;
                            TextCharNum = TextCharNum + 1;
                        }
                    }
                    else
                      if (TextNum < textfin)
                    {
                        FFTime += Time.deltaTime;
                        if (isClick)
                        {//追加する文字をクリアにし文章の番号を最初に戻す
                            DisplayText = "";
                            TextCharNum = 0;
                            TextNum++;
                            FFTime = 0;
                        }
                    }
                    else
                    {
                        FFTime += Time.deltaTime;
                        //全部読んだうえでクリックされたらもろもろを初期化
                        if (isClick)
                        {
                            DisplayText = "";
                            TextCharNum = 0;
                            TextNum = 0;
                            textstop = true;
                            FFTime = 0;
                            //吹き出しがあるうえでの読み込み処理をしているかで処理を?行うか否かがわかれる
                            if (speechbubble)
                            {
                                StartCoroutine(RemoveView());
                                ScrGamemanager.EventNow = false;
                            }
                        }
                    }
                }
                //UIに表示
                TxtWords_Label.text = DisplayText;
                isClick = false;
                //マウスが押されたら次の文章を読み込む
                if (!Auto && Input.GetKeyDown(KeyCode.Return) || (Auto && FFTime > 2.0f))//またはAutoModeがTrueでかつタイムデルタタイムが２秒経過したとき
                {
                    isClick = true;
                }
                yield return null;
            }
        }
        //Eventmanagerのcallsceneが今のsceneと合致している時、かつ、EventManagerのタイミングが特定の値の時、EventManagerに描いてある処理を行う
        if (Auto || nextscene != "")
            //次のシーンへフェードアウトする処理
            ScrFadeOut.FadeOutCall(nextscene);

        if (KeyLock)
            ScrGamemanager.KeyLock = false;
        ActionNow = false;
        yield break;
    }
    public IEnumerator CallView()
    {
        TxtWords_BackScreen.GetComponent<Image>().enabled = true;
        TxtWords_Icon.GetComponent<Image>().enabled = true;
        TxtWords_Frame.GetComponent<Image>().enabled = true;
        for (int i = 0; i < 20; i++)
        {
            //Removetext();
            yield return null;
        }
        ViewVisible = true;
        isTextView = true;
        yield break;
    }
    //吹き出しを閉じる
    public IEnumerator RemoveView()
    {
        TxtWords_BackScreen.GetComponent<Image>().enabled = false;
        TxtWords_Icon.GetComponent<Image>().enabled = false;
        TxtWords_Frame.GetComponent<Image>().enabled = false;
        for (int i = 0; i < 50; i++)
        {
            yield return null;
        }
        ViewVisible = false;
        isTextView = false;
        yield break;
    }
}
