using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    const int WIDTH = 11;
    //10*10のint型２次元配列を定義
    public COLOR[,] board = new COLOR[WIDTH, WIDTH];
    /* 合法手かどうか調べるのに使う */
    bool[,] legalCheckBoard = new bool[WIDTH, WIDTH];
    /* 囲い石の際にチェック済みかどうか調べるのに使う */
    bool[,] checkBoard = new bool[WIDTH, WIDTH];

    // enumを使って数字に名前をつける
    public enum COLOR
    {
        EMPTY,  //空欄 = 0
        BLACK,  //黒色 = 1
        WHITE,   //白色 = 2
        OUT   //盤外 = 3
    }

    //カメラ情報
    private Camera camera_object;
    private RaycastHit hit;

    //prefabs
    public GameObject whiteStone;
    public GameObject OutWhiteStone;
    public GameObject blackStone;

    public COLOR player = COLOR.WHITE;

    List<GameObject> lineStoneList = new List<GameObject>();
    List<Vector3> lineStonePosList = new List<Vector3>();

    public GameObject beamObj;
    public GameObject finishBeamObj;

    AudioSource audioSource;
    //public AudioClip sound1;

    int BlackNum = 1;
    int rndint = 1;


    // Start is called before the first frame update
    void Start()
    {
        //カメラ情報を取得
        camera_object = GameObject.Find("Main Camera").GetComponent<Camera>();

        //配列を初期化
        InitializeArray();

        PutBlack(2, 1);
        PutBlack(2, 3);
        PutBlack(1, 2);
        PutBlack(3, 2);
        PutBlack(6, 6);
        PutBlack(6, 7);
        //PutBlack(7, 6);
        //PutBlack(7, 7);
        PutBlack(3, 7);

        audioSource = GameObject.Find("Audio").GetComponent<AudioSource>();
        InvokeRepeating("MakeBlack", 1, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            //マウスのポジションを取得してRayに代入
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);
            //Debug.Log(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //x,zの値を取得
                int x = (int)hit.collider.gameObject.transform.position.x;
                int z = (int)hit.collider.gameObject.transform.position.z;
                Debug.Log(board[x, z]);
            }
        }

        //マウスがクリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            //マウスのポジションを取得してRayに代入
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);

            //マウスのポジションからRayを投げて何かに当たったらhitに入れる
            if (Physics.Raycast(ray, out hit))
            {
                //x,zの値を取得
                int x = (int)hit.collider.gameObject.transform.position.x;
                int z = (int)hit.collider.gameObject.transform.position.z;

                //マスが空のとき
                if (board[x, z] == COLOR.EMPTY)
                {
                    // 座標が1〜19ならば合法手がどうか調べる 
                    if (CheckLegal(player, x, z))
                    {
                        SetStone(player, x, z);
                    }
                }
            }
        }
    }

    //配列情報を初期化する
    private void InitializeArray()
    {
        //for文を利用して配列にアクセスする
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                //配列を空（値を０）にする
                board[i, j] = COLOR.EMPTY;

                if (i == 0 || i == WIDTH - 1 || j == 0 || j == WIDTH - 1)
                {
                    board[i, j] = COLOR.WHITE;
                    //Stoneを出力
                    GameObject stone = Instantiate(OutWhiteStone);
                    stone.transform.position = new Vector3(i, 0, j);
                }
            }
        }
    }

    void SetOutStone()
    {
        //for文を利用して配列にアクセスする
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (i == 0 || i == WIDTH - 1 || j == 0 || j == WIDTH - 1)
                {
                    board[i, j] = COLOR.WHITE;
                    //Stoneを出力
                    //GameObject stone = Instantiate(whiteStone);
                    //stone.transform.position = new Vector3(i, 0, j);
                }
            }
        }
    }

    public void CheckPosition(float X, float Z)
    {
        // センサーの値からRay飛ばす
        if (Physics.Raycast(new Vector3(X, 1, Z), Vector3.down, out hit))
        {
            //x,zの値を取得
            int x = (int)hit.collider.gameObject.transform.position.x;
            int z = (int)hit.collider.gameObject.transform.position.z;


            //マスが空のとき
            if (board[x, z] == COLOR.EMPTY)
            {
                // 座標が1〜19ならば合法手がどうか調べる 
                if (CheckLegal(player, x, z))
                {
                    SetStone(player, x, z);
                }
            }
        }
    }

    /*------------------------------------------------------------------*/
    /* 合法手かどうか調べる                                             */
    /*------------------------------------------------------------------*/
    public bool CheckLegal(COLOR color, int x, int z)
    {

        /* 自殺手なら置けません */
        if (CheckSuicide(color, x, z))
        {
            Debug.Log("自殺手");

            return false;
        }

        /* 以上のチェックをすべてクリアできたので置けます */
        return true;
    }

    /*------------------------------------------------------------------*/
    /* 自殺手かどうか調べる                                             */
    /*------------------------------------------------------------------*/
    public bool CheckSuicide(COLOR color, int x, int z)
    {
        /* 仮に石を置く */
        board[x, z] = color;

        /* マークのクリア */
        ClearLegalCheckBoard();
        /* 相手の色を求める */
        COLOR opponent = OpponentColor(color);

        /* その石は相手に囲まれているか調べる */
        /* 囲まれているならば自殺手の可能性あり */
        if (DoCheckRemoveStone(color, x, z))
        {
            /* その石を置いたことにより、隣の相手の石が取れるなら自殺手ではない */
            if (x >= 1)
            {
                /* 隣は相手？ */
                if (board[x - 1, z] == opponent)
                {
                    /* マークのクリア */
                    ClearLegalCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    /* 相手の石を取れるので自殺手ではない */
                    if (DoCheckRemoveStone(opponent, x - 1, z))
                    {
                        /* 盤を元に戻す */
                        board[x, z] = COLOR.EMPTY;

                        return false;
                    }
                }
            }
            if (z >= 1)
            {
                /* 隣は相手？ */
                if (board[x, z - 1] == opponent)
                {
                    /* マークのクリア */
                    ClearLegalCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    /* 相手の石を取れるので自殺手ではない */
                    if (DoCheckRemoveStone(opponent, x, z - 1))
                    {
                        /* 盤を元に戻す */
                        board[x, z] = COLOR.EMPTY;

                        return false;
                    }
                }
            }
            if (x <= (WIDTH - 2))
            {
                /* 隣は相手？ */
                if (board[x + 1, z] == opponent)
                {
                    /* マークのクリア */
                    ClearLegalCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    /* 相手の石を取れるので自殺手ではない */
                    if (DoCheckRemoveStone(opponent, x + 1, z))
                    {
                        /* 盤を元に戻す */
                        board[x, z] = COLOR.EMPTY;
                        Debug.Log("V");
                        return false;
                    }
                }
            }
            if (z <= (WIDTH - 2))
            {
                /* 隣は相手？ */
                if (board[x, z + 1] == opponent)
                {
                    /* マークのクリア */
                    ClearLegalCheckBoard();
                    /* 相手の石は囲まれているか？ */
                    /* 相手の石を取れるので自殺手ではない */
                    if (DoCheckRemoveStone(opponent, x, z + 1))
                    {
                        /* 盤を元に戻す */
                        board[x, z] = COLOR.EMPTY;
                        return false;
                    }
                }
            }

            /* 盤を元に戻す */
            board[x, z] = COLOR.EMPTY;

            /* 相手の石を取れないなら自殺手 */
            return true;
        }
        else
        {
            /* 盤を元に戻す */
            board[x, z] = COLOR.EMPTY;

            /* 囲まれていないので自殺手ではない */
            return false;
        }
    }
    /*------------------------------------------------------------------*/
    /* チェック用の碁盤をクリア                                         */
    /*------------------------------------------------------------------*/
    void ClearLegalCheckBoard()
    {
        int x, y;

        for (y = 1; y < (WIDTH - 1); y++)
        {
            for (x = 1; x < (WIDTH - 1); x++)
            {
                legalCheckBoard[x, y] = false;
            }
        }
    }
    /*------------------------------------------------------------------*/
    /* チェック用の碁盤をクリア                                         */
    /*------------------------------------------------------------------*/
    void ClearCheckStoneBoard()
    {
        int x, y;

        for (y = 1; y < (WIDTH - 1); y++)
        {
            for (x = 1; x < (WIDTH - 1); x++)
            {
                checkBoard[x, y] = false;
            }
        }
    }


    /*------------------------------------------------------------------*/
    /* 座標(x,y)にあるcolor石が相手に囲まれているか調べる               */
    /*------------------------------------------------------------------*/
    /* 空点があればFALSEを返し、空点がなければTRUEを返す */
    bool DoCheckRemoveStone(COLOR color, int x, int z)
    {
        COLOR opponent;

        /* その場所は既に調べた点ならおしまい */
        if (legalCheckBoard[x, z])
        {
            return true;
        }

        /* 調べたことをマークする */
        legalCheckBoard[x, z] = true;

        /* 相手の色を求める */
        if (color == COLOR.BLACK)
        {
            opponent = COLOR.WHITE;
        }
        else
        {
            opponent = COLOR.BLACK;
        }

        /* 何も置かれていないならばおしまい */
        if (board[x, z] == COLOR.EMPTY)
        {
            //Debug.Log("空だった");
            return false;
        }

        /* 同じ色の石ならばその石の隣も調べる */
        if (board[x, z] == color)
        {
            /* その石の左(x-1,y)を調べる */
            if (x >= 1)
            {
                if (DoCheckRemoveStone(color, x - 1, z) == false)
                {
                    //Debug.Log("左は空");
                    return false;
                }
            }
            /* その石の上(x,y+1)を調べる */
            if (z <= (WIDTH - 2))
            {
                if (DoCheckRemoveStone(color, x, z + 1) == false)
                {
                    //Debug.Log("上は空");
                    return false;
                }
            }
            /* その石の右(x+1,y)を調べる */
            if (x <= (WIDTH - 2))
            {
                if (DoCheckRemoveStone(color, x + 1, z) == false)
                {
                    //Debug.Log("右は空");
                    return false;
                }
            }
            /* その石の下(x,y-1)を調べる */
            if (z >= 1)
            {
                if (DoCheckRemoveStone(color, x, z - 1) == false)
                {
                    //Debug.Log("下は空");
                    return false;
                }
            }
        }
        /* 相手の色の石があった */
        //Debug.Log("相手の石があった");

        return true;
    }

    /*------------------------------------------------------------------*/
    /* 碁盤に石を置く                                                   */
    /*------------------------------------------------------------------*/
    void
   SetStone(COLOR color, int x, int z)
    {
        int prisonerN;    /* 取り除かれた石の数（上） */
        int prisonerE;    /* 取り除かれた石の数（右） */
        int prisonerS;    /* 取り除かれた石の数（下） */
        int prisonerW;    /* 取り除かれた石の数（左） */
        int prisonerAll;  /* 取り除かれた石の総数 */

        /* 座標(x,y)に石を置く */
        board[x, z] = color;

        //Stoneを出力
        GameObject stone = Instantiate(whiteStone, new Vector3(x, 0, z), Quaternion.identity);
        //SOUND
        audioSource.PlayOneShot(audioSource.GetComponent<Test>().sound1);


        /* 取り除かれた石の数 */
        prisonerN = prisonerE = prisonerS = prisonerW = 0;

        /* 置いた石の周囲の相手の石が死んでいれば碁盤から取り除く */
        if (z >= 1)
        {
            prisonerN = RemoveStone(color, x, z - 1);
        }
        if (x >= 1)
        {
            prisonerW = RemoveStone(color, x - 1, z);
        }
        if (z <= (WIDTH - 2))
        {
            prisonerS = RemoveStone(color, x, z + 1);
        }
        if (x <= (WIDTH - 2))
        {
            prisonerE = RemoveStone(color, x + 1, z);
        }

        /* 取り除かれた石の総数 */
        prisonerAll = prisonerN + prisonerE + prisonerS + prisonerW;
    }


    /*------------------------------------------------------------------*/
    /* 座標(x,y)の石が死んでいれば碁盤から取り除く                      */
    /*------------------------------------------------------------------*/
    int      /* 碁盤から取り除かれた石数 */
    RemoveStone(COLOR color, int x, int z)
    {
        int prisoner;  /* 取り除かれた石数 */

        /* 置いた石と同じ色なら取らない */
        if (board[x, z] == color)
        {
            return 0;
        }

        /* 空点なら取らない */
        if (board[x, z] == COLOR.EMPTY)
        {
            return 0;
        }

        /* マークのクリア */
        ClearLegalCheckBoard();

        /* 囲まれているなら取る */
        if (DoCheckRemoveStone(board[x, z], x, z))
        {
            prisoner = DoRemoveStone(board[x, z], x, z, 0);
            MakeLine();

            return (prisoner);
        }

        return (0);
    }
    /*------------------------------------------------------------------*/
    /* 座標(x,y)のcolor石を碁盤から取り除き、取った石の数を返す         */
    /*------------------------------------------------------------------*/
    int   /* アゲハマ */
    DoRemoveStone(COLOR color, int x, int z, int prisoner)
    {
        /* 取り除かれる石と同じ色ならば石を取る */
        if (board[x, z] == color)
        {

            /* 取った石の数を１つ増やす */
            prisoner++;

            StorageStone(color, x, z, 0);


            /* その座標に空点を置く */
            board[x, z] = COLOR.EMPTY;
            Debug.DrawRay(new Vector3(x, 1, z), Vector3.down, Color.red, 1f);
            //囲まれた石を消す
            if (Physics.Raycast(new Vector3(x, 1, z), Vector3.down, out hit))
            {
                Destroy(hit.collider.gameObject);
            }

            /* 左を調べる */
            if (x >= 1)
            {
                prisoner = DoRemoveStone(color, x - 1, z, prisoner);
            }
            /* 上を調べる */
            if (z >= 1)
            {
                prisoner = DoRemoveStone(color, x, z - 1, prisoner);
            }
            /* 右を調べる */
            if (x <= (WIDTH - 2))
            {
                prisoner = DoRemoveStone(color, x + 1, z, prisoner);
            }
            /* 下を調べる */
            if (z <= (WIDTH - 2))
            {
                prisoner = DoRemoveStone(color, x, z + 1, prisoner);
            }
        }

        /* 取った石の数を返す */
        return (prisoner);
    }

    //色によって適切なprefabを取得して返す
    int[] GetDirection(int dir)
    {
        int[] arr = new int[2];

        switch (dir)
        {
            case 0:   //左
                arr[0] = -1;
                arr[1] = 0;
                break;
            case 1:   //上
                arr[0] = 0;
                arr[1] = 1;
                break;
            case 2:   //右
                arr[0] = 1;
                arr[1] = 0;
                break;
            case 3:   //下
                arr[0] = 0;
                arr[1] = -1;
                break;
            default:            //それ以外の時(ここに入ることは想定していない)
                arr[0] = 0;
                arr[1] = 0;
                break;
        }
        return arr; //取得したPrefabを返す
    }

    void StorageStone(COLOR color, int x, int z, int d)
    {
        COLOR opponent = OpponentColor(color);

        /* その石の左(x-1,y)を調べる */
        if (x >= 1)
        {
            if (checkBoard[x, z] == false)
            {
                checkBoard[x, z] = true;
                for (int i = 0; i < 4; i++)
                {
                    int X = GetDirection(d)[0];
                    int Z = GetDirection(d)[1];
                    //Debug.Log(X);

                    if (board[x + X, z + Z] == opponent)
                    {

                        //囲んだきた石をリストに格納
                        if (Physics.Raycast(new Vector3(x + X, 1, z + Z), Vector3.down, out hit))
                        {
                            lineStoneList.Add(hit.collider.gameObject);
                            lineStonePosList.Add(hit.transform.position);
                            //Debug.Log(lineStonePosList[lineStonePosList.Count - 1]);
                        }
                    }
                    else if (board[x + X, z + Z] == color)
                    {
                        StorageStone(color, x + X, z + Z, d - 1);
                    }

                    d++;
                    if (d > 3)
                    {
                        d = 0;
                    }
                    if (i == 3)
                    {
                        //チェックしたことを記入
                    }
                }
            }

        }
        /* その石の上(x,y+1)を調べる */
        if (z <= (WIDTH - 2))
        {
            if (DoCheckRemoveStone(color, x, z + 1) == false)
            {
                Debug.Log("上は空");
                return;
            }
        }
        /* その石の右(x+1,y)を調べる */
        if (x <= (WIDTH - 2))
        {
            if (DoCheckRemoveStone(color, x + 1, z) == false)
            {
                Debug.Log("右は空");
                return;
            }
        }
        /* その石の下(x,y-1)を調べる */
        if (z >= 1)
        {
            if (DoCheckRemoveStone(color, x, z - 1) == false)
            {
                Debug.Log("下は空");
                return;
            }
        }
    }

    void MakeLine()
    {
        // ゲームオブジェクトを生成します。
        GameObject beam = Instantiate(finishBeamObj, new Vector3(0,0.5f,0), Quaternion.identity);
        // LineRenderer取得
        LineRenderer line = beam.GetComponent<LineRenderer>();
        //beam.GetComponent<Rigidbody2D>().simulated = true;
        //SOUND
        audioSource.PlayOneShot(audioSource.GetComponent<Test>().sound2);

        // 頂点数の設定
        //line.positionCount = PointList.Count;
        line.positionCount = lineStonePosList.Count;

        
        for (int i = 0; i < lineStonePosList.Count; i++)
        {
            line.SetPosition(i, lineStonePosList[i]);
            int x = Mathf.FloorToInt(lineStoneList[i].transform.position.x);
            int z = Mathf.FloorToInt(lineStoneList[i].transform.position.z);

            //石のラインを消す
            if (lineStoneList[i].gameObject)
            {
                
                    for (int j = 0; j < lineStoneList[i].gameObject.GetComponent<WhiteStoneManager>().lineList.Count; j++)
                    {
                        Destroy(lineStoneList[i].GetComponent<WhiteStoneManager>().lineList[j]);
                    }
                
                // 石を消す
                Destroy(lineStoneList[i]);
                board[x, z] = COLOR.EMPTY;
            }
        }
            beam.AddComponent<PolygonCollider2D>();
        beam.GetComponent<PolygonCollider2D>().isTrigger = true;

        lineStoneList.Clear();
        lineStonePosList.Clear();

        //盤外の石を再セットする
        SetOutStone();

    }

    COLOR OpponentColor(COLOR color)
    {
        if (color == COLOR.BLACK)
        {
            return COLOR.WHITE;
        }
        else
        {
            return COLOR.BLACK;
        }
    }


    void PutBlack(int x, int z)
    {
        //Squaresの値を更新
        board[x, z] = COLOR.BLACK;
        //Stoneを出力
        GameObject stone = Instantiate(blackStone);
        stone.transform.position = new Vector3(x, 0, z);
    }

    public void DestroyStone(int x, int z)
    {
        board[x,z] = GameController.COLOR.EMPTY;
    }


    void MakeBlack()
    {

        IntervalManager();
        if (rndint == 1)
        {

            BlackChecker();
            if (BlackNum < 30)
            {
                //ここで石生成
                int rndx = Random.Range(1, WIDTH - 1); // ※ 1～9の範囲でランダムな整数値が返る
                int rndz = Random.Range(1, WIDTH - 1);

                //Debug.Log(rndx +","+rndy);
                if (board[rndx, rndz] == COLOR.EMPTY && !CheckSuicide(COLOR.BLACK, rndx, rndz))
                {
                    PutBlack(rndx, rndz);
                    //interval=Random.Range(0.4f,2.0f);//石を置く間隔をランダムに指定
                }
                else
                {
                    MakeBlack();
                }
            }
        }

    }
    private void BlackChecker()
    {
        BlackNum = 0;
        //for文を利用して配列にアクセスする
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (board[i, j] == COLOR.BLACK)
                {
                    BlackNum++;
                }
            }
        }
        //Debug.Log(BlackNum);
    }

    void IntervalManager()
    {
        rndint = Random.Range(1, 4);
    }

}