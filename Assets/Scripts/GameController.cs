using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの流れを管理
/// </summary>
public class GameController : Singleton<GameController>
{
    /// <summary>
    /// 盤面上の1つの目のプレハブ
    /// </summary>
    public GameObject BoardCrossPrefab;
    /// <summary>
    /// 目の親オブジェクト
    /// </summary>
    public Transform BoardCrossParent;

    /// <summary>
    /// カメラ情報
    /// </summary>
    private RaycastHit hit;

    /// <summary>
    /// 自プレイヤーの使う石の色
    /// </summary>
    public BoardCross.Status playerColor = BoardCross.Status.White;
    /// <summary>
    /// 相手プレイヤーの使う石の色
    /// </summary>
    public BoardCross.Status opponentColor
    {
        get
        {
            switch (playerColor)
            {
                default:
                case BoardCross.Status.White:
                    return BoardCross.Status.Black;
                case BoardCross.Status.Black:
                    return BoardCross.Status.White;
            }
        }
    }

    /// <summary>
    /// 最後の手を打ってからの時間
    /// </summary>
    [HideInInspector]
    public float timeFromLastMove = 0f;

    protected override void Awake()
    {
        base.Awake();

        // 盤面生成
        for (int i=0; i<BoardCross.BOARD_SIZE; i++)
        {
            for (int j=0; j<BoardCross.BOARD_SIZE; j++)
            {
                GameObject go = Instantiate(BoardCrossPrefab, BoardCrossParent);
                BoardCross board = go.GetComponent<BoardCross>() ?? go.AddComponent<BoardCross>();
                board.Initialize(i, j);
            }
        }

        // 線生成
        ConnectingLineFactory.Instance.GenerateConnectingLineInstance();
    }

    private void Update()
    {
        // マウスクリック
        if (Input.GetMouseButtonDown(0))
        {
            // マウスのポジションを取得してRayに代入
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // マウスのポジションからRayを投げて何かに当たったらhitに入れる
            if (Physics.Raycast(ray, out hit))
            {
                // Boardレイヤーなら
                BoardCross board = hit.collider.GetComponent<BoardCross>();
                if (board != null)
                {
                    // 合法手がどうか調べる 
                    if (board.IsLegalMove(playerColor))
                    {
                        // 石を置く
                        board.BoardStatus = playerColor;

                        // 石を打つ音
                        AudioController.Instance.PlayPutStone();

                        // 隣の石を取り除く
                        List<BoardCross> Prisoners = board.RemoveStoneAll(playerColor);

                        // 石を取った石も取り除く
                        List<BoardCross> SiegingStone = BoardCross.SiegingBoardCross(Prisoners);
                        foreach (BoardCross sieging in SiegingStone)
                        {
                            sieging.BoardStatus = BoardCross.Status.None;
                        }
                        
                        // 取り除いた場合、音を鳴らす
                        if (Prisoners.Count > 0)
                        {
                            AudioController.Instance.PlayRemoveStone();
                        }

                        // 取り除かなかった場合、置いた石に接している石に対して、線が引けるなら引く
                        else
                        {
                            //board.ActivateOpponentLine();
                            foreach (BoardCross neighborhood in board.Neighborhood8)
                            {
                                if (neighborhood.BoardStatus == board.OpponentStatus)
                                {
                                    neighborhood.ActivateOpponentLine();
                                }
                            }
                        }

                        // 最後の手を打った時間をリセット
                        timeFromLastMove = 0f;
                    }
                    else
                    {
                        // エラー音
                        AudioController.Instance.PlayError();
                    }
                }
            }
        }

        
        // 相手（COM）の手
        if (OpponentController.Instance.IsCooledDown)
        {
            BoardCross board = OpponentController.Instance.Move(BoardCross.Field);
            if (board.IsLegalMove(opponentColor))
            {
                // 石を置く
                board.BoardStatus = opponentColor;

                // 石を打つ音
                AudioController.Instance.PlayPutStone();

                // 隣の石を取り除く
                List<BoardCross> Prisoners = board.RemoveStoneAll(opponentColor);

                // 石を取った石も取り除く
                List<BoardCross> SiegingStone = BoardCross.SiegingBoardCross(Prisoners);
                foreach (BoardCross sieging in SiegingStone)
                {
                    sieging.BoardStatus = BoardCross.Status.None;
                }

                // 音を鳴らす
                if (Prisoners.Count > 0)
                {
                    AudioController.Instance.PlayRemoveStone();
                }

                // 取り除かなかった場合、置いた石に線が引けるなら引く
                else
                {
                    board.ActivateOpponentLine();
                }
            }
        }
        

        // 相手の石が一定数を超えた場合はリセット
        // たぶん盤面が埋まることは無いので一旦保留

        // 時間更新
        timeFromLastMove += Time.deltaTime;
    }

    // デバッグ用
    int windowId = 0;
    Rect windowRect = new Rect(0, 0, 250, 200);
    private void OnGUI()
    {
        windowRect = GUI.Window(windowId, windowRect, (Id) =>
        {
            // プレイヤーの色を変更
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Player:");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(playerColor.ToString()))
            {
                if (playerColor == BoardCross.Status.White)
                {
                    playerColor = BoardCross.Status.Black;
                }
                else if (playerColor == BoardCross.Status.Black)
                {
                    playerColor = BoardCross.Status.White;
                }
            }
            GUILayout.EndHorizontal();

            // 盤面リセット
            if (GUILayout.Button("Reset Board"))
            {
                BoardCross.ClearStoneAll();
            }

            // 最後の手を打ってからの時間表示
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time from Last Move:");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{timeFromLastMove:0.00} sec");
            GUILayout.EndHorizontal();

            // 相手（COM）のクールダウン時間
            GUILayout.BeginHorizontal();
            GUILayout.Label("COM Span:");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{OpponentController.Instance.SpanAverage:0.00} sec");
            GUILayout.EndHorizontal();
            OpponentController.Instance.SpanAverage = GUILayout.HorizontalSlider(OpponentController.Instance.SpanAverage, 0f, 10f);

            // 移動可
            GUI.DragWindow();
        }, "Game Controller (Debug)");
    }
}