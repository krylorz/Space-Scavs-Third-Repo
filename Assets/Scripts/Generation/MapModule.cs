using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapModule : MonoBehaviour
{
    public enum Type
    {
        Room, Hall, Cap
    };

    public Type moduleType;

    private RoomConnection[] connectors;

    void Start()
    {
        if (connectors.Length == 0)
        {
            Debug.Log("Highly Probable Error: Each module should have at least one connector. Make sure the connector is a child of the parent.");
        }
        else if (moduleType == Type.Cap)
        {
            if (connectors.Length != 1)
            {
                Debug.Log("Probable Error: Caps are dead-ends to halls or cover up an open connector, and are thus intended to have only one connection.");
            }
        }
    }

    public RoomConnection[] GetConnectors()
    {
        if (connectors == null)
        {
            connectors = GetComponentsInChildren<RoomConnection>();
        }
        return connectors;
    }
}
