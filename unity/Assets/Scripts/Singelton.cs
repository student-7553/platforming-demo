public class Singelton
{
    private static Player player;

    public static void SetPlayer(Player _player)
    {
        player = _player;
    }

    public static Player GetPlayer()
    {
        return player;
    }
}
