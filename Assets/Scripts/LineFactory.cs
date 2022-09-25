using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �΂���鎞�ɏo�����鎇�̐��𐶐�����N���X
/// </summary>
public class LineFactory : Singleton<LineFactory>
{
    /// <summary>
    /// ��������v���n�u
    /// </summary>
    public LineInstance linePrefab;
    /// <summary>
    /// �������ꂽ�C���X�^���X
    /// </summary>
    private LineInstance lineInstance;
    /// <summary>
    /// �͂�ꂽ�ڂ���
    /// </summary>
    private List<BoardCross> SiegedBoardCross = new List<BoardCross>();

    /// <summary>
    /// �C���X�^���X�𐶐�����
    /// ���O�Ɉ͂��Ă���ڂ�AddSiegedBoardCross�őS�Ēǉ����Ă�������
    /// </summary>
    public void GenerateLineInstance()
    {
        // �C���X�^���X�𐶐����Ĉ͂�ꂽ�ڂ̃��X�g����
        lineInstance = Instantiate(linePrefab, transform);
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
}
