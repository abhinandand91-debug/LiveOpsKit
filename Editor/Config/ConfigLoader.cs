using System;
using System.IO;
using UnityEngine.Networking;

namespace LiveOpsKit.Editor
{
    public static class ConfigLoader
    {
        public static bool TryReadFile(string path, out string json, out string error)
        {
            json = null;
            error = null;
            if (File.Exists(path))
            {
                json = File.ReadAllText(path);
                return true;
            }

            error = $"File not found: {path}";
            return false;
        }

        // Calls back with the downloaded text, or with an error message.
        public static void Download(string url, Action<string, string> onDone)
        {
            var request = UnityWebRequest.Get(url);
            request.SendWebRequest().completed += _ =>
            {
                if (request.result == UnityWebRequest.Result.Success) onDone(request.downloadHandler.text, null);
                else onDone(null, $"{request.error} ({url})");
                request.Dispose();
            };
        }
    }
}
