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
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        UpdateIconImage();
    }

    public void UpdateIconImage()
    {
        GameObject prefab = ((SpawnableObject)serializedObject.targetObject).objectPrefab;

        if (prefab != null)
        {
            preview.style.backgroundImage = AssetPreview.GetAssetPreview(prefab);
        }
        else
        {
            string iconPath = AssetDatabase.FindAssets("t:Texture2D SpawnableIcon")[0];
            preview.style.backgroundImage = AssetPreview.GetMiniThumbnail(serializedObject.targetObject);
        }
    }


}
