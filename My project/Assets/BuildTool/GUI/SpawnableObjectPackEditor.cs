
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SpawnableObjectPack)), CanEditMultipleObjects]
public class SpawnableObjectPackGUI : Editor
{
    
    VisualElement root;
    VisualTreeAsset listItemTemplate;
    SpawnableObjectPack objectPack;
    VisualElement list;
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        
        string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset SpawnableObjectPackGUI")[0]);
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

        string listAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset ListItemGUI")[0]);
        listItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(listAssetPath);

        asset.CloneTree(root);

        Label header = root.Q<Label>("Header");
        Button addItem = root.Q<Button>("AddItem");
        Button removeItem = root.Q<Button>("RemoveItem");

        list = root.Q<VisualElement>("ItemList");

        header.text = serializedObject.targetObject.name;

        root.RegisterCallback<ChangeEvent<float>>(UpdateValues);
        root.RegisterCallback<ChangeEvent<UnityEngine.Object>>(ObjectListChanged);
        addItem.RegisterCallback<ClickEvent>(AddItem);
        removeItem.RegisterCallback<ClickEvent>(RemoveItem);

        objectPack = (SpawnableObjectPack)serializedObject.targetObject;

        UpdateList();
        
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        return root;
    }

    private void UpdateValues(ChangeEvent<float> change)
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateList()
    {
        list.Clear();

        foreach (var item in objectPack.spawnableObjects)
        {
            VisualElement newItem = new VisualElement();

            listItemTemplate.CloneTree(newItem);

            UnityEditor.UIElements.ObjectField data = newItem.Q<UnityEditor.UIElements.ObjectField>("DataValue");

            data.SetValueWithoutNotify(item);

            list.Add(newItem);
        }
    }

    private void ObjectListChanged(ChangeEvent<UnityEngine.Object> e)
    {
        UpdatePackObjects();
    }

    private void AddItem(ClickEvent e)
    {
        VisualElement newItem = new VisualElement();
        listItemTemplate.CloneTree(newItem);
        list.Add(newItem);

        UpdatePackObjects();
    }

    private void RemoveItem(ClickEvent e)
    {
        if (list.childCount > 0) 
        {
            list.RemoveAt(list.childCount - 1);
            UpdatePackObjects();
        }
        
    }

    private void UpdatePackObjects()
    {
        objectPack.spawnableObjects.Clear();

        foreach (var obj in list.Children())
        {
            UnityEditor.UIElements.ObjectField data = obj.Q<UnityEditor.UIElements.ObjectField>("DataValue");

            SpawnableObject newObject = ScriptableObject.CreateInstance<SpawnableObject>();

            if (data.value != null)
            {
                try
                {
                    newObject = (SpawnableObject)data.value;
                    objectPack.spawnableObjects.Add(newObject);
                }
                catch
                {
                    Debug.LogError("!! Item Placed In Pack Is Not A Spawnable Object !!");
                }
            }
        }

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
