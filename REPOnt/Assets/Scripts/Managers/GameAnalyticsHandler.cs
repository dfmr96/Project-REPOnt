using System;
using System.Collections.Generic;
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
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            initialized = true;
            Debug.Log("[Analytics] Unity Analytics initialized.");
        }
    }

    public static void TrackMatchResult(string winningTeam)
    {
        if (!initialized) return;

        AnalyticsService.Instance.RecordEvent(new CustomEvent("winnerTeam")
        {
            { "winner", winningTeam },
        });
        Debug.Log($"[Analytics] Sent match_won: {winningTeam}");
    }

    public static void TrackObjectPlaced(int playerId)
    {
        if (!initialized) return;

        AnalyticsService.Instance.RecordEvent(new CustomEvent("onObjectDelivered")
        {
            { "player_id", playerId },
        });
        Debug.Log($"[Analytics] Sent match_won: {playerId}");
    }
}
