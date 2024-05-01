public class Singelton
{
    private static PlayerMovementHandler playerMovementHandler;

    public static void SetPlayerInputHandler(PlayerMovementHandler _playerMovementHandler)
    {
        playerMovementHandler = _playerMovementHandler;
    }

    public static PlayerMovementHandler GetPlayerInputHandler()
    {
        return playerMovementHandler;
    }
}
