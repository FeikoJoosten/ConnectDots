using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class ConnectDotsElementBehaviour : MonoBehaviour, IPointerClickHandler
{
    public event Action<ConnectDotsElementBehaviour> OnElementClickedEvent;

    [SerializeField] private Color startingColor = Color.white;
    [SerializeField] private Graphic graphic;

    public bool IsColorChanged => startingColor != CurrentColor;
    public Color CurrentColor => graphic != null ? graphic.color : startingColor;

    public void SetColor(Color color) {
        if (graphic != null) graphic.color = color;
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnElementClickedEvent?.Invoke(this);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (graphic == null) graphic = GetComponent<Graphic>();
        graphic.color = startingColor;
    }
#endif
}
