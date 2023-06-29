using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
public class ActionGround : MonoBehaviour
{
    //���̋������e���Ǘ�����enum
    public enum GroundType
    {
        //�㉺�ɓ���
        UpDown,
        //���E�ɓ���
        LeftRight,
        //��]����
        Rotation,
        //�g��k������
        Scale,
        //����������ʒu�܂ňړ�����
        Move,
        //���J�E���g�Ŕ�������
        Explosion,
        //���J�E���g�ŏ�����
        Vanish,
    }

    //���̏��̋������e
    public GroundType MoveType;

    [SerializeField]
    [Tooltip("���̑��x")]
    private float _MoveSpeed;

    public float Amount_of_movement = 2.0f;

    private int _Movevec = 1;

    [Tooltip("�����ʒu")]
    private Vector3 _DefTransform;

    //�ړ��O��̃|�W�V����
    private Vector3 _BeforePosition;

    private Vector3 _AfterPosition;

    Transform _PlayerTransform;
    //�v���C���[������Ă��邩�̔���
    private bool _PlayerOn = false;

    //���삳������e

    void Start()
    {
        _DefTransform = transform.position;
    }

    private void FixedUpdate()
    {
        //�����̃A�N�V������ݒ�  
        SetAction(MoveType);
    }

    //�����̃A�N�V������ݒ�
    public void SetAction(GroundType type)
    {
        switch (type)
        {
            case GroundType.UpDown:
                //�㉺�ɓ���
                //�ړ��O�̃|�W�V�������擾
                _BeforePosition = transform.position;
                //���͈͂��㉺�ړ�������
                transform.position = new Vector3(_DefTransform.x, _DefTransform.y + Mathf.PingPong(Time.time * _MoveSpeed, Amount_of_movement), _DefTransform.z);
                //�ړ���̃|�W�V�������擾
                _AfterPosition = transform.position;
                //������m�[�}���C�Y
                Vector3 _TotalDistance = _AfterPosition - _BeforePosition;
                //�v���C���[������Ă���ꍇ
                if (_PlayerOn)
                {
                    //�v���C���[�̃|�W�V�������ړ��������ړ�
                    _PlayerTransform.position += _TotalDistance;

                }
                break;
            case GroundType.LeftRight:
                //���E�ɓ���
                //�ړ��O�̃|�W�V�������擾
                _BeforePosition = transform.position;
                //���͈͂����E�ړ�������
                transform.position = new Vector3(_DefTransform.x + Mathf.PingPong(Time.time * _MoveSpeed, Amount_of_movement), _DefTransform.y, _DefTransform.z);
                //�ړ���̃|�W�V�������擾
                _AfterPosition = transform.position;
                //������m�[�}���C�Y
                _TotalDistance = _AfterPosition - _BeforePosition;
                //�v���C���[������Ă���ꍇ
                if (_PlayerOn)
                {
                    //�v���C���[�̃|�W�V�������ړ��������ړ�
                    _PlayerTransform.position += _TotalDistance;
                }
                break;
            case GroundType.Rotation:
                //��]����
                break;
            case GroundType.Scale:
                //�g��k������
                break;
            case GroundType.Move:
                //����������ʒu�܂ňړ�����
                break;
            case GroundType.Explosion:
                break;
            case GroundType.Vanish:
                //���J�E���g�ŏ�����
                break;
        }
    }

    //�������鏰�̏���
    private IEnumerator Explosion()
    {
        //�J�E���g
        int _Count = 3;
        //���J�E���g�Ŕ�������
        while (_Count != 0)
        {
            //��b�҂�
            yield return new WaitForSeconds(1.0f);
            _Count--;
        }

        //��������
        Destroy(gameObject);

    }

    //�v���C���[���I�u�W�F�N�g�̏�ɏ�����ꍇ�̏���
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _PlayerOn = true;
            //�������鏰�̏ꍇ�A�������鏰�̏������s��
            if (MoveType == GroundType.Explosion)
            {
                StartCoroutine(Explosion());
            }
            //�v���C���[�̃g�����X�t�H�[�����擾
            _PlayerTransform = collision.gameObject.transform;
        }
    }

    //�v���C���[���I�u�W�F�N�g���痣�ꂽ�ꍇ�̏���
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _PlayerOn = false;
            _PlayerTransform = null;
        }
    }
}
