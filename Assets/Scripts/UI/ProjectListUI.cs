using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViSNET.API;
using ViSNET.Managers;
using UnityEngine.EventSystems;
namespace ViSNET.UI
{
    /// <summary>
    /// Attach to the Project Panel.
    ///
    /// Inspector wiring:
    ///   ProjectItemPrefab → Prefab with a Button + TMP_Text child named "Label"
    ///   ContentParent     → Scroll View → Viewport → Content (with VerticalLayoutGroup)
    ///   BackButton        → Button
    ///   LoadingOverlay    → GameObject
    /// </summary>
    public class ProjectListUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject projectItemPrefab;
        public Transform  contentParent;
        public Button     backButton;
        public GameObject loadingOverlay;

        private void Start()
        {
            backButton?.onClick.AddListener(OnBackClicked);
            LoadProjects();
        }

        // ── Data loading ──────────────────────────────────────────────────────

        private void LoadProjects()
        {
            SetLoading(false);

            ProjectAPI.Instance.GetProjects(
                onSuccess: projects =>
                {
                    SetLoading(false);
                    PopulateList(projects);
                },
                onError: err =>
                {
                    SetLoading(false);
                    ToastManager.Instance?.Show("Failed to load projects.");
                    Debug.LogError("[ProjectListUI] " + err);
                }
            );
        }

        // ── List population ───────────────────────────────────────────────────

        private void PopulateList(List<Project> projects)
        {
            // Clear stale items
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            foreach (var project in projects)
            {
                var item = Instantiate(projectItemPrefab, contentParent);

                // Set label text
                var label = item.GetComponentInChildren<TMP_Text>();
                if (label) label.text = project.name;

                // Wire button click (capture by value)
                int   id   = project.id;
                string name = project.name;
                var btn = item.GetComponent<Button>();
                 btn?.onClick.AddListener(() => OnProjectSelected(id, name));

               
                EventTrigger trigger = item.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => OnProjectSelected(id, name));
                trigger.triggers.Add(entry);

                
            }
        }

        // ── Event handlers ────────────────────────────────────────────────────

        private void OnProjectSelected(int projectId, string projectName)
        {
           // SetLoading(true);

            Debug.Log("Button was clicked");
            SessionManager.Instance.SetProject(projectId, projectName);
            ToastManager.Instance?.Show($"Project Selected: {projectName}");
            NavigationManager.Instance.GoToFloors();
        }

        private void OnBackClicked() => NavigationManager.Instance.Back();

        // ── Helpers ───────────────────────────────────────────────────────────

        private void SetLoading(bool loading)
        {
            if (loadingOverlay) loadingOverlay.SetActive(loading);
        }
    }
}
