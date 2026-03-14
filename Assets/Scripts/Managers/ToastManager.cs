using System.Collections;
using TMPro;
using UnityEngine;

namespace ViSNET.Managers
{
    /// <summary>
    /// World-space toast notification system.
    ///
    /// Setup:
    ///   1. Create a Canvas (Render Mode: World Space) as a child of your XR Rig camera.
    ///   2. Add a Panel with a TextMeshPro child named "ToastText".
    ///   3. Drag that Panel into the ToastPanel field and the TMP into ToastText field.
    ///   4. Make sure a CanvasGroup component is on ToastPanel (for alpha fade).
    /// </summary>
    public class ToastManager : MonoBehaviour
    {
        public static ToastManager Instance { get; private set; }

        [Header("References")]
        public GameObject  toastPanel;
        public TMP_Text    toastText;
        public CanvasGroup canvasGroup;

        [Header("Timing")]
        public float displaySeconds = 2.5f;
        public float fadeSeconds    = 0.4f;

        private Coroutine _active;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (toastPanel) toastPanel.SetActive(false);
        }

        // ── Public API ────────────────────────────────────────────────────────

        public void Show(string message)
        {
            if (_active != null) StopCoroutine(_active);
            _active = StartCoroutine(ShowCoroutine(message));
        }

        // ── Coroutine ─────────────────────────────────────────────────────────

        private IEnumerator ShowCoroutine(string message)
        {
            toastText.text = message;
            toastPanel.SetActive(true);
            canvasGroup.alpha = 1f;

            // Hold
            yield return new WaitForSeconds(displaySeconds);

            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeSeconds)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeSeconds);
                elapsed += Time.deltaTime;
                yield return null;
            }

            toastPanel.SetActive(false);
            _active = null;
        }
    }
}
