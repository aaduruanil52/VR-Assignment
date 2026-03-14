using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;

namespace ViSNET.XR
{
    /// <summary>
    /// Attach this to every World-Space Canvas in your scenes.
    ///
    /// It registers the canvas with the XR UI Input Module so ray-casting
    /// from XR controllers works without additional wiring.
    ///
    /// Requirements:
    ///   - Unity XR Interaction Toolkit 2.x or later
    ///   - An XRRayInteractor on each controller
    ///   - XRUIInputModule (replaces StandaloneInputModule on EventSystem)
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class XRUISetup : MonoBehaviour
    {
        [Tooltip("Distance in metres from the XR camera at which the panel floats.")]
        [Range(0.5f, 5f)]
        public float panelDistance = 2f;

        [Tooltip("Follow the main camera on start (useful for scene entry).")]
        public bool positionInFrontOfCamera = true;

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
        }

        private void Start()
        {
            if (positionInFrontOfCamera && Camera.main != null)
            {
                Transform cam = Camera.main.transform;
                transform.position = cam.position + cam.forward * panelDistance;
                transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
            }

            // Ensure a GraphicRaycaster is present (required for pointer events)
            if (!TryGetComponent<UnityEngine.UI.GraphicRaycaster>(out _))
                gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
    }
}
