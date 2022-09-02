﻿/*
 * Copyright (c) 2019 Dummiesman
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
*/

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
#endif


namespace Dummiesman
{
    public enum SplitMode {
        None,
        Object,
        Material
    }
    
    public class OBJLoader
    {
        public SplitMode SplitMode = SplitMode.Object;

        //global lists, accessed by objobjectbuilder
        internal List<Vector3> Vertices = new List<Vector3>();
        internal List<Vector3> Normals = new List<Vector3>();
        internal List<Vector2> UVs = new List<Vector2>();

        //materials, accessed by objobjectbuilder
        internal Dictionary<string, Material> Materials;

        //file info for files loaded from file path, used for GameObject naming and MTL finding
        private FileInfo _objInfo;

        public GameObject Load(Stream input)
        {
            var reader = new StreamReader(input);
            //var reader = new StringReader(inputReader.ReadToEnd());

            Dictionary<string, OBJObjectBuilder> builderDict = new Dictionary<string, OBJObjectBuilder>();
            OBJObjectBuilder currentBuilder = null;
            string currentMaterial = "default";

            //lists for face data
            //prevents excess GC
            List<int> vertexIndices = new List<int>();
            List<int> normalIndices = new List<int>();
            List<int> uvIndices = new List<int>();

            //helper func
            Action<string> setCurrentObjectFunc = (string objectName) =>
            {
                if (!builderDict.TryGetValue(objectName, out currentBuilder))
                {
                    currentBuilder = new OBJObjectBuilder(objectName, this);
                    builderDict[objectName] = currentBuilder;
                }
            };

            //create default object
            setCurrentObjectFunc.Invoke("default");

			//var buffer = new DoubleBuffer(reader, 256 * 1024);
			var buffer = new CharWordReader(reader, 4 * 1024);

			//do the reading
			while (true)
            {
				buffer.SkipWhitespaces();

				if (buffer.endReached == true) {
					break;
				}

				buffer.ReadUntilWhiteSpace();
				
                //comment or blank
                if (buffer.Is("#"))
                {
					buffer.SkipUntilNewLine();
                    continue;
                }
				
				if (buffer.Is("v")) {
					Vertices.Add(buffer.ReadVector());
					continue;
				}

				//normal
				if (buffer.Is("vn")) {
                    Normals.Add(buffer.ReadVector());
                    continue;
                }

                //uv
				if (buffer.Is("vt")) {
                    UVs.Add(buffer.ReadVector());
                    continue;
                }

                //new material
				if (buffer.Is("usemtl")) {
					buffer.SkipWhitespaces();
					buffer.ReadUntilNewLine();
					string materialName = buffer.GetString();
                    currentMaterial = materialName;

                    if(SplitMode == SplitMode.Material)
                    {
                        setCurrentObjectFunc.Invoke(materialName);
                    }
                    continue;
                }

                //new object
                if ((buffer.Is("o") || buffer.Is("g")) && SplitMode == SplitMode.Object) {
                    buffer.ReadUntilNewLine();
                    string objectName = buffer.GetString(1);
                    setCurrentObjectFunc.Invoke(objectName);
                    continue;
                }

                //face data (the fun part)
                if (buffer.Is("f"))
                {
                    //loop through indices
                    while (true)
                    {
						bool newLinePassed;
						buffer.SkipWhitespaces(out newLinePassed);
						if (newLinePassed == true) {
							break;
						}

                        int vertexIndex = int.MinValue;
                        int normalIndex = int.MinValue;
                        int uvIndex = int.MinValue;

						vertexIndex = buffer.ReadInt();
						if (buffer.currentChar == '/') {
							buffer.MoveNext();
							if (buffer.currentChar != '/') {
								uvIndex = buffer.ReadInt();
							}
							if (buffer.currentChar == '/') {
								buffer.MoveNext();
								normalIndex = buffer.ReadInt();
							}
						}

                        //"postprocess" indices
                        if (vertexIndex > int.MinValue)
                        {
                            if (vertexIndex < 0)
                                vertexIndex = Vertices.Count - vertexIndex;
                            vertexIndex--;
                        }
                        if (normalIndex > int.MinValue)
                        {
                            if (normalIndex < 0)
                                normalIndex = Normals.Count - normalIndex;
                            normalIndex--;
                        }
                        if (uvIndex > int.MinValue)
                        {
                            if (uvIndex < 0)
                                uvIndex = UVs.Count - uvIndex;
                            uvIndex--;
                        }

                        //set array values
                        vertexIndices.Add(vertexIndex);
                        normalIndices.Add(normalIndex);
                        uvIndices.Add(uvIndex);
                    }

                    //push to builder
                    currentBuilder.PushFace(currentMaterial, vertexIndices, normalIndices, uvIndices);

                    //clear lists
                    vertexIndices.Clear();
                    normalIndices.Clear();
                    uvIndices.Clear();

					continue;
                }

				buffer.SkipUntilNewLine();
            }


            //수정 전 오리진
            // //finally, put it all together
            // GameObject obj = new GameObject(_objInfo != null ? Path.GetFileNameWithoutExtension(_objInfo.Name) : "WavefrontObject");
            // obj.transform.localScale = new Vector3(-1f, 1f, 1f);

            // foreach (var builder in builderDict)
            // {
            //     //empty object
            //     if (builder.Value.PushedFaceCount == 0)
            //         continue;

            //     var builtObj = builder.Value.Build();
            //     builtObj.transform.SetParent(obj.transform, false);
            // }

            // return obj


            //수정, 오브젝트 하나만 리턴한다고 가정한다.
            foreach (var builder in builderDict)
            {
                //empty object
                if (builder.Value.PushedFaceCount == 0)
                    continue;

                GameObject obj = builder.Value.Build();
                obj.SetActive(false);
                return obj;
            }

            return null;
        }
    }
}