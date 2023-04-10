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
    //자식 레이어 전체 레이어를 변경하는 함수
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

    //비활성화된 자식 컴포넌트를 찾음
    static public TType GetComponentInChildren<TType>(GameObject objRoot) where TType : Component
    {
        // if we don't find the component in this object 
        // recursively iterate children until we do
        TType tRetComponent = objRoot.GetComponent<TType>();

        if (null == tRetComponent)
        {
            // transform is what makes the hierarchy of GameObjects, so 
            // need to access it to iterate children
            Transform trnsRoot = objRoot.transform;
            int iNumChildren = trnsRoot.childCount;

            // could have used foreach(), but it causes GC churn
            for (int iChild = 0; iChild < iNumChildren; ++iChild)
            {
                // recursive call to this function for each child
                // break out of the loop and return as soon as we find 
                // a component of the specified type
                tRetComponent = GetComponentInChildren<TType>(trnsRoot.GetChild(iChild).gameObject);
                if (null != tRetComponent)
                {
                    break;
                }
            }
        }

        return tRetComponent;
    }

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

    public static GameObject GetPivotCenter(GameObject targetGo)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        // 자식 오브젝트들의 MeshFilter 컴포넌트 검사
        foreach (MeshFilter mf in targetGo.GetComponentsInChildren<MeshFilter>())
        {
            // 메쉬 정보를 가져와서 메쉬의 중심점 계산
            Vector3 meshCenter = mf.mesh.bounds.center;

            // 로컬 좌표계 상의 중심점을 전역 좌표계 상의 좌표로 변환
            meshCenter = mf.transform.TransformPoint(meshCenter);

            // 중심점 누적값 계산
            center += meshCenter;
            ++count;
        }

        if (count > 0)
        {
            // 자식 오브젝트들의 중심점 평균값 반환
            center /= count;
        }

        // 피벗 중심의 위치에 회전을 위한 부모 오브젝트 생성
        GameObject rotateAxis = new GameObject($"Rotate Axis for {targetGo.name}");
        rotateAxis.transform.position = center;

        // 회전 축을 부모로 지정
        targetGo.transform.SetParent(rotateAxis.transform);

        // 회전축 오브젝트를 리턴
        return rotateAxis;
    }

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

    public static float GetLongestVertexLengthWithScale(Mesh mesh, Transform transform, bool isDrawLine, int sampleCount = 1000)
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

        if(isDrawLine)
            DrawLine(pos1, pos2, longestLength);

        return longestLength;
    }

    public static void DrawLine(Vector3 start, Vector3 end, float scale)
    {
        // Load LineRenderer resource from the Resources folder
        GameObject lineRendererPrefab = Resources.Load<GameObject>("LineRenderer");
        if (lineRendererPrefab == null)
        {
            Debug.LogError("LineRenderer prefab not found in the Resources folder.");
            return;
        }

        // Instantiate LineRenderer and set its positions
        GameObject lineRendererObj = GameObject.Instantiate(lineRendererPrefab);

        lineRendererObj.transform.SetParent(MeshController.Instance.CurrentGo.transform);
        lineRendererObj.transform.localPosition = Vector3.zero;

        LineRenderer lineRenderer = lineRendererObj.GetComponent<LineRenderer>();
        lineRenderer.endWidth = lineRenderer.startWidth = scale / 50.0f;
        lineRenderer.SetPositions(new Vector3[] { start, end });

        // Destroy LineRenderer after 3 seconds
        GameObject.Destroy(lineRendererObj, 3f);
    }
}
