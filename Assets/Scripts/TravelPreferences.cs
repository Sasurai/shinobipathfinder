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

        // TODO: I've seen that some routes have both walking/caravan/ninja and ship/train/dirigible options, not sure this will work as it is
        public RouteData PickBestTravelOption(Route route)
        {
            foreach (var opt in route.options)
            {
                var type = opt.type;
                if (type == PrefRouteType)
                {
                    return opt;
                }
                // TODO if multiple options of ship/train/dirigible are available, it'll pick the first one and ignore the others, even if shorter
                if (type == RouteType.Ship && UseShips)
                {
                    return opt;
                }
                if (type == RouteType.Train && UseTrains)
                {
                    return opt;
                }
                if (type == RouteType.Dirigible && UseDirigibles)
                {
                    return opt;
                }
            }
            return null;
        }
    }
}
