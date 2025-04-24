using Unity.VisualScripting;
using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

[CustomEditor(typeof(SpawnableObjectPackManager)), CanEditMultipleObjects]
public class PackEditorGUI : Editor
{

    VisualElement root;
    VisualTreeAsset listItemTemplate;
    VisualElement list;
    VisualElement imageList;
    UnityEditor.UIElements.ObjectField activePack;
    SpawnableObjectPackManager manager;
    public override VisualElement CreateInspectorGUI()
    {
        
        root = new VisualElement();

        string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset PackManagerGUI")[0]);
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

        string listAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset PackListItemGUI")[0]);
        listItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(listAssetPath);

        asset.CloneTree(root);
        Button addItem = root.Q<Button>("AddItem");

        list = root.Q<VisualElement>("PackList");
        imageList = root.Q<VisualElement>("ImageList");
        activePack = root.Q<UnityEditor.UIElements.ObjectField>("SelectedPack");

        manager = (SpawnableObjectPackManager)serializedObject.targetObject;


        root.RegisterCallback<ChangeEvent<float>>(UpdateValues);
        root.RegisterCallback<ChangeEvent<UnityEngine.Object>>(ObjectListChanged);
        addItem.RegisterCallback<ClickEvent>(AddItem);



        UpdateList();
        UpdateSelectedPack();


        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

       

        return root;
    }

    private void UpdateSelectedPack()
    {
        activePack.value = manager.GetActiveAssetPack();
        string iconPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:Texture2D SpawnableIcon")[0]);
        Texture2D defaultTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

        if (manager.GetActiveAssetPack().spawnableObjects.Count > 0)
        {
            imageList.style.backgroundImage = AssetPreview.GetAssetPreview(manager.GetActiveAssetPack().spawnableObjects[0].objectPrefab);
        }
        else imageList.style.backgroundImage = defaultTex;

        int i = 1;
        foreach (var item in imageList.Children())
        {
            if (manager.GetActiveAssetPack().spawnableObjects.Count > i)
            {
                item.style.backgroundImage = AssetPreview.GetAssetPreview(manager.GetActiveAssetPack().spawnableObjects[i].objectPrefab);
            }
            else
            {
                item.style.backgroundImage = defaultTex;
            }
            i++;
        }
    }

    private void UpdateValues(ChangeEvent<float> change)
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateList()
    {
        list.Clear();

        int buttonIndex = 0;

        foreach (var item in manager.assetBasePacks)
        {
            
            VisualElement newItem = new VisualElement();

            listItemTemplate.CloneTree(newItem);

            UnityEditor.UIElements.ObjectField data = newItem.Q<UnityEditor.UIElements.ObjectField>("DataValue");

            data.SetValueWithoutNotify(item);

            Button select = newItem.Q<Button>("Select");
            Button remove = newItem.Q<Button>("Remove");

            int myIndex = buttonIndex;

            select.RegisterCallback<ClickEvent>(e => SelectItem(e, myIndex));
            remove.RegisterCallback<ClickEvent>(e => RemoveItem(e, myIndex));

            list.Add(newItem);
            buttonIndex++;
        }

        
    }

    private void ObjectListChanged(ChangeEvent<UnityEngine.Object> e)
    {
        UpdatePackManager();
    }

    private void SelectItem(ClickEvent e, int buttonIndex)
    {
        if (list.childCount > 0)
        {
            UnityEditor.UIElements.ObjectField data = list.ElementAt(buttonIndex).Q<UnityEditor.UIElements.ObjectField>("DataValue");
            if (data.value != null)
            {
                manager.selectedPack = (SpawnableObjectPack)data.value;
            }
        }

        UpdateSelectedPack();
    }

    private void AddItem(ClickEvent e)
    {
        VisualElement newItem = new VisualElement();
        listItemTemplate.CloneTree(newItem);

        Button select = newItem.Q<Button>("Select");
        Button remove = newItem.Q<Button>("Remove");

        int buttonIndex = list.childCount;
        select.RegisterCallback<ClickEvent>(e => SelectItem(e, buttonIndex));
        remove.RegisterCallback<ClickEvent>(e => RemoveItem(e, buttonIndex));

        list.Add(newItem);

        UpdatePackManager();
    }

    private void RemoveItem(ClickEvent e, int buttonIndex)
    {
        if (list.childCount > 0)
        {
            UnityEditor.UIElements.ObjectField data = list.ElementAt(buttonIndex).Q<UnityEditor.UIElements.ObjectField>("DataValue");

            if (data.value != null) list.RemoveAt(buttonIndex);
            UpdatePackManager();
            UpdateList();

        }

    }

    private void UpdatePackManager()
    {
        manager.assetBasePacks.Clear();

        foreach (var obj in list.Children())
        {
            UnityEditor.UIElements.ObjectField data = obj.Q<UnityEditor.UIElements.ObjectField>("DataValue");

            SpawnableObjectPack newObject = ScriptableObject.CreateInstance<SpawnableObjectPack>();

            if (data.value != null)
            {
                try
                {
                    newObject = (SpawnableObjectPack)data.value;
                    manager.assetBasePacks.Add(newObject);
                }
                catch
                {
                    Debug.LogError("!! Item Placed In List Is Not A Spawnable Object Pack !!");
                }
            }
        }

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        UpdateSelectedPack();
    }
}
