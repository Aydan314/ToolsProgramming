using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditor.Search;

[CustomEditor(typeof(SpawnableObject)),CanEditMultipleObjects]

public class SpawnableObjectEditor : Editor
{
    public VisualElement root;
    public Label header;
    public Button preview;
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();

        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BuildTool/UXML/SpawnableObjectGUI.uxml");
        
        asset.CloneTree(root);

        header = root.Q<Label>("Header");
        preview = root.Q<Button>("Preview");

        header.text = serializedObject.targetObject.name;

        root.RegisterCallback<ChangeEvent<float>>(UpdateRotation);
        
        root.RegisterCallback<MouseOverEvent>(UpdatePrefab);
        

        UpdateIconImage();

        return root;
    }

    private void UpdateRotation(ChangeEvent<float> change)
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void UpdatePrefab(MouseOverEvent e)
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
            Background image = new Background();
            image.texture = AssetPreview.GetAssetPreview(prefab);

            preview.iconImage = image;
        }
    }


}
