using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uOSC;


public class OSCController : MonoBehaviour
{
    // Start is called before the first frame update

    Camera cam;

    //ParticleSystem�^��ϐ�ps�Ő錾���܂��B
    public GameObject ps;
    //GameObject�^�ŕϐ�obj��錾���܂��B
    GameObject obj;
    //�}�E�X�ŃN���b�N���ꂽ�ʒu���i�[����܂��B
    private Vector3 mousePosition;
    private Vector3 sensorPosition;

    public GameController gameController;

    void Start()
    {
        var server = GetComponent<uOscServer>();
        server.onDataReceived.AddListener(OnDataReceived);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDataReceived(Message message)
    {
        //        Debug.Log(message.ToString());

        if (message.address == "/pos")
        {
            /*
            if (float.Parse(message.values[0].ToString()) > 1.5 || float.Parse(message.values[0].ToString()) < -0.7
                || float.Parse(message.values[1].ToString()) > 2.6 || float.Parse(message.values[1].ToString()) < 0.2)
            {
                return;
            }
            */
            float X = float.Parse(message.values[0].ToString());
            float Z = float.Parse(message.values[1].ToString());
            //Debug.Log(X);

            float a, b, c, d, e, f, g, h;
            a = -7.09321087f;
            b = -1.22964594f;
            c = 9.10761932f;
            d = -1.19379422f;
            e = 6.96728368f;
            f = -4.22980069f;
            g = -0.00586784749f;
            h = 0.00566685508f;

            float HomoX = a * X + b * Z + c;
            float HomoZ = d * X + e * Z + f;
            float HomoY = g * X + h * Z + 1;

            //Debug.Log(HomoY);


            X = HomoX / HomoY;
            Z = HomoZ / HomoY;


            //ここでunityの座標に変換されているのが理想(xz座標)

            //Debug.Log(X + ", " + Z);
            //�}�E�X�J�[�\���̈ʒu���擾�B
            sensorPosition = new Vector3(X, 0, Z);
            //Debug.Log(sensorPosition);
            //Instantiate(ps, Camera.main.ScreenToWorldPoint(sensorPosition),
            //    Quaternion.identity);
            //nstantiate(ps, sensorPosition, Quaternion.identity);

            //PosCheck(sensorPosition, X, Z);
            gameController.CheckPosition(X, Z);
        }
        //var msg = message.address + ": ";

        //// arguments (object array)
        //foreach (var value in message.values)
        //{
        //    if (value is int) msg += (int)value;
        //    else if (value is float) msg += (float)value;
        //    else if (value is string) msg += (string)value;
        //    else if (value is bool) msg += (bool)value;
        //    else if (value is byte[]) msg += "byte[" + ((byte[])value).Length + "]";
        //}

        //Debug.Log(msg);
    }

    public void PosCheck(Vector3 Pos, float X, float Z)
    {
        GameObject clickedGameObject = null;

        Vector3 vec3 = new Vector3(0, 2.86f, -10);
        Vector3 dir = new Vector3(X, -0.245f, Z);


        // �������΂�
        //Ray ray = Camera.main.ScreenPointToRay(Pos);
        Ray ray = new Ray(vec3, dir);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f, false);
        RaycastHit hit = new RaycastHit();

        // ���������ɓ���������
        if (Physics.Raycast(ray, out hit))
        {
            // �I�u�W�F�N�g���擾����
            clickedGameObject = hit.collider.gameObject;
            Debug.Log("switch clicked");
        }
        else
        {
            return;
        }

        // �X�C�b�`�������珈�����s���B
        if (clickedGameObject.layer == 5)
        {
            clickedGameObject.GetComponent<Image>().enabled = false;
            Debug.Log(clickedGameObject.layer);
        }
    }
}
