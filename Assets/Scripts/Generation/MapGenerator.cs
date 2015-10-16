using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public enum MapType
    {
       Space, Assholevillage, Dimension512LRQ
    };

    private string filePath = "Prefabs/";
    //private MapType dungeonType;
    [SerializeField]
    private int minimumRooms = 15;
    [SerializeField]
    private int numRooms = 0;

    private Object[] roomArray;
    private Object[] hallArray;
    //private GameObject[] capArray;
    private MapModule entranceModule;

    [HideInInspector]
    public GameObject currentDungeon;
    private List<GameObject> existingModules = new List<GameObject>();

    void Start()
    {
        GenerateMap();//MapType.Dongs);
    }

    public void GenerateMap()// MapType inputType)
    {
        //dungeonType = inputType;
        //switch (dungeonType)
        //{
        //    case MapType.Dongs:
        //        filePath += "Ruins/";
        //        break;
        //    default:
        //        Debug.LogError("No valid MapType passed");
        //        break;
        //}
        string name = "map";//dungeonType.ToString();
        currentDungeon = new GameObject(name);
        roomArray = Resources.LoadAll(filePath + "Rooms", typeof(GameObject));
        hallArray = Resources.LoadAll(filePath + "Hallways", typeof(GameObject));
        //capArray = (GameObject[])Resources.LoadAll(filePath + "Caps", typeof(GameObject));
        entranceModule = (Instantiate((GameObject)GetRandom(Resources.LoadAll(filePath + "Rooms", typeof(GameObject))))).GetComponent<MapModule>();
        existingModules.Add(entranceModule.gameObject);
        entranceModule.transform.parent = currentDungeon.transform;
        StartCoroutine(Generation(entranceModule));
    }

    IEnumerator Paint()
    {
        yield return null;
    }

    IEnumerator Generation(MapModule start)
    {
        int count = 0;
        List<RoomConnection> availableConnectors = new List<RoomConnection>();
        availableConnectors.AddRange(start.GetConnectors());
        while (numRooms < minimumRooms)
        {

            List<RoomConnection> newConnectors = new List<RoomConnection>();

            int currentIterations = 0;
            int maxIterations = 1;
            float acceptableTime = 1f / 60f;
            if (currentIterations >= maxIterations) //my weird coroutine optimization
            {
                if (Time.deltaTime <= acceptableTime)
                {
                    maxIterations++;
                }
                else
                {
                    maxIterations--;
                }
                currentIterations = 0;
                yield return null;
            }
            foreach (RoomConnection connector in availableConnectors)
            {
                if (currentIterations >= maxIterations) //my weird coroutine optimization
                {
                    if (Time.deltaTime <= acceptableTime)
                    {
                        maxIterations++;
                    }
                    else
                    {
                        maxIterations--;
                    }
                    currentIterations = 0;
                    yield return null;
                }
                currentIterations++;

                MapModule.Type type = connector.GetComponentInParent<MapModule>().moduleType;
                GameObject newModulePrefab;
                List<Object> prefabList = new List<Object>();
                if (type == MapModule.Type.Room)
                {
                    //if (Random.Range(0, 3) < 2) //Hall
                    //{
                        prefabList.AddRange(hallArray);
                    //}
                    //else //Room
                    //{
                        //prefabList.AddRange(roomArray);
                    //}
                }
                else// type == MapModule.Type.Hall or entrance
                {
                    prefabList.AddRange(roomArray);
                    //prefabList.AddRange(hallArray);
                }

                MapModule newModule = null;
                RoomConnection[] newModuleConnectors = new RoomConnection[0];
                RoomConnection connectorToMatch;

                bool intersects;

                do//if the new prefab intersects with another piece, then we have to pick a different prefab
                {
                    intersects = false;

                    if (newModule != null)
                    {
                        Destroy(newModule.gameObject);
                    }
                    if (prefabList.Count != 0)
                    {
                        count++;

                        newModulePrefab = (GameObject)GetRandom(prefabList.ToArray());
                        newModule = (MapModule)Instantiate(newModulePrefab).GetComponent<MapModule>();
                        newModuleConnectors = newModule.GetConnectors();

                        connectorToMatch = FindMatchingConnector(connector, newModuleConnectors);

                        if (connectorToMatch != null)
                        {
                            MatchConnectors(connector, connectorToMatch);
                            newModule.transform.parent = currentDungeon.transform;
                            //newModule.transform.localScale = new Vector3(0.99f, 0.99f, 0.99f);
                        }

                        else
                        {
                            prefabList.Remove(newModulePrefab);
                            Destroy(newModule.gameObject);
                            newModule = null;
                        }                    

                        if (newModule != null)
                        {
                            foreach (GameObject module in existingModules)
                            {
                                if (module != connector.transform.parent.gameObject)
                                {
                                    foreach (PolygonCollider2D oldCollider in module.GetComponentsInChildren<PolygonCollider2D>())
                                    {
                                        foreach (PolygonCollider2D newCollider in newModule.GetComponentsInChildren<PolygonCollider2D>())
                                        {
                                            //if (oldMesh.bounds.Intersects(newMesh.bounds))
                                            if (oldCollider.bounds.Intersects(newCollider.bounds))
                                            {
                                                intersects = true;
                                                prefabList.Remove(newModulePrefab);
                                                break;
                                            }
                                        }
                                        if (intersects) break;
                                    }
                                    if (intersects) break;
                                }
                            }
                            //newModule.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        
                    }
                    else
                    {
                        connectorToMatch = null;
                    }
                } while (intersects);

                //newModule.transform.parent = currentDungeon.transform;
                connector.partner = connectorToMatch;
                if (connectorToMatch != null)
                {
                    if (connectorToMatch.transform.GetComponentInParent<MapModule>().moduleType == MapModule.Type.Room)
                    {
                        numRooms++;
                    }
                    connectorToMatch.transform.parent.parent = currentDungeon.transform;
                    existingModules.Add(connectorToMatch.transform.parent.gameObject);
                    connector.partner.partner = connector;
                    newConnectors.AddRange(newModuleConnectors.Where(c => c != connectorToMatch));
                }
                else
                {
                    newConnectors.Add(connector);

                }
            }


            availableConnectors = newConnectors;
        }

        List<MapModule> modsToTrim = new List<MapModule>();
        foreach (RoomConnection c in availableConnectors)
        {
            MapModule module = c.GetComponentInParent<MapModule>();
            if (module != null && module.moduleType == MapModule.Type.Hall)
            {
                RoomConnection[] connectors = module.GetConnectors();
                if (connectors.Length > 2)
                {
                    int i = 0;
                    foreach (RoomConnection con in connectors)
                    {
                        if (con.partner != null)
                        {
                            i++;
                        }
                    }
                    if (i <= 1)
                    {
                        modsToTrim.Add(module);
                    }
                }
                else
                {
                    modsToTrim.Add(module);
                }
            }
        }

        foreach (MapModule m in modsToTrim)
        {
            Destroy(m.gameObject);
        }

        yield return null;
    }

    private void MatchConnectors(RoomConnection oldConnector, RoomConnection newConnector)
    {
        //bool 

        Transform newModule = newConnector.transform.parent;
        //Vector3 forwardVectorToMatch = -oldConnector.transform.up;
        //float correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newConnector.transform.up);
        //newModule.RotateAround(newConnector.transform.position, Vector3.forward, correctiveRotation);
        Vector3 correctiveTranslation = oldConnector.transform.position - newConnector.transform.position;
        newModule.transform.position += correctiveTranslation;
    }

    private RoomConnection FindMatchingConnector(RoomConnection oldConnector, RoomConnection[] newConnectors)
    {
        RoomConnection con;

        con = null;

        foreach(RoomConnection connect in newConnectors)
        {
            Vector3 forwardVectorToMatch = oldConnector.transform.up;
            float correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(connect.transform.up);

            if(Mathf.Abs(correctiveRotation) == 180)
            {
                con = connect;

                break;
            }
        }

        return con;
    }

    private static TItem GetRandom<TItem>(TItem[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(-Vector3.up, vector) * Mathf.Sign(vector.x);
    }
}
