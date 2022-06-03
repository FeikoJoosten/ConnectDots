using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UpdateSliderTextBehaviour : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TMP_Text text;

    private void Awake() {
        if (text == null) {
            Debug.LogError($"Cannot update {nameof(text)} because it is null!");
            return;
        }

        slider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);
    }

    private void OnDestroy() {
        slider.onValueChanged.RemoveListener(HandleOnSliderValueChangedEvent);
    }

    private void HandleOnSliderValueChangedEvent(float currentValue) {
        text.text = currentValue.ToString(); // Ideally we'd have a string cache to prevent garbage generation, but for the sake of this assessment this is fine
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(slider == null) slider = GetComponent<Slider>();
    }
#endif
}
