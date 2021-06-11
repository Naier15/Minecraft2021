using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Random,
    Stone,
    Grass,
    Dirty,
    Badrock,
    Water
}

public class WorldGenerator : MonoBehaviour
{
    public GameObject kriper;
    public GameObject voxel;
    public GameObject stone;
    public GameObject dirty;
    public GameObject water;
    public GameObject badrock;
    public int size;
    public int terra_height;
    public List<Vector3> water_blocks = new List<Vector3>();
    public List<GameObject> all_blocks = new List<GameObject>();

    public void CreateVoxel(Vector3 position, BlockType blockType)
    {
        if (blockType == BlockType.Random)
        {
            int i = Random.Range(1, 4);
            switch (i)
            {
                case 1:
                    blockType = BlockType.Grass;
                    break;
                case 2:
                    blockType = BlockType.Dirty;
                    break;
                case 3:
                    blockType = BlockType.Stone;
                    break;
            }
        }

        GameObject go = null;
        switch (blockType)
        {
            case BlockType.Grass:
                go = voxel;
                break;
            case BlockType.Stone:
                go = stone;
                break;
            case BlockType.Dirty:
                go = dirty;
                break;
            case BlockType.Badrock:
                go = badrock;
                break;
            case BlockType.Water:
                go = water;
                break;
        }

        if (go != null)
        {
            var gg = Instantiate(go, position, Quaternion.identity);
            if (blockType == BlockType.Water)
                water_blocks.Add(position);
            all_blocks.Add(gg);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                var h = terra_height;
                for (var y = 0; y < h; y++)
                {
                    bool chance = Random.value > 0.8;

                    if (((x == size / 2) || (x == size / 2 - 1) || (x == size / 2 + 1)) && (y == terra_height - 1 || y == terra_height - 2))
                    { CreateVoxel(new Vector3(x, y, z), BlockType.Water); }
                    else if (y == 0 || y < 3 && chance)
                    { CreateVoxel(new Vector3(x, y, z), BlockType.Badrock); } 
                    else if (x == 0 || x == size-1 || z == 0 || z == size-1)
                    { CreateVoxel(new Vector3(x, y, z), BlockType.Badrock); }
                    else if (y + 1 == h)
                    { CreateVoxel(new Vector3(x, y, z), BlockType.Grass); }
                    else
                    { CreateVoxel(new Vector3(x, y, z), BlockType.Random); }                    
                }
            }
        }
        for(int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                if ((x == 0 || x == size-1) || (z == 0 || z == size - 1)){
                    for (int k = 0; k < 3; ++k)
                        CreateVoxel(new Vector3(x, terra_height+k, z), BlockType.Badrock);
                }
            }
        }

        int mount_y = terra_height * 5;
        int i = 0;
        int j = 0;
        for (int x = size - 1; x > 0; --x)
        {
            int mount_y2 = mount_y - i;
            for (int z = size - 1; z > 0; --z)
            {
                for (int y = mount_y2; y >= terra_height; y--)
                { CreateVoxel(new Vector3(x, y, z), BlockType.Grass); }
                mount_y2--;
            }
            if (j % 3 == 0)
            { i++; j++; }
            else { i += 2; j++; }
        }
        mount_y = terra_height * 3;
        i = 0; j = 0;
        for (int x = size - 1; x > 0; --x)
        {
            int mount_y2 = mount_y - i;
            for (int z = 0; z < size; ++z)
            {
                for (int y = mount_y2; y >= terra_height; y--)
                { CreateVoxel(new Vector3(x, y, z), BlockType.Grass); }
                mount_y2--;
            }
            if (j % 2 == 0)
            { i++; j++; }
            else { i += 3; j++; }
        }
        for (int k = 0; k < 5; ++k)
        { Instantiate(kriper, new Vector3(Random.Range(2, size), terra_height*5+3, Random.Range(2, size)), Quaternion.identity); }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
