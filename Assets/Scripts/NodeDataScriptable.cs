using System;
using UnityEngine;

namespace ShinobiPathfinder
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NodeDataScriptable", order = 1)]
    public class NodeDataScriptable : ScriptableObject
    {
        public string nodeName;
        public Route[] routes;

        // Used by Dijkstra for optimisation reasons
        [HideInInspector]
        public int index;
    }
    [Serializable]
    public class Route
    {
        public NodeDataScriptable target;
        public RouteData[] options;
    }

    [Serializable]
    public class RouteData
    {
        public RouteType type;
        public int days;
        public int hours;
        public int minutes;

        // Used for Dijkstra calculations
        public int FullTimeInMinutes
        {
            get
            {
                return (days * 24 + hours) * 60 + minutes;
            }
        }
    }

    public enum RouteType
    {
        Walking = 0,
        Caravan = 1,
        Ninja = 2,
        Ship,
        Train,
        Dirigible
    }
}