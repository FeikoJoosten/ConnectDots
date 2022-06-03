using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GridLayoutGroup), typeof(RectTransform))]
public class ConnectDotsManagerBehaviour : MonoBehaviour
{
    public event Action OnInitializedEvent;

    [Header("Settings")]
    [SerializeField] private Color team1Color = Color.red;
    [SerializeField] private Color team2Color = Color.yellow;

    [Header("Configuration")]
    [SerializeField] private ConnectDotsElementBehaviour elementPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private TMPro.TMP_Text gameStateBannerText;

    public Vector2Int PlayfieldSize { get; private set; }

    private ConnectDotsElementBehaviour[][] playField;
    private Color currentTeamColor;
    private int remainingMoves;
    private int dotsToConnectGoal;
    private bool isGameActive;

    private readonly Dictionary<ConnectDotsElementBehaviour, Vector2Int> indexByElement = new Dictionary<ConnectDotsElementBehaviour, Vector2Int>();

    public void Initialize(int playfieldWidth, int playfieldHeight, int dotsToConnect) {
        PlayfieldSize = new Vector2Int(playfieldWidth, playfieldHeight);
        dotsToConnectGoal = dotsToConnect;

        if (elementPrefab == null) {
            Debug.LogError($"The {nameof(elementPrefab)} is null, meaning the game cannot be initialized!");
            return;
        }

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = PlayfieldSize.x;

        Unsubscribe(); // Delete old elements so we start with a clean slate. Would be more efficient to reuse elements, but this is sufficient for the assessment

        elementPrefab.gameObject.SetActive(true);
        playField = new ConnectDotsElementBehaviour[playfieldWidth][];
        for (int x = 0; x < playfieldWidth; x++) {
            playField[x] = new ConnectDotsElementBehaviour[playfieldHeight];
            for (int y = 0; y < playfieldHeight; y++) {
                ConnectDotsElementBehaviour spawnedElement = Instantiate(elementPrefab, gridLayoutGroup.transform);
                spawnedElement.OnElementClickedEvent += HandleOnElementClickedEvent;

                indexByElement[spawnedElement] = new Vector2Int(x, y);
                playField[x][y] = spawnedElement;
            }
        }
        elementPrefab.gameObject.SetActive(false);

        remainingMoves = playfieldWidth * playfieldHeight;

        gameStateBannerText.text = "Current team";
        currentTeamColor = Random.value > 0.5f ? team2Color : team1Color; // Pick an inversed random team to start the game
        SwitchCurrentTeam(); // Switch teams so we get the correct team AND properly setup the UI

        isGameActive = true;
        OnInitializedEvent?.Invoke();
    }

    private void OnDestroy() {
        Unsubscribe();
    }

    private void Unsubscribe() {
        if (playField == null) return;
        for (int x = 0; x < PlayfieldSize.x; x++) {
            for (int y = 0; y < PlayfieldSize.y; y++) {
                ConnectDotsElementBehaviour element = playField[x][y];
                if(element == null) continue;

                element.OnElementClickedEvent -= HandleOnElementClickedEvent;
                Destroy(element.gameObject);
            }
        }
    }

    private void HandleOnElementClickedEvent(ConnectDotsElementBehaviour clickedElement) {
        if (!isGameActive) return;
        if (!indexByElement.TryGetValue(clickedElement, out Vector2Int clickElementIndex)) {
            Debug.LogError($"Failed to retrieve {nameof(clickElementIndex)}!");
            return;
        }

        ConnectDotsElementBehaviour placedElement = null;
        Vector2Int placedElementIndex = Vector2Int.zero;
        for (int y = 0; y < PlayfieldSize.y; y++) {
            ConnectDotsElementBehaviour potentialElement = playField[clickElementIndex.x][y];
            if(potentialElement.IsColorChanged) continue;

            placedElement = potentialElement;
            placedElementIndex = new Vector2Int(clickElementIndex.x, y);
            break;
        }

        if (placedElement == null) return; // Valid case, no need to log
        
        placedElement.SetColor(currentTeamColor);
        
        if (TryDetectFourInARow(currentTeamColor, placedElementIndex)) {
            gameStateBannerText.text = "GAME WON!";
            isGameActive = false;
            return;
        }

        remainingMoves -= 1;

        if (remainingMoves <= 0) {
            gameStateBannerText.text = "DRAW!";
            gameStateBannerText.color = Color.white;
            isGameActive = false;
            return;
        }

        SwitchCurrentTeam();
    }

    private bool TryDetectFourInARow(Color colorToTest, Vector2Int startingIndex) {
        void CountSameColoredDotsInDirection(Vector2Int direction, Vector2Int currentIndex, ref int numbersOfDotsInDirection) {
            Vector2Int nextIndex = currentIndex + direction;
            if (nextIndex.x >= PlayfieldSize.x || nextIndex.x < 0) return;
            if (nextIndex.y >= PlayfieldSize.y || nextIndex.y < 0) return;
            if (playField[nextIndex.x][nextIndex.y].CurrentColor != colorToTest) return;

            numbersOfDotsInDirection += 1;
            CountSameColoredDotsInDirection(direction, nextIndex, ref numbersOfDotsInDirection);
        }

        // Verify horizontal axis
        int numberOfSameColoredDots = 0;
        CountSameColoredDotsInDirection(Vector2Int.left, startingIndex, ref numberOfSameColoredDots);
        CountSameColoredDotsInDirection(Vector2Int.right, startingIndex, ref numberOfSameColoredDots);

        // Always remove 1 from dotsToConnectGoal as the starting index is already the correct color
        if (numberOfSameColoredDots == dotsToConnectGoal - 1) return true;

        // Verify vertical axis
        numberOfSameColoredDots = 0;
        CountSameColoredDotsInDirection(Vector2Int.up, startingIndex, ref numberOfSameColoredDots);
        CountSameColoredDotsInDirection(Vector2Int.down, startingIndex, ref numberOfSameColoredDots);
        if (numberOfSameColoredDots == dotsToConnectGoal - 1) return true;

        // Verify diagonally
        numberOfSameColoredDots = 0;
        CountSameColoredDotsInDirection(Vector2Int.one, startingIndex, ref numberOfSameColoredDots);
        CountSameColoredDotsInDirection(-Vector2Int.one, startingIndex, ref numberOfSameColoredDots);
        if (numberOfSameColoredDots == dotsToConnectGoal - 1) return true;

        // Verify diagonally other direction
        numberOfSameColoredDots = 0;
        CountSameColoredDotsInDirection(new Vector2Int(1, -1), startingIndex, ref numberOfSameColoredDots);
        CountSameColoredDotsInDirection(new Vector2Int(-1, 1), startingIndex, ref numberOfSameColoredDots);
        return numberOfSameColoredDots == dotsToConnectGoal - 1;
    }

    private void SwitchCurrentTeam() {
        currentTeamColor = currentTeamColor == team1Color ? team2Color : team1Color; // Switch teams
        gameStateBannerText.color = currentTeamColor;
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (gridLayoutGroup == null) gridLayoutGroup = GetComponent<GridLayoutGroup>() ?? gameObject.AddComponent<GridLayoutGroup>();
    }
#endif
}
