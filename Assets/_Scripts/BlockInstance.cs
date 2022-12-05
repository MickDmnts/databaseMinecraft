using UnityEngine;

public class BlockInstance : MonoBehaviour, IInteractable
{
    int blockID = -1;
    [SerializeField] BlockType blockType;

    public void SetBlockID(int value) => blockID = value;
    public int GetBlockID() => blockID;

    public BlockType GetBlockType() => blockType;

    public void DeleteBlock()
    {
        Destroy(gameObject);

        SQLiteHandler.DeleteBlockFromWorld(blockID);
    }

    public void PlaceBlock(BlockType blockType)
    {
        GameObject newBlock = Instantiate(BlockManager.S.GetBlockByType(blockType));

        //Calculate normal ray hit here and send info through the interaction.
        newBlock.transform.position = transform.position + transform.up;
        //--------------------------------------------------------------------

        newBlock.transform.SetParent(BlockManager.S.transform, true);
        BlockInstance inst = newBlock.GetComponent<BlockInstance>();

        inst.SetBlockID(SQLiteHandler.GetWorldBlockCount() + 1);
        SQLiteHandler.SetWorldBlockCount(SQLiteHandler.GetWorldBlockCount() + 1);

        SQLiteHandler.AddBlockToWorld((int)newBlock.transform.position.x, (int)newBlock.transform.position.y, (int)newBlock.transform.position.z, (int)blockType, inst.GetBlockID());
    }
}
