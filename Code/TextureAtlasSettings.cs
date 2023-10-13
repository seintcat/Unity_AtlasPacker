using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureAtlasSettings 
{
    // Horizontal and vertical cell counts are must be same.
    [SerializeField]
    public int atlasSizeInBlocks = 16;

    // Using for UV, when making mesh with code.
    public float normalizedBlockTextureSize
    {
        get { return 1f / (float)atlasSizeInBlocks; }
    }
}
