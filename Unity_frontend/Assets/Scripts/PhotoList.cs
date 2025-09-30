using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Storage;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PhotoList : MonoBehaviour
{
    [SerializeField] private GameObject photoItemPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text statusText;

    private Queue<Texture2D> recentPhotos = new Queue<Texture2D>();
    private int maxPhotos = 4;

    private IEnumerator Start()
    {
        // Așteaptă Firebase
        yield return new WaitUntil(() => FirebaseInitializer.isFirebaseReady);

        if (photoItemPrefab == null || contentParent == null || statusText == null)
        {
            Debug.LogError("[PhotoList] Referințe lipsă în Inspector.");
            yield break;
        }

        yield return LoadPhotosForCurrentUser();
    }

    public void AddPhoto(Texture2D newPhoto)
    {
        if (newPhoto == null)
        {
            Debug.LogWarning("Poza e null, nu poate fi adăugată.");
            return;
        }

        if (recentPhotos.Count >= maxPhotos)
        {
            recentPhotos.Dequeue();

            if (contentParent.childCount > 0)
            {
                Destroy(contentParent.GetChild(0).gameObject);
            }
        }

        recentPhotos.Enqueue(newPhoto);

        GameObject photoGo = Instantiate(photoItemPrefab, contentParent);

        var thumbnail = photoGo.transform.Find("Thumbnail");
        if (thumbnail != null)
        {
            var img = thumbnail.GetComponent<RawImage>();
            if (img != null)
            {
                img.texture = newPhoto;
            }
            else
            {
                Debug.LogError("[PhotoList] Thumbnail fără RawImage.");
            }
        }
        else
        {
            Debug.LogError("[PhotoList] Thumbnail nu a fost găsit în prefab.");
        }
    }

    private IEnumerator LoadPhotosForCurrentUser()
    {
        statusText.text = "Photos are loading...";

        string currentUserId = UserSession.CurrentUserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            statusText.text = "You are not logged in!";
            yield break;
        }

        var firestore = FirebaseFirestore.DefaultInstance;
        var query = firestore.Collection("photos").WhereEqualTo("userId", currentUserId);
        var getTask = query.GetSnapshotAsync();
        yield return new WaitUntil(() => getTask.IsCompleted);

        if (getTask.Exception != null)
        {
            Debug.LogError("[PhotoList] Firestore query failed: " + getTask.Exception);
            statusText.text = "Error loading photos!";
            yield break;
        }

        foreach (var doc in getTask.Result.Documents)
        {
            string storagePath = doc.GetValue<string>("storagePath");
            string equipName = doc.GetValue<string>("equipmentName");
            string notes = doc.ContainsField("notes") ? doc.GetValue<string>("notes") : "";

            var storageRef = FirebaseStorage.DefaultInstance.GetReference(storagePath);
            var urlTask = storageRef.GetDownloadUrlAsync();
            yield return new WaitUntil(() => urlTask.IsCompleted);

            if (urlTask.Exception != null)
            {
                Debug.LogError("[PhotoList] Failed to get image URL: " + urlTask.Exception);
                continue;
            }

            string imageUrl = urlTask.Result.ToString();

            GameObject item = Instantiate(photoItemPrefab, contentParent);

            var nameText = item.transform.Find("EquipmentName")?.GetComponent<TMP_Text>();
            if (nameText != null) nameText.text = equipName;

            var notesText = item.transform.Find("Notes")?.GetComponent<TMP_Text>();
            if (notesText != null) notesText.text = notes;

            var img = item.transform.Find("Thumbnail")?.GetComponent<RawImage>();
            if (img != null)
            {
                UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl);
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    img.texture = DownloadHandlerTexture.GetContent(uwr);
                }
                else
                {
                    Debug.LogError("[PhotoList] Failed to load texture: " + uwr.error);
                }
            }
            else
            {
                Debug.LogWarning("[PhotoList] Thumbnail lipsă în instanță.");
            }
        }

        statusText.text = "Done!";
    }
}
