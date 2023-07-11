using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
    [System.Serializable]
    private class DroppedObject
    {
        public string objectColor;
        public List<GameObject> droppedObjectsList;
    }

    [System.Serializable]
    private class Box
    {
        public string boxColor;
        public GameObject boxPrefab;
    }

    [System.Serializable]
    private class ObjectCreator
    {
        public GameObject referanceObject;
        public List<GameObject> objectPrefabs;
        public int numberOfColors;
        public int numberOfEachColor;
    }

    
    [SerializeField] private DroppedObject[] droppedObjectsArray;
    [SerializeField] private Box[] boxArray;
    [SerializeField] private List<GameObject> boxesInScene;
    [SerializeField] private List<GameObject> createdBoxes;
    [SerializeField] private ObjectCreator[] objectCreatorArray;
    private GameManager gameManager;

    private int totalNumberOfObjects;
    private int totalNumberOfColors;
    private int numberOfDroppedObjects = 0;

    [SerializeField] private List<string> selectedColors;
    private List<GameObject> objectsInScene;
    private GameObject draggableObject;
    private GameObject objectToInstantiate;

    private int randomIndex;
    private Vector3 spawnPoint;
    private float xOffset, yOffset, zOffset;
    private float radius;
    private levelTypes selectedType;
    

    // Start is called before the first frame update
    void Start()
    {   
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        selectedType = GameObject.Find("LevelTypesController").GetComponent<LevelTypes>().GetSelectedLevelType();

        objectsInScene = new List<GameObject>();

        foreach (ObjectCreator obj in objectCreatorArray)
        {
            totalNumberOfColors += obj.numberOfColors;
            totalNumberOfObjects += (obj.numberOfColors * obj.numberOfEachColor);
        }

        droppedObjectsArray = new DroppedObject[totalNumberOfColors];
        selectedColors = new List<string>();
       

        string[] boxColors = { "Blue", "Green", "Orange", "Pink", "Purple", "Red", "Yellow" };

        if (boxArray.Length != 0)
        {
            for (int i = 0; i < boxColors.Length; i++)
                boxArray[i].boxColor = boxColors[i];
            
        }
        
        foreach (ObjectCreator obj in objectCreatorArray)
        {
            Transform refObjectTransform = obj.referanceObject.transform;
            BoxCollider collider = obj.referanceObject.GetComponent<BoxCollider>();

            for (int i = 0; i < obj.numberOfColors; i++)
            {
                randomIndex = Random.Range(0, obj.objectPrefabs.Count);

                foreach(string color in selectedColors)
                {
                  if(color == obj.objectPrefabs[randomIndex].tag)
                  {
                        obj.objectPrefabs.RemoveAt(randomIndex);
                        randomIndex = Random.Range(0, obj.objectPrefabs.Count);
                  }
                }
                
                objectToInstantiate = obj.objectPrefabs[randomIndex];

                selectedColors.Add(objectToInstantiate.tag);

                //Debug.Log("objectToInstantiate.name " + objectToInstantiate.name);
                //Debug.Log("objectToInstantiate.tag " + objectToInstantiate.tag);

                float minX = collider.bounds.min.x;
                float maxX = collider.bounds.max.x;

                float minY = collider.bounds.min.y;
                float maxY = collider.bounds.max.y;

                float minZ = collider.bounds.min.z;
                float maxZ = collider.bounds.max.z;
                
               
                if (selectedType == levelTypes.Torus)
                {
                    yOffset = collider.transform.position.y + objectToInstantiate.transform.GetComponent<BoxCollider>().size.y/2;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x;
                    zOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.z*2;
                    radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.x;
                    Debug.Log("minx " + minX + "maxx " + maxX + "minz " + minZ + "maxz " + maxZ);
                }
                else if (selectedType == levelTypes.Pencil)
                {
                    yOffset = collider.transform.position.y + objectToInstantiate.transform.GetComponent<BoxCollider>().size.z/2;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.y;
                    zOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x;
                    //radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.y/2;
                    radius = 0.16f;
                    Debug.Log("minx " + minX + "maxx " + maxX + "minz " + minZ + "maxz " + maxZ);
                }
                else if (selectedType == levelTypes.Book)
                {
                    yOffset = collider.transform.position.y + objectToInstantiate.transform.GetComponent<BoxCollider>().size.z / 2;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.y;
                    zOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x;
                    radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.y;
                    Debug.Log("minx " + minX + "maxx " + maxX + "minz " + minZ + "maxz " + maxZ);
                }
                else if (selectedType == levelTypes.Field)
                {
                    yOffset = collider.transform.position.y + objectToInstantiate.transform.GetComponent<BoxCollider>().size.y/2 - 0.01f;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x;
                    zOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.z/2;
                    radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.x/2;
                    Debug.Log("minx " + minX + "maxx " + maxX + "minz " + minZ + "maxz " + maxZ);
                }
                else if (selectedType == levelTypes.Tree)
                {
                    yOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.y;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x;
                    zOffset = collider.bounds.min.z - objectToInstantiate.transform.GetComponent<BoxCollider>().size.z*2;
                    radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.x / 2;
                    Debug.Log("minx " + minX + "maxx " + maxX + "minz " + minZ + "maxz " + maxZ);
                }
                else if (selectedType == levelTypes.Train)
                {
                    yOffset = collider.transform.position.y + objectToInstantiate.transform.GetComponent<BoxCollider>().size.y/2;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x*2;
                    zOffset = obj.referanceObject.transform.position.z;
                    radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.x+0.02f;
                    Debug.Log("minx " + minX + "maxx " + maxX + "minz " + minZ + "maxz " + maxZ);
                }
                else if (selectedType == levelTypes.Car)
                {
                    yOffset = collider.transform.position.y;
                    xOffset = objectToInstantiate.transform.localScale.x * 8;
                    zOffset = objectToInstantiate.transform.localScale.z * 2;
                    radius = obj.objectPrefabs[0].transform.localScale.x * 2.5f;
                }
                else if (selectedType == levelTypes.Demo)
                {
                    yOffset = collider.transform.position.y;
                    xOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.x;
                    zOffset = objectToInstantiate.transform.GetComponent<BoxCollider>().size.z - 0.2f;
                    radius = objectToInstantiate.transform.localScale.x/2;
                }


                for (int j = 0; j < obj.numberOfEachColor; j++)
                {
                    if (selectedType == levelTypes.Train)
                    {
                        spawnPoint = new Vector3(obj.referanceObject.transform.GetComponent<BoxCollider>().bounds.min.x+xOffset, yOffset, zOffset);
                        xOffset += objectToInstantiate.transform.GetComponent<BoxCollider>().size.z/2;
                    }
                    else if(selectedType == levelTypes.Tree)
                    {
                        spawnPoint = new Vector3(Random.Range(minX + xOffset, maxX - xOffset), Random.Range(minY + yOffset, maxY - yOffset), zOffset);
                    }
                    else
                        spawnPoint = new Vector3(Random.Range(minX + xOffset, maxX - xOffset), yOffset, Random.Range(minZ + zOffset, maxZ - zOffset));

                    //Debug.Log("SPAWN POINTTTTTTTTTTT " + spawnPoint);
                    Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, radius, LayerMask.GetMask("Draggable"));
                    if (hitColliders.Length == 0)
                    {
                        draggableObject = Instantiate(objectToInstantiate, spawnPoint, objectToInstantiate.transform.rotation);
                        objectsInScene.Add(draggableObject);
                    }
                    else
                        j--;
                }

                obj.objectPrefabs.RemoveAt(randomIndex);

            }
        }


        for (int i = 0; i < selectedColors.Count; i++)
        {
            Debug.Log("selectedColor " + selectedColors[i]);
            droppedObjectsArray[i] = new DroppedObject();
            droppedObjectsArray[i].droppedObjectsList = new List<GameObject>();
            droppedObjectsArray[i].objectColor = selectedColors[i];

            foreach (Box box in boxArray)
            {
                if (box.boxColor == selectedColors[i])
                {
                    GameObject newBox = Instantiate(box.boxPrefab, boxesInScene[i].transform.position, boxesInScene[i].transform.rotation);
                    createdBoxes.Add(newBox);
                    if (boxesInScene[i].transform.childCount == 1)
                    {
                        boxesInScene[i].transform.GetChild(0).tag = selectedColors[i] + "Box";
                        boxesInScene[i].transform.GetChild(0).parent = null;
                    }
                    Destroy(boxesInScene[i]);
                    break;
                }
            }
        }

    }


    public void AddDroppedObject(GameObject droppedObject, string objectTag)
    {
        foreach(DroppedObject obj in droppedObjectsArray)
        {
            if (obj.objectColor == objectTag)
            {
                obj.droppedObjectsList.Add(droppedObject);
                numberOfDroppedObjects++;

                if (numberOfDroppedObjects == totalNumberOfObjects)
                {
                    gameManager.ShowLevelEndPanel();
                }
            }
        }
    }

    public bool IsDroppedListEmpty(string objectTag)
    {
        foreach(DroppedObject obj in droppedObjectsArray)
        {
            if (obj.objectColor == objectTag)
            {
                return obj.droppedObjectsList.Count == 0;
            }
        }

        return false;
    }

    public GameObject GetLastDroppedObject(string objectTag)
    {
        foreach (DroppedObject obj in droppedObjectsArray)
        {
            if (obj.objectColor == objectTag)
            {
                return obj.droppedObjectsList[obj.droppedObjectsList.Count - 1];
            }
        }

        return null;
    }

    public List<GameObject> GetBoxes()
    {
        return createdBoxes;
    }

}
