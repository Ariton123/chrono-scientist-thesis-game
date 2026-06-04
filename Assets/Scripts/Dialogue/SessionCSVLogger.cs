using System;
using System.IO;
using UnityEngine;

public static class SessionCSVLogger
{
    private static readonly string SessionId =
        DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + UnityEngine.Random.Range(1000, 9999);

    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "chrono_scientist_session_log.csv");

    public static void LogEvent(
        string eventType,
        string stageId = "",
        string language = "",
        string playerGender = "",
        float completionTimeSeconds = -1f,
        int mistakes = -1,
        int retries = -1,
        string rank = "",
        bool astragalosBadge = false,
        string extra = "")
    {
        EnsureFileExists();

        string row =
            $"{Escape(SessionId)}," +
            $"{Escape(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))}," +
            $"{Escape(eventType)}," +
            $"{Escape(stageId)}," +
            $"{Escape(language)}," +
            $"{Escape(playerGender)}," +
            $"{FormatFloat(completionTimeSeconds)}," +
            $"{FormatInt(mistakes)}," +
            $"{FormatInt(retries)}," +
            $"{Escape(rank)}," +
            $"{(astragalosBadge ? "1" : "0")}," +
            $"{Escape(extra)}\n";

        File.AppendAllText(FilePath, row);

        Debug.Log($"[SessionCSVLogger] {eventType} logged. CSV path: {FilePath}");
    }

    public static string GetLogFilePath()
    {
        return FilePath;
    }

    private static void EnsureFileExists()
    {
        if (File.Exists(FilePath))
            return;

        string header =
            "SessionID,Timestamp,EventType,StageID,Language,PlayerGender,CompletionTimeSeconds,Mistakes,Retries,Rank,AstragalosBadge,Extra\n";

        File.WriteAllText(FilePath, header);

        Debug.Log($"[SessionCSVLogger] Created CSV log file at: {FilePath}");
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }

    private static string FormatFloat(float value)
    {
        return value < 0f ? "" : value.ToString("0.00");
    }

    private static string FormatInt(int value)
    {
        return value < 0 ? "" : value.ToString();
    }
}