using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    private class CustomPlayerProperty<T> where T : struct
    {
        public CustomPlayerProperty(string key, T defaultvValue)
        {
            this.key = key;
            this.defaultValue = defaultvValue;
            this.table = new PhotonHashtable() { { key, defaultvValue } };
        }

        private readonly string key;
        private readonly T defaultValue;
        private readonly PhotonHashtable table;

        public void Set(Player player, T value)
        {
            table[key] = value;
            player.SetCustomProperties(table);
        }

        public T Get(Player player)
        {
            if (player.CustomProperties.TryGetValue(key, out object value))
                return (T)value;
            else
                return defaultValue;
        }
    }

    public const string READY = "Ready";
    public const string LOAD = "Load";

    private static readonly CustomPlayerProperty<bool> s_ready = new CustomPlayerProperty<bool>(READY, false);
    private static readonly CustomPlayerProperty<bool> s_load = new CustomPlayerProperty<bool>(LOAD, false);

    public static void SetReady(this Player Player, bool ready) => s_ready.Set(Player, ready);
    public static bool GetReady(this Player Player) => s_ready.Get(Player);

    public static void SetLoad(this Player Player, bool load) => s_load.Set(Player, load);
    public static bool GetLoad(this Player Player) => s_load.Get(Player);
}
