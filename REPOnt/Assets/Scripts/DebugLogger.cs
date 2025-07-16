using System;
using System.IO;
using UnityEngine;

public static class DebugLogger
{
    private static readonly string logPath = Path.Combine(Application.persistentDataPath, "debug_log.txt");
    private static bool initialized = false;

    public static void Log(string message)
    {
        if (!initialized) TryCreateLogFile();

        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string fullMessage = $"[{timestamp}] {message}\n";

        try { File.AppendAllText(logPath, fullMessage + Environment.NewLine); }
        catch (Exception) { }
    }

    private static void TryCreateLogFile()
    {
        try
        {
            if (!File.Exists(logPath))
                File.WriteAllText(logPath, string.Empty);
            initialized = true;
        }
        catch (Exception) { }
    }

    public static void ClearLog()
    {
        try { File.WriteAllText(logPath, string.Empty); }
        catch (Exception) { }
    }
}