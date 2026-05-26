using TMPro;
using UnityEngine;

public class PointCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private string prefix = "X";

    private void OnEnable()
    {
        if (PointManager.Instance != null)
            PointManager.Instance.OnPointsChanged.AddListener(UpdateText);
    }

    private void OnDisable()
    {
        if (PointManager.Instance != null)
            PointManager.Instance.OnPointsChanged.RemoveListener(UpdateText);
    }

    private void Start()
    {
        if (PointManager.Instance != null)
            UpdateText(PointManager.Instance.CurrentPoints);
    }

    private void UpdateText(int points)
    {
        if (pointsText == null)
            return;

        pointsText.text = prefix + points;
    }
}