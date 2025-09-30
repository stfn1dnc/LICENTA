using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections;
using System.IO;
using Firebase.Firestore;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AiChat : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;
    public Button sendBtn;
    public Button saveBtn;

    [Header("API Settings")]
    [SerializeField] private string apiKey = "";
    public string gptModel = "text-davinci-003";
    public int tokenLimit = 150;
    public float creativity = 0.7f;

    private void Start()
    {
        if (sendBtn != null) sendBtn.onClick.AddListener(SendClicked);
        if (saveBtn != null) saveBtn.onClick.AddListener(SaveClicked);

        StartCoroutine(LoadApiKeyFromFile());
    }

    private IEnumerator LoadApiKeyFromFile()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "openai_config.json");
        string json = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load API key on Android: " + www.error);
            outputText.text = "Missing or unreadable API key file.";
            yield break;
        }

        json = www.downloadHandler.text;
#else
        if (!File.Exists(path))
        {
            Debug.LogError("API key file not found at: " + path);
            outputText.text = "Missing API key file.";
            yield break;
        }

        json = File.ReadAllText(path);
#endif

        ApiKeyData data = JsonConvert.DeserializeObject<ApiKeyData>(json);
        apiKey = data.openai;

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API key in file is empty.");
            outputText.text = "API key in file is empty!";
        }
        else
        {
            Debug.Log("API key loaded successfully.");
        }
    }

    public void SendClicked()
    {
        if (inputField == null || outputText == null || string.IsNullOrWhiteSpace(inputField.text))
        {
            outputText.text = "Input is empty.";
            return;
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            outputText.text = "No API key available.";
            return;
        }

        StartCoroutine(CallOpenAI(inputField.text));
    }

    private IEnumerator CallOpenAI(string prompt)
    {
        outputText.text = "Thinking...";
        sendBtn.interactable = false;

        var payload = new
        {
            model = gptModel,
            prompt = prompt,
            max_tokens = tokenLimit,
            temperature = creativity
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);
        UnityWebRequest req = new UnityWebRequest("https://api.openai.com/v1/completions", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return req.SendWebRequest();
        sendBtn.interactable = true;

        if (req.responseCode == 401)
        {
            outputText.text = "Invalid API key.";
            yield break;
        }

        if (req.result != UnityWebRequest.Result.Success)
        {
            outputText.text = "Error: " + req.error;
            yield break;
        }

        var response = JsonConvert.DeserializeObject<OpenAIResponse>(req.downloadHandler.text);
        if (response != null && response.choices.Length > 0)
        {
            outputText.text = response.choices[0].text.Trim();
        }
        else
        {
            outputText.text = "No response.";
        }
    }

    public void SaveClicked()
    {
        if (!FirebaseInitializer.isFirebaseReady)
        {
            Debug.LogWarning("Firebase is not ready yet.");
            outputText.text = "Can't save. Firebase not ready.";
            return;
        }

        if (string.IsNullOrEmpty(UserSession.CurrentUserId))
        {
            Debug.LogWarning("User ID is missing.");
            outputText.text = "Can't save. User not logged in.";
            return;
        }

        string text = outputText?.text;
        if (string.IsNullOrWhiteSpace(text)) return;

        var data = new Dictionary<string, object>
        {
            { "userId", UserSession.CurrentUserId },
            { "prompt", inputField.text },
            { "response", text },
            { "timestamp", Timestamp.GetCurrentTimestamp() },
            { "model", gptModel }
        };

        FirebaseFirestore.DefaultInstance
            .Collection("ai_chats")
            .AddAsync(data)
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Save error: " + task.Exception);
                    outputText.text = "Failed to save to Firebase.";
                }
                else
                {
                    Debug.Log("Saved AI response to Firebase.");
                    outputText.text += "\n\nSaved to Firebase.";
                }
            });
    }
}

[Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

[Serializable]
public class Choice
{
    public string text;
}

[Serializable]
public class ApiKeyData
{
    public string openai;
}
