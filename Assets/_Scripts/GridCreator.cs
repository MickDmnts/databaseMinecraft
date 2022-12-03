using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    [SerializeField] int worldSize;
    [SerializeField] float tileSize;

    [SerializeField] GameObject stonePrefab;

    List<Vector3> blocks;

    int idCount = 0;

    private async void Start()
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        await CreateWorld(cancellationTokenSource.Token);
    }

    async Task CreateWorld(CancellationToken token)
    {
        blocks = new List<Vector3>();

        SQLiteHandler.CreateNewBlockTable();

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
                block.transform.SetParent(transform, true);

                block.GetComponent<BlockInstance>().SetBlockID(idCount++);

                blocks.Add(blockPos);

                SQLiteHandler.AddBlockToWorld((int)blockPos.x, (int)blockPos.y, (int)blockPos.z);

                await Task.Delay(5);
            }
        }
    }
}
