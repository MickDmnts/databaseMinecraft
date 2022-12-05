using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] int worldSize;
    [SerializeField] float tileSize;
    [SerializeField] Transform blocksParent;
    [SerializeField] GameObject stonePrefab;

    List<Vector3> blocks;

    CancellationTokenSource cancellationTokenSource;

    private void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    private void Start()
    {
        if (!SQLiteHandler.CheckForCreatedWorld())
        {
            CreateWorld();
        }
        else
        {
            LoadPastWorld();
        }
    }

    public async void CreateWorld()
    {
        Task buildTask = CreateNewWorld(cancellationTokenSource.Token);

        try
        {
            SQLiteHandler.CreateNewBlockTable();
            SQLiteHandler.SetHasWorldValue(true);

            await buildTask;
        }
        catch (Exception e)
        {
            Debug.LogFormat("New world canceled {0}", e);
        }
    }

    async Task CreateNewWorld(CancellationToken token)
    {
        blocks = new List<Vector3>();

        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                if (token.IsCancellationRequested) break;

                Vector3 blockPos = new Vector3(x, 0, z);

                //Create world block
                GameObject block = Instantiate(stonePrefab);
                block.transform.position = new Vector3(blockPos.x * tileSize, blockPos.y * tileSize, blockPos.z * tileSize);
                block.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                block.transform.SetParent(blocksParent, true);

                BlockInstance blockInstance = block.GetComponent<BlockInstance>();
                blockInstance.SetBlockID(SQLiteHandler.GetWorldBlockCount());

                blocks.Add(blockPos);

                SQLiteHandler.AddBlockToWorld((int)blockPos.x, (int)blockPos.y, (int)blockPos.z, (int)blockInstance.GetBlockType(), blockInstance.GetBlockID());
                SQLiteHandler.SetWorldBlockCount(SQLiteHandler.GetWorldBlockCount() + 1);

                await Task.Delay(1, token);
            }
        }
    }

    public async void LoadPastWorld()
    {
        Task buildTask = LoadWorldData(cancellationTokenSource.Token);

        await buildTask;

        try
        {
            await buildTask;
        }
        catch (Exception e)
        {
            Debug.LogFormat("Past world canceled {0}", e);
        }
    }

    async Task LoadWorldData(CancellationToken token)
    {
        blocks = new List<Vector3>();

        int blockCount = SQLiteHandler.GetWorldBlockCount();

        for (int i = 1; i != blockCount; i++)
        {
            if (token.IsCancellationRequested) break;

            BlockInfo blockInfo = SQLiteHandler.GetBlockInfo(i);
            Vector3 blockPos = blockInfo.Position;

            //Create world block
            GameObject block = Instantiate(BlockManager.S.GetBlockByType((BlockType)blockInfo.BlockType));
            block.transform.position = new Vector3(blockPos.x * tileSize, blockPos.y * tileSize, blockPos.z * tileSize);
            block.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
            block.transform.SetParent(blocksParent, true);

            BlockInstance blockInstance = block.GetComponent<BlockInstance>();
            blockInstance.SetBlockID(i);

            blocks.Add(blockPos);

            await Task.Delay(1, token);
        }
    }

    public void ResetDatabaseWorldState()
    {
        SQLiteHandler.SetHasWorldValue(false);
        SQLiteHandler.SetWorldBlockCount(0);
        SQLiteHandler.CreateNewBlockTable();

        cancellationTokenSource.Cancel();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif

#if UNITY_STANDALONE && !UNITY_EDITOR
        SQLiteHandler.CreateNewLocalTables();
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();

        cancellationTokenSource = new CancellationTokenSource();
    }
}
