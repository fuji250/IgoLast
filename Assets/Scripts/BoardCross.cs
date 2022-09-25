using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 盤上の目1つを表す
/// </summary>
public class BoardCross: MonoBehaviour, IComparable<BoardCross>
{
    #region 座標等
    /// <summary>
    /// 盤の1辺の長さ9+端の分2
    /// </summary>
    public readonly static int BOARD_SIZE = 11;
    /// <summary>
    /// x座標 1～9
    /// </summary>
    public int X
    {
        private set; get;
    }
    /// <summary>
    /// z座標 1～9
    /// </summary>
    public int Z
    {
        private set; get;
    }
    /// <summary>
    /// (x, z)
    /// </summary>
    public Vector2 coordinate
    {
        get
        {
            return new Vector2(X, Z);
        }
    }
    /// <summary>
    /// 天元の座標
    /// </summary>
    public static Vector2 CenterCoordinate
    {
        set; get;
    }
    /// <summary>
    /// この目が碁盤の外にあるかどうか
    /// </summary>
    public bool IsOut
    {
        get
        {
            return X < 1 || BOARD_SIZE - 2 < X || Z < 1 || BOARD_SIZE - 2 < Z;
        }
    }
    /// <summary>
    /// 比較演算子（ソートに使用）
    /// 左下が弱く、右上が強い
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(BoardCross other)
    {
        if (other == null) return 1;
        return (X + Z) - (other.X + other.Z);
    }
    #endregion

    #region 目の状態等
    /// <summary>
    /// 現在の目の状態
    /// </summary>
    public enum Status
    {
        None,   // 何も置かれていない
        Black,  // 黒石が置かれている
        White,  // 白石が置かれている
        Out,    // 盤面の外
    }
    private Status boardStatus;
    /// <summary>
    /// 現在の目の状態
    /// </summary>
    public Status BoardStatus
    {
        get
        {
            return boardStatus;
        }
        set
        {
            boardStatus = value;

            // ビジュアルも反映させる
            UpdateBoardVis(value);
        }
    }
    /// <summary>
    /// 現在置かれている石と対立する石の色
    /// </summary>
    public Status OpponentStatus
    {
        get
        {
            if (BoardStatus == Status.Black) return Status.White;
            else if (BoardStatus == Status.White) return Status.Black;
            else
            {
                return Status.None;
            }
        }
    }
    /// <summary>
    /// この目に乗る黒石
    /// </summary>
    [SerializeField]
    private Transform BlackStone;
    /// <summary>
    /// この目に乗る白石
    /// </summary>
    [SerializeField]
    private Transform WhiteStone;
    #endregion

    #region 目の相対関係等
    /// <summary>
    /// 盤面
    /// </summary>
    public static List<BoardCross> Field;
    /// <summary>
    /// 特定の座標のBoardCrossを取得する
    /// </summary>
    /// <param name="x">x座標 1～9</param>
    /// <param name="z">z座標 1～9</param>
    /// <returns></returns>
    public static BoardCross Find(int x, int z)
    {
        foreach (BoardCross c in Field)
        {
            if (c.X == x && c.Z == z)
            {
                return c;
            }
        }
        Debug.LogError($"BoardCross: ({x}, {z}) is not found! Field.count = {Field.Count}");
        return null;
    }
    /// <summary>
    /// 特定の座標のBoardCrossを取得する
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public static BoardCross Find(Vector2 coordinate)
    {
        return Find((int)coordinate.x, (int)coordinate.y);
    }
    /// <summary>
    /// この目の上側
    /// </summary>
    public BoardCross Up
    {
        get
        {
            return Find(X, Z + 1);
        }
    }
    /// <summary>
    /// この目の下側
    /// </summary>
    public BoardCross Down
    {
        get
        {
            return Find(X, Z - 1);
        }
    }
    /// <summary>
    /// この目の右側
    /// </summary>
    public BoardCross Right
    {
        get
        {
            return Find(X + 1, Z);
        }
    }
    /// <summary>
    /// この目の左側
    /// </summary>
    public BoardCross Left
    {
        get
        {
            return Find(X - 1, Z);
        }
    }
    /// <summary>
    /// この目の右上側
    /// </summary>
    public BoardCross UpRight
    {
        get
        {
            return Find(X + 1, Z + 1);
        }
    }
    /// <summary>
    /// この目の左上側
    /// </summary>
    public BoardCross UpLeft
    {
        get
        {
            return Find(X - 1, Z + 1);
        }
    }
    /// <summary>
    /// この目の右下側
    /// </summary>
    public BoardCross DownRight
    {
        get
        {
            return Find(X + 1, Z - 1);
        }
    }
    /// <summary>
    /// この目の左下側
    /// </summary>
    public BoardCross DownLeft
    {
        get
        {
            return Find(X - 1, Z - 1);
        }
    }
    /// <summary>
    /// 隣接する縦横4つの目
    /// </summary>
    public BoardCross[] Neighborhood4
    {
        get
        {
            return new BoardCross[4] { Up, Down, Right, Left };
        }
    }
    /// <summary>
    /// 隣接する縦横斜め8つの目
    /// </summary>
    public BoardCross[] Neighborhood8
    {
        get
        {
            return new BoardCross[8] { Left, UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, };
        }
    }
    /// <summary>
    /// Fieldのうち、何も置かれていない場所
    /// </summary>
    public static List<BoardCross> EmptyField
    {
        get
        {
            List<BoardCross> result = new List<BoardCross>();
            foreach (BoardCross board in Field)
            {
                if (board.BoardStatus == Status.None)
                {
                    result.Add(board);
                }
            }
            return result;
        }
    }
    #endregion

