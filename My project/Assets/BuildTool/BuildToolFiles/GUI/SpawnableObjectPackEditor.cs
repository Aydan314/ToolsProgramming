#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Search;
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
        // Get GUI elements from UXML //
        root = new VisualElement();
        
        string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset SpawnableObjectPackGUI")[0]);
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

        string listAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset ListItemGUI")[0]);
        listItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(listAssetPath);

        asset.CloneTree(root);

        Label header = root.Q<Label>("Header");
        Button addItem = root.Q<Button>("AddItem");
        
        list = root.Q<VisualElement>("ItemList");

        header.text = serializedObject.targetObject.name;

        root.RegisterCallback<ChangeEvent<float>>(UpdateValues);
        root.RegisterCallback<ChangeEvent<UnityEngine.Object>>(ObjectListChanged);
        addItem.RegisterCallback<ClickEvent>(AddItem);
        
        objectPack = (SpawnableObjectPack)serializedObject.targetObject;

        UpdateList();
        
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        return root;
    }

    private void UpdateValues(ChangeEvent<float> change)
    {
        // Error check if grid size is negativve //
        FloatField gridSize = root.Q<FloatField>("gridSize");

        if (gridSize.value < 0f)
        {
            Debug.LogError("!! Grid size of \"" + serializedObject.targetObject.name + "\" cannot be less than 0 !!");
        }
        
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
        
    }

    private void UpdateList()
    {
        // Reset list of spawnable objects //
        list.Clear();

        if (objectPack.spawnableObjects != null)
        {
            // Populate list with values from scriptable object //
            int buttonIndex = 0;
            foreach (var item in objectPack.spawnableObjects)
            {
                VisualElement newItem = new VisualElement();

                listItemTemplate.CloneTree(newItem);

                UnityEditor.UIElements.ObjectField data = newItem.Q<UnityEditor.UIElements.ObjectField>("DataValue");

                data.SetValueWithoutNotify(item);

                Button button = newItem.Q<Button>("Remove");

                int myIndex = buttonIndex;
                button.RegisterCallback<ClickEvent>(e => RemoveItem(e, myIndex));

                list.Add(newItem);
                buttonIndex++;
            }
        }
    }

    private void ObjectListChanged(ChangeEvent<UnityEngine.Object> e)
    {
        // Error check if grid size 0 or brush not set //
        FloatField gridSize = root.Q<FloatField>("gridSize");
        UnityEditor.UIElements.ObjectField objectField = root.Q<UnityEditor.UIElements.ObjectField>("brush");

        if (gridSize.value == 0f)
        {
            Debug.LogError("!! Grid size of \"" + serializedObject.targetObject.name + "\" cannot be 0 !!");
        }

        if (objectField.value == null)
        {
            Debug.LogError("!! Brush for \"" + serializedObject.targetObject.name + "\" cannot be null !!");
        }

        UpdatePackObjects();
        
    }

    private void AddItem(ClickEvent e)
    {
        // Get list element GUI from UXML //
        VisualElement newItem = new VisualElement();
        listItemTemplate.CloneTree(newItem);

        Button button = newItem.Q<Button>("Remove");
        int buttonIndex = list.childCount;
        button.RegisterCallback<ClickEvent>(e => RemoveItem(e, buttonIndex));

        list.Add(newItem);

        UpdatePackObjects();
    }

    private void RemoveItem(ClickEvent e, int buttonIndex)
    {
        if (list.childCount > 0) 
        {
            // Remove list entry //
            UnityEditor.UIElements.ObjectField data = list.ElementAt(buttonIndex).Q<UnityEditor.UIElements.ObjectField>("DataValue");

            if (data.value != null) list.RemoveAt(buttonIndex);
            UpdatePackObjects();
            UpdateList();
            
        }
        
    }

    private void UpdatePackObjects()
    {
        if (objectPack.spawnableObjects != null)
        {
            // Clear scriptable objects list //
            objectPack.spawnableObjects.Clear();

            // Populate list with values from GUI elements //
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

            // Save changes //
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(objectPack);
            AssetDatabase.SaveAssetIfDirty(objectPack);

        }
    }
}

#endif