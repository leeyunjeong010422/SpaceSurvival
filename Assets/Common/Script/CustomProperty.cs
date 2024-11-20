using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    private static PhotonHashtable customProperty = new PhotonHashtable();

    public const string READY = "Ready";
    public static void SetReady(this Player Player, bool ready)
    {
        customProperty.Clear();
        customProperty.Add(READY, ready);
        Player.SetCustomProperties(customProperty);
    }
    public static bool GetReady(this Player Player)
    {
        PhotonHashtable hashTable = Player.CustomProperties;

        if (!hashTable.ContainsKey(READY)) return false;

        return (bool)hashTable[READY];
    }

    public const string LOAD = "Load";
    public static void SetLoad(this Player Player, bool load)
    {
        customProperty.Clear();
        customProperty.Add(LOAD, load);
        Player.SetCustomProperties(customProperty);
    }
    public static bool GetLoad(this Player load)
    {
        PhotonHashtable hashTable = load.CustomProperties;

        if (!hashTable.ContainsKey(LOAD)) return false;

        return (bool)hashTable[LOAD];
    }
}
