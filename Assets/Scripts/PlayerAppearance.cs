using Unity.Netcode;
using UnityEngine;

public class PlayerAppearance : NetworkBehaviour
{
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Material[] playerMaterials;

    private NetworkVariable<int> materialIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        materialIndex.OnValueChanged += OnMaterialIndexChanged;

        if (IsServer && playerMaterials != null && playerMaterials.Length > 0)
        {
            materialIndex.Value = (int)(OwnerClientId % (ulong)playerMaterials.Length);
        }

        ApplyMaterial(materialIndex.Value);
    }

    public override void OnNetworkDespawn()
    {
        materialIndex.OnValueChanged -= OnMaterialIndexChanged;
    }

    private void OnMaterialIndexChanged(int oldIndex, int newIndex)
    {
        ApplyMaterial(newIndex);
    }

    private void ApplyMaterial(int index)
    {
        if (playerRenderer == null || playerMaterials == null || playerMaterials.Length == 0)
        {
            return;
        }

        int safeIndex = index % playerMaterials.Length;
        playerRenderer.material = playerMaterials[safeIndex];
    }
}
