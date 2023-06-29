using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
/// <summary>
/// �䎌�̕\�����Ǘ�����N���X
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

    private int _TextNum;//�ǂݍ��񂾃e�L�X�g�̔ԍ�
    private string _DisplayText = "";
    private int _TextCharNum;//�e�L�X�g�̕����̔ԍ�

    //�����̕`�摬�x�����p�ϐ�
    private int _CntCharDrawSpeed = 0;
    [SerializeField]
    private int _DispCharSpeed = 1;

    private bool _isClick = false;//�N���b�N�̔���

    private float _FFTime = 0;//�����Ńe�L�X�g��\�����鎞��

    public bool isAction = false;

    public bool InterruptEvent = false;//�����C�x���g���ۂ�

    public bool ViewVisible = false;

    public bool CoroutineAct = false;

    public bool isEventFin = false;//�C�x���g���I���������ۂ��@��X�v���C�x�[�g�ɕύX����

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
    //���x���ɑ䎌��\�����鏈��
    public IEnumerator DispText(bool Auto, bool speechbubble, int textstart, int textfin, string nextscene, bool KeyLock)
    {
        isAction = true;
        bool textstop = false;//���[�h�̂����肪���������ꍇ
        if ("Demo" != SceneManager.GetActiveScene().name)
        {
            _CompGamemanager.KeyLock = KeyLock;
        }
        _TextNum = textstart;
        int nowicon = 0;

        while (!textstop)
        {
            //�����C�x���g�����������ꍇ�͑S�Ă̓�����~�߁A�C�x���g�p�̃e�L�X�g��\������
            if (InterruptEvent)
            {
                _DisplayText = "";
                _TextCharNum = 0;
                _TextNum = 0;
                textstop = true;
                _FFTime = 0;
                InterruptEvent = false;
                //�����o�������邤���ł̓ǂݍ��ݏ��������Ă����ꍇ�A�����o�����B��
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
                    //�C�x���g�R�[�h��ǂݍ��񂾏ꍇ
                    if (_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._specialflag != "")
                    {
                        if (!CoroutineAct)
                        {
                            CoroutineAct = true;
                            //�Ή��C�x���g�����s
                            _CompGamemanager.EventAct(_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._specialflag);
                        }
                        else
                        {
                            if (isEventFin)
                            {
                                //�C�x���g�I������玟�̍s�ǂݍ���
                                if (_TextNum < textfin)//�ǂݍ��ލs���ŏI�s�łȂ��ꍇ
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
                    //�������I�[�܂ōs���ĂȂ��ꍇ
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
                        //�A�C�R���̕\���ʒu�ύX
                        SetIconPosition(int.Parse(_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._iconpos));

                        nowicon = ChangeIconContent();


                        _CntCharDrawSpeed++;
                        if (_CntCharDrawSpeed % _DispCharSpeed == 0)//�����̕`�摬�x�̒���
                        {
                            //�\�����镶�����1���������ǂݍ��񂾃e�L�X�g������ǉ�����
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
                        {//�ǉ����镶�����N���A�ɂ����͂̔ԍ����ŏ��ɖ߂�
                            _DisplayText = "";
                            _TextCharNum = 0;
                            _TextNum++;
                            _FFTime = 0;
                        }
                    }
                    else
                    {
                        _FFTime += Time.deltaTime;
                        //�S���ǂ񂾂����ŃN���b�N���ꂽ���������������
                        if (_isClick)
                        {
                            _DisplayText = "";
                            _TextCharNum = 0;
                            _TextNum = 0;
                            textstop = true;
                            _FFTime = 0;
                            //�����o�������邤���ł̓ǂݍ��ݏ��������Ă��邩�ŏ�����?�s�����ۂ����킩���
                            if (speechbubble)
                            {
                                StartCoroutine(RemoveView());
                                _CompGamemanager.EventNow = false;
                            }
                        }
                    }
                }
                //UI�ɕ\��
                _ObjTxt.text = _DisplayText;
                _isClick = false;
                //�}�E�X�������ꂽ�玟�̕��͂�ǂݍ���
                if (!Auto && Input.GetButtonDown("Hint") || (Auto && _FFTime > 2.0f))//�܂���AutoMode��True�ł��^�C���f���^�^�C�����Q�b�o�߂����Ƃ�(���݉��ŃW�����v�Ɠ����{�^���ɂ��Ă���)
                {
                    _isClick = true;
                }



                yield return null;
            }
        }
        //Eventmanager��callscene������scene�ƍ��v���Ă��鎞�A���AEventManager�̃^�C�~���O������̒l�̎��AEventManager�ɕ`���Ă��鏈�����s��
        if (Auto || nextscene != "")
            //���̃V�[���փt�F�[�h�A�E�g���鏈��
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
        if (_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._character == "�L�b�g")
        {
            if (nowicon != 1)
            {
                nowicon = 1;
                _IconNowImage.sprite = Sprite.Create(_PlayerImage, new Rect(0, 0, _PlayerImage.width, _PlayerImage.height), Vector2.zero);
            }
        }
        else if (_CompJsonReader.GetWordsManager.wordsmanagement[_TextNum]._character == "���[��")
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
    //�����o�������
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
