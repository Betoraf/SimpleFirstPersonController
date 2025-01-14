using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Betoraf.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UniversalButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Button Settings")]
        [SerializeField] private Vector3 pressedScale = new Vector3(0.9f, 0.9f, 1f);
        [SerializeField] private Vector3 normalScale = Vector3.one;
        public UnityEvent onPressDown;
        public UnityEvent onPressUp;

        private bool isButtonPressed = false;
        private RectTransform rectTransform;

        public bool IsButtonPressed => isButtonPressed;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isButtonPressed)
            {
                isButtonPressed = true;
                onPressDown.Invoke();
                UpdateButtonScale(pressedScale);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isButtonPressed)
            {
                isButtonPressed = false;
                onPressUp.Invoke();
                UpdateButtonScale(normalScale);
            }
        }

        private void UpdateButtonScale(Vector3 scale)
        {
            if (rectTransform != null)
            {
                rectTransform.localScale = scale;
            }
        }
    }
}