using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

using System.Threading;

namespace uGIF
{
    public class CaptureToGIF : MonoBehaviour
    {
		[SerializeField] private List<Texture2D> textures;

        public float frameRate = 15;
        public bool capture;
        public int downscale = 1;
        public float captureTime = 10;
        public bool useBilinearScaling = true;

        [System.NonSerialized]
        public byte[] bytes = null;

		private void Start() {
			Encode();
		}

        public void Encode()
        {
            bytes = null;
			_Encode();
            StartCoroutine(WaitForBytes());
        }

        IEnumerator WaitForBytes()
        {
            while (bytes == null) yield return null;
            System.IO.File.WriteAllBytes("/Users/METABANK/Desktop/test.gif", bytes);
            bytes = null;
        }

        public void _Encode()
        {
			frames.Clear();

			foreach(Texture2D texture in textures)
				frames.Add(new Image(texture));

            capture = false;

            var ge = new GIFEncoder();
            ge.useGlobalColorTable = true;
            ge.repeat = 0;
            ge.FPS = frameRate;
            ge.transparent = new Color32(255, 0, 255, 255);
            ge.dispose = 1;

            var stream = new MemoryStream();
            ge.Start(stream);
            foreach (var f in frames)
            {
                if (downscale != 1)
                {
                    if (useBilinearScaling)
                    {
                        f.ResizeBilinear(f.width / downscale, f.height / downscale);
                    }
                    else
                    {
                        f.Resize(downscale);
                    }
                }
                f.Flip();
                ge.AddFrame(f);
            }
            ge.Finish();
            bytes = stream.GetBuffer();
            stream.Close();
        }

        List<Image> frames = new List<Image>();
        Texture2D colorBuffer;
        float period;
        float T = 0;
        float startTime = 0;

    }
}