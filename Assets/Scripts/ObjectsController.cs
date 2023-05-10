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


    //[SerializeField] private List<GameObject> objectPrefabs;
    [SerializeField] private DroppedObject[] droppedObjectsArray;
    [SerializeField] private Box[] boxArray;
    [SerializeField] private List<GameObject> boxesInScene;
    [SerializeField] private ObjectCreator[] objectCreatorArray;
    //[SerializeField] private int numberOfColors;
    //[SerializeField] private int numberOfEachColor;
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


    // Start is called before the first frame update
    void Start()
    {
        objectsInScene = new List<GameObject>();

        foreach(ObjectCreator obj in objectCreatorArray)
        {
            totalNumberOfColors += obj.numberOfColors;
            totalNumberOfObjects += (obj.numberOfColors * obj.numberOfEachColor);
        }

        droppedObjectsArray = new DroppedObject[totalNumberOfColors];
        selectedColors = new List<string>();
       

        string[] boxColors = { "Blue", "Green", "Orange", "Pink", "Purple", "Red", "Yellow" };

        for (int i = 0; i < boxColors.Length; i++)
        {
            boxArray[i].boxColor = boxColors[i];
        }


        foreach (ObjectCreator obj in objectCreatorArray)
        {
            Transform refObjectTransform = obj.referanceObject.transform;

            xOffset = refObjectTransform.localScale.x / 2;
            yOffset = refObjectTransform.position.y + 0.1f;
            zOffset = refObjectTransform.localScale.z / 2;

            float minX = refObjectTransform.position.x - xOffset;
            float maxX = refObjectTransform.position.x + xOffset;

            float minZ = refObjectTransform.position.z - zOffset;
            float maxZ = refObjectTransform.position.z + zOffset;

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

                Debug.Log("objectToInstantiate.name " + objectToInstantiate.name);
                Debug.Log("objectToInstantiate.tag " + objectToInstantiate.tag);
      

                for (int j = 0; j < obj.numberOfEachColor; j++)
                {
                    spawnPoint = new Vector3(Random.Range(minX, maxX), yOffset, Random.Range(minZ, maxZ));

                    Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, obj.objectPrefabs[0].GetComponent<BoxCollider>().size.x / 2, LayerMask.GetMask("Draggable"));
                    if (hitColliders.Length == 0)
                    {
                        draggableObject = Instantiate(objectToInstantiate, spawnPoint, Quaternion.identity);
                        objectsInScene.Add(draggableObject);
                    }
                    else
                        j--;
                }

                obj.objectPrefabs.RemoveAt(randomIndex);

            }

        }

        Debug.Log("selectedColors.Length " + selectedColors.Count);

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
                    Debug.Log("boxesInScene[i].name " + boxesInScene[i].name);
                    Debug.Log("boxesInScene[i].transform.position " + boxesInScene[i].transform.position);
                    Instantiate(box.boxPrefab, boxesInScene[i].transform.position, Quaternion.identity);
                    if (boxesInScene[i].transform.childCount == 1)
                    {
                        Debug.Log("ifffffffff");
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
                    GameManager.instance.LoadNextLevel();
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

}
