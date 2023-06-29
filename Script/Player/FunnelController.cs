using UnityEngine;
using DG.Tweening;
using Mono.CompilerServices.SymbolWriter;

public class FunnelController : MonoBehaviour
{
    public Transform player; // �v���C���[��Transform���Q�Ƃ��邽�߂̕ϐ�
    public float floatingSpeed = 1f; // �t�@���l���̕��V���x
    public float rotationSpeed = 1f; // �t�@���l���̉�]���x

    private Vector3 originalPosition; // �����ʒu

    private Tweener tweener; //�I�u�W�F�N�g�̈ړ����s���Ă��邩�ۂ�
    private Tweener floatingTweener;

    public float floatingHeight = 0.5f;
    public float floatingDuration = 2f;

    //�G�𔭌����Ă��邩�ۂ�
    private bool isFindEnemy = false;

    //�G��Transform
    private GameObject enemy;
    private Enemy scrEnemy;

    //���[�U�[�r�[���̃I�u�W�F�N�g
    public GameObject laserBeam;

    //�v���C���[�Ƃ̃I�t�Z�b�g�ʒu
    public Vector3 offset = new Vector3(5, 0, 0);
    public float xoffset = 5;
    private void Start()
    {
        originalPosition = transform.position; // �����ʒu��ۑ�
                                               // �㉺�ɂӂ�ӂ�ƈړ�����Tween�̐ݒ�
        floatingTweener = transform.DOMoveY(transform.position.y + floatingHeight, floatingDuration)
            .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).SetAutoKill(false).Pause();

    }

    private void Update()
    {
        // �t�@���l���𕂗V������
        float newY = originalPosition.y + Mathf.Sin(Time.time * floatingSpeed);

        // �t�@���l���̈ʒu��Y���͏�Ƀv���C���[�ƈ��̈ʒu�ɂ�����̂Ƃ���
        transform.position += new Vector3(0, newY, 0);

        if (isFindEnemy)
        {
            //enemy.transform�����ł��Ă���ꍇ
            if (enemy == null)
            {
                isFindEnemy = false;
                return;
            }
            //�G�̕���1�b�����Ĉړ�����
            //transform.DOMove(enemy.transform.position, 1f);
            //�G�̎��͂̃����_���Ȉʒu�Ɉړ�����
            if (tweener == null || !tweener.IsActive())
            {
                tweener = transform.DOMove(enemy.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), 1f);
                //���[�U�[����
                FireLaser();
            }
        }
        else
        {
            //�v���C���[���W�̏�������ǂ�������悤�Ɉړ�����@��{transform�ړ�
            //transform.position = player.position;
            //�ړ����Ȃ���1�b�����ăv���C���[�̌��֏���������
            if (tweener == null || !tweener.IsActive() && player.transform.position.x - player.transform.position.x + offset.x <= 0)
            {
                tweener = transform.DOMove(player.position + offset, 0.5f);
            }
            // �㉺�ɂӂ�ӂ�ƈړ�����Tween���Đ����łȂ���΍Đ�����
            if (!floatingTweener.IsActive())
            {
                floatingTweener.Play();
            }
        }
    }

    //�t�@���l���̃X�t�B�A�R���C�_�[����enemy�I�u�W�F�N�g���������ꍇ�̏���
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            //�G����ʓ��ɂ���ꍇ
            if (other.gameObject.GetComponent<Enemy>().GetIsInsideCamera() && !isFindEnemy)
            {
                //�G���擾
                enemy = other.gameObject;
                scrEnemy = enemy.GetComponent<Enemy>();
                scrEnemy.OnDead += OnDead;
                isFindEnemy = true;
            }
        }
    }

    //�G���|���ꂽ���̌��m����
    private void OnDead()
    {
        isFindEnemy = false;
    }

    //�t�@���l���̃X�t�B�A�R���C�_�[����enemy�I�u�W�F�N�g�����Ȃ��Ȃ����ꍇ�̏���
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            //�v���C���[�̕���1�b�����Ĉړ�����
            transform.DOMove(player.position, 1f);
            isFindEnemy = false;
        }
    }

    //�t�@���l�����烌�[�U�[���o��
    public void FireLaser()
    {
        //���[�U�[���o��
        GameObject laser = Instantiate(laserBeam, transform.position, Quaternion.identity);
        //���[�U�[�̌�����G�̕����Ɍ�����
        laser.transform.LookAt(enemy.transform);
    }
}
