using SFKMod.Patches;
using SuperFantasyKingdom;
using SuperFantasyKingdom.Spawner;
using UnityEngine;

namespace SFKMod
{
    public static class ShardAPI
    {

        /// <summary>
        /// Generic spawn helper if you want other resource types too.
        /// </summary>
        public static void SpawnResource(Vector3 position, int amount, ResourceType type, string sourceType = "CustomItem", string sourceId = "Unknown")
        {
            if (!DroppedShardSpawner.Instance)
            {
                Plugin.Logger.LogWarning("[ShardAPI] DroppedShardSpawner not found in scene.");
                return;
            }

            SourceContext.Type = sourceType;
            SourceContext.Id = sourceId;
            try
            {
                DroppedShardSpawner.Instance.Spawn(position, amount, type);
            }
            finally
            {
                SourceContext.Type = null;
                SourceContext.Id = null;
            }
        }
    }
}
