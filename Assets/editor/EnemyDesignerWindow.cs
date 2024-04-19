using Type;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;


public class EnemyDesignerWindow : EditorWindow {

    Texture2D headerSectionTexture;
    Texture2D mageSectionTexture;
    Texture2D warriorSectionTexture;
    Texture2D rogueSectionTexture;

    static MageData mageData;
    static WarriorData warriorData;
    static RogueData rogueData;

    public static MageData MageInfo{get{return mageData;}}
    public static WarriorData WarriorInfo{get{return warriorData;}}
    public static RogueData RogueInfo{get{return rogueData;}}
    

    Color headerSectionColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    Rect headerSection;
    Rect mageSection;
    Rect warriorSection;
    Rect rogueSection;

    [MenuItem("EditorScripting/Enemy Designer")]
    private static void OpenWindow() {
        var window = GetWindow<EnemyDesignerWindow>();
        window.titleContent = new GUIContent("EnemyDesignerWindow");
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    private void OnEnable() {
        InitTextures();
        InitData();
    }

    public static void InitData(){
        mageData = (MageData)CreateInstance(typeof(MageData));
        warriorData = (WarriorData)CreateInstance(typeof(WarriorData));
        rogueData = (RogueData)CreateInstance(typeof(RogueData));
    }
    void InitTextures(){
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        mageSectionTexture = Resources.Load<Texture2D>("icons/editor mage");
        warriorSectionTexture = Resources.Load<Texture2D>("icons/editor warrior");
        rogueSectionTexture = Resources.Load<Texture2D>("icons/editor rogue");
    }

    private void OnGUI() {
        DrawLayout();
        DrawHeader();
        DrawMageSettings();
        DrawWarriorSettings();
        DrawRogueSettings();
    }

    private void DrawLayout(){
        headerSection = new Rect(0, 0, position.width,50);


        mageSection = new Rect(0, 50, position.width/3f,position.width-50);
        warriorSection = new Rect(position.width/3f, 50, position.width/3f,position.width-50);
        rogueSection = new Rect(position.width/3f*2, 50, position.width/3f,position.width-50);

        GUI.DrawTexture(headerSection, headerSectionTexture);
        GUI.DrawTexture(mageSection, mageSectionTexture);
        GUI.DrawTexture(warriorSection, warriorSectionTexture);
        GUI.DrawTexture(rogueSection, rogueSectionTexture);
    }

    private void DrawHeader(){
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("Enemy Designer", EditorStyles.boldLabel);

        GUILayout.EndArea();
    }

    private void DrawMageSettings(){
        GUILayout.BeginArea(mageSection);

        GUILayout.Label("Mage", EditorStyles.boldLabel);
        mageData.dmgType = (MageDmgType)EditorGUILayout.EnumPopup("Damage Type", mageData.dmgType);
        mageData.weaponType = (MageWeaponType)EditorGUILayout.EnumPopup("Weapon Type", mageData.weaponType);
        
        
        if(GUILayout.Button("Creat",GUILayout.Height(40)))
            GeneralSettings.OpenWindow(GeneralSettings.SettingsType.Mage);
            
        GUILayout.EndArea();
    }

    private void DrawWarriorSettings(){
        GUILayout.BeginArea(warriorSection);

        GUILayout.Label("Warrior", EditorStyles.boldLabel);
        warriorData.classType = (WarriorClassType)EditorGUILayout.EnumPopup("Damage Type", warriorData.classType);
        warriorData.weaponType = (WarriorWeaponType)EditorGUILayout.EnumPopup("Weapon Type", warriorData.weaponType);

        if(GUILayout.Button("Creat",GUILayout.Height(40)))
            GeneralSettings.OpenWindow(GeneralSettings.SettingsType.Warrior);


        GUILayout.EndArea();
    }

    private void DrawRogueSettings(){
        GUILayout.BeginArea(rogueSection);

        GUILayout.Label("Rogue", EditorStyles.boldLabel);
        rogueData.weaponType = (RogueWeaponType)EditorGUILayout.EnumPopup("Damage Type", rogueData.weaponType);
        rogueData.strategyType = (RogueStategyType)EditorGUILayout.EnumPopup("Damage Type", rogueData.strategyType);

        if(GUILayout.Button("Creat",GUILayout.Height(40)))
            GeneralSettings.OpenWindow(GeneralSettings.SettingsType.Rogue);

        GUILayout.EndArea();
    }
}

public class GeneralSettings:EditorWindow{
    public enum SettingsType{
        Mage,
        Warrior,
        Rogue
    }

