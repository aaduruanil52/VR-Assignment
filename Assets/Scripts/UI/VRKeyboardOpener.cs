using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ViSNET.UI
{
    /// <summary>
    /// Attach this to any TMP_InputField GameObject
    /// to open Meta Quest keyboard automatically
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class VRKeyboardOpener : MonoBehaviour, IPointerClickHandler
    {
        private TMP_InputField _inputField;
        private TouchScreenKeyboard _keyboard;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OpenKeyboard();
        }

        private void OpenKeyboard()
        {
            if (_inputField == null) return;

            TouchScreenKeyboard.hideInput = true;

            if (_inputField.contentType == TMP_InputField.ContentType.Password)
            {
                _keyboard = TouchScreenKeyboard.Open(
                    _inputField.text,
                    TouchScreenKeyboardType.Default,
                    false,  // autocorrect
                    false,  // multiline
                    true,   // secure (password)
                    false,  // alert
                    "",     // placeholder
                    0       // characterLimit
                );
            }
            else
            {
                _keyboard = TouchScreenKeyboard.Open(
                    _inputField.text,
                    TouchScreenKeyboardType.Default,
                    true,   // autocorrect
                    false,  // multiline
                    false,  // secure
                    false,  // alert
                    "",     // placeholder
                    0       // characterLimit
                );
            }
        }

        private void Update()
        {
            if (_keyboard == null) return;

            // Copy keyboard text to input field
            if (_keyboard.status == TouchScreenKeyboard.Status.Visible)
            {
                _inputField.text = _keyboard.text;
            }

            // Keyboard closed
            if (_keyboard.status == TouchScreenKeyboard.Status.Done ||
                _keyboard.status == TouchScreenKeyboard.Status.Canceled ||
                _keyboard.status == TouchScreenKeyboard.Status.LostFocus)
            {
                _keyboard = null;
            }
        }
    }
}