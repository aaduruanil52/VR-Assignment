using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ViSNET.API
{
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    public class APIManager : MonoBehaviour
    {
        public static APIManager Instance { get; private set; }

        [Header("Backend")]
        public string baseUrl = "https://vr-assignment.onrender.com";

        private string _authToken;
        public string AuthToken => _authToken;
        public bool IsLoggedIn => !string.IsNullOrEmpty(_authToken);

        public static bool IsInternetAvailable = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12;

            StartCoroutine(CheckInternet());
        }

        private IEnumerator CheckInternet()
        {
            Debug.Log("[API] Checking internet...");
            Debug.Log("[API] Network Reachability: " + Application.internetReachability);

            using var req = UnityWebRequest.Get("https://vr-assignment.onrender.com");
            req.certificateHandler = new BypassCertificate();
            req.timeout = 10;
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success ||
                req.responseCode > 0)
            {
                IsInternetAvailable = true;
                Debug.Log("[API] Internet CHECK: WORKING ✅ Code: " + req.responseCode);
            }
            else
            {
                IsInternetAvailable = false;
                Debug.LogError("[API] Internet CHECK: FAILED ❌ Error: " + req.error);
                Debug.LogError("[API] Reachability: " + Application.internetReachability);
            }
        }

        public void SetToken(string token) => _authToken = token;
        public void ClearToken() => _authToken = null;

        public IEnumerator PostJson(string endpoint, string jsonBody,
            Action<string> onSuccess, Action<string> onError)
        {
            // Check internet first
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("[API] No internet connection!");
                onError?.Invoke("No internet connection! Please check your Wi-Fi.");
                yield break;
            }

            string url = baseUrl + endpoint;
            Debug.Log("[API] POST URL: " + url);
            Debug.Log("[API] Body: " + jsonBody);

            using var req = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.certificateHandler = new BypassCertificate();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.timeout = 30;

            if (!string.IsNullOrEmpty(_authToken))
                req.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return req.SendWebRequest();

            Debug.Log("[API] Result: " + req.result);
            Debug.Log("[API] Code: " + req.responseCode);
            Debug.Log("[API] Response: " + req.downloadHandler.text);
            Debug.Log("[API] Error: " + req.error);

            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
                onError?.Invoke(req.error);
        }

        public IEnumerator GetJson(string endpoint,
            Action<string> onSuccess, Action<string> onError)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("[API] No internet connection!");
                onError?.Invoke("No internet connection! Please check your Wi-Fi.");
                yield break;
            }

            string url = baseUrl + endpoint;
            Debug.Log("[API] GET URL: " + url);

            using var req = UnityWebRequest.Get(url);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.certificateHandler = new BypassCertificate();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.timeout = 30;

            if (!string.IsNullOrEmpty(_authToken))
                req.SetRequestHeader("Authorization", "Bearer " + _authToken);

            yield return req.SendWebRequest();

            Debug.Log("[API] Result: " + req.result);
            Debug.Log("[API] Response: " + req.downloadHandler.text);

            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
                onError?.Invoke(req.error);
        }
    }
}
