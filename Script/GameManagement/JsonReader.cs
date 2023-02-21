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
    public bool _specialflag;
    public bool _finish;
    public string _callscene;
    public float _timing;
    public string _callevent;
    public float _eventcode;
}

public class JsonReader : MonoBehaviour
{
    //�V���O���g���Ŏ�����z��
    public static JsonReader Singleton;
    
    public WordsManager wordsmanager;
    public EventManager eventmanager;

    public int StartWords = 0;
    public int EndWords = 0;

    private int CallEventNum = 0;

    private string CallScene = "";
    private LabelController ScrLabelController;
    [SerializeField]
    [Tooltip("�䎌�̕������\������e�L�X�g�t�B�[���h")]
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
        //demo�V�[���̃t�@�C����ǂݍ��񂾂��`�F�b�N����֐�
        bool EOF = false;
        if (EventCode == "Demo")
        {
            //�s���Ă��Ȃ������������A�������s�킹��
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
        else if (EventCode == "Begin")//���݂̃V�[����MainGame�̏ꍇ
        {
            if (CallScene == SceneManager.GetActiveScene().name)
            {
                //�s���Ă��Ȃ������������A�������s�킹��
                while (eventmanager.eventflag[CallEventNum]._callevent == "Begin" && !EOF)
                {
                    if (!eventmanager.eventflag[CallEventNum]._finish)
                    {
                        eventmanager.eventflag[CallEventNum]._finish = true;
                        //�V�[���J�n���̏���������KeyLock��True��
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
        Debug.Log(wordsmanager.wordsmanagement[0]._words);  // ���[�h�o���Ă邩�̊m�F�p
    }

    private void EventRead()
    {
        string datapath = "EventFlag";
        string inputString = Resources.Load<TextAsset>(datapath).ToString();
        eventmanager = JsonUtility.FromJson<EventManager>(inputString);
        Debug.Log(eventmanager.eventflag[0]._callevent);  // ���[�h�o���Ă邩�̊m�F�p
    }
    private string EventManagement()
    {
        string CallEvent = "NOT FOUND";
        //Read����Event�̗v�f����A�^�C�~���O�̍��v���鏈�����������ꍇ�ɊY��Event��Call���鏈��
        for (int i = 0; i < eventmanager.eventflag.Length; i++)
        {
            //�Ώۂ̃V�[�����t�H�[�J�X
            if (eventmanager.eventflag[i]._callscene == SceneManager.GetActiveScene().name)
            {
                if (!eventmanager.eventflag[i]._finish)
                {
                    //Timing��0�Ȃ�ⓚ���p�ł��̑Ώۂ̊֐��͂���Call
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

    //�X�e�[�W�ɔz�u���Ă���Z���T�[����C�x���g�����̒ʒm�������ꍇ�̏���
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
