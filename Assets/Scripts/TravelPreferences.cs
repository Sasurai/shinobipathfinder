using UnityEngine;

namespace ShinobiPathfinder
{
    public class TravelPreferences
    {
        // Use ninja by default
        private RouteType prefRouteType = RouteType.Ninja;
        public RouteType PrefRouteType
        {
            get { return prefRouteType; }
            set
            {
                switch (value)
                {
                    case RouteType.Walking:
                    case RouteType.Caravan:
                    case RouteType.Ninja:
                        prefRouteType = value;
                        return;
                    default:
                        Debug.LogError("Only Walking, Caravan and Ninja are supported as preferred travel types, using ninja by default");
                        prefRouteType = RouteType.Ninja;
                        return;
                }
            }
        }

        // Use everything by default
        public bool UseShips { get; set; } = true;
        public bool UseTrains { get; set; } = true;
        public bool UseDirigibles { get; set; } = true;
    }
}
