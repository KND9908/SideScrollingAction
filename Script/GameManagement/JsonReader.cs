using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WordsManager
{
    //Json�t�@�C���̌��o���̖��O�ƕϐ��̖��O�͓��ꂷ�邱�Ɓi�ǂ݂�����Ȃ��Ȃ邽�߁j
    public WordsManagement[] wordsmanagement;
}

[System.Serializable]
public class EventManager
{
    public EventFlag[] eventflag;
}
// �v���C���[�̃Z���t����є����C�x���g���Ǘ����Ă���CSV��ǂݍ��ރN���X
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
// �V�[�����ƂɃC�x���g�𔭐�������^�C�~���O���Ǘ����Ă���N���X
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
///�@Json����t�@�C����ǂݎ��A�l���Ǘ�����N���X
/// </summary>
public class JsonReader : MonoBehaviour
{
    //�V���O���g���Ŏ�����z��
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
    [Tooltip("�䎌�̕������\������e�L�X�g�t�B�[���h")]
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
        //demo�V�[���̃t�@�C����ǂݍ��񂾂��`�F�b�N����֐�
        bool EOF = false;
        if (EventCode == "Demo")
        {
            //�ǂݍ���ł��Ȃ����͂������A�������s�킹��
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
        else if (EventCode == "Begin")//���݂̃V�[����MainGame�̏ꍇ
        {
            if (_CallScene == SceneManager.GetActiveScene().name)
            {
                //�s���Ă��Ȃ������������A�������s�킹��
                while (_EventManager.eventflag[_CallEventNum].callevent == "Begin" && !EOF)
                {
                    if (!_EventManager.eventflag[_CallEventNum].finish)
                    {
                        _EventManager.eventflag[_CallEventNum].finish = true;
                        //�V�[���J�n���̏���������KeyLock��True��
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
        Debug.Log(_CompWordsManager.wordsmanagement[0]._words);  // ���[�h�o���Ă邩�̊m�F�p
    }

    private void _EventRead()
    {
        string datapath = "EventFlag";
        string inputString = Resources.Load<TextAsset>(datapath).ToString();
        _EventManager = JsonUtility.FromJson<EventManager>(inputString);
        Debug.Log(_EventManager.eventflag[0].callevent);  // ���[�h�o���Ă邩�̊m�F�p
    }
    private string _EventManagement()
    {
        string CallEvent = "NOT FOUND";
        //Read����Event�̗v�f����A�^�C�~���O�̍��v���鏈�����������ꍇ�ɊY��Event��Call���鏈��
        for (int i = 0; i < _EventManager.eventflag.Length; i++)
        {
            //�Ώۂ̃V�[�����t�H�[�J�X
            if (_EventManager.eventflag[i].callscene == SceneManager.GetActiveScene().name)
            {
                if (!_EventManager.eventflag[i].finish)
                {
                    //Timing��0�Ȃ�ⓚ���p�ł��̑Ώۂ̊֐��͂���Call
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

    //�x���g�����̒ʒm�������ꍇ�̏���
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
    //�e�L�X�g�̓ǂݍ��ݏ���
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
