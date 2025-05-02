
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;

public class PackEditorGUI : EditorWindow
{
    
    VisualElement root;
    VisualTreeAsset listItemTemplate;
    VisualElement list;
    VisualElement imageList;
    Label toolActive;
    UnityEditor.UIElements.ObjectField activePack;
    public SpawnableObjectPackManager manager;
    
    BuildTool buildTool;

    [MenuItem("BuildTool/Create Pack Editor Window")]
    public static void ShowWindow()
    {
        PackEditorGUI wnd = GetWindow<PackEditorGUI>();
        wnd.titleContent = new GUIContent("BuildTool Pack Editor");
    }
    public void CreateGUI()
    {
        // Get GUI elements from UXML //
        root = rootVisualElement;

        string assetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset PackManagerGUI")[0]);
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

        string listAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset PackListItemGUI")[0]);
        listItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(listAssetPath);

        asset.CloneTree(root);
        Button addItem = root.Q<Button>("AddItem");

        list = root.Q<VisualElement>("PackList");
        imageList = root.Q<VisualElement>("ImageList");
        activePack = root.Q<UnityEditor.UIElements.ObjectField>("SelectedPack");
        toolActive = root.Q<Label>("toolActive");

        if (manager == null) Debug.LogError("!! Object Pack Editor Not Set !!");

        GameObject found = GameObject.FindGameObjectWithTag("BuildTool");

        if (found == null) Debug.LogError("!! Cannot find build tool in scene !!");
        else buildTool = found.GetComponent<BuildTool>();
        
        root.RegisterCallback<ChangeEvent<UnityEngine.Object>>(ObjectListChanged);
        addItem.RegisterCallback<ClickEvent>(AddItem);

        UpdateList();
        UpdateSelectedPack();
    }

    private void UpdateSelectedPack()
    {
        // Get default spawnable object icon //
        string iconPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:Texture2D SpawnableIcon")[0]);
        Texture2D defaultTex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

        activePack.value = manager.GetActiveAssetPack();

        // Update largest preview with first item in new pack //
        if (manager.GetActiveAssetPack() != null)
        {
            if (manager.GetActiveAssetPack().spawnableObjects.Count > 0) imageList.style.backgroundImage = AssetPreview.GetAssetPreview(manager.GetActiveAssetPack().spawnableObjects[0].objectPrefab);
            else imageList.style.backgroundImage = defaultTex;
        }
        else imageList.style.backgroundImage = defaultTex;

        // Iterate through other 3 smaller previews and set with pack prefab icons //
        int i = 1;
        foreach (var item in imageList.Children())
        {
            if ( manager.GetActiveAssetPack() != null)
            {
                if (manager.GetActiveAssetPack().spawnableObjects.Count > i)
                {
                    item.style.backgroundImage = AssetPreview.GetAssetPreview(manager.GetActiveAssetPack().spawnableObjects[i].objectPrefab);
                }
                else
                {
                    item.style.backgroundImage = defaultTex;
                }
            }
            else
            {
                item.style.backgroundImage = defaultTex;
            }

            i++;
        }

    }

    private void Update()
    {
        if (buildTool != null)
        {
            if (buildTool.toolActive)
            {
                toolActive.text = "-Build Tool Enabled -";
                toolActive.style.backgroundColor = new Color(0.25f, 0.5f, 0.25f);
            }
            else
            {
                toolActive.text = "-Build Tool Disabled -";
                toolActive.style.backgroundColor = new Color(0.5f, 0.25f, 0.25f);
            }
        }
        else
        {
            toolActive.text = "!! Cannot Find Tool !!";
        }
    }

    private void UpdateList()
    {
        // Clear list of packs //
        list.Clear();

        // Populate list with packs from scriptable object //
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
            // Extract data from selected GUI field //
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
        // Get list item GUI from UXML //
        VisualElement newItem = new VisualElement();
        listItemTemplate.CloneTree(newItem);

        // Add functionality to GUI //
        Button select = newItem.Q<Button>("Select");
        Button remove = newItem.Q<Button>("Remove");

        int buttonIndex = list.childCount;

        select.clicked += () => SelectItem(null, buttonIndex);
        remove.clicked += () => RemoveItem(null, buttonIndex);
            
        list.Add(newItem);

        UpdatePackManager();
    }

    private void RemoveItem(ClickEvent e, int buttonIndex)
    {
        if (list.childCount > 0)
        {
            // Removve item from pack list //
            UnityEditor.UIElements.ObjectField data = list.ElementAt(buttonIndex).Q<UnityEditor.UIElements.ObjectField>("DataValue");

            if (data.value != null) list.RemoveAt(buttonIndex);
            UpdatePackManager();
            UpdateList();

        }

    }

    private void UpdatePackManager()
    {
        // Clear scriptable Objects list of packs //
        manager.assetBasePacks.Clear();

        // Populate list with values from GUI //
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

        EditorUtility.SetDirty(manager);
        AssetDatabase.SaveAssetIfDirty(manager);

        UpdateSelectedPack();
    }
}
