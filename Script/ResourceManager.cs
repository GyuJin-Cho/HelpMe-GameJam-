using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private static ResourceManager instance;
    
    public Sprite[] numberSprites;
    public Sprite[] sprites;
    public Card[] cards;
    public GameObject[] CharPrefabs;
    public Sprite[] endingImages;
    
    void Awake()
    {
        instance = this;
    }

    // 캐릭터 프리팹 관리하실때 쓰세요
    public GameObject GetPrefabWithName(string prefabName)
    {
        foreach (var prefab in CharPrefabs)
        {
            if (prefab.name == prefabName)
            {
                return prefab;
            }
        }

        return null;
    }

    public Sprite GetEndingImageWithName(string endingImageName)
    {
        foreach (var image in endingImages)
        {
            if (image.name == endingImageName)
            {
                return image;
            }
        }

        return null;
    }
}
