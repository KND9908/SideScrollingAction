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

    [Tooltip("�v���C���[�@�������Q�Ƃ��邽�߂Ɏg�p�@�C���X�y�N�^�[����̑���͊�{�f�o�b�O�ł����g��Ȃ��z��")]
    [SerializeField]
    protected GameObject Player;
    public GameObject GetSetPlayer { get { return Player; } set { Player = value; } }

    [Tooltip("�_���[�W���󂯂��Ƃ��ɕ\�����鐔���̃A�C�R��")]
    [SerializeField]
    protected GameObject DamageText;
    public GameObject GetSetDamageText { get { return GetSetDamageText; } set { GetSetDamageText = value; } }
    protected void VisibleDamage(int num)
    {
        GameObject damagetext;
        damagetext = Instantiate(DamageText);
        GameObject tx = damagetext.transform.GetChild(0).gameObject;
        TextMeshProUGUI txm = tx.GetComponent<TextMeshProUGUI>();
        txm.text = num.ToString();
        txm.color += new Color(0, 0, 0, 0);
        damagetext.transform.position = new Vector3(this.transform.position.x, transform.position.y, 0);
    }

}

