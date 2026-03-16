using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViSNET.API;
using ViSNET.Managers;

namespace ViSNET.UI
{
    /// <summary>
    /// Attach to the Login Panel.
    ///
    /// Inspector wiring:
    ///   UsernameField  → TMP_InputField (Username)
    ///   PasswordField  → TMP_InputField (Password, ContentType: Password)
    ///   LoginButton    → Button
    ///   ErrorText      → TMP_Text  (initially hidden)
    ///   LoadingOverlay → GameObject (spinner / overlay)
    /// </summary>
    public class LoginUI : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_InputField usernameField;
        public TMP_InputField passwordField;
        public Button         loginButton;
        public TMP_Text       errorText;
        public GameObject     loadingOverlay;

        private void Start()
        {
            if (errorText)        errorText.gameObject.SetActive(false);
            if (loadingOverlay)   loadingOverlay.SetActive(false);

            loginButton?.onClick.AddListener(OnLoginClicked);
        }

        // ── Button handler ────────────────────────────────────────────────────

        private void OnLoginClicked()
        {
            string username = usernameField?.text.Trim() ?? "";
            string password = passwordField?.text ?? "";

           // string username = "testuser";
           // string password = "123456";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter username and password.");
                return;
            }

            SetLoading(true);

            AuthAPI.Instance.Login(username, password,
                onSuccess: resp =>
                {
                    SetLoading(true);
                    SessionManager.Instance.SetUser(resp.user.id, resp.user.name);
                    ToastManager.Instance?.Show("Login Successful");
                    NavigationManager.Instance.GoToProjects();
                },
                onError: msg =>
                {
                    SetLoading(false);
                    ShowError("Invalid credentials. Please try again.");
                    ToastManager.Instance?.Show("Invalid Credentials");
                }
            );
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void ShowError(string msg)
        {
            if (errorText)
            {
                errorText.text = msg;
                errorText.gameObject.SetActive(true);
            }
        }

        private void SetLoading(bool loading)
        {
            loginButton?.gameObject.SetActive(!loading);
            if (loadingOverlay) loadingOverlay.SetActive(loading);
            if (!loading && errorText) errorText.gameObject.SetActive(false);
        }


    }
}
