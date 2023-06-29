using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Gamemanager : MonoBehaviour
{
    [Tooltip("���삷��v���C���[")]
    [SerializeField]
    private GameObject _ObjPlayer;

    [Tooltip("�t�F�[�h�A�E�g�̍���")]
    [SerializeField]
    private GameObject _FadeOut;

    [Tooltip("�N���A�������ɕ\�����郍�S")]
    [SerializeField]
    private GameObject _Clear;

    [Tooltip("�v���C�J�n���ɕ\�����郍�S")]
    [SerializeField]
    private GameObject _Ready;

    [Tooltip("�{�X��J�n�O�ɕ\�����郍�S")]
    [SerializeField]
    private GameObject _Warning;

    [Tooltip("�Q�[�����Ńv���C���[��Ǐ]����J����")]
    [SerializeField]
    private GameObject _ObjCinemaChine;

    [Tooltip("�V�l�}�V�[���ŒǏ]����X�e�[�W���͈̔�")]
    [SerializeField]
    private GameObject _ObjStageCamera;

    [Tooltip("�{�X��̃G���A�����f���X�e�[�W���͈̔�")]
    [SerializeField]
    private GameObject _ObjBossCamera;

    [Tooltip("���y���Đ�����Audio")]
    [SerializeField]
    private GameObject _ObjAudioBGM;

    [Tooltip("Json����t�@�C����ǂݏ�������I�u�W�F�N�gJsonReader�������ɐݒ�")]
    [SerializeField]
    private GameObject _ObjJsonReader;

    [Tooltip("�{�X�I�u�W�F�N�g��GameObject�������ɐݒ�")]
    [SerializeField]
    private GameObject _ObjBoss;

    [Tooltip("�q���g��\������A�C�R��")]
    [SerializeField]
    private GameObject _ObjCallIcon;

    [Tooltip("�䎌���Ǘ�����R���g���[���[")]
    [SerializeField]
    private GameObject _ObjLabelController;

    public bool KeyLock = true;

    [Tooltip("�ʏ펞BGM")]
    [SerializeField]
    private AudioClip _ObjNormalSong;

    [Tooltip("�{�X�펞BGM")]
    [SerializeField]
    private AudioClip _ObjBossSong;

    [Tooltip("�X�e�[�W�N���A�̎��̃W���O��")]
    [SerializeField]
    private AudioClip _ObjStageClearzingle;

    public bool EventNow = true;

    //�C�j�V�����C�Y����
    private bool _init = false;

    public int StackWords = 0;

    public bool BossDeadFlag = false;
        
    //�J�����̃f�t�H���g�ʒu
    private Vector3 _Cam_DefaultOffset;

    private Image _NoticeImage;

    private FadeOut _CompFadeOut;
    private CinemachineConfiner2D _CompCinemachineConfiner2D;
    private PolygonCollider2D _CompBossPolygonCollider2D;
    private CinemachineVirtualCamera _CinemachineVirtualCamera;
    private CinemachineFramingTransposer _CompCinemachineFramingTransposer;
    private JsonReader _CompJsonReader;
    private LabelController _CompLabelController;
    private AudioSource _CompAudioSource;
    private Player _CompPlayer;
    private Boss _CompBoss;

    public bool Clearflag = false;
    private void Start()
    {
        _NoticeImage = _ObjCallIcon.GetComponent<Image>();

        _CinemachineVirtualCamera = _ObjCinemaChine.GetComponent<CinemachineVirtualCamera>();
        _CompCinemachineFramingTransposer = _CinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _Cam_DefaultOffset = _CompCinemachineFramingTransposer.m_TrackedObjectOffset;
        _CompFadeOut = _FadeOut.GetComponent<FadeOut>();
        _CompCinemachineConfiner2D = _ObjCinemaChine.GetComponent<CinemachineConfiner2D>();
        _CompBossPolygonCollider2D = _ObjBossCamera.GetComponent<PolygonCollider2D>();
        _CompJsonReader = _ObjJsonReader.GetComponent<JsonReader>();
        _CompLabelController = _ObjLabelController.GetComponent<LabelController>();
        _CompAudioSource = _ObjAudioBGM.GetComponent<AudioSource>();
        _CompPlayer = _ObjPlayer.GetComponent<Player>();
        _CompBoss = _ObjBoss.GetComponent<Boss>();
    }

    // Update is called once per frame
    private void Update()
    {
        //�v���C���[�̎��S�t���O����������Q�[���Ƀ��b�N��������
        if (_CompPlayer.GetSetDeathFlag)
            KeyLock = true;

        if ("Demo" != SceneManager.GetActiveScene().name)
        {   //�f���V�[���ȊO�ł̏���
            //�Q�[���J�n���̏���
            if (!_init && !EventNow && !KeyLock)
            {
                _init = true;

                _CompAudioSource.Play();
                StartCoroutine(_GameStart());
            }
            //���t�̃X�^�b�N������ꍇ�A�q���g��\������
            if (StackWords != 0)
            {
                //�q���g��\���ł���ʒm������������A�A�C�R����\��
                if (!_NoticeImage.enabled)
                {
                    _NoticeImage.enabled = true;
                }
                //F�{�^�����������邱�ƂŃq���g��\��
                if (Input.GetButtonDown("Hint"))
                {
                    _CallHint();
                }
            }
            //�\�����镶���̃X�^�b�N���Ȃ��Ȃ����ꍇ�A�C�R�����\��
            else if (_NoticeImage.enabled)
            {
                _NoticeImage.enabled = false;
            }

            //�C�x���g���������A�C�R�����\��
            if (KeyLock)
            {
                if (_NoticeImage.enabled)
                {
                    _NoticeImage.enabled = false;
                }
            }

            //�v���C���[�̕����ɍ��킹�ăJ�������ړ�������
            if (_CompPlayer.GetSetVisualInversion)
            {
                _CompCinemachineFramingTransposer.m_TrackedObjectOffset = new Vector3(_Cam_DefaultOffset.x * -1, _Cam_DefaultOffset.y, _Cam_DefaultOffset.z);
            }
            else
            {
                _CompCinemachineFramingTransposer.m_TrackedObjectOffset = _Cam_DefaultOffset;
            }

            //�v���C���[�����񂾂Ƃ��ɃQ�[���I�[�o�[��ʂ֑J�ڂ���
            if (_CompPlayer.GetSetDeathFlag)
            {
                _GameOverCall();
            }

            //Clearflag����������X�e�[�W�N���A��������������
            if (Clearflag)
            {
                Clearflag = false;
                StartCoroutine(_StageClear());
            }

            //�{�X�̃��C�t��0�ɂȂ�����C�x���g���R�[���@��قǃR�[���o�b�N�̌`���ɂ���
            if (_CompBoss.GetSetMaxHP <= 0 && !BossDeadFlag)
            {
                _BossDeathCall();
            }
        }
    }
    //�{�X�����ꂽ���̏���
    private void _BossDeathCall()
    {
        BossDeadFlag = true;
        _CompJsonReader.Callevent(21, "", true);
    }


    private void _CallHint()
    {
        if (!_CompLabelController.isAction)
        {
            //�����ɂ��܂�䎌�͎��R�ɕ\���ł�����̂ɂȂ邽�߁A�����C�x���g�̂��̂͂Ȃ��̂ő�3������false�A��ʑJ�ڂ��Ȃ��̂ő�2������false
            _CompJsonReader.Callevent(StackWords, "", false);
            StackWords = 0;
        }
    }
    private IEnumerator _GameStart()
    {
        //Ready?��\�����ăQ�[���J�n
        KeyLock = true;
        float NowTime = 0;

        _Ready.SetActive(true);

        const float WAITTIME = 2.0f;
        while (NowTime < WAITTIME)//WaitTime
        {
            NowTime += Time.deltaTime;
            yield return null;
        }
        _Ready.SetActive(false);
        KeyLock = false;

        yield break;
    }
    //�Q�[���I�[�o�[��ʑJ�ڗp
    private void _GameOverCall()
    {
        _CompFadeOut.FadeOutCall("Scene/GameOver");
    }
    //�Ó]����
    private void _NormalFadeOut()
    {
        _CompFadeOut.FadeOutCall("");
    }
    //�X�e�[�W�N���A����
    private IEnumerator _StageClear()
    {
        // StageClear��\�����邽�߂̃A�j���[�V�����J�n
        StartCoroutine(_CompLabelController.RemoveView());

        // StageClear�p�̃W���O�����Đ�����
        _CompAudioSource.clip = _ObjStageClearzingle;
        _CompAudioSource.PlayOneShot(_CompAudioSource.clip);

        // ����L�[�̃��b�N��L��������
        KeyLock = true;
        float NowTime = 0;

        // StageClear�\�����A�N�e�B�u������
        _Clear.SetActive(true);

        // StageClear���̃W���O���̍Đ����͏�����ҋ@����
        while (_CompAudioSource.isPlaying)
        {
            NowTime += Time.deltaTime;
            yield return null;
        }

        // StageClear�\�����A�N�e�B�u������
        _Clear.SetActive(false);

        // ����L�[�̃��b�N�𖳌�������
        KeyLock = false;

        //��ʂ��Ó]������
        _CompFadeOut.FadeOutCall("Title");

        // LabelController�̃C�x���g�I���t���O��L��������
        _CompLabelController.isEventFin = true;

        // �R���[�`���̏I��
        yield break;
    }
    //CSV�ɋL�q����Ă���C�x���g�R�[�h�����s���鏈��
    public void EventAct(string str)
    {
        if (str == "bossbattlestart")
        {
            StartCoroutine(_BossBattleStart());
        }
        if (str == "anten")
        {
            StartCoroutine(_BlackOut());
        }
        if (str == "StageClear")
        {
            StartCoroutine(_StageClear());
        }
        if (str == "bgmstop")
        {
            StartCoroutine(_BgmStop());
        }
        if (str == "BossAppear")
        {
            //�{�X�̃I�u�W�F�N�g���A�N�e�B�u������
            _ObjBoss.SetActive(true);
            _CompLabelController.isEventFin = true;
        }
    }

    /// <summary>
    /// �{�X��J�n���Ɏ��s�����R���[�`��
    /// </summary>
    private IEnumerator _BossBattleStart()
    {
        // ���t�̃��x�����\���ɂ���
        StartCoroutine(_CompLabelController.RemoveView());

        // �{�X��p�̃I�[�f�B�I��ݒ肷��
        _AudioChg(_ObjBossSong);

        // �{�X��p�I�[�f�B�I���Đ�����
        _CompAudioSource.Play();

        // ���[�j���O���b�Z�[�W�̕\�����J�n����
        _Warning.SetActive(true);

        // 2�b�ԑҋ@����
        float time = 0;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // ���[�j���O���b�Z�[�W�̔�\�������s��
        _Warning.SetActive(false);

        // LabelController�̃C�x���g�I���t���O��L��������
        _CompLabelController.isEventFin = true;

        // �R���[�`���̏I��
        yield break;
    }

    private IEnumerator _BgmStopAndBossFlag()
    {
        //BGM���~�߂�
        StartCoroutine(_BgmStop());
        //�J�����̕`�ʔ͈͂��{�X�펞�̂��̂ɕύX����
        _CompCinemachineConfiner2D.m_BoundingShape2D = _CompBossPolygonCollider2D;
        //�{�X�̃I�u�W�F�N�g���A�N�e�B�u������
        _ObjBoss.SetActive(true);
        //�C�x���g�̏I������t���O
        _CompLabelController.isEventFin = true;
        yield break;
    }

    private IEnumerator _BgmStop()
    {
        float AudioVolume = _CompAudioSource.volume;
        //���X�ɉ��ʂ�����������
        while (_CompAudioSource.volume != 0)
        {
            _CompAudioSource.volume = 0;
            yield return null;
        }
        _CompAudioSource.Stop();
        _CompAudioSource.volume = AudioVolume;

        // LabelController�̃C�x���g�I���t���O��L��������
        _CompLabelController.isEventFin = true;
    }
    /// <summary>
    /// �������y�̕ύX
    /// </summary>
    /// <param name="clip">�������y��clip</param>
    private void _AudioChg(AudioClip clip)
    {
        _CompAudioSource.clip = clip;
    }

    private IEnumerator _BlackOut()
    {
        _NormalFadeOut();
        _CompLabelController.isEventFin = true;
        yield break;
    }

    public float maxSpecialAttackGauge = 100f;  // �K�E�Z�Q�[�W�̍ő�l
    public float specialAttackIncrement = 10f;  // �G��|�������ɑ�������K�E�Z�Q�[�W�̗�

    private float _currentSpecialAttackGauge = 0f;  // ���݂̕K�E�Z�Q�[�W�̗�

    //���݂̕K�E�Z�Q�[�W�̗ʂ̃Q�b�^�[�Z�b�^�[
    public float CurrentSpecialAttackGuide { get => _currentSpecialAttackGauge; set => _currentSpecialAttackGauge = value; }

    /// <summary>
    /// �G��|�������ɌĂяo�����֐� 
    /// </summary>
    public void EnemyDefeated()
    {
        _currentSpecialAttackGauge += specialAttackIncrement;  // �K�E�Z�Q�[�W�𑝉�������

        if (_currentSpecialAttackGauge > maxSpecialAttackGauge)
        {
            _currentSpecialAttackGauge = maxSpecialAttackGauge;  // �K�E�Z�Q�[�W���ő�l�𒴂����ꍇ�͍ő�l�ɐݒ肷��
        }
    }

    /// <summary>
    /// �K�E�Z�Q�[�W�����^���ɂȂ����Ƃ��ɍs������  
    /// <summary>
    public void SpecialAttack()
    {
        _currentSpecialAttackGauge = 0f;  // �K�E�Z�Q�[�W��0�ɂ���
        //�v���C���[�̕K�E�Z�������s��
    }
}
