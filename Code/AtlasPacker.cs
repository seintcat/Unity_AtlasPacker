#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO; 

public class AtlasPacker : EditorWindow 
{
    // Pixel counts of one block.
    Vector2Int blockSize = new Vector2Int(1, 1);
    // Horizontal and vertical cell counts are must be same.
    int atlasSizeInBlocks = 16;
    // atlasSize = blockSize * atlasSizeInBlocks
    Vector2Int atlasSize = new Vector2Int();

    // rawTextures = resource input
    Object[] rawTextures;
    // sortedTextures = A collection of resources without errors
    List<Texture2D> sortedTextures = new List<Texture2D>();
    // Result of AtlasPacker.
    Texture2D atlas;

    string saveDirectory = "/Resources/Atlas/Result/";
    string saveFileName = "Packed_Atlas";
    // Resource outside the Resources folder are not available.
    string loadFolderDirectory = "Atlas/Input";

    string logs = "";
    Vector2 scrollPos = new Vector2();

    [MenuItem ("Custom Tools/2D Atlas Packer")] 

    public static void ShowWindow()
    {
        GetWindow(typeof(AtlasPacker));
    }

    private void OnGUI() {
        CalculateAtlasSize();
        GUILayout.Label("2D Atlas Packer", EditorStyles.boldLabel);

        // Editable fields
        blockSize = EditorGUILayout.Vector2IntField("Block Size", blockSize);
        atlasSizeInBlocks = EditorGUILayout.IntField("Atlas Size (in blocks)", atlasSizeInBlocks);
        saveDirectory = EditorGUILayout.TextField("Atlas Result Directory", saveDirectory);
        saveFileName = EditorGUILayout.TextField("Result File Name", saveFileName);
        loadFolderDirectory = EditorGUILayout.TextField("Image Folder Directory", loadFolderDirectory);

        // Help
        if (GUILayout.Button("Atlas Packer Help"))
        {
            logs = "Atlas Packer Manual";
            logs += "\nBlock Size";
            logs += "\n >> pixels of 1 block's X or Y";
            logs += "\nAtlas Size";
            logs += "\n >> blocks of total result's X or Y";
            logs += "\nAtlas Result Directory";
            logs += "\n >> Unity project/Assets~~";
            logs += "\nResult File Name";
            logs += "\n >> result file will save as ~~~.png";
            logs += "\nImage Folder Directory";
            logs += "\n >> Unity project/Assets/Resources/~~";
            logs += "\n\n***** PLEASE make input image *****";
            logs += "\n >> Default/2D";
            logs += "\n >> Alpha is transparency";
            logs += "\n >> Read/Write enable";
            logs += "\n >> Wrap Mode = repeat";
        }
        // Help(KOR)
        if (GUILayout.Button("Atlas Packer Help(KOR)"))
        {
            logs = "전체 다 한글화하기는 좀 귀찮아서 메뉴얼만 한글버전 추가 작성\n\nAtlas Packer 사용법";
            logs += "\nBlock Size";
            logs += "\n >> 한개 블록의 XY방향 픽셀 수";
            logs += "\nAtlas Size";
            logs += "\n >> 아틀라스에 XY방향으로 몇 개의 블록이 들어가는지";
            logs += "\nAtlas Result Directory";
            logs += "\n >> Unity project/Assets~~";
            logs += "\nResult File Name";
            logs += "\n >> ~~~.png로 저장될 예정";
            logs += "\nImage Folder Directory";
            logs += "\n >> Unity project/Assets/Resources/~~";
            logs += "\n\n***** 아틀라스 재료 이미지들 설정 필수 *****";
            logs += "\n >> Default/2D";
            logs += "\n >> Alpha is transparency";
            logs += "\n >> Read/Write enable";
            logs += "\n >> Wrap Mode = repeat";
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(600), GUILayout.Height(600));

        GUILayout.Label(logs);
        GUILayout.Label(atlas);

        EditorGUILayout.EndScrollView();

        // Load Textures
        if (GUILayout.Button("Load Textures"))
        { 
            LoadTextures();
            PackAtlas();
            logs += "\nAtlas Packer: Textures loaded.";
        }

        // Clear Textures
        if (GUILayout.Button("Clear Textures"))
        {
            atlas = new Texture2D(atlasSize.x, atlasSize.y);
            logs = "Atlas Packer: Textures cleared.";
        }

        // Save Atlas to .png
        if (GUILayout.Button("Save Atlas"))
        {
            byte[] bytes = atlas.EncodeToPNG();
            try
            {
                // Path
                File.WriteAllBytes(Application.dataPath + saveDirectory + saveFileName + ".png", bytes);
                logs = "Atlas Packer: Atlas file sucessfully saved. " + bytes.Length;
            }
            catch
            {
                logs = "Atlas Packer: Couldn't save atlas to file.";
            }
        }
    }

    void LoadTextures () {
        logs = "Loading Textures...";
        
        sortedTextures.Clear();

        // Get all Texture2D in "loadFolderName" folder.
        rawTextures = Resources.LoadAll(loadFolderDirectory, typeof(Texture2D));

        // Test to see if it's available.
        foreach (Object tex in rawTextures) {
            Texture2D t = (Texture2D)tex;
            if (t.width == blockSize.x && t.height == blockSize.y)
                sortedTextures.Add(t);
            else
                logs += "\nAsset Packer: " + tex.name + " incorrect size. Texture not loaded.";
        }

        logs += "\nAtlas Packer: " + sortedTextures.Count + " successfully loaded.";
    }

    // Make Atlas.
    void PackAtlas () {
        atlas = new Texture2D(atlasSize.x, atlasSize.y); 
        Color[] pixels = new Color[atlasSize.x * atlasSize.y];

        for (int x = 0; x < atlasSize.x; x++)
            for (int y = 0; y < atlasSize.y; y++)
            {
                int currentBlockX = x / blockSize.x; 
                int currentBlockY = y / blockSize.y; 
                int index = currentBlockY * atlasSizeInBlocks + currentBlockX; 

                int currentPixelX = x - (currentBlockX * blockSize.x);
                int currentPixelY = y - (currentBlockY * blockSize.y);

                if (index < sortedTextures.Count)
                    // UV will start down left, end at up right. 
                    pixels[y * atlasSize.y + x] = sortedTextures[index].GetPixel(x, y);
                else
                    pixels[y * atlasSize.y + x] = new Color(0f, 0f, 0f, 0f);
            }

        atlas.SetPixels(pixels);
        atlas.Apply();
    }

    void CalculateAtlasSize()
    {
        atlasSize = blockSize * atlasSizeInBlocks;

        rawTextures = new Object[atlasSize.x * atlasSize.y];
    }
}
#endif