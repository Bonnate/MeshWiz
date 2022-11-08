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

public class OBJExportManager : MonoBehaviour
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

    public void Export()
    {
        //MeshToFile(GetComponent<MeshFilter>(), "Resources", "B");

        //업로드하기
        //StartCoroutine(Upload());

        //다운로드 하기
        WebGLFileSaver.SaveFile(MeshToString(GetComponent<MeshFilter>()), "baked_mesh.obj");
        MeshToFile(GetComponent<MeshFilter>(), "A", "B");
    }

    public void ExportToLocal()
    {
        //MeshToFile(GetComponent<MeshFilter>(), "Resources", "B");

        //업로드하기
        //StartCoroutine(Upload());

        //다운로드 하기
        string str = MeshToString(GetComponent<MeshFilter>());
        FileStream stream = new FileStream(FileBrowserRuntime.Instance.mCurrentPath + "_resized.obj", FileMode.OpenOrCreate);
        stream.Write(System.Text.Encoding.UTF8.GetBytes(str));
        stream.Close();
    }    

    IEnumerator Upload()
    {
        byte[] myData = System.Text.Encoding.UTF8.GetBytes(MeshToString(GetComponent<MeshFilter>()));
        UnityWebRequest www = UnityWebRequest.Put("http://museum.metabank3dmall.com/realmeta3d/0.01/baked_mesh.obj" + System.DateTime.Now, myData);

        Debug.Log(www.GetRequestHeader("Access-Control-Allow-Origin"));

        www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
        www.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
        www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        www.SetRequestHeader("Access-Control-Allow-Origin", "*");

        Debug.Log(www.GetRequestHeader("Access-Control-Allow-Origin"));

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Upload complete!");
        }
    }

    string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList)
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
            sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
        }
        sb.Append("\n");

        foreach (Vector3 lv in m.normals)
        {
            Vector3 wv = mf.transform.TransformDirection(lv);

            sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
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

    void MeshToFile(MeshFilter mf, string folder, string filename)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
        {
            sw.Write("mtllib ./" + filename + ".mtl\n");

            sw.Write(MeshToString(mf, materialList));
        }
    }

    private string MeshToString(MeshFilter mf)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

        StringBuilder str = new StringBuilder();

        str.Append("mtllib ./" + "tempHere" + ".mtl\n");
        str.Append(MeshToString(mf, materialList));

        return str.ToString();
    }

    void MeshesToFile(MeshFilter[] mf, string folder, string filename)
    {
        Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

        using (StreamWriter sw = new StreamWriter(folder + "/" + filename + ".obj"))
        {
            sw.Write("mtllib ./" + filename + ".mtl\n");

            for (int i = 0; i < mf.Length; i++)
            {
                sw.Write(MeshToString(mf[i], materialList));
            }
        }
    }

    // bool CreateTargetFolder()
    // {
    //     try
    //     {
    //         System.IO.Directory.CreateDirectory(targetFolder);
    //     }
    //     catch
    //     {
    //         EditorUtility.DisplayDialog("Error!", "Failed to create target folder!", "");
    //         return false;
    //     }

    //     return true;
    // }


    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetButtonDown("Fire3"))
    //     {

    //         if (!CreateTargetFolder())
    //             return;

    //         Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);

    //         if (selection.Length == 0)
    //         {
    //             EditorUtility.DisplayDialog("No source object selected!", "Please select one or more target objects", "");
    //             return;
    //         }

    //         int exportedObjects = 0;

    //         ArrayList mfList = new ArrayList();

    //         for (int i = 0; i < selection.Length; i++)
    //         {
    //             Component[] meshfilter = selection[i].GetComponentsInChildren(typeof(MeshFilter));

    //             for (int m = 0; m < meshfilter.Length; m++)
    //             {
    //                 exportedObjects++;
    //                 mfList.Add(meshfilter[m]);
    //             }
    //         }

    //         if (exportedObjects > 0)
    //         {
    //             MeshFilter[] mf = new MeshFilter[mfList.Count];

    //             for (int i = 0; i < mfList.Count; i++)
    //             {
    //                 mf[i] = (MeshFilter)mfList[i];
    //             }

    //             string filename = EditorApplication.currentScene + "_" + exportedObjects;

    //             int stripIndex = filename.LastIndexOf('/');//FIXME: Should be Path.PathSeparator

    //             if (stripIndex >= 0)
    //                 filename = filename.Substring(stripIndex + 1).Trim();

    //             MeshesToFile(mf, targetFolder, filename);


    //             EditorUtility.DisplayDialog("Objects exported", "Exported " + exportedObjects + " objects to " + filename, "");
    //         }
    //         else
    //             EditorUtility.DisplayDialog("Objects not exported", "Make sure at least some of your selected objects have mesh filters!", "");
    //     }
    // }
}

