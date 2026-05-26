using UnityEngine;
using UnityEngine.Events;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }

    [Header("Points")]
    [SerializeField] private int currentPoints;

    [Header("Events")]
    public UnityEvent<int> OnPointsChanged;

    public int CurrentPoints => currentPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        NotifyPointsChanged();
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;

        currentPoints += amount;
        Debug.Log("Points = " + currentPoints);
        NotifyPointsChanged();
    }

    public void ResetPoints()
    {
        currentPoints = 0;
        NotifyPointsChanged();
    }

    private void NotifyPointsChanged()
    {
        OnPointsChanged?.Invoke(currentPoints);
    }
}