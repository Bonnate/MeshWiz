using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System.Linq;

public class OBJExportManager : Singleton<OBJExportManager>
{
    struct ObjMaterial
    {
        public string name;
        public string textureName;
    }

    public int vertexOffset = 0;
    public int normalOffset = 0;
    public int uvOffset = 0;
    public string targetFolder = "ExportedObj";

    public void ExportToLocal()
    {
        //로컬에 다운로드
        // string str = MeshToString(GetComponent<MeshFilter>());
        string str = UtilityManager.MeshToObjString(GetComponent<MeshFilter>());
        FileStream stream = new FileStream(FileBrowserRuntime.Instance.mCurrentPath.Replace(".obj", "") + "_resized.obj", FileMode.OpenOrCreate);
        stream.Write(System.Text.Encoding.UTF8.GetBytes(str));
        stream.Close();
    }    

    private string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList)
    {
        Mesh m = mf.mesh;
        Material[] mats = mf.GetComponent<MeshRenderer>().materials;

        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (Vector3 lv in m.vertices)
        {
            Vector3 wv = mf.transform.TransformPoint(lv);

            //This is sort of ugly - inverting x-component since we're in
            //a different coordinate system than "everyone" is "used to".
            sb.Append(string.Format("v {0} {1} {2}\n", wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 lv in m.normals)
        {
            Vector3 wv = mf.transform.TransformDirection(lv);

            sb.Append(string.Format("vn {0} {1} {2}\n", wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }

        for (int material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            //See if this material is already in the materiallist.
            try
            {
                ObjMaterial objMaterial = new ObjMaterial();

                objMaterial.name = mats[material].name;

                if (mats[material].mainTexture)
                {
                    // objMaterial.textureName = EditorUtility.GetAssetPath(mats[material].mainTexture);
                    objMaterial.textureName = "A";
                }
                else
                {
                    objMaterial.textureName = null;
                }

                materialList.Add(objMaterial.name, objMaterial);
            }
            catch (ArgumentException)
            {
                //Already in the dictionary
            }


            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                //Because we inverted the x-component, we also needed to alter the triangle winding.
                sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                                       triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
            }
        }

        vertexOffset += m.vertices.Length;
        normalOffset += m.normals.Length;
        uvOffset += m.uv.Length;

        return sb.ToString();
    }

    void Clear()
    {
        vertexOffset = 0;
        normalOffset = 0;
        uvOffset = 0;
    }

    Dictionary<string, ObjMaterial> PrepareFileWrite()
    {
        Clear();

        return new Dictionary<string, ObjMaterial>();
    }

    public string MeshToString(MeshFilter mf)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

        StringBuilder str = new StringBuilder();

        // str.Append("mtllib ./" + "tempHere" + ".mtl\n");
        str.Append(MeshToString(mf, materialList));

        return str.ToString();
    }
}

