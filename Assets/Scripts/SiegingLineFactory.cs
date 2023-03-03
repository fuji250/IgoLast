using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �΂���鎞�ɏo�����鎇�̐��𐶐�����N���X
/// </summary>
public class SiegingLineFactory : Singleton<SiegingLineFactory>
{
    /// <summary>
    /// �v���C���[������̐΂���������ɐ�������v���n�u
    /// </summary>
    public SiegingLine linePrefab;
    /// <summary>
    /// ���肪�v���C���[�̐΂���������ɐ�������v���n�u
    /// </summary>
    public SiegingLine linePrefabOpponent;
    /// <summary>
    /// �͂�ꂽ�ڂ���
    /// </summary>
    private List<BoardCross> SiegedBoardCross = new List<BoardCross>();

    #region �΂���������̐�
    /// <summary>
    /// �C���X�^���X�𐶐�����
    /// ���O�Ɉ͂��Ă���ڂ�AddSiegedBoardCross�őS�Ēǉ����Ă�������
    /// </summary>
    public void GenerateSiegingLineInstance(bool isPlayer = true)
    {
        // �C���X�^���X�𐶐����Ĉ͂�ꂽ�ڂ̃��X�g����
        SiegingLine lineInstance = Instantiate(isPlayer ?linePrefab : linePrefabOpponent, transform);
        lineInstance.Initialize(SiegedBoardCross);
        lineInstance.FadeOut();

        // ���X�g�폜
        ResetSiegedBoardCross();
    }
    /// <summary>
    /// �͂��Ă���ڂ�ǉ�����
    /// </summary>
    /// <param name="sieged"></param>
    public void AddSiegedBoardCross(BoardCross sieged)
    {
        // ����
        SiegedBoardCross.Add(sieged);
    }
    /// <summary>
    /// �S�Ă̗v�f���폜����
    /// </summary>
    public void ResetSiegedBoardCross()
    {
        SiegedBoardCross.RemoveAll((board) => true);
    }
    #endregion
}
