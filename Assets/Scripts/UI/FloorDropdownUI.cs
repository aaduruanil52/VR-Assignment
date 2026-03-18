using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ViSNET.API;
using ViSNET.Managers;

namespace ViSNET.UI
{
    public class FloorDropdownUI : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Dropdown floorDropdown;
        public TMP_Text selectedFloorText;
        public TMP_Text projectNameText;
        public Button backButton;
        public GameObject loadingOverlay;

        [Header("Dropdown Size Settings")]
        public float itemHeight = 80f;
        public float maxListHeight = 400f;
        public float itemSpacing = 5f;

        [Header("Colors")]
        public Color normalColor = new Color(0.18f, 0.18f, 0.18f, 0.85f);
        public Color selectedColor = new Color(0.10f, 0.55f, 0.95f, 1.00f);

        private List<string> _floors = new List<string>();
        private bool _isSelecting = false; // debounce flag

        private void Start()
        {
            backButton?.onClick.AddListener(OnBackClicked);

            if (projectNameText && SessionManager.Instance != null)
                projectNameText.text = SessionManager.Instance.ProjectName;

            floorDropdown?.onValueChanged.AddListener(OnDropdownChanged);

            if (floorDropdown != null)
            {
                EventTrigger trigger = floorDropdown.gameObject.GetComponent<EventTrigger>()
                                    ?? floorDropdown.gameObject.AddComponent<EventTrigger>();

                // PointerDown — works with finger poke
                EventTrigger.Entry pokeEntry = new EventTrigger.Entry();
                pokeEntry.eventID = EventTriggerType.PointerDown;
                pokeEntry.callback.AddListener((data) => OnDropdownClicked());
                trigger.triggers.Add(pokeEntry);

                // PointerClick — works with ray + pinch
                EventTrigger.Entry clickEntry = new EventTrigger.Entry();
                clickEntry.eventID = EventTriggerType.PointerClick;
                clickEntry.callback.AddListener((data) => OnDropdownClicked());
                trigger.triggers.Add(clickEntry);
            }

            LoadFloors();
        }

        // ── Dropdown Clicked ──────────────────────────────────────────────────

        private void OnDropdownClicked()
        {
            Debug.Log("[FloorDropdownUI] Dropdown clicked");
            StartCoroutine(FixDropdownHeight());
        }

        // ── Fix Dropdown Height + Add PointerDown to Items ────────────────────

        private IEnumerator FixDropdownHeight()
        {
            yield return new WaitForEndOfFrame();

            Transform dropdownList = floorDropdown.transform.Find("Dropdown List");
            if (dropdownList == null) yield break;

            // Fix list height and position
            RectTransform listRect = dropdownList.GetComponent<RectTransform>();
            listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, 90);
            listRect.anchoredPosition = new Vector2(0, -16f);

            // Fix each item
            Transform content = dropdownList.Find("Viewport/Content");
            if (content != null)
            {
                int realIndex = 0; // use counter not SiblingIndex
                foreach (RectTransform item in content)
                {
                    item.sizeDelta = new Vector2(item.sizeDelta.x, 25);

                    // Remove old trigger
                    EventTrigger oldTrigger = item.GetComponent<EventTrigger>();
                    if (oldTrigger != null) Destroy(oldTrigger);

                    // Add new PointerDown trigger
                    EventTrigger trigger = item.gameObject.AddComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    int capturedIndex = realIndex; // capture counter value
                    entry.callback.AddListener((data) =>
                    {
                        if (_isSelecting) return; // block double fire
                        _isSelecting = true;

                        floorDropdown.value = capturedIndex;
                        floorDropdown.Hide();

                        StartCoroutine(ResetSelecting());
                    });
                    trigger.triggers.Add(entry);

                    realIndex++; // increment after each item
                }
            }
        }

        // ── Reset Debounce ────────────────────────────────────────────────────

        private IEnumerator ResetSelecting()
        {
            yield return new WaitForSeconds(0.3f);
            _isSelecting = false;
        }

        // ── Load Floors ───────────────────────────────────────────────────────

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
                onSuccess: floors => { SetLoading(false); _floors = floors; PopulateDropdown(floors); },
                onError:   err    => { SetLoading(false); ToastManager.Instance?.Show("Failed to load floors."); Debug.LogError("[FloorDropdownUI] " + err); }
            );
        }

        // ── Populate Dropdown ─────────────────────────────────────────────────

        private void PopulateDropdown(List<string> floors)
        {
            if (floorDropdown == null) return;
            floorDropdown.ClearOptions();
            floorDropdown.AddOptions(floors);
            floorDropdown.value = 0;
            floorDropdown.RefreshShownValue();
        }

        // ── On Floor Selected ─────────────────────────────────────────────────

        private void OnDropdownChanged(int index)
        {
            if (index < 0 || index >= _floors.Count) return;

            string floorName = _floors[index];

            if (selectedFloorText)
                selectedFloorText.text = floorName;

            SessionManager.Instance.SetFloor(floorName);
            ToastManager.Instance?.Show($"Selected: {floorName}");

            Debug.Log($"[FloorDropdownUI] Floor selected: {floorName}");
        }

        // ── Back ──────────────────────────────────────────────────────────────

        private void OnBackClicked() => NavigationManager.Instance.Back();

        // ── Loading ───────────────────────────────────────────────────────────

        private void SetLoading(bool loading)
        {
            if (loadingOverlay) loadingOverlay.SetActive(loading);
        }
    }
}
