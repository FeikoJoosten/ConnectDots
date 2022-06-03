using UnityEngine;
using UnityEngine.UI;

public class ConnectDotsInitializerBehaviour : MonoBehaviour
{
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider dotsToConnectSlider;

    [SerializeField] private Button startGameButton;
    [SerializeField] private ConnectDotsManagerBehaviour connectDotsManager;

    private void Awake() {
        bool VerifyObject(Object objectToVerify, string objectName) {
            bool isValid = objectToVerify != null;
            if (!isValid) {
                Debug.LogError($"Cannot Initialize {nameof(ConnectDotsManagerBehaviour)} because {objectName} is null!");
            }

            return isValid;
        }

        if (!VerifyObject(widthSlider, nameof(widthSlider))) return;
        if (!VerifyObject(heightSlider, nameof(heightSlider))) return;
        if (!VerifyObject(dotsToConnectSlider, nameof(dotsToConnectSlider))) return;
        if (!VerifyObject(startGameButton, nameof(startGameButton))) return;
        if (!VerifyObject(connectDotsManager, nameof(connectDotsManager))) return;

        widthSlider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);
        heightSlider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);
        dotsToConnectSlider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);

        startGameButton.onClick.AddListener(HandleOnStartGameButtonClickedEvent);
        HandleOnSliderValueChangedEvent(dotsToConnectSlider.value);
    }

    private void OnDestroy() {
        if(startGameButton != null) startGameButton.onClick.RemoveListener(HandleOnStartGameButtonClickedEvent);
        if(widthSlider != null) widthSlider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);
        if(heightSlider != null) heightSlider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);
        if(dotsToConnectSlider != null) dotsToConnectSlider.onValueChanged.AddListener(HandleOnSliderValueChangedEvent);
    }

    private void HandleOnSliderValueChangedEvent(float currentValue) {
        dotsToConnectSlider.maxValue = Mathf.Max(widthSlider.value, heightSlider.value);
    }

    private void HandleOnStartGameButtonClickedEvent() {
        connectDotsManager.Initialize((int)widthSlider.value, (int)heightSlider.value, (int)dotsToConnectSlider.value);
    }
}
