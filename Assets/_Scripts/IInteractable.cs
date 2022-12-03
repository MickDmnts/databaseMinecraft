public enum BlockType
{
    Dirt,
    Stone,
}

public interface IInteractable
{
    public void PlaceBlock(BlockType blockType);
    public void DeleteBlock();
}