    static SettingsType settingsType;
    static GeneralSettings window;

    public static void OpenWindow(SettingsType type){
        settingsType = type;
        window = (GeneralSettings)GetWindow(typeof(GeneralSettings));
        window.minSize = new Vector2(250, 200);
        window.Show();
    }


    void OnGUI(){
        switch(settingsType){
            case SettingsType.Mage:
                DrawingSettings((CharacterData)EnemyDesignerWindow.MageInfo);
                break;
            case SettingsType.Warrior:
                DrawingSettings((CharacterData)EnemyDesignerWindow.WarriorInfo);
                break;
            case SettingsType.Rogue:
                DrawingSettings((CharacterData)EnemyDesignerWindow.RogueInfo);
                break;
        }
    }

    void DrawingSettings(CharacterData data){
        data.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", data.prefab, typeof(GameObject), false);
        data.maxHealth = EditorGUILayout.FloatField("Max Health", data.maxHealth);
        data.maxEnerge = EditorGUILayout.FloatField("Max Energe", data.maxEnerge);
        data.critChance = EditorGUILayout.Slider("Crit Chance", data.critChance,0,100);
        data.power = EditorGUILayout.Slider("Power", data.power,0,100);
        data.name = EditorGUILayout.TextField("Name", data.name);

        if(data.prefab == null)
            EditorGUILayout.HelpBox("Prefab is not selected", MessageType.Warning);

        if(GUILayout.Button("Finish and Save",GUILayout.Height(30))){
            SaveCharacterData();
            window.Close();
        }
    }

    private void SaveCharacterData(){
        string prefabPath;
        string newPrefabPath = "Assets/prefabs/characters/";
        string dataPath = "Assets/Resources/CharacterData/data/";

        switch(settingsType){
            case SettingsType.Mage:
                dataPath +="mage/"+EnemyDesignerWindow.MageInfo.name+".asset";
                AssetDatabase.CreateAsset(EnemyDesignerWindow.MageInfo, dataPath);
                
                newPrefabPath += "mage/" +EnemyDesignerWindow.MageInfo.name+".prefab";
                prefabPath = AssetDatabase.GetAssetPath(EnemyDesignerWindow.MageInfo.prefab);
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject magePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if(!magePrefab.GetComponent<Mage>())
                    magePrefab.AddComponent<Mage>();
                magePrefab.GetComponent<Mage>().data = EnemyDesignerWindow.MageInfo;
                break;         
            case SettingsType.Warrior:
                  dataPath +="warrior/"+EnemyDesignerWindow.WarriorInfo.name+".asset";
                AssetDatabase.CreateAsset(EnemyDesignerWindow.WarriorInfo, dataPath);
                
                newPrefabPath += "warrior/" +EnemyDesignerWindow.WarriorInfo.name+".prefab";
                prefabPath = AssetDatabase.GetAssetPath(EnemyDesignerWindow.WarriorInfo.prefab);
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject warriorPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if(!warriorPrefab.GetComponent<Warrior>())
                    warriorPrefab.AddComponent<Warrior>();
                warriorPrefab.GetComponent<Warrior>().data = EnemyDesignerWindow.WarriorInfo;
                break;
            case SettingsType.Rogue:
                  dataPath +="Rogue/"+EnemyDesignerWindow.RogueInfo.name+".asset";
                AssetDatabase.CreateAsset(EnemyDesignerWindow.RogueInfo, dataPath);
                
                newPrefabPath += "Rogue/" +EnemyDesignerWindow.RogueInfo.name+".prefab";
                prefabPath = AssetDatabase.GetAssetPath(EnemyDesignerWindow.RogueInfo.prefab);
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                GameObject RoguePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
                if(!RoguePrefab.GetComponent<Rogue>())
                    RoguePrefab.AddComponent<Rogue>();
                RoguePrefab.GetComponent<Rogue>().data = EnemyDesignerWindow.RogueInfo;
                break;
        }

    }
}