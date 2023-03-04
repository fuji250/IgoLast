using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    /// <summary>
    /// 縦線の親オブジェクト
    /// </summary>
    [SerializeField] Transform m_VerticalParent;

    /// <summary>
    /// 横線の親オブジェクト
    /// </summary>
    [SerializeField] Transform m_HorizontalParent;

    /// <summary>
    /// 一マスあたりの大きさ
    /// </summary>
    private float m_BlockSize = 0.125f;

    /// <summary>
    /// 最大マス数
    /// </summary>
    private int m_MaxBlockNum = 16;

    /// <summary>
    /// Lineを非活性化させたり短くしたりする
    /// </summary>
    /// <param name="p_hLength">盤面の横のマス数</param>
    /// <param name="p_vLength">盤面の縦のマス数</param>
    public void AdjustLines(int p_hLength, int p_vLength)
    {
        //横線について
        for(int x = 0;x < m_MaxBlockNum; x++)
        {
            //そもそも使わないLineだったら
            if(x >= p_hLength)
            {
                //非活性化する
                m_VerticalParent.GetChild(x).gameObject.SetActive(false);
            }

            //使うLineだったら
            //指定されたマス数に合わせて長さを調整する
            m_HorizontalParent.GetChild(x).localScale = new Vector3(m_BlockSize * (p_hLength-1), 1, 1);
        }

        //縦線について
        for(int y = 0;y < m_MaxBlockNum;y++)
        {
            //そもそも使わないLineだったら
            if (y >= p_vLength)
            {
                //非活性化する
                m_HorizontalParent.GetChild(y).gameObject.SetActive(false);
            }

            //使うLineだったら
            //指定されたマス数に合わせて長さを調整する
            m_VerticalParent.GetChild(y).localScale = new Vector3(m_BlockSize * (p_vLength-1), 1, 1);
        }
    }
}
