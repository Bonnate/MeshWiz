//ObjFromStream.cs

//Runtime OBJ Importer의 에셋 제공자인 Dummiesman의 네임스페이스 사용
using Dummiesman;

using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//List 사용을 위해
using System.Collections.Generic;

public class ObjFromStream : MonoBehaviour
{
    //CONSTS
    //URL 주소
    const string URL_PATH = "http://museum.metabank3dmall.com/objects/";

    //현재 적용하는 텍스쳐 개수
    //[0]       텍스쳐맵
    //[1]       노멀맵
    //[2]       어클루전
    const int APPLY_TEXTURES_CNT = 3;

    //다운로드가 완료되었는지 확인하는 코루틴의 딜레이 초
    WaitForSeconds DELAYTIME = new WaitForSeconds(1f);

    //현재 진행중인 아이템 세트(obj + textures)의 남은 개수
    public static uint mQueueLength = 0;

    //아이템 세트 하나에서 필요한 모든 다운로드가 완료되었는지 확인하기위한 클래스 구조체
    public class QueueChecker
    {
        //씬에 배치될 오브젝트
        public GameObject instantiatedObj;
        //텍스쳐 파일들
        public Texture2D[] textures;
        //텍스쳐 파일이 없어서 스킵하는지 확인
        public bool[] isPassed;
        //오브젝트 name을 서버에서 찾을 수 없는경우 다운로드 작업을 중단하기위해 확인
        public bool isObjectNull;

        //기본 생성자
        public QueueChecker()
        {
            isPassed = new bool[APPLY_TEXTURES_CNT];
            textures = new Texture2D[APPLY_TEXTURES_CNT];
            isObjectNull = false;

            for (int i = 0; i < isPassed.Length; ++i) { isPassed[i] = false; }
        }
    }

    //스트림으로 받아온 오브젝트를 특정 부모의 자식으로 취급하기 위해 사용한다.
    [SerializeField] private Transform[] mParentObj;

    //스트림으로 받아온 오브젝트를 관리하기위한 리스트
    private List<GameObject> mInstantiatedObj;

    private void Start()
    {
        mInstantiatedObj = new List<GameObject>();

        StartDownloadFile("baked_mesh");
    }

    //해당 함수를 통해 다운로드를 시작한다.
    public void StartDownloadFile(string objName, int parentID = -1)
    {
        ++mQueueLength;

        StartCoroutine(StartDownloadQueue(objName, parentID));
    }

    //다운로드를 시작한다.
    private IEnumerator StartDownloadQueue(string objName, int parentID)
    {
        //새로 다운로드를 시작한다.
        //queueData에서 다운로드 확인에 필요한 데이터들을 체크한다.
        QueueChecker queueData = new QueueChecker();

        //오브젝트 다운로드를 시작한다.
        StartCoroutine(DownloadObj(objName, queueData));

        //텍스쳐 다운로드를 시작한다.
        for (int i = 0; i < APPLY_TEXTURES_CNT; ++i)
        {
            StartCoroutine(DownloadTexture(objName, queueData, i));
        }

        int iterator;
        while (true)
        {
            if (queueData.isObjectNull)
            {
                Debug.Log("서버에서 " + objName + "을 찾을 수 없음");

                --mQueueLength;
                yield return DELAYTIME;
                yield break;
            }

            //오브젝트 파일을 다운로드 받았는지 확인한다.
            if (queueData.instantiatedObj == null)
            {
                Debug.Log("obj 다운로드증..");

                yield return DELAYTIME;
                continue;
            }

            //텍스쳐 파일을 다운로드 받았는지 확인한다.
            for (iterator = 0; iterator < APPLY_TEXTURES_CNT; ++iterator)
            {
                if (queueData.isPassed[iterator]) continue;

                if (queueData.textures[iterator] == null)
                {
                    Debug.Log(iterator + "번째 이미지 다운로드중..");

                    yield return DELAYTIME;
                    break;
                }
            }

            if (iterator != APPLY_TEXTURES_CNT)
            {
                continue;
            }

            //여기에서 오브젝트를 활성화시키고, 텍스쳐를 입힌다.
            {
                //URP Lit Standard 쉐이더 코드 참조
                //https://github.com/Unity-Technologies/MeasuredMaterialLibraryURP/blob/master/Assets/Measured%20Materials%20Library/ClearCoat/Shaders/Lit.shader

                //SetFloat 함수 (메탈릭 스무스 값)
                //https://forum.unity.com/threads/set-smoothness-of-material-in-script.381247/

                Debug.Log("다운로드 모두 완료");

                MeshRenderer renderer = queueData.instantiatedObj.GetComponent<MeshRenderer>();

                //(가능)텍스쳐 변경 
                renderer.material.mainTexture = queueData.textures[0];

                //(가능)노멀맵 변경 
                renderer.material.SetTexture("_BumpMap", queueData.textures[1]);

                //(가능)어클루전맵 변경
                renderer.material.SetTexture("_OcclusionMap", queueData.textures[2]);

                //메탈릭 스무스 값 설정 (0~1)
                renderer.material.SetFloat("_Smoothness", 0f);

                //리스트에 해당 오브젝트를 관리하기위해 레퍼런스를 넣는다.
                mInstantiatedObj.Add(queueData.instantiatedObj);

                //만약 parentID가 -1이 아니라면 (의도하여 부모에게 넣으라고 했다면..) 해당 부모 오브젝트의 자식으로 처리한다.
                if (parentID != -1)
                {
                    queueData.instantiatedObj.transform.parent = mParentObj[parentID];
                }

                //오브젝트를 활성화 시킨다.
                queueData.instantiatedObj.AddComponent<StreamActivator>();
                queueData.instantiatedObj.SetActive(true);
                --mQueueLength;
                yield break;
            }
        }
    }

    //obj 파일을 다운로드 받는다. Dummiesman의 스크립트를 사용한다.
    private IEnumerator DownloadObj(string objName, QueueChecker queueData)
    {
        string finalURL = URL_PATH + objName + ".obj";

        using (UnityWebRequest www = UnityWebRequest.Get(finalURL))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                queueData.isObjectNull = true;
            }
            else
            {
                var textStream = new MemoryStream(www.downloadHandler.data);

                queueData.instantiatedObj = new OBJLoader().Load(textStream);
                queueData.instantiatedObj.name = objName;
            }
        }
    }

    //texture을 다운받아 QueueChecker에 넣는다.
    private IEnumerator DownloadTexture(string objName, QueueChecker queueData, int imageID)
    {
        Debug.Log("텍스쳐 이미지 다운로드 시작");

        string finalURL = URL_PATH + objName;

        switch (imageID)
        {
            case 0:
                {
                    finalURL += "_tex0.png";
                    break;
                }

            case 1:
                {
                    finalURL += "_norm0.png";
                    break;
                }

            case 2:
                {
                    finalURL += "_ao0.png";
                    break;
                }
        }

        using (UnityWebRequest www = UnityWebRequest.Get(finalURL))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                queueData.isPassed[imageID] = true;
            }
            else
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(www.downloadHandler.data);
                queueData.textures[imageID] = tex;
            }
        }
    }

    public void DownloadHeart()
    {
        StartDownloadFile("red_heart");
    }

    public void Download100()
    {
        StartCoroutine(StartDownload());
    }

    IEnumerator StartDownload()
    {
        for (int i = 0; i < 100; ++i)
        {
            StartDownloadFile("red_heart");

            yield return null;
        }
    }
}
