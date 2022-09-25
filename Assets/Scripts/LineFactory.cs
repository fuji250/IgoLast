using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石を取る時に出現する紫の線を生成するクラス
/// </summary>
public class LineFactory : Singleton<LineFactory>
{
    /// <summary>
    /// 生成するプレハブ
    /// </summary>
    public LineInstance linePrefab;
    /// <summary>
    /// 生成されたインスタンス
    /// </summary>
    private LineInstance lineInstance;
    /// <summary>
    /// 囲われた目たち
    /// </summary>
    private List<BoardCross> SiegedBoardCross = new List<BoardCross>();

    /// <summary>
    /// インスタンスを生成する
    /// 事前に囲われている目をAddSiegedBoardCrossで全て追加しておくこと
    /// </summary>
    public void GenerateLineInstance()
    {
        // インスタンスを生成して囲われた目のリストを代入
        lineInstance = Instantiate(linePrefab, transform);
        lineInstance.Initialize(SiegedBoardCross);
        lineInstance.FadeOut();

        // リスト削除
        ResetSiegedBoardCross();
    }
    /// <summary>
    /// 囲われている目を追加する
    /// </summary>
    /// <param name="sieged"></param>
    public void AddSiegedBoardCross(BoardCross sieged)
    {
        // 足す
        SiegedBoardCross.Add(sieged);
    }
    /// <summary>
    /// 全ての要素を削除する
    /// </summary>
    public void ResetSiegedBoardCross()
    {
        SiegedBoardCross.RemoveAll((board) => true);
    }
}
