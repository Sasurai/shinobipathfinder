using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ShinobiPathfinder
{
    // This is ugly but should work :)
    public class RouteEntryView : MonoBehaviour
    {
        [SerializeField]
        private Text _fromText;
        [SerializeField]
        private Text _toText;
        [SerializeField]
        private Text _modeText;
        [SerializeField]
        private Text _durationText;

        public void SetData(NodeDataScriptable origin, NodeDataScriptable target, TravelPreferences preferences)
        {
            _fromText.text = origin.nodeName;
            _toText.text = target.nodeName;
            var usedRoute = preferences.PickBestTravelOption(GetRoute(origin, target));
            _modeText.text = RouteTypeTexts[usedRoute.type];
            _durationText.text = GetDurationText(usedRoute);
        }

        private Route GetRoute(NodeDataScriptable origin, NodeDataScriptable target)
        {
            foreach(var route in origin.routes)
            {
                if (route.target == target) return route;
            }

            Debug.LogError($"Wrong origin-target combination when trying to print route. From: {origin.nodeName} To: {target.nodeName}");
            return null;
        }

        private string GetDurationText(RouteData data)
        {
            var builder = new StringBuilder();

            if (data.days > 0) builder.Append("Días: ").Append(data.days).Append(", ");
            if (data.hours > 0) builder.Append("Horas: ").Append(data.hours).Append(", ");
            if (data.minutes > 0) builder.Append("Minutos: ").Append(data.minutes).Append(", ");

            // Remove trailing comma and whitespace
            builder.Remove(builder.Length - 2, 2);
            return builder.ToString();
        }

        // Probably want to move this to some utils or something
        Dictionary<RouteType, string> RouteTypeTexts = new Dictionary<RouteType, string> { 
            { RouteType.Walking, "A pie" },
            { RouteType.Caravan, "Caravana" },
            { RouteType.Ninja, "Ninja" },
            { RouteType.Ship, "Barco" },
            { RouteType.Train, "Tren" },
            { RouteType.Dirigible, "Dirigible" }
        };
    }
}
