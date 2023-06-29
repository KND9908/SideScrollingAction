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
    public bool specialflag;
    public bool finish;
    public string callscene;
    public float timing;
    public string callevent;
    public float eventcode;
}
/// <summary>
///　Jsonからファイルを読み取り、値を管理するクラス
/// </summary>
public class JsonReader : MonoBehaviour
{
    //シングルトンで実装を想定
    public static JsonReader Singleton;
    
    private WordsManager _CompWordsManager;
    public WordsManager GetWordsManager => _CompWordsManager;
    private EventManager _EventManager;
    public EventManager GetEventManager => _EventManager;

    public int StartWords = 0;
    public int EndWords = 0;

    private int _CallEventNum = 0;

    private string _CallScene = "";
    private LabelController _ScrLabelController;
    [SerializeField]
    [Tooltip("台詞の文字列を表示するテキストフィールド")]
    private GameObject _TextField;
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
        _JsonRead();
        _EventRead();
        _ScrLabelController = _TextField.GetComponent<LabelController>();
    }


    private void Update()
    {
        string EventCode = _EventManagement();
        //demoシーンのファイルを読み込んだかチェックする関数
        bool EOF = false;
        if (EventCode == "Demo")
        {
            //読み込んでいない文章を見つけ、処理を行わせる
            while (_EventManager.eventflag[_CallEventNum].callevent == "Demo" && !EOF)
            {
                if (!_EventManager.eventflag[_CallEventNum].finish)
                {
                    _EventManager.eventflag[_CallEventNum].finish = true;
                    FileRead(true,false,_EventManager.eventflag[_CallEventNum].eventcode,EventCode,"title",false);
                }
                if (_CallEventNum < _EventManager.eventflag.Length - 1)
                    _CallEventNum++;
                else
                    EOF = true;
            }
        }
        else if (EventCode == "Begin")//現在のシーンがMainGameの場合
        {
            if (_CallScene == SceneManager.GetActiveScene().name)
            {
                //行っていない処理を見つけ、処理を行わせる
                while (_EventManager.eventflag[_CallEventNum].callevent == "Begin" && !EOF)
                {
                    if (!_EventManager.eventflag[_CallEventNum].finish)
                    {
                        _EventManager.eventflag[_CallEventNum].finish = true;
                        //シーン開始時の処理だからKeyLockはTrueで
                        FileRead(false,true,_EventManager.eventflag[_CallEventNum].eventcode, EventCode, "",true);
                    }
                    if (_CallEventNum < _EventManager.eventflag.Length - 1)
                        _CallEventNum++;
                    else
                        EOF = true;
                }
            }
        }
    }
    private void _JsonRead()
    {
        string datapath = "WordsManagement";
        string inputString = Resources.Load<TextAsset>(datapath).ToString();
        _CompWordsManager = JsonUtility.FromJson<WordsManager>(inputString);
        Debug.Log(_CompWordsManager.wordsmanagement[0]._words);  // ロード出来てるかの確認用
    }

    private void _EventRead()
    {
        string datapath = "EventFlag";
        string inputString = Resources.Load<TextAsset>(datapath).ToString();
        _EventManager = JsonUtility.FromJson<EventManager>(inputString);
        Debug.Log(_EventManager.eventflag[0].callevent);  // ロード出来てるかの確認用
    }
    private string _EventManagement()
    {
        string CallEvent = "NOT FOUND";
        //ReadしたEventの要素から、タイミングの合致する処理があった場合に該当EventをCallする処理
        for (int i = 0; i < _EventManager.eventflag.Length; i++)
        {
            //対象のシーンをフォーカス
            if (_EventManager.eventflag[i].callscene == SceneManager.GetActiveScene().name)
            {
                if (!_EventManager.eventflag[i].finish)
                {
                    //Timingが0なら問答無用でその対象の関数はすぐCall
                    if (_EventManager.eventflag[i].timing == 0)
                    {
                        _CallScene = _EventManager.eventflag[i].callscene;
                        CallEvent = _EventManager.eventflag[i].callevent;
                        _CallEventNum = i;
                        return CallEvent;
                    }
                }
            }
        }
        return CallEvent;
    }

    //ベント処理の通知が来た場合の処理
    public void Callevent(int CallCode,string NextScene,bool KeyLock)
    {
        bool StartCheck = false;
        for (int i = 0; i < _CompWordsManager.wordsmanagement.Length; i++)
        {
            if (_CompWordsManager.wordsmanagement[i]._callscene == SceneManager.GetActiveScene().name
    && _CompWordsManager.wordsmanagement[i]._eventcode == CallCode)
            {
                if (!StartCheck)
                {
                    StartCheck = true;
                    StartWords = i;
                }
                EndWords = i;
            }
        }
        StartCoroutine(_ScrLabelController.DispText(false, true, StartWords, EndWords, NextScene, KeyLock));
    }
    //テキストの読み込み処理
    private void FileRead(bool AutoMode,bool hukidasi,float flagcode,string timing,string nextscene,bool KeyLock)
    {
        bool StartCheck = false;
        for (int i = 0; i < _CompWordsManager.wordsmanagement.Length; i++)
        {
            if (_CompWordsManager.wordsmanagement[i]._callscene == SceneManager.GetActiveScene().name
                && _CompWordsManager.wordsmanagement[i]._eventcode == flagcode && _CompWordsManager.wordsmanagement[i]._timing == timing)
            {
                if (!StartCheck)
                {
                    StartCheck = true;
                    StartWords = i;
                }
                EndWords = i;
            }
        }
        StartCoroutine(_ScrLabelController.DispText(AutoMode, hukidasi, StartWords, EndWords,nextscene,KeyLock));
    }
}
