using System;

namespace RL_Map_Loader.Models
{
    public class Events
    {
        public delegate void MapDeletedEventHandler(MapDeletedEventArgs e);

        public static event MapDeletedEventHandler MapDeleted;

        private static void OnMapDeleted(MapDeletedEventArgs e) => MapDeleted?.Invoke(e);

        public class MapDeletedEventArgs : EventArgs
        {
            public Map Map;

            public MapDeletedEventArgs(Map map) => Map = map;
        }
    }
}
