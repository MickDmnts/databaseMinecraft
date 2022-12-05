using System.Collections.Generic;
using UnityEngine;

public struct BlockInfo
{
    public Vector3 Position;
    public int BlockType;
}

public class BlockManager : MonoBehaviour
{
    public static BlockManager S;

    [SerializeField] List<GameObject> blocks;

    private void Awake()
    {
        S = this;
    }

    public GameObject GetBlockByType(BlockType blockType)
    {
        return blocks[(int)blockType];
    }

    private void OnDestroy()
    {
        S = null;
    }
}
