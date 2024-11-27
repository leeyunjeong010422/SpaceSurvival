using Photon.Realtime;
using UnityEngine;
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

    private class CustomRoomProperty<T> where T : struct
    {
        public CustomRoomProperty(string key, T defaultvValue)
        {
            this.key = key;
            this.defaultValue = defaultvValue;
            this.table = new PhotonHashtable() { { key, defaultvValue } };
        }

        private readonly string key;
        private readonly T defaultValue;
        private readonly PhotonHashtable table;

        public void Set(Room room, T value)
        {
            table[key] = value;
            room.SetCustomProperties(table);
        }

        public T Get(Room room)
        {
            if (room.CustomProperties.TryGetValue(key, out object value))
                return (T)value;
            else
                return defaultValue;
        }
    }

    public const string READY = "Ready";
    public const string LOAD = "Load";
    public const string WINNIN_POINT = "WnPt";
    public const string COLOR_NUBER = "ColorNum";
    public static Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue, Color.cyan, Color.black, Color.white, Color.gray, Color.magenta/* 마지막 원소는 Default color */ };
    public const string LEVEL = "Level";

    private static readonly CustomPlayerProperty<bool> s_ready = new CustomPlayerProperty<bool>(READY, false);
    private static readonly CustomPlayerProperty<bool> s_load = new CustomPlayerProperty<bool>(LOAD, false);
    private static readonly CustomPlayerProperty<int> s_winningPoint = new CustomPlayerProperty<int>(WINNIN_POINT, 0);
    private static readonly CustomPlayerProperty<int> S_colorNumber = new CustomPlayerProperty<int>(COLOR_NUBER, -1);
    private static readonly CustomPlayerProperty<long> s_level = new CustomPlayerProperty<long>(LEVEL, 1);


    public static void SetReady(this Player Player, bool ready) => s_ready.Set(Player, ready);
    public static bool GetReady(this Player Player) => s_ready.Get(Player);

    public static void SetLoad(this Player Player, bool load) => s_load.Set(Player, load);
    public static bool GetLoad(this Player Player) => s_load.Get(Player);

    public static void SetWinningPoint(this Player Player, int point) => s_winningPoint.Set(Player, point);
    public static int GetWinningPoint(this Player Player) => s_winningPoint.Get(Player);
    public static void SetColorNumber(this Player Player, int number) => S_colorNumber.Set(Player, number);
    public static int GetColorNumber(this Player Player) => S_colorNumber.Get(Player);
    public static Color GetNumberColor(this Player Player)
    {
        int number = S_colorNumber.Get(Player);
        if (number == -1)
            number = colors.Length - 1;

        return colors[number];
    }
    public static void SetLevel(this Player Player, long point) => s_level.Set(Player, point);
    public static long GetLevel(this Player Player) => s_level.Get(Player);




    public const string GOAL_POINT = "GlPt";

    private static readonly CustomRoomProperty<int> s_goalPoint = new CustomRoomProperty<int>(GOAL_POINT, 30);

    public static void SetGoalPoint(this Room room, int point) => s_goalPoint.Set(room, point);
    public static int GetGoalPoint(this Room room) => s_goalPoint.Get(room);
}
