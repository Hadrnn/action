using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// A ScriptableObject for holding a network prefabs list, which can be
/// shared between multiple NetworkManagers.
///
/// When NetworkManagers hold references to this list, modifications to the
/// list at runtime will be picked up by all NetworkManagers that reference it.
/// </summary>
[CreateAssetMenu(fileName = "PlayerPrefabs", menuName = "Ours/Player Prefabs List")]
public class PlayerPrefabsList : ScriptableObject
{
    internal delegate void OnAddDelegate(GameObject prefab);
    internal OnAddDelegate OnAdd;

    internal delegate void OnRemoveDelegate(GameObject prefab);
    internal OnRemoveDelegate OnRemove;

    [SerializeField]
    internal bool IsDefault;

    [FormerlySerializedAs("Prefabs")]
    [SerializeField]
    internal List<GameObject> List = new List<GameObject>();

    /// <summary>
    /// Read-only view into the prefabs list, enabling iterating and examining the list.
    /// Actually modifying the list should be done using <see cref="Add"/>
    /// and <see cref="Remove"/>.
    /// </summary>
    public IReadOnlyList<GameObject> PrefabList => List;

    /// <summary>
    /// Adds a prefab to the prefab list. Performing this here will apply the operation to all
    /// <see cref="NetworkManager"/>s that reference this list.
    /// </summary>
    /// <param name="prefab"></param>
    public void Add(GameObject prefab)
    {
        List.Add(prefab);
        OnAdd?.Invoke(prefab);
    }

    /// <summary>
    /// Removes a prefab from the prefab list. Performing this here will apply the operation to all
    /// <see cref="NetworkManager"/>s that reference this list.
    /// </summary>
    /// <param name="prefab"></param>
    public void Remove(GameObject prefab)
    {
        List.Remove(prefab);
        OnRemove?.Invoke(prefab);
    }

    /// <summary>
    /// Check if the given GameObject is present as a prefab within the list
    /// </summary>
    /// <param name="prefab">The prefab to check</param>
    /// <returns>Whether or not the prefab exists</returns>
    public bool Contains(GameObject prefab)
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i] == prefab)
            {
                return true;
            }
        }

        return false;
    }
}

