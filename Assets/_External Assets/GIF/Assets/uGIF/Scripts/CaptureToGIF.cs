using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

using System.Threading;
using TMPro;

namespace uGIF
{
    // 420 300
    public class CaptureToGIF : Singleton<CaptureToGIF>
    {
        private List<Image> frames = new List<Image>();
        private byte[] bytes = null;

        [SerializeField] private TMP_InputField mFrameRateField;

        public void ExportAsGIF(List<Texture2D> textures, string exportFolderPath)
        {
            frames.Clear();

            foreach (Texture2D texture in textures)
                frames.Add(new Image(texture));

            Debug.Log(frames.Count);

            System.Threading.Tasks.Task.Run(() =>
             Encode(exportFolderPath, $"rotate_{System.DateTime.Now:yyyyMMddHHmmssms}")
            );
        }

        public void Encode(string exportFolderPath, string fileName, int downscale = 1)
        {
            var ge = new GIFEncoder();
            ge.useGlobalColorTable = true;
            ge.repeat = 0;
            ge.FPS = float.Parse(mFrameRateField.text);
            ge.transparent = new Color32(0, 0, 0, 0);
            ge.dispose = -1;

            var stream = new MemoryStream();
            ge.Start(stream);
            foreach (var f in frames)
            {
                // f.Resize(downscale);

                // if (downscale != 1)
                // {
                //     if (useBilinearScaling)
                //     {
                //         f.ResizeBilinear(1920, 1080);
                //     }
                //     else
                //     {
                //         Debug.Log("B");
                //         f.Resize(downscale);
                //     }
                // }

                f.Flip();
                ge.AddFrame(f);
            }
            ge.Finish();
            bytes = stream.GetBuffer();
            stream.Close();

            Debug.Log("Export!");

            System.IO.File.WriteAllBytes($"{exportFolderPath}/{fileName}.gif", bytes);
            bytes = null;
        }
    }
}