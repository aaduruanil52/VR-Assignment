using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace ViSNET.API
{
    // ── Response models ───────────────────────────────────────────────────────

    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class LoginUser
    {
        public int    id;
        public string name;
    }

    [Serializable]
    public class LoginResponse
    {
        public bool      success;
        public string    token;
        public LoginUser user;
        public string    message;
    }

    // ── API wrapper ───────────────────────────────────────────────────────────
    public class AuthAPI : MonoBehaviour
    {
        public static AuthAPI Instance { get; private set; }

       // [Header("Debug Text (optional - for device testing)")]
       // public TMP_Text user;
      //  public TMP_Text pass;
       // public TMP_Text bodyA;
      //  public TMP_Text api;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Login(string username, string password,
            Action<LoginResponse> onSuccess,
            Action<string> onError)
        {
            var body = JsonUtility.ToJson(new LoginRequest
            {
                username = username,
                password = password
            });

            // Debug logs
            Debug.Log("[AUTH] Username: " + username);
            Debug.Log("[AUTH] Password: " + password);
            Debug.Log("[AUTH] Body: " + body);
            Debug.Log("[AUTH] URL: " + APIManager.Instance.baseUrl + "/api/login");

            // Show on screen for device testing
           // SetDebugText(user, "1 User: " + username);
           // SetDebugText(pass, "2 Pass: " + password);
          //  SetDebugText(bodyA, "3 Body: " + body);
          //  SetDebugText(api, "4 URL: " + APIManager.Instance.baseUrl + "/api/login");

            StartCoroutine(APIManager.Instance.PostJson("/api/login", body,
                raw =>
                {
                    Debug.Log("[AUTH] Raw Response: " + raw);
                   // SetDebugText(bodyA, "5 RAW: " + raw);

                    try
                    {
                        var resp = JsonUtility.FromJson<LoginResponse>(raw);

                        if (resp == null)
                        {
                            string nullErr = "Response parsed as NULL!";
                            Debug.LogError("[AUTH] " + nullErr);
                         //   SetDebugText(api, "ERR: " + nullErr);
                            onError?.Invoke(nullErr);
                            return;
                        }

                        if (resp.success)
                        {
                            Debug.Log("[AUTH] Login Success! User: " + resp.user?.name);
                         //  SetDebugText(api, "SUCCESS! User: " + resp.user?.name);
                            APIManager.Instance.SetToken(resp.token);
                            onSuccess?.Invoke(resp);
                        }
                        else
                        {
                            string failMsg = resp.message ?? "Invalid credentials";
                            Debug.Log("[AUTH] Login Failed: " + failMsg);
                          // SetDebugText(api, "FAILED: " + failMsg);
                            onError?.Invoke(failMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        string parseErr = "Parse Error: " + ex.Message;
                        Debug.LogError("[AUTH] " + parseErr);
                      //  SetDebugText(api, parseErr);
                        onError?.Invoke(parseErr);
                    }
                },
                err =>
                {
                    Debug.LogError("[AUTH] Network Error: " + err);
                  // SetDebugText(api, "NET ERR: " + err);
                    onError?.Invoke(err);
                }
            ));
        }

        public void Logout()
        {
            APIManager.Instance.ClearToken();
        }

        // Safe helper - wont crash if text object is null
        private void SetDebugText(TMP_Text textObj, string message)
        {
            if (textObj != null)
                textObj.text = message;
        }
    }
}
