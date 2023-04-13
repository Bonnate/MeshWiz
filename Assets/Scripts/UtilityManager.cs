using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class UtilityManager : MonoBehaviour
{
    /// <summary>
    /// 자식 레이어 전체 레이어를 변경
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="newLayer"></param>
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    /// <summary>
    /// 대상 오브젝트를 피벗의 관계 없이 destination의 위치로 중간에 위치시킴
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="destination"></param>
    public static void SetPositionViaCenter(Transform targetTransform, Vector3 destination)
    {
        BoxCollider collider = targetTransform.GetComponent<BoxCollider>();
        if (collider != null)
            Destroy(collider);
        collider = targetTransform.gameObject.AddComponent<BoxCollider>();

        targetTransform.transform.position = destination;

        targetTransform.Translate(targetTransform.position - targetTransform.TransformPoint(collider.center), Space.World);

        Destroy(collider);
    }

    /// <summary>
    /// meshFilter의 mesh를 string으로 작성
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <returns></returns>
    public static string MeshToObjString(MeshFilter meshFilter)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("g mesh"); // Object group name

        // Transform matrix with rotation and scale
        Matrix4x4 transformMatrix = Matrix4x4.TRS(
            meshFilter.transform.position,
            meshFilter.transform.rotation,
            meshFilter.transform.lossyScale
        );

        // Apply transform to vertices
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transformMatrix.MultiplyPoint(vertices[i]);
        }

        foreach (Vector3 v in vertices)
        {
            sb.AppendFormat("v {0} {1} {2}\n", -v.x, v.y, v.z); // Vertex position
        }

        foreach (Vector3 n in meshFilter.mesh.normals)
        {
            Vector3 transformedNormal = transformMatrix.MultiplyVector(n);
            sb.AppendFormat("vn {0} {1} {2}\n", -transformedNormal.x, transformedNormal.y, transformedNormal.z); // Vertex normal
        }

        foreach (Vector2 uv in meshFilter.mesh.uv)
        {
            sb.AppendFormat("vt {0} {1}\n", uv.x, uv.y); // Texture coordinates
        }

        for (int i = 0; i < meshFilter.mesh.subMeshCount; i++)
        {
            int[] triangles = meshFilter.mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                sb.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1); // Face indices
            }
        }

        return sb.ToString();
    }


    public static float GetLongestVertexLengthWithScale(Mesh mesh, Transform transform, int sampleCount = 1000)
    {
        Vector3 pos1 = Vector3.zero, pos2 = Vector3.zero;

        float longestLength = 0f;

        Vector3[] vertices = mesh.vertices;
        int vertexCount = vertices.Length;

        if (vertexCount < 2) return longestLength;

        int step = vertexCount / sampleCount;
        if (step < 1) step = 1;

        Matrix4x4 matrix = transform.localToWorldMatrix;

        for (int i = 0; i < vertexCount; i += step)
        {
            for (int j = i + step; j < vertexCount; j += step)
            {
                Vector3 p1 = matrix.MultiplyPoint3x4(vertices[i]);
                Vector3 p2 = matrix.MultiplyPoint3x4(vertices[j]);
                float distance = Vector3.Distance(p1, p2);
                if (distance > longestLength)
                {
                    pos1 = p1;
                    pos2 = p2;

                    longestLength = distance;
                }
            }
        }

        return longestLength;
    }

    private static List<GameObject> _DrawLineInstantiatedGos = new List<GameObject>();

    /// <summary>
    /// 두 점 사이를 렌더러로 그림
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="scale"></param>
    public static void DrawLine(Vector3 start, Vector3 end, float width, float duration = 5.0f)
    {
        for(int i = _DrawLineInstantiatedGos.Count - 1; i >= 0; --i)
        {
            Destroy(_DrawLineInstantiatedGos[i]);
            _DrawLineInstantiatedGos.RemoveAt(i);
        }

        GameObject lineRendererPrefab = Resources.Load<GameObject>("LineRenderer(Length)");
        GameObject spherePrefab = Resources.Load<GameObject>("Sphere(Length)");
        if (lineRendererPrefab == null)
        {
            Debug.LogError("LineRenderer prefab not found in the Resources folder.");
            return;
        }
        if (spherePrefab == null)
        {
            Debug.LogError("Sphere prefab not found in the Resources folder.");
            return;
        }

        GameObject lineRendererObj = GameObject.Instantiate(lineRendererPrefab);
        _DrawLineInstantiatedGos.Add(lineRendererObj);

        lineRendererObj.transform.SetParent(MeshController.Instance.CurrentGo.transform.parent);
        lineRendererObj.transform.localPosition = Vector3.zero;

        LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();
        lineRenderer.endWidth = lineRenderer.startWidth = width / 100.0f;
        lineRenderer.SetPositions(new Vector3[] { start, end });

        GameObject leftArrow = GameObject.Instantiate(spherePrefab);
        _DrawLineInstantiatedGos.Add(leftArrow);
        leftArrow.transform.SetParent(lineRendererObj.transform);
        leftArrow.transform.position = start;
        leftArrow.transform.localScale = Vector3.one * width / 50.0f;
        leftArrow.transform.SetParent(MeshController.Instance.CurrentGo.transform.parent);
        leftArrow.transform.eulerAngles = -Camera.main.transform.right;
        GameObject.Destroy(leftArrow, duration);

        GameObject rightArrow = GameObject.Instantiate(spherePrefab);

        GameObject.Destroy(lineRendererObj, duration);
    }

    /// <summary>
    /// Bounds를 기준으로 그 모습을 렌더러로 그림
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    /// <param name="width"></param>
    public static void DrawBounds(Bounds bounds, float width, float duration = 5.0f)
    {
        for(int i = _DrawBoundsInstantiatedGos.Count - 1; i >= 0; --i)
        {
            Destroy(_DrawBoundsInstantiatedGos[i]);
            _DrawBoundsInstantiatedGos.RemoveAt(i);
        }

        GameObject lineRendererPrefab = Resources.Load<GameObject>("LineRenderer(Bounding)");
        GameObject spherePrefab = Resources.Load<GameObject>("Sphere(Bounding)");
        if (lineRendererPrefab == null)
        {
            Debug.LogError("LineRenderer prefab not found in the Resources folder.");
            return;
        }
        if (spherePrefab == null)
        {
            Debug.LogError("Sphere prefab not found in the Resources folder.");
            return;
        }

        GameObject lineRendererObj = GameObject.Instantiate(lineRendererPrefab);
        _DrawBoundsInstantiatedGos.Add(lineRendererObj);
        lineRendererObj.transform.SetParent(MeshController.Instance.CurrentGo.transform.parent);

        Vector3[] corners = new Vector3[8];
        corners[0] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
        corners[1] = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
        corners[2] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
        corners[3] = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
        corners[4] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
        corners[5] = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
        corners[6] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        corners[7] = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);

        for (int i = 0; i < corners.Length; i++)
        {
            GameObject sphereObj = GameObject.Instantiate(spherePrefab);
            _DrawBoundsInstantiatedGos.Add(sphereObj);
            sphereObj.transform.SetParent(lineRendererObj.transform);
            sphereObj.transform.position = corners[i];
            sphereObj.transform.localScale = Vector3.one * width / 50.0f;
            sphereObj.transform.SetParent(MeshController.Instance.CurrentGo.transform.parent);
            GameObject.Destroy(sphereObj, duration);
        }

        LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();
        lineRenderer.endWidth = lineRenderer.startWidth = width / 200.0f;
        lineRenderer.positionCount = 16;

        lineRenderer.SetPosition(0, corners[0]);
        lineRenderer.SetPosition(1, corners[1]);
        lineRenderer.SetPosition(2, corners[2]);
        lineRenderer.SetPosition(3, corners[3]);
        lineRenderer.SetPosition(4, corners[0]);
        lineRenderer.SetPosition(5, corners[4]);
        lineRenderer.SetPosition(6, corners[5]);
        lineRenderer.SetPosition(7, corners[1]);
        lineRenderer.SetPosition(8, corners[5]);
        lineRenderer.SetPosition(9, corners[6]);
        lineRenderer.SetPosition(10, corners[2]);
        lineRenderer.SetPosition(11, corners[6]);
        lineRenderer.SetPosition(12, corners[7]);
        lineRenderer.SetPosition(13, corners[3]);
        lineRenderer.SetPosition(14, corners[7]);
        lineRenderer.SetPosition(15, corners[4]);

        GameObject.Destroy(lineRendererObj, duration);        
    }

    /// <summary>
    /// Bounding Box를 현재 모습으로 재생성
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <returns></returns>
    public static Bounds RecalculateBoundingBox(MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.sharedMesh;

        // 원본 mesh의 vertices와 triangles를 복사
        Vector3[] originalVertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // 현재 transform의 rotation, scale 값을 적용하여 vertices를 재생성
        Vector3[] vertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = meshFilter.transform.TransformPoint(originalVertices[i]);
        }

        // mesh의 vertices를 재설정
        mesh.vertices = vertices;
        mesh.RecalculateBounds();

        // bounding box를 계산
        Bounds bounds = mesh.bounds;

        // 원본 mesh의 vertices와 triangles를 복원
        mesh.vertices = originalVertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        return bounds;
    }

    /// <summary>
    /// Bounding Box에서 가장 긴 축을 리턴
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static (Vector3, Vector3) GetLongestAxisVectors(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3 xVector = new Vector3(extents.x, 0f, 0f);
        Vector3 yVector = new Vector3(0f, extents.y, 0f);
        Vector3 zVector = new Vector3(0f, 0f, extents.z);

        Vector3 xMaxPoint = center + xVector;
        Vector3 xMinPoint = center - xVector;

        Vector3 yMaxPoint = center + yVector;
        Vector3 yMinPoint = center - yVector;

        Vector3 zMaxPoint = center + zVector;
        Vector3 zMinPoint = center - zVector;

        float xLength = Vector3.Distance(xMaxPoint, xMinPoint);
        float yLength = Vector3.Distance(yMaxPoint, yMinPoint);
        float zLength = Vector3.Distance(zMaxPoint, zMinPoint);

        if (xLength >= yLength && xLength >= zLength)
        {
            return (xMaxPoint, xMinPoint);
        }
        else if (yLength >= xLength && yLength >= zLength)
        {
            return (yMaxPoint, yMinPoint);
        }
        else
        {
            return (zMaxPoint, zMinPoint);
        }
    }
}
