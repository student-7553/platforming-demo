public class Singelton
{
    private static PlayerState playerState;

    public static void SetPlayerState(PlayerState _playerState)
    {
        playerState = _playerState;
    }

    public static PlayerState GetPlayerState()
    {
        return playerState;
    }
}
