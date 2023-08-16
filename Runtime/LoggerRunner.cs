using UnityEngine;

namespace Logger
{
    public static class LoggerRunner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Log.LoggerInit();
        }
    }
}