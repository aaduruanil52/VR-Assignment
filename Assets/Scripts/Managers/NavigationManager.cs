using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
namespace ViSNET.Managers
{
    /// <summary>
    /// Centralises all scene transitions.
    /// Scene names must match exactly what is registered in Build Settings:
    ///   LoginScene | ProjectScene | FloorScene
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager Instance { get; private set; }

        // Scene name constants – adjust if you rename scenes
        private const string LOGIN_SCENE   = "LoginScene";
        private const string PROJECT_SCENE = "ProjectScene";
        private const string FLOOR_SCENE   = "FloorScene";

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── Forward navigation ────────────────────────────────────────────────

        public void GoToLogin()   => LoadScene(LOGIN_SCENE);
        public void GoToProjects() => LoadScene(PROJECT_SCENE);
        public void GoToFloors()   => LoadScene(FLOOR_SCENE);

        // ── Backward navigation ───────────────────────────────────────────────

        public void Back()
        {
            string current = SceneManager.GetActiveScene().name;
            switch (current)
            {
                case FLOOR_SCENE:   GoToProjects(); break;
                case PROJECT_SCENE: GoToLogin();    break;
                default:            Debug.Log("Already at root scene."); break;
            }
        }

        // ── Logout ────────────────────────────────────────────────────────────

        public void Logout()
        {
            SessionManager.Instance?.ClearSession();
            API.AuthAPI.Instance?.Logout();
            GoToLogin();
        }

        // ── Private ───────────────────────────────────────────────────────────

        private static void LoadScene(string sceneName)
        {
            Debug.Log($"[NavigationManager] Loading {sceneName}");
           // SceneManager.LoadScene(sceneName);

            Instance.StartCoroutine(Instance.LoadSceneDelay(sceneName));
        }
        private IEnumerator LoadSceneDelay(string sceneName)
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(sceneName);
        }

    }
}
