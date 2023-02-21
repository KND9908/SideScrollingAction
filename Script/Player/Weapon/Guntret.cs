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
    private float TotalDistance;

    [Tooltip("�K���g���b�g�̔��ˑ��x")]
    [SerializeField]
    private float Speed = 3.0f;

    [SerializeField]
    public GameObject Player;

    [Tooltip("���˂����Ƃ��ɕ\�����鉌")]
    [SerializeField]
    private GameObject ParticleSmoke;

    [Tooltip("�I�u�W�F�N�g�ɐG�ꂽ���ɔ��������锚��")]
    [SerializeField]
    private GameObject ParticleExplosion;

    public enum Mode
    {
        Beat,
        RocketPunch
    }

    public Mode NowMode { get; set; } = Mode.Beat;

    private Vector3 DefaultPosition;

    public float VectorX = 0;
    public float VectorY = 0;

    private Vector2 MinusVect = new Vector2(1,1);

    private float Theta = 0;

    private float Deg = 0;

    private bool Inversion = false;
    public bool GetSetInversion { get { return Inversion; } set { Inversion = value; } }
    public Player _Player => Player.GetComponent<Player>();
    private int AttackPower => _Player.AttackPower;
    public int GetSetAttackPower => AttackPower;
    private void Start()
    {
        switch (NowMode){
            case Mode.Beat:
                break;
            case Mode.RocketPunch:
                RocketPunch();
                break;
        }
    }

    private void RocketPunch()
    {
        MinusVect.x = Inversion ? -1 : 1;
        MinusVect.y = VectorY < 0 ? -1 : 1;
        //���̓L�[�̔䗦�ɂ���Ĕ�΂��p�x�����肷��
        float xhanten = Inversion ? 180 : 0;
        if (VectorY != 0)
        {
            if (VectorX == 0)
            {
                VectorX = 1;
            }
            Theta = Mathf.Abs(VectorY) / (Mathf.Abs(VectorX) + Mathf.Abs(VectorY)) * 90;
        }
        //�I�u�W�F�N�g�̕\������p�x��ύX
        transform.rotation = Quaternion.Euler(0, xhanten, Theta * MinusVect.y - 90);

        Instantiate(ParticleSmoke, this.transform.position, Quaternion.identity);

        //�I�u�W�F�N�g�̔��˂���p�x������
        Deg = Mathf.Deg2Rad * Theta;

        DefaultPosition = transform.position;
        //����
        transform.DOMove(new Vector3(DefaultPosition.x + TotalDistance * MinusVect.x, DefaultPosition.y + TotalDistance * Mathf.Sin(Deg) * MinusVect.y, 0), Speed)
        .SetEase(Ease.InQuad).OnComplete(Remove);
    }

    private void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //�I�u�W�F�N�g�ƂԂ������ꍇ�ɔ�����\��
        Instantiate(ParticleExplosion, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
