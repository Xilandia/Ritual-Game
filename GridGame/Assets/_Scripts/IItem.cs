public interface IItem
{
    public bool[,] GetHitMap();
    public int[,] GetDamageMap();

    public void DebugItem();
}
