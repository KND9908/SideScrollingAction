using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;//�I�u�W�F�N�g�̈ړ��ɂ�DoTween���g�p

public class Guntret : MonoBehaviour
{
    [SerializeField]
    public bool SpecialMove = false;

    [Tooltip("�K���g���b�g����΂���MAX�̋���")]
    [SerializeField]
    private float _TotalDistance;

    [Tooltip("�K���g���b�g�̔��ˑ��x")]
    [SerializeField]
    private float _Speed = 3.0f;

    [SerializeField]
    public GameObject Player;

    [Tooltip("���˂����Ƃ��ɕ\�����鉌")]
    [SerializeField]
    private GameObject _ParticleSmoke;

    [Tooltip("�I�u�W�F�N�g�ɐG�ꂽ���ɔ��������锚��")]
    [SerializeField]
    private GameObject _ParticleExplosion;

    public enum Mode
    {
        Beat,
        RocketPunch
    }

    public Mode NowMode { get; set; } = Mode.Beat;

    private Vector3 _DefaultPosition;

    public float VectorX = 0;
    public float VectorY = 0;

    private Vector2 _MinusVect = new Vector2(1,1);

    private float _Theta = 0;

    private float _Deg = 0;

    private bool _Inversion = false;
    public bool GetSetInversion { get { return _Inversion; } set { _Inversion = value; } }
    public Player _Player => Player.GetComponent<Player>();
    private int _AttackPower => _Player.AttackPower;
    public int GetSetAttackPower => _AttackPower;
    private void Start()
    {
        switch (NowMode){
            case Mode.Beat:
                break;
            case Mode.RocketPunch:
                _RocketPunch();
                break;
        }
    }

    private void _RocketPunch()
    {
        _MinusVect.x = _Inversion ? -1 : 1;
        _MinusVect.y = VectorY < 0 ? -1 : 1;
        //���̓L�[�̔䗦�ɂ���Ĕ�΂��p�x�����肷��
        float xhanten = _Inversion ? 180 : 0;
        if (VectorY != 0)
        {
            if (VectorX == 0)
            {
                VectorX = 1;
            }
            _Theta = Mathf.Abs(VectorY) / (Mathf.Abs(VectorX) + Mathf.Abs(VectorY)) * 90;
        }
        //�I�u�W�F�N�g�̕\������p�x��ύX
        transform.rotation = Quaternion.Euler(0, xhanten, _Theta * _MinusVect.y - 90);

        Instantiate(_ParticleSmoke, this.transform.position, Quaternion.identity);

        //�I�u�W�F�N�g�̔��˂���p�x������
        _Deg = Mathf.Deg2Rad * _Theta;

        _DefaultPosition = transform.position;
        //����
        transform.DOMove(new Vector3(_DefaultPosition.x + _TotalDistance * _MinusVect.x, _DefaultPosition.y + _TotalDistance * Mathf.Sin(_Deg) * _MinusVect.y, 0), _Speed)
        .SetEase(Ease.InQuad).OnComplete(_Remove);
    }

    private void _Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //�I�u�W�F�N�g�ƂԂ������ꍇ�ɔ�����\��
        Instantiate(_ParticleExplosion, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
