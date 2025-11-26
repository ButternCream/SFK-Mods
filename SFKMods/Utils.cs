using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace SFKMod
{
    public class Utils
    {
        public static void LogStackTrace(int maxStackSize = 10)
        {
            var stackTrace = new StackTrace(1, false);
            var stackFrames = stackTrace.GetFrames().Take(maxStackSize);
            foreach (var frame in stackFrames)
            {
                Plugin.Logger.LogInfo($" at {frame}");
            }
        }

        public static string GetHierarchyPath(Transform t)
        {
            var path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
    }
}
