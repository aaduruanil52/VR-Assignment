using UnityEngine;

namespace ViSNET.Managers
{
    /// <summary>
    /// Persists user session data across scene loads.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }

        // ── Session data ─────────────────────────────────────────────────────
        public int    UserId       { get; private set; }
        public string UserName     { get; private set; }
        public int    ProjectId    { get; private set; }
        public string ProjectName  { get; private set; }
        public string SelectedFloor { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetUser(int id, string name)
        {
            UserId   = id;
            UserName = name;
        }

        public void SetProject(int id, string name)
        {
            ProjectId   = id;
            ProjectName = name;
        }

        public void SetFloor(string floor) => SelectedFloor = floor;

        public void ClearSession()
        {
            UserId        = 0;
            UserName      = null;
            ProjectId     = 0;
            ProjectName   = null;
            SelectedFloor = null;
        }
    }
}
