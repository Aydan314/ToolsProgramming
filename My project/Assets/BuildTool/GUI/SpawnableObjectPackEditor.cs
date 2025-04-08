using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(SpawnableObjectPack)), CanEditMultipleObjects]
public class SpawnableObjectPackGUI : Editor
{
    VisualElement root;
    SpawnableObjectPack objectPack;
    VisualElement list;
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();

        string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset SpawnableObjectPackGUI")[0]);

        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

        asset.CloneTree(root);

        Label header = root.Q<Label>("Header");
        list = root.Q<VisualElement>("ItemList");
       
        header.text = serializedObject.targetObject.name;

        root.RegisterCallback<ChangeEvent<float>>(UpdateValues);

        objectPack = (SpawnableObjectPack)serializedObject.targetObject;
        
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        return root;
    }

    private void UpdateValues(ChangeEvent<float> change)
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateList()
    {
        foreach (var item in objectPack.spawnableObjects)
        {

        }
}