    /// <summary>
    /// 囲い石の際にチェック済みかどうか調べるのに使う
    /// </summary>
    private bool isCheckedSieged = false;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void Initialize(int x, int z)
    {
        // 変数初期化
        X = x;
        Z = z;

        // 位置初期化
        CenterCoordinate = Vector2.zero;
        TranslateBoard(x, z);

        // 石をリセットする
        BoardStatus = IsOut ? Status.Out : Status.None;

        // 盤面Fieldがnullの場合に盤面を生成
        if (Field == null)
        {
            Field = new List<BoardCross>();
        }
        // 盤面に自身を追加
        if (!Field.Contains(this))
        {
            Field.Add(this);
        }
    }
    /// <summary>
    /// 石を全て取り除く
    /// </summary>
    public static void ClearStone()
    {
        foreach (BoardCross board in Field)
        {
            board.BoardStatus = board.IsOut ? Status.Out : Status.None;
        }
    }
    /// <summary>
    /// 碁盤上の所定座標に移動する
    /// </summary>
    /// <param name="pos">(1, 9)×(1, 9)</param>
    public void TranslateBoard(Vector2 coordinate)
    {
        TranslateBoard((int)coordinate.x, (int)coordinate.y);
    }
    /// <summary>
    /// 碁盤上の所定座標に移動する
    /// </summary>
    /// <param name="x">x座標 1～9</param>
    /// <param name="z">z座標 1～9</param>
    public void TranslateBoard(int x, int z)
    {
        transform.position = new Vector3(x, 0f, z) + new Vector3(CenterCoordinate.x, 0f, CenterCoordinate.y) - new Vector3(5f, 0f, 5f);
    }
    /// <summary>
    /// 盤面のビジュアルを更新する
    /// </summary>
    /// <param name="nextStatus"></param>
    private void UpdateBoardVis(Status nextStatus)
    {
        // ビジュアルの更新
        BlackStone.gameObject.SetActive(nextStatus == Status.Black);
        WhiteStone.gameObject.SetActive(nextStatus == Status.White);
    }
    /// <summary>
    /// 調べる場所にある石が相手に囲まれているか調べる
    /// </summary>
    /// <param name="thisBoardCross">最初に調べる場所</param>
    /// <returns></returns>
    public bool IsSieged(BoardCross thisBoardCross)
    {
        // チェック履歴をクリアする
        ClearAlreadyCheckedFlag();

        // 外側なら何もしない
        if (IsOut)
        {
            return false;
        }

        // 再帰関数を回す
        var result = IsSiegedItr(BoardStatus, thisBoardCross); // ほんとは他と同じでキュー方式にしたい
        return result;
    }
    /// <summary>
    /// IsSiegedの再帰処理部分
    /// </summary>
    /// <param name="originalStatus">最初に調べる場所の状態</param>
    /// <param name="thisBoardCross">（再帰の中で）現在注目している場所</param>
    /// <returns>この場所の石が囲まれていない</returns>
    private bool IsSiegedItr(Status originalStatus, BoardCross thisBoardCross)
    {
        // この場所を既に調べていたら終わり
        if (thisBoardCross.isCheckedSieged)
        {
            return true;
        }

        // 調べた場所をマークする
        thisBoardCross.isCheckedSieged = true;

        // 何も置かれていなければ終わり
        if (thisBoardCross.BoardStatus == Status.None)
        {
            return false;
        }

        // 外なら囲われているのと同じ
        if (thisBoardCross.boardStatus == Status.Out)
        {
            return true;
        }

        // 今調べている石とオリジナルの石が同じ色ならばその隣の石も調べる
        // 隣の石が生きているならば、この石も生きている
        if (thisBoardCross.BoardStatus == originalStatus)
        {
            //Debug.Log($"{thisBoardCross.coordinate}");
            var result = true;
            // 左側を捜査
            if (!thisBoardCross.Left.IsOut)
            {
                result = IsSiegedItr(originalStatus, thisBoardCross.Left);
                if (!result)
                {
                    return false;
                }
            }
            // 下側を捜査
            if (!thisBoardCross.Down.IsOut)
            {
                result = IsSiegedItr(originalStatus, thisBoardCross.Down);

                // 脇の石が囲われていないならば、自分も囲われていない
                if (!result)
                {
                    return false;
                }
            }
            // 右側を捜査
            if (!thisBoardCross.Right.IsOut)
            {
                result = IsSiegedItr(originalStatus, thisBoardCross.Right);
                if (!result)
                {
                    return false;
                }
            }
            // 上側を捜査
            if (!thisBoardCross.Up.IsOut)
            {
                result = IsSiegedItr(originalStatus, thisBoardCross.Up);
                if (!result)
                {
                    return false;
                }
            }
        }
        // この石が囲われている
        return true;
    }
    /// <summary>
    /// 置いた石の上下左右に相手の囲われた石があるなら取る
    /// </summary>
    /// <param name="playerColor"></param>
    /// <returns>取った石があった目</returns>
    public List<BoardCross> RemoveStoneAll(Status playerColor)
    {
        List<BoardCross> result = new List<BoardCross>();
        result.AddRange(Up.RemoveStone(playerColor));
        result.AddRange(Down.RemoveStone(playerColor));
        result.AddRange(Right.RemoveStone(playerColor));
        result.AddRange(Left.RemoveStone(playerColor));

        return result;
    }
    /// <summary>
    /// この目の石が死んでいれば碁盤から取り除く
    /// </summary>
    /// <param name="playerColor">自分が置いた石の色</param>
    /// <returns></returns>
    public List<BoardCross> RemoveStone(Status playerColor)
    {
        // 外側なら何もしない
        if (IsOut)
        {
            return new List<BoardCross>();
        }

        // 置いた石と同じ色なら取らない
        if (BoardStatus == playerColor)
        {
            return new List<BoardCross>();
        }

        // 追加する前にリセット
        LineFactory.Instance.ResetSiegedBoardCross();

        // 囲まれてないなら何もしない
        if (!IsSieged(this))
        {
            return new List<BoardCross>();
        }

        ClearAlreadyCheckedFlag();

        // 返り値
        List<BoardCross> result = new List<BoardCross>();

        // 取る石の候補
        Queue<BoardCross> Candidates = new Queue<BoardCross>();

        // キューに自身を追加
        Candidates.Enqueue(this);

        while (Candidates.Count > 0)
        {
            // キューから1つ取り出す
            BoardCross candidate = Candidates.Dequeue();

            // もう調べてあるなら何もしない
            if (candidate.isCheckedSieged) continue;

            // 調べたことを記録
            candidate.isCheckedSieged = true;

            // 色が元の色と異なるなら何もしない
            if (candidate.BoardStatus != BoardStatus) continue;

            // 返り値に追加
            result.Add(candidate);

            // LineFactoryの囲われた石のリストにも追加
            LineFactory.Instance.AddSiegedBoardCross(candidate);

            // 候補の隣の石も候補に加える
            foreach (BoardCross neighborhood in candidate.Neighborhood4)
            {
                if (!neighborhood.IsOut)
                {
                    Candidates.Enqueue(neighborhood);
                }
            }
        }

        // 返り値になっている目の石を取り除く
        foreach (BoardCross board in result)
        {
            board.BoardStatus = Status.None;
        }

        // 線を生成する
        LineFactory.Instance.GenerateLineInstance();

        return result;
    }
    /// <summary>
    /// 自殺手かどうか調べる
    /// </summary>
    /// <param name="nextMove">次に置きたい手</param>
    /// <returns></returns>
    public bool IsSuicide(Status nextMove)
    {
        // 仮に石を置く
        BoardStatus = nextMove;

        // 仮に置いた石が相手に囲まれているならば自殺手の可能性あり
        if (IsSieged(this))
        {
            // その石を置いたことにより、隣の相手の石が取れるなら自殺手ではない
            // 隣は相手？
            foreach (BoardCross neighborhood in Neighborhood4)
            {
                if (neighborhood.BoardStatus == OpponentStatus)
                {
                    // 相手のが囲まれているなら自殺手ではない
                    if (neighborhood.IsSieged(neighborhood))
                    {
                        BoardStatus = Status.None;
                        return false;
                    }
                }
            }

            // 盤を元に戻す
            BoardStatus = Status.None;

            // 相手の石を取れないなら自殺手
            return true;
        }
        else
        {
            BoardStatus = Status.None;

            // 囲まれていないので自殺手ではない
            return false;
        }
    }
    /// <summary>
    /// 合法手かどうか調べる
    /// </summary>
    /// <param name="nextMove">次に置きたい石の色</param>
    /// <returns>合法手である</returns>
    public bool IsLegalMove(Status nextMove)
    {
        // 空点でないと置けない
        if (boardStatus != Status.None)
        {
            return false;
        }

        // 一手前にコウを取られていたら置けない
        // 履歴作るのちょっと面倒くさいので略
        // UndoSystemを作るという説がある

        // 自殺手なら置けない
        if (IsSuicide(nextMove))
        {
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// 囲い石の際のチェック用の変数をクリアする
    /// </summary>
    public static void ClearAlreadyCheckedFlag()
    {
        foreach (BoardCross board in Field)
        {
            board.isCheckedSieged = false;
        }
    }

    /// <summary>
    /// 引数の目を囲むように配置された石のリストを返す
    /// </summary>
    /// <param name="sieged"></param>
    /// <returns></returns>
    public static List<BoardCross> SiegingBoardCross(List<BoardCross> sieged)
    {
        var result = new List<BoardCross>();
        foreach (BoardCross board in sieged)
        {
            // 囲われた石の周りで捜査
            foreach (BoardCross neighborhood in board.Neighborhood4)
            {
                // 相手の石があったら追加
                // 黒も白もカウントする
                if (neighborhood.BoardStatus == BoardCross.Status.Black || neighborhood.BoardStatus == BoardCross.Status.White)
                {
                    result.Add(neighborhood);
                }

                // 角だったらboard自身を追加
                if (neighborhood.BoardStatus == BoardCross.Status.Out && !result.Contains(board))
                {
                    result.Add(board);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 与えたリストが一筆書きになるようにリストをソートする
    /// </summary>
    /// <param name="list">輪を描くような石の列</param>
    public static List<BoardCross> SortOneStroke(List<BoardCross> list)
    {
        var result = new List<BoardCross>();

        // チェック用のフラグをクリア
        ClearAlreadyCheckedFlag();

        // 代表点を一つ取り出す
        list.Sort();
        // 左下
        BoardCross next = list[0];
        //Debug.Log($"BoardCross.SortOnStroke: fist = {next.coordinate}");
        result.Add(next);

        // 何かの拍子に無限ループしちゃった時の対策
        int count = 0;

        // resutlの大きさがlistのサイズと同じになるまで続ける
        while (result.Count < list.Count)
        {
            for (int i = 0; i < next.Neighborhood8.Length; i++)
            {
                // まだ捜査しておらず、かつlistに含まれる隣の目について、リストへの追加を行う
                if (list.Contains(next.Neighborhood8[i]) && !next.Neighborhood8[i].isCheckedSieged)
                {
                    next.Neighborhood8[i].isCheckedSieged = true;

                    result.Add(next.Neighborhood8[i]);

                    // 次はその目を中心にして捜査する
                    next = next.Neighborhood8[i];

                    break;
                }
            }

            count++;

            // ある程度countが大きくなりすぎたらエラー吐いて強制終了
            if (count == 100)
            {
                Debug.LogError($"BoardCross.SortOneStroke: Too much iteration!");
                break;
            }
        }

        return result;
    }
}
