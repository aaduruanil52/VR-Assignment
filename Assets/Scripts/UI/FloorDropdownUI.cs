using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViSNET.API;
using ViSNET.Managers;

namespace ViSNET.UI
{
    /// <summary>
    /// Attach to the Floor Panel.
    ///
    /// Inspector wiring:
    ///   FloorItemPrefab   → Prefab with Button + TMP_Text "Label" + Image "Highlight"
    ///   ContentParent     → Scroll View → Viewport → Content (VerticalLayoutGroup)
    ///   ProjectNameText   → TMP_Text showing the current project name
    ///   BackButton        → Button
    ///   LoadingOverlay    → GameObject
    ///
    /// Color constants at the top can be adjusted to match Meta XR palette.
    /// </summary>
    public class FloorDropdownUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject floorItemPrefab;
        public Transform  contentParent;
        public TMP_Text   projectNameText;
        public Button     backButton;
        public GameObject loadingOverlay;

        [Header("Selection Colors")]
        public Color normalColor   = new Color(0.18f, 0.18f, 0.18f, 0.85f);
        public Color selectedColor = new Color(0.10f, 0.55f, 0.95f, 1.00f);

        // Track which item is currently selected
        private GameObject _selectedItem;

        private void Start()
        {
            backButton?.onClick.AddListener(OnBackClicked);

            // Show which project we're viewing floors for
            if (projectNameText && SessionManager.Instance != null)
                projectNameText.text = SessionManager.Instance.ProjectName;

            LoadFloors();
        }

        // ── Data loading ──────────────────────────────────────────────────────

        private void LoadFloors()
        {
            if (SessionManager.Instance == null || SessionManager.Instance.ProjectId == 0)
            {
                ToastManager.Instance?.Show("No project selected.");
                NavigationManager.Instance.Back();
                return;
            }

            SetLoading(true);

            ProjectAPI.Instance.GetFloors(
                SessionManager.Instance.ProjectId,
                onSuccess: floors =>
                {
                    SetLoading(false);
                    PopulateDropdown(floors);
                },
                onError: err =>
                {
                    SetLoading(false);
                    ToastManager.Instance?.Show("Failed to load floors.");
                    Debug.LogError("[FloorDropdownUI] " + err);
                }
            );
        }

        // ── Dropdown population ───────────────────────────────────────────────

        private void PopulateDropdown(List<string> floors)
        {
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            _selectedItem = null;

            foreach (var floorName in floors)
            {
                var item  = Instantiate(floorItemPrefab, contentParent);
                var label = item.GetComponentInChildren<TMP_Text>();
                if (label) label.text = floorName;

                // Default background
                SetItemColor(item, normalColor);

                string capturedName = floorName;
                var btn = item.GetComponent<Button>();
                btn?.onClick.AddListener(() => OnFloorSelected(capturedName, item));
            }
        }

        // ── Event handlers ────────────────────────────────────────────────────

        private void OnFloorSelected(string floorName, GameObject item)
        {
            // Deselect previous
            if (_selectedItem != null)
                SetItemColor(_selectedItem, normalColor);

            // Highlight new selection
            _selectedItem = item;
            SetItemColor(item, selectedColor);

            // Persist and notify
            SessionManager.Instance.SetFloor(floorName);
            ToastManager.Instance?.Show($"Selected Floor: {floorName}");
        }

        private void OnBackClicked() => NavigationManager.Instance.Back();

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void SetItemColor(GameObject item, Color color)
        {
            var img = item.GetComponent<Image>();
            if (img) img.color = color;
        }

        private void SetLoading(bool loading)
        {
            if (loadingOverlay) loadingOverlay.SetActive(loading);
        }
    }
}
