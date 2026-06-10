using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class SynchronizedInstructionVideos : MonoBehaviour
{
    [Serializable]
    public class InstructionVideoEntry
    {
        public string label;
        public VideoPlayer videoPlayer;
        public string fileName;

        [HideInInspector] public bool prepared;
        [HideInInspector] public bool finished;
    }

    [Header("Videos inside Assets/StreamingAssets/Videos/")]
    [SerializeField] private List<InstructionVideoEntry> videos = new List<InstructionVideoEntry>();

    [Header("Playback")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool restartTogetherWhenAllFinished = true;

    private Coroutine prepareRoutine;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartVideoCycle();
        }
    }

    private void OnDisable()
    {
        StopVideoCycle();
    }

    public void StartVideoCycle()
    {
        StopVideoCycle();
        prepareRoutine = StartCoroutine(PrepareAllNextFrame());
    }

    private IEnumerator PrepareAllNextFrame()
    {
        // Wait one frame so all UI objects and VideoPlayers are fully active.
        yield return null;

        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            entry.prepared = false;
            entry.finished = false;

            VideoPlayer vp = entry.videoPlayer;

            vp.Stop();

            vp.source = VideoSource.Url;
            vp.url = GetStreamingVideoUrl(entry.fileName);

            vp.playOnAwake = false;
            vp.isLooping = false;
            vp.waitForFirstFrame = true;
            vp.skipOnDrop = true;

            vp.prepareCompleted -= OnVideoPrepared;
            vp.loopPointReached -= OnVideoFinished;
            vp.errorReceived -= OnVideoError;

            vp.prepareCompleted += OnVideoPrepared;
            vp.loopPointReached += OnVideoFinished;
            vp.errorReceived += OnVideoError;

            Debug.Log($"[SynchronizedInstructionVideos] {entry.label} URL: {vp.url}");

#if !UNITY_WEBGL || UNITY_EDITOR
            string localPath = GetLocalStreamingVideoPath(entry.fileName);
            if (!File.Exists(localPath))
            {
                Debug.LogError($"[SynchronizedInstructionVideos] Missing file for {entry.label}: {localPath}");
            }
#endif

            vp.Prepare();
        }
    }

    private void OnVideoPrepared(VideoPlayer preparedPlayer)
    {
        InstructionVideoEntry entry = FindEntry(preparedPlayer);
        if (entry == null)
            return;

        entry.prepared = true;

        Debug.Log($"[SynchronizedInstructionVideos] Prepared: {entry.label}");

        if (AllValidVideosPrepared())
        {
            PlayAllTogether();
        }
    }

    private void PlayAllTogether()
    {
        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            entry.finished = false;

            VideoPlayer vp = entry.videoPlayer;
            vp.time = 0;
            vp.Play();
        }

        Debug.Log("[SynchronizedInstructionVideos] All instruction videos started together.");
    }

    private void OnVideoFinished(VideoPlayer finishedPlayer)
    {
        InstructionVideoEntry entry = FindEntry(finishedPlayer);
        if (entry == null)
            return;

        entry.finished = true;

        // Freeze on the last frame while waiting for the other videos.
        finishedPlayer.Pause();

        Debug.Log($"[SynchronizedInstructionVideos] Finished and waiting: {entry.label}");

        if (restartTogetherWhenAllFinished && AllValidVideosFinished())
        {
            RestartAllTogether();
        }
    }

    private void RestartAllTogether()
    {
        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            entry.finished = false;

            VideoPlayer vp = entry.videoPlayer;
            vp.Stop();
            vp.time = 0;
        }

        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            entry.videoPlayer.Play();
        }

        Debug.Log("[SynchronizedInstructionVideos] All instruction videos restarted together.");
    }

    private void StopVideoCycle()
    {
        if (prepareRoutine != null)
        {
            StopCoroutine(prepareRoutine);
            prepareRoutine = null;
        }

        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            VideoPlayer vp = entry.videoPlayer;

            vp.prepareCompleted -= OnVideoPrepared;
            vp.loopPointReached -= OnVideoFinished;
            vp.errorReceived -= OnVideoError;

            vp.Stop();

            entry.prepared = false;
            entry.finished = false;
        }
    }

    private string GetStreamingVideoUrl(string fileName)
    {
        string path = $"{Application.streamingAssetsPath}/Videos/{fileName}";
        path = path.Replace("\\", "/");

#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL needs a browser-accessible hosted URL.
        return path;
#else
        // Windows, macOS, Linux, and Editor need a proper local file URI.
        return new Uri(path).AbsoluteUri;
#endif
    }

    private string GetLocalStreamingVideoPath(string fileName)
    {
        return Path.Combine(Application.streamingAssetsPath, "Videos", fileName);
    }

    private void OnVideoError(VideoPlayer vp, string message)
    {
        InstructionVideoEntry entry = FindEntry(vp);
        string label = entry != null ? entry.label : vp.name;

        Debug.LogError($"[SynchronizedInstructionVideos] Video error on {label}: {message}");
        Debug.LogError($"[SynchronizedInstructionVideos] URL was: {vp.url}");
    }

    private InstructionVideoEntry FindEntry(VideoPlayer vp)
    {
        foreach (InstructionVideoEntry entry in videos)
        {
            if (entry != null && entry.videoPlayer == vp)
                return entry;
        }

        return null;
    }

    private bool AllValidVideosPrepared()
    {
        int validCount = 0;
        int preparedCount = 0;

        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            validCount++;

            if (entry.prepared)
                preparedCount++;
        }

        return validCount > 0 && preparedCount == validCount;
    }

    private bool AllValidVideosFinished()
    {
        int validCount = 0;
        int finishedCount = 0;

        foreach (InstructionVideoEntry entry in videos)
        {
            if (!IsValid(entry))
                continue;

            validCount++;

            if (entry.finished)
                finishedCount++;
        }

        return validCount > 0 && finishedCount == validCount;
    }

    private bool IsValid(InstructionVideoEntry entry)
    {
        return entry != null &&
               entry.videoPlayer != null &&
               !string.IsNullOrWhiteSpace(entry.fileName);
    }
}