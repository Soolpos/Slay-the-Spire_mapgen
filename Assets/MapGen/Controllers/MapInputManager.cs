using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInputManager : MonoBehaviour
{
    public GameObject CameraObj;
    float CameraMoveK = 2.5f;
    float CameraScrollK = 160;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Управление камерой
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 CameraPos = CameraObj.transform.position;
            CameraPos.y += CameraMoveK;
            CameraObj.transform.position = CameraPos;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Vector3 CameraPos = CameraObj.transform.position;
            CameraPos.x += -1 * CameraMoveK;
            CameraObj.transform.position = CameraPos;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 CameraPos = CameraObj.transform.position;
            CameraPos.y += -1 * CameraMoveK;
            CameraObj.transform.position = CameraPos;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 CameraPos = CameraObj.transform.position;
            CameraPos.x += CameraMoveK;
            CameraObj.transform.position = CameraPos;
        }

        float mw = Input.GetAxis("Mouse ScrollWheel");

        if (mw != 0)
        {
            Camera.main.orthographicSize += -1 * mw * CameraScrollK;
        }
    }
}