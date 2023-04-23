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


    [SerializeField] private List<GameObject> objectPrefabs;
    [SerializeField] private DroppedObject[] droppedObjectsArray;
    [SerializeField] private Box[] boxArray;
    [SerializeField] private List<GameObject> boxesInScene;
    [SerializeField] private GameObject referanceObject;
    [SerializeField] private int numberOfColors;
    [SerializeField] private int numberOfEachColor;
    private int totalNumberOfObjects;
    private int numberOfDroppedObjects = 0;

    private string[] selectedColors;
    private List<GameObject> objectsInScene;
    private GameObject draggableObject;
    private GameObject objectToInstantiate;

    private float xOffset, yOffset, zOffset;
    private int randomIndex;
    private Vector3 spawnPoint;

   
    // Start is called before the first frame update
    void Start()
    {
        xOffset = referanceObject.transform.localScale.x / 2;
        yOffset = objectPrefabs[0].transform.position.y;
        zOffset= referanceObject.transform.localScale.z / 2;
        objectsInScene = new List<GameObject>();
        droppedObjectsArray = new DroppedObject[numberOfColors];
        selectedColors = new string[numberOfColors];
        totalNumberOfObjects = numberOfColors * numberOfEachColor;

        float minX = referanceObject.transform.position.x - xOffset;
        float maxX = referanceObject.transform.position.x + xOffset;

        float minZ = referanceObject.transform.position.z - zOffset;
        float maxZ = referanceObject.transform.position.z + zOffset;

        string[] boxColors = { "Blue", "Green", "Orange", "Pink", "Purple", "Red", "Yellow" };

        for (int i = 0; i < boxColors.Length; i++)
        {
            boxArray[i].boxColor = boxColors[i];
        }


        for (int i=0; i<numberOfColors; i++)
        {
            randomIndex = Random.Range(0, objectPrefabs.Count);
            objectToInstantiate = objectPrefabs[randomIndex];
           
            droppedObjectsArray[i] = new DroppedObject();
            droppedObjectsArray[i].droppedObjectsList = new List<GameObject>();

            droppedObjectsArray[i].objectColor = objectToInstantiate.tag;
            selectedColors[i] = objectToInstantiate.tag;


            for (int j = 0; j < numberOfEachColor; j++)
            {
                spawnPoint = new Vector3(Random.Range(minX, maxX), yOffset, Random.Range(minZ, maxZ));
                
                Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, objectPrefabs[0].GetComponent<BoxCollider>().size.x/2, LayerMask.GetMask("Draggable"));
                if (hitColliders.Length == 0)
                {
                    draggableObject = Instantiate(objectToInstantiate, spawnPoint, Quaternion.identity);
                    objectsInScene.Add(draggableObject);
                }
                else
                    j--;
            }

            objectPrefabs.RemoveAt(randomIndex);

            foreach(Box box in boxArray)
            {
                if (box.boxColor == selectedColors[i])
                {
                    Instantiate(box.boxPrefab, boxesInScene[i].transform.position, Quaternion.identity);
                    if (boxesInScene[i].transform.childCount != 0)
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
        foreach(DroppedObject element in droppedObjectsArray)
        {
            if (element.objectColor == objectTag)
            {
                element.droppedObjectsList.Add(droppedObject);
                numberOfDroppedObjects++;

                if (numberOfDroppedObjects == totalNumberOfObjects)
                {
                    GameManager.instance.LoadNextLevel();
                }
            }
        }
    }

    public bool IsDroppedListEmpty(string objectTag)
    {
        foreach(DroppedObject element in droppedObjectsArray)
        {
            if (element.objectColor == objectTag)
            {
                return element.droppedObjectsList.Count == 0;
            }
        }

        return false;
    }

    public GameObject GetLastDroppedObject(string objectTag)
    {
        foreach (DroppedObject element in droppedObjectsArray)
        {
            if (element.objectColor == objectTag)
            {
                return element.droppedObjectsList[element.droppedObjectsList.Count - 1];
            }
        }

        return null;
    }

}
