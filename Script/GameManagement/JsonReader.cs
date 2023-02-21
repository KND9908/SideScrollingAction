using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WordsManager
{
    //Jsonファイルの見出しの名前と変数の名前は統一すること（読みだされなくなるため）
    public WordsManagement[] wordsmanagement;
}

[System.Serializable]
public class EventManager
{
    public EventFlag[] eventflag;
}
// プレイヤーのセリフおよび発生イベントを管理しているCSVを読み込むクラス
[System.Serializable]
public class WordsManagement
{
    public string _words;
    public string _character;
    public string _iconpos;
    public string _iconfacecode;
    public string _callscene;
    public string _timing;
    public float _eventcode;
    public bool _autoread;
    public string _specialflag;
}
// シーンごとにイベントを発生させるタイミングを管理しているクラス
[System.Serializable]
public class EventFlag
{
    public bool _specialflag;
    public bool _finish;
    public string _callscene;
    public float _timing;
    public string _callevent;
    public float _eventcode;
}

public class JsonReader : MonoBehaviour
{
    //シングルトンで実装を想定
    public static JsonReader Singleton;
    
    public WordsManager wordsmanager;
    public EventManager eventmanager;

    public int StartWords = 0;
    public int EndWords = 0;

    private int CallEventNum = 0;

    private string CallScene = "";
    private LabelController ScrLabelController;
    [SerializeField]
    [Tooltip("台詞の文字列を表示するテキストフィールド")]
    private GameObject TextField;
    private void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
            Debug.Log("makeSingletonInstance:JsonReader");
        }
        else
        {
            Destroy(gameObject);
        }
        JsonRead();
        EventRead();
        ScrLabelController = TextField.GetComponent<LabelController>();
    }

    // Update is called once per frame
    private void Update()
    {
        string EventCode = EventManagement();
        //demoシーンのファイルを読み込んだかチェックする関数
        bool EOF = false;
        if (EventCode == "Demo")
        {
            //行っていない処理を見つけ、処理を行わせる
            while (eventmanager.eventflag[CallEventNum]._callevent == "Demo" && !EOF)
            {
                if (!eventmanager.eventflag[CallEventNum]._finish)
                {
                    eventmanager.eventflag[CallEventNum]._finish = true;
                    CallEvent(true,false,eventmanager.eventflag[CallEventNum]._eventcode,EventCode,"title",false);
                }
                if (CallEventNum < eventmanager.eventflag.Length - 1)
                    CallEventNum++;
                else
                    EOF = true;
            }
        }
        else if (EventCode == "Begin")//現在のシーンがMainGameの場合
        {
            if (CallScene == SceneManager.GetActiveScene().name)
            {
                //行っていない処理を見つけ、処理を行わせる
                while (eventmanager.eventflag[CallEventNum]._callevent == "Begin" && !EOF)
                {
                    if (!eventmanager.eventflag[CallEventNum]._finish)
                    {
                        eventmanager.eventflag[CallEventNum]._finish = true;
                        //シーン開始時の処理だからKeyLockはTrueで
                        CallEvent(false,true,eventmanager.eventflag[CallEventNum]._eventcode, EventCode, "",true);
                    }
                    if (CallEventNum < eventmanager.eventflag.Length - 1)
                        CallEventNum++;
                    else
                        EOF = true;
                }
            }
        }
    }
    private void JsonRead()
    {
        string datapath = "WordsManagement";
        string inputString = Resources.Load<TextAsset>(datapath).ToString();
        wordsmanager = JsonUtility.FromJson<WordsManager>(inputString);
        Debug.Log(wordsmanager.wordsmanagement[0]._words);  // ロード出来てるかの確認用
    }

    private void EventRead()
    {
        string datapath = "EventFlag";
        string inputString = Resources.Load<TextAsset>(datapath).ToString();
        eventmanager = JsonUtility.FromJson<EventManager>(inputString);
        Debug.Log(eventmanager.eventflag[0]._callevent);  // ロード出来てるかの確認用
    }
    private string EventManagement()
    {
        string CallEvent = "NOT FOUND";
        //ReadしたEventの要素から、タイミングの合致する処理があった場合に該当EventをCallする処理
        for (int i = 0; i < eventmanager.eventflag.Length; i++)
        {
            //対象のシーンをフォーカス
            if (eventmanager.eventflag[i]._callscene == SceneManager.GetActiveScene().name)
            {
                if (!eventmanager.eventflag[i]._finish)
                {
                    //Timingが0なら問答無用でその対象の関数はすぐCall
                    if (eventmanager.eventflag[i]._timing == 0)
                    {
                        CallScene = eventmanager.eventflag[i]._callscene;
                        CallEvent = eventmanager.eventflag[i]._callevent;
                        CallEventNum = i;
                        return CallEvent;
                    }
                }
            }
        }
        return CallEvent;
    }

    //ステージに配置してあるセンサーからイベント処理の通知が来た場合の処理
    public void CensorCallevent(int CallCode,string NextScene,bool KeyLock)
    {
        bool StartCheck = false;
        for (int i = 0; i < wordsmanager.wordsmanagement.Length; i++)
        {
            if (wordsmanager.wordsmanagement[i]._callscene == SceneManager.GetActiveScene().name
    && wordsmanager.wordsmanagement[i]._eventcode == CallCode)
            {
                if (!StartCheck)
                {
                    StartCheck = true;
                    StartWords = i;
                }
                EndWords = i;
            }
        }
        StartCoroutine(ScrLabelController.DispText(false, true, StartWords, EndWords, NextScene, KeyLock));
    }
    private void CallEvent(bool AutoMode,bool hukidasi,float flagcode,string timing,string nextscene,bool KeyLock)
    {
        bool StartCheck = false;
        for (int i = 0; i < wordsmanager.wordsmanagement.Length; i++)
        {
            if (wordsmanager.wordsmanagement[i]._callscene == SceneManager.GetActiveScene().name
                && wordsmanager.wordsmanagement[i]._eventcode == flagcode && wordsmanager.wordsmanagement[i]._timing == timing)
            {
                if (!StartCheck)
                {
                    StartCheck = true;
                    StartWords = i;
                }
                EndWords = i;
            }
        }
        StartCoroutine(ScrLabelController.DispText(AutoMode, hukidasi, StartWords, EndWords,nextscene,KeyLock));
    }
}
