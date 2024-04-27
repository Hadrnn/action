using Unity.Netcode;
using UnityEngine;

public class UnityNetworkFlagBase : NetworkBehaviour
{
    public NetworkVariable<int> teamNumber;
    public Light teamLight;
}
