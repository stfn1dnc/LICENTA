using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LogInandRegister : MonoBehaviour
{
    public GameObject Panel_Upload;
    public GameObject Scrollview_Photolist;
    public GameObject Panel_AI;
    public GameObject Panel_Login;

    public TMP_Text Enter_email;
    public TMP_Text Enter_password;
    public TMP_Text Enter_username;
    public TMP_Text statusText;

    public TMP_InputField InputField_email;
    public TMP_InputField InputField_password;
    public TMP_InputField InputField_username;

    public Button Button_submit;
    public Button Button_register;

    public InputActionReference submitAction;
    public InputActionReference registerAction;

    [Header("Firebase")]
    public FirebaseInitializer firebaseInitializer; 

    private string loginURL = "https://arapp-vcpy.onrender.com/api/auth/login";
    private string registerURL = "https://arapp-vcpy.onrender.com/api/auth/register";

    private bool isRegisterMode = false;

    [System.Serializable]
    public class User
    {
        public string email;
        public string password;
        public string username;
    }

    void Start()
    {
        Panel_Login.SetActive(true);
        Panel_Upload.SetActive(false);
        Panel_AI.SetActive(false);
        Scrollview_Photolist.SetActive(false);

        Button_submit.onClick.AddListener(OnLoginSubmitClick);
        Button_register.onClick.AddListener(OnRegisterButtonClick);

        SetRegisterMode(false);
        statusText.text = "";
    }

    void SetRegisterMode(bool active)
    {
        isRegisterMode = active;
        InputField_username.gameObject.SetActive(active);
        Enter_username.gameObject.SetActive(active);

        if (active)
        {
            Button_register.GetComponentInChildren<TMP_Text>().text = "Send Register";
            Button_submit.gameObject.SetActive(false);
        }
        else
        {
            Button_register.GetComponentInChildren<TMP_Text>().text = "Register";
            Button_submit.gameObject.SetActive(true);
            InputField_username.text = "";
        }
    }

    void OnEnable()
    {
        if (submitAction != null)
        {
            submitAction.action.Enable();
            submitAction.action.performed += OnSubmitAction;
        }

        if (registerAction != null)
        {
            registerAction.action.Enable();
            registerAction.action.performed += OnRegisterAction;
        }
    }

    void OnDisable()
    {
        if (submitAction != null)
            submitAction.action.performed -= OnSubmitAction;

        if (registerAction != null)
            registerAction.action.performed -= OnRegisterAction;
    }

    private void OnSubmitAction(InputAction.CallbackContext ctx) => OnLoginSubmitClick();
    private void OnRegisterAction(InputAction.CallbackContext ctx) => OnRegisterButtonClick();

    public void OnRegisterButtonClick()
    {
        if (!isRegisterMode)
        {
            SetRegisterMode(true);
            statusText.text = "Fill in all fields and press Register again to create your account.";
            return;
        }

        string email = InputField_email.text;
        string password = InputField_password.text;
        string username = InputField_username.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            statusText.text = "Please complete all fields!";
            return;
        }

        RegisterUser(email, password, username);
    }

    public void OnLoginSubmitClick()
    {
        if (isRegisterMode)
        {
            SetRegisterMode(false);
            statusText.text = "";
            return;
        }

        string email = InputField_email.text;
        string password = InputField_password.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Please complete email and password!";
            return;
        }

        LoginUser(email, password);
    }

    public void LoginUser(string email, string password)
    {
        StartCoroutine(LoginUserServiceHandler(new User { email = email, password = password }));
    }

    IEnumerator LoginUserServiceHandler(User user)
    {
        string json = JsonUtility.ToJson(user);

        using (UnityWebRequest request = new UnityWebRequest(loginURL, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Login error: " + request.error);
                statusText.text = "Login error! (" + request.responseCode + ")";
            }
            else
            {
                statusText.text = "Login success!";
                Debug.Log("Login response: " + request.downloadHandler.text);

                
                if (firebaseInitializer != null)
                {
                    firebaseInitializer.InitFirebaseAfterLogin(
                        onSuccess: () => {
                            Debug.Log("Firebase initialized after login.");
                            ActivateMainPanels();
                        },
                        onFailure: (error) => {
                            Debug.LogError("Firebase init failed: " + error);
                            ActivateMainPanels(); 
                        }
                    );
                }
                else
                {
                    Debug.LogWarning("FirebaseInitializer not set in Inspector.");
                    ActivateMainPanels(); // fallback
                }
            }
        }
    }

    void ActivateMainPanels()
    {
        Panel_Login.SetActive(false);
        Panel_Upload.SetActive(true);
        Panel_AI.SetActive(true);
        Scrollview_Photolist.SetActive(true);
    }

    public void RegisterUser(string email, string password, string username)
    {
        StartCoroutine(RegisterUserServiceHandler(new User { email = email, password = password, username = username }));
    }

    IEnumerator RegisterUserServiceHandler(User user)
    {
        string json = JsonUtility.ToJson(user);

        using (UnityWebRequest request = new UnityWebRequest(registerURL, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Registration error: " + request.error);
                statusText.text = "Register error! (" + request.responseCode + ")";
            }
            else
            {
                statusText.text = "Registration successful! You can now login.";
                Debug.Log("Registration response: " + request.downloadHandler.text);
                SetRegisterMode(false);
            }
        }
    }
}
