using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup), typeof(RectTransform), typeof(ConnectDotsManagerBehaviour))]
public class ConnectDotsGridScalerBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 desiredCellSize = new Vector2(100, 100);
    [SerializeField] private Vector2 desiredCellSpacing = new Vector2(20, 20);

    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private ConnectDotsManagerBehaviour connectDotsManager;

    private void Awake() {
        connectDotsManager.OnInitializedEvent += HandleOnInitializedEvent;
    }

    private void OnDestroy() {
        connectDotsManager.OnInitializedEvent -= HandleOnInitializedEvent;
    }

    private void HandleOnInitializedEvent() {
        OnRectTransformDimensionsChange();
    }

    private void OnRectTransformDimensionsChange() {
        if (!isActiveAndEnabled) return;
        float CalculateSize(float screenSize, int numberOfElements, float spacing, float cellSize) {
            float availableSizeWithSpacing = screenSize / numberOfElements;
            float desiredSizeWithSpacing = cellSize + spacing;
            return availableSizeWithSpacing / desiredSizeWithSpacing;
        }

        Rect playfieldRect = rectTransform.rect;
        RectOffset gridPadding = gridLayoutGroup.padding;
        Vector2Int playfieldSize = connectDotsManager.PlayfieldSize;
        float widthScaleDifference = CalculateSize(playfieldRect.width - gridPadding.horizontal, playfieldSize.x, desiredCellSpacing.x, desiredCellSize.x);
        float heightScaleDifference = CalculateSize(playfieldRect.height - gridPadding.vertical, playfieldSize.y, desiredCellSpacing.y, desiredCellSize.y);
        float smallestScaleDifference = Mathf.Min(widthScaleDifference, heightScaleDifference);

        gridLayoutGroup.spacing = new Vector2(desiredCellSpacing.x * smallestScaleDifference, desiredCellSpacing.y * smallestScaleDifference);
        gridLayoutGroup.cellSize = new Vector2(desiredCellSize.x * smallestScaleDifference, desiredCellSize.y * smallestScaleDifference);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (gridLayoutGroup == null) gridLayoutGroup = GetComponent<GridLayoutGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if(connectDotsManager == null) connectDotsManager = GetComponent<ConnectDotsManagerBehaviour>();
    }
#endif
}
