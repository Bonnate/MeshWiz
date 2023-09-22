using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using System;
using Dummiesman;
using System.Text;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ImageLoadController : MonoBehaviour
{
    public static Texture2D? _TEXTURE_IMAGE { private set; get; } = null;
    public static Texture2D? _NORMAL_IMAGE { private set; get; } = null;
    public static Texture2D? _AO_IMAGE { private set; get; } = null;

    private Button mButton;

    [SerializeField] GameObject mRemoveBtnGo;
    private Sprite mButtonBaseSprite;

    private void Awake()
    {
        mButton = GetComponent<Button>();
        mButtonBaseSprite = GetComponent<Image>().sprite;
    }

    IEnumerator CoLoadImageDialog(int imageType)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        FileBrowserRuntime.IsDialogEnabled = false;

        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(filePath);

            string fileExtension = Path.GetExtension(filePath).ToLower();

            if (IsImageFile(fileExtension))
            {
                // 이미지를 텍스처로 변환합니다.
                Texture2D texture = new Texture2D(2, 2); // 기본 크기는 임시값입니다.
                texture.LoadImage(bytes);

                // Texture2D를 Sprite로 변환합니다.
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                mButton.GetComponent<Image>().sprite = sprite;

                switch (imageType)
                {
                    case 0:
                        _TEXTURE_IMAGE = texture;
                        break;
                    case 1:
                        _NORMAL_IMAGE = texture;
                        break;
                    case 2:
                        _AO_IMAGE = texture;
                        break;
                }

                MeshController.Instance.RefreshMaterial();

                mRemoveBtnGo.SetActive(true);
            }
            else
            {
                DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Selected file is not supported!", "OK");
            }
        }
    }

    private bool IsImageFile(string fileExtension)
    {
        // 이미지 파일 확장자 목록을 정의합니다.
        string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };

        // 대소문자 구분 없이 검사하기 위해 확장자를 소문자로 변환합니다.
        fileExtension = fileExtension.ToLower();

        // 이미지 확장자 목록에 포함되어 있는지 확인합니다.
        return imageExtensions.Contains(fileExtension);
    }

    public void BTN_LoadImage(int imageType)
    {
        StartCoroutine(CoLoadImageDialog(imageType));
    }

    public void BTN_RemoveImage(int imageType)
    {
        switch (imageType)
        {
            case 0:
                _TEXTURE_IMAGE = null;
                break;
            case 1:
                _NORMAL_IMAGE = null;
                break;
            case 2:
                _AO_IMAGE = null;
                break;
        }

        MeshController.Instance.RefreshMaterial();       
        GetComponent<Image>().sprite = mButtonBaseSprite;
        mRemoveBtnGo.SetActive(false); 
    }
}
