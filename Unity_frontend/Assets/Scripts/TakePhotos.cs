using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.InputSystem;
using System.Collections;

public class TakePhotos : MonoBehaviour
{
    public RawImage previewImage;
    public TMP_Text statusText;
    public PhotoList photoList;
    public InputActionReference takePhotoAction;

    private string lastPhotoPath;
    private bool cameraActive = false;
    private bool allowInputTrigger = false;

    private void OnEnable()
    {
        if (takePhotoAction != null)
        {
            takePhotoAction.action.Enable();
            takePhotoAction.action.performed += OnTakePhotoTriggered;
        }
    }

    private void OnDisable()
    {
        if (takePhotoAction != null)
        {
            takePhotoAction.action.performed -= OnTakePhotoTriggered;
            takePhotoAction.action.Disable();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && cameraActive)
        {
            Debug.LogWarning("[TakePhotos] Application paused while camera active. Resetting state.");
            cameraActive = false;
        }
    }

    private void OnTakePhotoTriggered(InputAction.CallbackContext ctx)
    {
        if (allowInputTrigger && !cameraActive)
        {
            TakePhoto();
        }
    }

    public void EnablePhotoCapture()
    {
        allowInputTrigger = true;
    }

    public void OnTakePictureButtonClick()
    {
        if (!cameraActive)
        {
            TakePhoto();
        }
    }

    public string GetLastPhotoPath()
    {
        return lastPhotoPath;
    }

    public void TakePhoto()
    {
        if (cameraActive)
            return;

        StartCoroutine(RequestCameraAndTakePhoto());
    }

    private IEnumerator RequestCameraAndTakePhoto()
    {
        // Verifică permisiunea
        if (!NativeCamera.CheckPermission(true))
        {
            bool finished = false;

            NativeCamera.RequestPermissionAsync((permission) =>
            {
                finished = true;

                if (permission != NativeCamera.Permission.Granted)
                {
                    Debug.Log("[TakePhotos] Camera permission denied.");
                    if (statusText != null)
                        statusText.text = "Camera permission not granted!";
                }
            }, true);

            while (!finished)
                yield return null;

            if (!NativeCamera.CheckPermission(true))
                yield break;
        }

        // ✅ Activează camera doar când e sigur
        cameraActive = true;

        NativeCamera.TakePicture((path) =>
        {
            cameraActive = false;

            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log("[TakePhotos] Photo saved at: " + path);
                lastPhotoPath = path;

                StartCoroutine(LoadImageAndAddToList(path));

                if (statusText != null)
                    statusText.text = "Photo saved at " + path;
            }
            else
            {
                Debug.Log("[TakePhotos] No photo taken.");
                if (statusText != null)
                    statusText.text = "No photo has been made!";
            }
        }, maxSize: 2048);
    }

    private IEnumerator LoadImageAndAddToList(string path)
    {
        string url = "file://" + path;

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                if (texture != null)
                {
                    if (previewImage != null)
                    {
                        previewImage.texture = texture;
                        previewImage.gameObject.SetActive(true);
                    }

                    if (photoList != null)
                    {
                        photoList.AddPhoto(texture);
                    }
                }
                else
                {
                    Debug.LogError("[TakePhotos] Loaded texture is null.");
                    if (statusText != null)
                        statusText.text = "Error: Loaded texture is null.";
                }
            }
            else
            {
                Debug.LogError("[TakePhotos] UnityWebRequest Error: " + uwr.error);
                if (statusText != null)
                    statusText.text = "Error loading image: " + uwr.error;
            }
        }
    }
}
