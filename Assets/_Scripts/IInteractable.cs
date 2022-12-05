public enum BlockType
{
    None = -1,

    Dirt = 0,
    Stone = 1,
}

public interface IInteractable
{
    public void PlaceBlock(BlockType blockType);
    public void DeleteBlock();
}
