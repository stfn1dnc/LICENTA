using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using System;
using System.Collections;

public class FirebaseInitializer : MonoBehaviour
{
    public static bool isFirebaseReady = false;
    public static DependencyStatus FirebaseDependencyStatus = DependencyStatus.UnavailableOther;
    public static FirebaseUser currentUser;

    private static bool isCheckingDependencies = false; 
    public void InitFirebaseAfterLogin(Action onSuccess = null, Action<string> onFailure = null)
    {
        if (isCheckingDependencies) return;
        isCheckingDependencies = true;
            

        Debug.Log("[Firebase] Starting dependency check...");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            isCheckingDependencies = false;
            FirebaseDependencyStatus = task.Result;
            if (FirebaseDependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("[Firebase] Dependencies available. Attempting anonymous login...");
                SignInAnonymously(onSuccess, onFailure);
            }
            else
            {
                Debug.LogError("Firebase dependencies not resolved: " + FirebaseDependencyStatus);
                isFirebaseReady = false;
                onFailure?.Invoke("Firebase dependencies not met: " + FirebaseDependencyStatus);
            }
        });
    }

    private void SignInAnonymously(Action onSuccess = null, Action<string> onFailure = null)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("[FirebaseAuth] Anonymous sign-in failed: " + task.Exception);
                isFirebaseReady = false;
                onFailure?.Invoke("Anonymous Firebase sign-in failed.");
            }
            else if (task.IsCompletedSuccessfully)
            {
                currentUser = task.Result.User;
                Debug.Log("[FirebaseAuth] Anonymous login successful. UID: " + currentUser.UserId);
                isFirebaseReady = true;
                onSuccess?.Invoke();
            }
        });
    }

   
    public IEnumerator WaitForFirebaseReady(float timeout = 10f)
    {
        float elapsed = 0f;
        while (!isFirebaseReady && elapsed < timeout)
        {
            Debug.Log($"[Firebase] Waiting for full init... {elapsed:0.0}s");
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isFirebaseReady)
        {
            Debug.Log("[Firebase] Fully initialized and authenticated (anonymous).");
        }
        else
        {
            Debug.LogError("[Firebase] Init failed or timed out.");
        }
    }
}
