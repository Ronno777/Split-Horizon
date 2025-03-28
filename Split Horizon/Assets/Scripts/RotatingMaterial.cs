using UnityEngine;

[ExecuteInEditMode]
public class PersistentMaterialAssigner : MonoBehaviour
{
    public Material[] materials;

    void OnEnable()
    {
        if (!Application.isPlaying)
            AssignMaterials();
    }

    void AssignMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            int matIndex = i % materials.Length;
            renderers[i].sharedMaterial = materials[matIndex]; // sharedMaterial = edit-time
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject); // Marks object dirty so Unity saves it
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }
}
