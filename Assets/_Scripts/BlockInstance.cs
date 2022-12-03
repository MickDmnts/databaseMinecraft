using UnityEngine;

public class BlockInstance : MonoBehaviour, IInteractable
{
    int blockID = -1;

    public void SetBlockID(int value) => blockID = value;
    public int GetBlockID() => blockID;

    public void DeleteBlock()
    {
        Destroy(gameObject);

        SQLiteHandler.DeleteBlockFromWorld(blockID);
    }

    public void PlaceBlock(BlockType blockType)
    {
        GameObject newBlock = Instantiate(BlockManager.S.GetBlockByType(blockType));
        newBlock.transform.position = transform.position + transform.up;

        newBlock.GetComponent<BlockInstance>().SetBlockID(blockID + 1);

        SQLiteHandler.UpdateBlockInWorld((int)newBlock.transform.position.x, (int)newBlock.transform.position.y, (int)newBlock.transform.position.z, 1 + blockID);
    }
}