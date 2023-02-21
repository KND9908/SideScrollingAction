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

    private int TextNum;//�ǂݍ��񂾃e�L�X�g�̔ԍ�
    private string DisplayText = "";
    private int TextCharNum;//�e�L�X�g�̕����̔ԍ�
    
    //�����̕`�摬�x�����p�ϐ�
    private int CntCharDrawSpeed = 0;
    [SerializeField]
    private int dispcharspeed = 1;

    private bool isClick = false;//�N���b�N�̔���

    private float FFTime = 0;

    public bool ActionNow = false;

    public bool InterruptEvent = false;//�����C�x���g���ۂ�

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
        bool textstop = false;//���[�h�̂����肪���������ꍇ
        if ("Demo" != SceneManager.GetActiveScene().name)
        {
            ScrGamemanager.KeyLock = KeyLock;
        }
        TextNum = textstart;
        int nowicon = 0;

        while (!textstop)
        {
            //�����C�x���g�����������ꍇ�͑S�Ă̓�����~�߁A�C�x���g�p�̃e�L�X�g��\������
            if (InterruptEvent)
            {
                DisplayText = "";
                TextCharNum = 0;
                TextNum = 0;
                textstop = true;
                FFTime = 0;
                InterruptEvent = false;
                //�����o�������邤���ł̓ǂݍ��ݏ��������Ă����ꍇ�A�����o�����B��
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
                    //�C�x���g�R�[�h��ǂݍ��񂾏ꍇ
                    if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._specialflag != "")
                    {
                        if (!CoroutineAct)
                        {
                            CoroutineAct = true;
                            //�Ƃ肠�������̓��e�ŏ�����ς��Ă݂�̂𒼖󂵂��֐��ŏ���
                            ScrGamemanager.EventAct(ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._specialflag);
                        }
                        else
                        {
                            if (isEventFin)
                            {
                                //�C�x���g�I������玟�̍s�ǂݍ���
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
                    //�������I�[�܂ōs���ĂȂ��ꍇ
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
                        //�A�C�R���̕\���ʒu�ύX
                        if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._iconpos == "1")
                        {
                            TxtWords_Icon.transform.localPosition = new Vector3(-267,130,0);
                        }
                        else if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._iconpos == "2")
                        {
                            TxtWords_Icon.transform.localPosition = new Vector3(282, 130, 0);
                        }
                        //�A�C�R���̓��e�ύX
                        if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._character == "�L�b�g")
                        {
                            if (nowicon != 1)
                            {
                                nowicon = 1;
                                IconNowImage.sprite = Sprite.Create(PlayerImage, new Rect(0, 0, PlayerImage.width, PlayerImage.height), Vector2.zero);
                            }
                        }
                        else if (ScrJsonReader.wordsmanager.wordsmanagement[TextNum]._character == "���[��")
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
                        if (CntCharDrawSpeed % dispcharspeed == 0)//�����̕`�摬�x�̒���
                        {
                            //�\�����镶�����1���������ǂݍ��񂾃e�L�X�g������ǉ�����
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
                        {//�ǉ����镶�����N���A�ɂ����͂̔ԍ����ŏ��ɖ߂�
                            DisplayText = "";
                            TextCharNum = 0;
                            TextNum++;
                            FFTime = 0;
                        }
                    }
                    else
                    {
                        FFTime += Time.deltaTime;
                        //�S���ǂ񂾂����ŃN���b�N���ꂽ���������������
                        if (isClick)
                        {
                            DisplayText = "";
                            TextCharNum = 0;
                            TextNum = 0;
                            textstop = true;
                            FFTime = 0;
                            //�����o�������邤���ł̓ǂݍ��ݏ��������Ă��邩�ŏ�����?�s�����ۂ����킩���
                            if (speechbubble)
                            {
                                StartCoroutine(RemoveView());
                                ScrGamemanager.EventNow = false;
                            }
                        }
                    }
                }
                //UI�ɕ\��
                TxtWords_Label.text = DisplayText;
                isClick = false;
                //�}�E�X�������ꂽ�玟�̕��͂�ǂݍ���
                if (!Auto && Input.GetKeyDown(KeyCode.Return) || (Auto && FFTime > 2.0f))//�܂���AutoMode��True�ł��^�C���f���^�^�C�����Q�b�o�߂����Ƃ�
                {
                    isClick = true;
                }
                yield return null;
            }
        }
        //Eventmanager��callscene������scene�ƍ��v���Ă��鎞�A���AEventManager�̃^�C�~���O������̒l�̎��AEventManager�ɕ`���Ă��鏈�����s��
        if (Auto || nextscene != "")
            //���̃V�[���փt�F�[�h�A�E�g���鏈��
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
    //�����o�������
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
