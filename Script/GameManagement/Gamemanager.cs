using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    [Tooltip("���삷��v���C���[")]
    [SerializeField]
    private GameObject ObjPlayer;

    [Tooltip("�t�F�[�h�A�E�g�̎��̍�����")]
    [SerializeField]
    private GameObject FadeOut;

    [Tooltip("�N���A�������ɕ\�����郍�S")]
    [SerializeField]
    private GameObject Clear;

    [Tooltip("�v���C�J�n���ɕ\�����郍�S�@Ready")]
    [SerializeField]
    private GameObject Ready;

    [Tooltip("�{�X��J�n�O�ɕ\�����郍�S")]
    [SerializeField]
    private GameObject warning;

    [Tooltip("�Q�[�����Ńv���C���[��Ǐ]����J����")]
    [SerializeField]
    private GameObject CinemaChine;

    [Tooltip("�V�l�}�V�[���ŒǏ]����X�e�[�W���͈̔�")]
    [SerializeField]
    private GameObject StageCamera;

    [Tooltip("�{�X��̃G���A�����f���X�e�[�W���͈̔�")]
    [SerializeField]
    private GameObject BossCamera;

    [Tooltip("���y���Đ�����Audio")]
    [SerializeField]
    private GameObject AudioBGM;

    [Tooltip("Json����t�@�C����ǂݏ�������I�u�W�F�N�gJsonReader�������ɐݒ�")]
    [SerializeField]
    private GameObject JsonReader;

    [Tooltip("�{�X�I�u�W�F�N�g��GameObject�������ɐݒ�")]
    [SerializeField]
    private GameObject ObjBoss;

    [Tooltip("�q���g��\������A�C�R��")]
    [SerializeField]
    private GameObject CallIcon;

    [Tooltip("�䎌���Ǘ�����R���g���[���[")]
    [SerializeField]
    private GameObject Labelcontroller;

    public bool KeyLock = true;

    [Tooltip("�ʏ펞BGM")]
    [SerializeField]
    private AudioClip NormalSong;

    [Tooltip("�{�X�펞BGM")]
    [SerializeField]
    private AudioClip BossSong;

    [Tooltip("�X�e�[�W�N���A�̎��̃W���O��")]
    [SerializeField]
    private AudioClip StageClearzingle;
    
    public bool EventNow = true;
    
    //�C�j�V�����C�Y����
    private bool init = false;

    public int StackWords = 0;

    //�J�����̃f�t�H���g�ʒu
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
        //�v���C���[�̎��S�t���O����������Q�[���Ƀ��b�N��������
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
                //�q���g��\���ł���ʒm������������A�A�C�R����\��
                if (!NoticeImage.enabled)
                {
                    NoticeImage.enabled = true;
                }
                //F�{�^�����������邱�ƂŃq���g��\��
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CallHint();
                }
            }
            //�\�����镶���̃X�^�b�N���Ȃ��Ȃ����ꍇ�A�C�R�����\��
            else if (NoticeImage.enabled)
            {
                NoticeImage.enabled = false;
            }

            //�C�x���g���������A�C�R�����\��
            if (KeyLock)
            {
                if (NoticeImage.enabled)
                {
                    NoticeImage.enabled = false;
                }
            }

            //�v���C���[�̕����ɍ��킹�ăJ�������ړ�������
            if (ScrPlayer.GetSetVisualInversion)
            {
                CFT.m_TrackedObjectOffset = new Vector3(Cam_DefaultOffset.x * -1, Cam_DefaultOffset.y, Cam_DefaultOffset.z);
            }
            else
            {
                CFT.m_TrackedObjectOffset = Cam_DefaultOffset;
            }

            //�v���C���[�����񂾂Ƃ��ɃQ�[���I�[�o�[��ʂ֑J�ڂ���
            if (ScrPlayer.GetSetDeathFlag)
            {
                GameOverCall();
            }

            //Clearflag����������X�e�[�W�N���A��������������
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
            //�����ɂ��܂�䎌�͎��R�ɕ\���ł�����̂ɂȂ邽�߁A�����C�x���g�̂��̂͂Ȃ��̂ő�3������false�A��ʑJ�ڂ��Ȃ��̂ő�2������false
            _JsonReader.CensorCallevent(StackWords, "", false);
            StackWords = 0;
        }
    }
    private IEnumerator GameStart()
    {
        //Ready?��\�����ăQ�[���J�n
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
    //�Q�[���I�[�o�[��ʑJ�ڗp
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
        //StageClear��\��
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
    //�X�e�[�W�̉ӏ��ɂ���đ䎌�𒝂�邩�ۂ���CSV�ɂĊm�F����
    //CSV����ǂݍ���ł��f�[�^����������ۂɊ֐���Call����L�q���`����Ă����ꍇ�ɂ͂����ɂď���������
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
        //WARNING�I�ȓz��\�����鏈��
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
        //���X�ɉ��ʂ�����������
        while (_AudioSource.volume != 0)
        {
            _AudioSource.volume = 0;
            yield return null;
        }
        _AudioSource.Stop();
        _AudioSource.volume = AudioVolume;
    }
    /// <summary>
    /// �������y�̕ύX
    /// </summary>
    /// <param name="clip">�������y��clip</param>
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
