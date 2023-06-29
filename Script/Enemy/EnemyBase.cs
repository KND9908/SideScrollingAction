using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor;
public class EnemyBase : CreatureBase
{
#if UNITY_EDITOR
    //�ݒ肳�ꂽ�p�����[�^���C���X�y�N�^�[�ォ�猩���悤�ɂ���
    [CustomEditor(typeof(CreatureBase))]
#endif
    protected Rigidbody _Rigidbody => GetComponent<Rigidbody>();

    //�@�J�������ɂ��邩�ǂ���
    private bool isInsideCamera;

    [Tooltip("�v���C���[�@�������Q�Ƃ��邽�߂Ɏg�p�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject _Player;
    public GameObject GetSetPlayer { get { return _Player; } set { _Player = value; } }

    [Tooltip("�_���[�W���󂯂��Ƃ��ɕ\�����鐔���̃A�C�R��")]
    [SerializeField]
    protected GameObject _DamageText;
    public GameObject GetSetDamageText { get { return GetSetDamageText; } set { GetSetDamageText = value; } }
    protected void VisibleDamage(int num)
    {
        GameObject damagetext;
        damagetext = Instantiate(_DamageText);
        GameObject tx = damagetext.transform.GetChild(0).gameObject;
        TextMeshProUGUI txm = tx.GetComponent<TextMeshProUGUI>();
        txm.text = num.ToString();
        txm.color += new Color(0, 0, 0, 0);
        damagetext.transform.position = new Vector3(this.transform.position.x, transform.position.y, 0);
    }

    //isinsidecamera�̃Q�b�^�[
    public bool GetIsInsideCamera()
    {
        return isInsideCamera;
    }

    //�J��������O�ꂽ
    private void OnBecameInvisible()
    {
        isInsideCamera = false;
    }
    //�J�������ɓ�����
    private void OnBecameVisible()
    {
        isInsideCamera = true;
    }

}

