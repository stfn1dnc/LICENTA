using UnityEngine;
using Firebase.Storage;
using Firebase.Firestore;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PhotoUpload : MonoBehaviour
{
    public TMP_InputField equipmentNameInput;
    public TMP_InputField notesInput;
    public TMP_Text statusText;
    public TakePhotos takePhotosReference;

    public void OnUploadPictureButtonClick()
    {
        if (!FirebaseInitializer.isFirebaseReady)
        {
            Debug.LogWarning("Firebase is not ready.");
            statusText.text = "Firebase not ready!";
            return;
        }

        if (equipmentNameInput == null || notesInput == null || takePhotosReference == null)
        {
            Debug.LogError("PhotoUpload: UI references not assigned.");
            statusText.text = "UI not linked!";
            return;
        }

        string equipmentName = equipmentNameInput.text.Trim();
        string notes = notesInput.text.Trim();
        string userId = UserSession.CurrentUserId;
        string photoPath = takePhotosReference.GetLastPhotoPath();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("UserSession.CurrentUserId is null or empty.");
            statusText.text = "User not logged in!";
            return;
        }

        if (string.IsNullOrEmpty(photoPath) || string.IsNullOrEmpty(equipmentName))
        {
            statusText.text = "Missing required fields!";
            return;
        }

        StartCoroutine(UploadImageCoroutine(photoPath, equipmentName, notes, userId));
    }

    private IEnumerator UploadImageCoroutine(string localFilePath, string equipmentName, string notes, string userId)
    {
        statusText.text = "Uploading...";

        if (!File.Exists(localFilePath))
        {
            Debug.LogError("Photo not found at path: " + localFilePath);
            statusText.text = "Photo not found!";
            yield break;
        }

        byte[] fileData = File.ReadAllBytes(localFilePath);
        string fileName = "photos/" + System.Guid.NewGuid() + ".jpg";
        var storageRef = FirebaseStorage.DefaultInstance.GetReference(fileName);

        var uploadTask = storageRef.PutBytesAsync(fileData);
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.IsFaulted || uploadTask.IsCanceled)
        {
            Debug.LogError("Firebase Storage upload failed.");
            statusText.text = "Upload failed!";
            yield break;
        }

        var getUrlTask = storageRef.GetDownloadUrlAsync();
        yield return new WaitUntil(() => getUrlTask.IsCompleted);

        if (getUrlTask.IsFaulted || getUrlTask.IsCanceled)
        {
            Debug.LogError("Failed to get download URL.");
            statusText.text = "Failed to get download URL!";
            yield break;
        }

        string downloadUrl = getUrlTask.Result.ToString();

        var firestore = FirebaseFirestore.DefaultInstance;
        var doc = new Dictionary<string, object>
        {
            { "photoUrl", downloadUrl },
            { "equipmentName", equipmentName },
            { "notes", notes },
            { "storagePath", fileName },
            { "userId", userId },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        };

        var writeTask = firestore.Collection("photos").AddAsync(doc);
        yield return new WaitUntil(() => writeTask.IsCompleted);

        if (writeTask.IsFaulted)
        {
            Debug.LogError("Failed to write document to Firestore.");
            statusText.text = "Failed to save Firestore!";
        }
        else
        {
            Debug.Log("Photo uploaded and saved in Firestore.");
            statusText.text = "Photo uploaded!";
        }
    }
}
