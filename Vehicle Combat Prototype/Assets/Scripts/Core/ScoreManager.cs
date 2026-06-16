using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int scorePerKill = 100;

    private int blueScore;
    private int redScore;

    public int BlueScore => blueScore;
    public int RedScore => redScore;

    private void OnEnable()
    {
        GameEvents.VehicleDestroyed += HandleVehicleDestroyed;
    }

    private void OnDisable()
    {
        GameEvents.VehicleDestroyed -= HandleVehicleDestroyed;
    }

    private void HandleVehicleDestroyed(VehicleDestroyedEventArgs eventArgs)
    {

        AddScore(eventArgs.AttackerTeamId, scorePerKill);
    }

    private void AddScore(TeamId teamId, int amount)
    {
        if (amount <= 0)
            return;

        switch (teamId)
        {
            case TeamId.Blue:
                blueScore += amount;
                break;

            case TeamId.Red:
                redScore += amount;
                break;

            default:
                return;
        }

        Debug.Log($"Score Updated | Blue: {blueScore} | Red: {redScore}");
    }

    public void ResetScores()
    {
        blueScore = 0;
        redScore = 0;

        Debug.Log($"Score Reset | Blue: {blueScore} | Red: {redScore}");
    }
}
