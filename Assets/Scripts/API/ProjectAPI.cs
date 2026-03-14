using System;
using System.Collections.Generic;
using UnityEngine;

namespace ViSNET.API
{
    // ── Response models ────────────────────────────────────────────────────────

    [Serializable]
    public class Project
    {
        public int    id;
        public string name;
    }

    [Serializable]
    public class ProjectListResponse
    {
        public List<Project> projects;
    }

    [Serializable]
    public class FloorsResponse
    {
        public List<string> floors;
    }

    // ── API wrapper ────────────────────────────────────────────────────────────

    /// <summary>
    /// Provides GetProjects and GetFloors calls.
    /// Attach to the same persistent "API Manager" GameObject as APIManager.
    /// </summary>
    public class ProjectAPI : MonoBehaviour
    {
        public static ProjectAPI Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>Fetches all projects from the backend.</summary>
        public void GetProjects(Action<List<Project>> onSuccess, Action<string> onError)
        {
            StartCoroutine(APIManager.Instance.GetJson("/api/projects",
                raw =>
                {
                    var resp = JsonUtility.FromJson<ProjectListResponse>(raw);
                    onSuccess?.Invoke(resp.projects);
                },
                err => onError?.Invoke(err)
            ));
        }

        /// <summary>Fetches floors for a specific project ID.</summary>
        public void GetFloors(int projectId, Action<List<string>> onSuccess, Action<string> onError)
        {
            StartCoroutine(APIManager.Instance.GetJson($"/api/projects/{projectId}/floors",
                raw =>
                {
                    var resp = JsonUtility.FromJson<FloorsResponse>(raw);
                    onSuccess?.Invoke(resp.floors);
                },
                err => onError?.Invoke(err)
            ));
        }
    }
}
