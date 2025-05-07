#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(SpawnableObject)),CanEditMultipleObjects]

public class SpawnableObjectEditor : Editor
{
    public VisualElement root;
    public Label header;
    public VisualElement preview;

    public override VisualElement CreateInspectorGUI()
    {
        // Get GUI elements from UXML //
        root = new VisualElement();

        string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset SpawnableObjectGUI")[0]);

        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
    
        asset.CloneTree(root);

        header = root.Q<Label>("Header");
        preview = root.Q<VisualElement>("Preview");

        header.text = serializedObject.targetObject.name;

        root.RegisterCallback<ChangeEvent<float>>(UpdateRotation);
        
        root.RegisterCallback<ChangeEvent<UnityEngine.Object>>(UpdatePrefab);
        

        UpdateIconImage();

        return root;
    }

    private void UpdateRotation(ChangeEvent<float> change)
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void UpdatePrefab(ChangeEvent<UnityEngine.Object> e)
    {
        GameObject prefab = ((SpawnableObject)serializedObject.targetObject).objectPrefab;

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        // Error check if objects prefab isnt set //
        if (prefab == null)
        {
            Debug.LogError("!! Object Prefab in \"" + serializedObject.targetObject.name + "\" cannot be Null !!");
        }

        UpdateIconImage();
    }

    public void UpdateIconImage()
    {
        GameObject prefab = ((SpawnableObject)serializedObject.targetObject).objectPrefab;

        if (prefab != null)
        {
            // Show preview of object prefab //
            preview.style.backgroundImage = AssetPreview.GetAssetPreview(prefab);
        }
        else
        {
            // Show default spawnable object icon //
            string iconPath = AssetDatabase.FindAssets("t:Texture2D SpawnableIcon")[0];
            preview.style.backgroundImage = AssetPreview.GetMiniThumbnail(serializedObject.targetObject);
        }
    }
}
#endif