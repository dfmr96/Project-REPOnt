using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public static class GameAnalyticsHandler
{
    private static bool initialized = false;

    public static async void Initialize()
    {
        if (!initialized)
        {
            DebugLogger.ClearLog();
            DebugLogger.Log($"Unity Analytics initialiced successfuly");
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            initialized = true;
        }
    }

    public static void TrackMatchResult(string winningTeam, string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"winnerTeam called, winner = {winningTeam} in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("winnerTeam")
        {
            { "winner", winningTeam },
            { "roomName", roomName }
        });
    }

    public static void TrackMatchDuration(float durationSeconds, string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"onGameFinished called, match_duration_seconds = {durationSeconds} in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("onGameFinished")
        {
            { "match_duration_seconds", durationSeconds },
            { "roomName", roomName }
        });
    }

    public static void TrackObjectPlaced(int playerId, string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"onObjectDelivered called, player_id = {playerId} in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("onObjectDelivered")
        {
            { "player_id", playerId },
            { "roomName", roomName }
        });
    }

    public static void TrackGhostAFK(float duration, Vector3 position, string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"onGhostAFK called, duration_seconds = {duration}, position_x = {position.x}, position_z = {position.z}  in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("onGhostAFK")
        {
            { "duration_seconds", duration },
            { "position_x", position.x },
            { "position_z", position.z },
            { "roomName", roomName }
        });
    }

    public static void TrackCapturedPlayers(int playerId, string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"onPlayerCaptured called, player_id = {playerId} in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("onPlayerCaptured")
        {
            { "player_id", playerId },
            { "roomName", roomName }
        });
    }

    public static void TrackUnCapturedPlayers(int playerId, string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"onPlayerRescued called, player_id = {playerId} in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("onPlayerRescued")
        {
            { "player_id", playerId },
            { "roomName", roomName }
        });
    }

    public static void TrackObjectDropped(int playerId, int objectId ,string roomName)
    {
        if (!initialized) return;

        DebugLogger.Log($"onObjectDropped called, player_id = {playerId}, object_id = {objectId} in roomName = {roomName}");
        AnalyticsService.Instance.RecordEvent(new CustomEvent("onObjectDropped")
        {
            { "player_id", playerId },
            { "object_id", objectId },
            { "roomName", roomName }
        });
    }
}
