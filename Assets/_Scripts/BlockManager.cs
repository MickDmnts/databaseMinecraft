using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager S;

    private void Awake()
    {
        S = this;
    }

    [SerializeField] List<GameObject> blocks;

    public GameObject GetBlockByType(BlockType blockType)
    {
        GameObject temp = null;

        switch (blockType)
        {
            case BlockType.Dirt:
                temp = blocks[0];
                break;

            case BlockType.Stone:
                temp = blocks[1];
                break;
        }

        return temp;
    }

    private void OnDestroy()
    {
        S = null;
    }
}
