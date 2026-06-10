using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public static class SessionCSVLogger
{
    private const string ParticipantCounterPrefsKey = "CSVLogger_ParticipantCounter";

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebGLDownloadTextFile(string fileName, string text);
#endif

    private static readonly string SessionId =
        DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) +
        "_" +
        UnityEngine.Random.Range(1000, 9999);

    private static readonly string ParticipantId = CreateParticipantIdForThisSession();

    private static int eventIndex = 0;

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
        if (ShouldSkipEvent(eventType))
        {
            Debug.Log($"[SessionCSVLogger] Skipped duplicate/non-essential event: {eventType}");
            return;
        }

        EnsureFileExists();

        eventIndex++;

        string row =
            $"{Escape(ParticipantId)}," +
            $"{Escape(SessionId)}," +
            $"{eventIndex}," +
            $"{Escape(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))}," +
            $"{Escape(eventType)}," +
            $"{Escape(stageId)}," +
            $"{Escape(language)}," +
            $"{Escape(playerGender)}," +
            $"{FormatFloat(completionTimeSeconds)}," +
            $"{FormatInt(mistakes)}," +
            $"{FormatInt(retries)}," +
            $"{Escape(rank)}," +
            $"{FormatBool(astragosBadge: astragalosBadge)}," +
            $"{Escape(extra)}\n";

        File.AppendAllText(FilePath, row, Encoding.UTF8);

        Debug.Log(
            $"[SessionCSVLogger] Event #{eventIndex} logged: {eventType}. " +
            $"Participant: {ParticipantId}. Session: {SessionId}. CSV path: {FilePath}"
        );
    }

    public static void DownloadCsvFile()
    {
        LogEvent(
            "CSV_DOWNLOAD_CONFIRMED",
            extra: "CSV download confirmed from Rewards panel."
        );

        EnsureFileExists();

        string csvText = File.ReadAllText(FilePath, Encoding.UTF8);
        string fileName = $"chrono_scientist_{ParticipantId}_{SessionId}.csv";

#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLDownloadTextFile(fileName, csvText);
        Debug.Log($"[SessionCSVLogger] WebGL CSV download triggered: {fileName}");
#else
        string exportedPath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(exportedPath, csvText, Encoding.UTF8);

        Debug.Log($"[SessionCSVLogger] CSV exported for desktop/editor at: {exportedPath}");
        Application.OpenURL(new Uri(exportedPath).AbsoluteUri);
#endif
    }

    public static string GetParticipantId()
    {
        return ParticipantId;
    }

    public static string GetSessionId()
    {
        return SessionId;
    }

    public static string GetLogFilePath()
    {
        return FilePath;
    }

    public static void ResetParticipantCounter()
    {
        PlayerPrefs.DeleteKey(ParticipantCounterPrefsKey);
        PlayerPrefs.Save();

        Debug.Log("[SessionCSVLogger] Participant counter reset. Restart the game for P_01.");
    }

    private static string CreateParticipantIdForThisSession()
    {
        int nextParticipantNumber = PlayerPrefs.GetInt(ParticipantCounterPrefsKey, 0) + 1;

        PlayerPrefs.SetInt(ParticipantCounterPrefsKey, nextParticipantNumber);
        PlayerPrefs.Save();

        string participantId = $"P_{nextParticipantNumber:00}";

        Debug.Log($"[SessionCSVLogger] Created participant for this session: {participantId}");

        return participantId;
    }

    private static bool ShouldSkipEvent(string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            return false;

        return eventType == "CARD_RANK_ASSIGNED" ||
               eventType == "ASTRAGALOS_BADGE_AWARDED";
    }

    private static void EnsureFileExists()
    {
        if (File.Exists(FilePath))
            return;

        string header =
            "ParticipantID," +
            "SessionID," +
            "EventIndex," +
            "TimestampLocal," +
            "EventType," +
            "StageID," +
            "Language," +
            "PlayerGender," +
            "CompletionTimeSeconds," +
            "Mistakes," +
            "Retries," +
            "Rank," +
            "AstragalosBadge," +
            "Extra\n";

        File.WriteAllText(FilePath, header, Encoding.UTF8);

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
        if (value < 0f)
            return "";

        return value.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private static string FormatInt(int value)
    {
        if (value < 0)
            return "";

        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static string FormatBool(bool astragosBadge)
    {
        return astragosBadge ? "1" : "0";
    }
}