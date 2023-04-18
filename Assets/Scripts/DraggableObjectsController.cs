using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObjectsController : MonoBehaviour
{
    [System.Serializable]
    private class DroppedObjects
    {
        public string objectColor;
        public List<GameObject> droppedObjectsList;
    }


    [SerializeField] private List<GameObject> objectPrefabs;
    [SerializeField] private DroppedObjects[] droppedObjectsArray;
    private List<GameObject> objectsInScene;
    [SerializeField] private GameObject referanceObject;
    private float xOffset, yOffset, zOffset;
    [SerializeField] private int numberOfColors;
    [SerializeField] private int numberOfEachColor;
    private int randomIndex;
    private Vector3 spawnPoint;
    private GameObject draggableObject;
    private GameObject objectToInstantiate;

   
    // Start is called before the first frame update
    void Start()
    {
        xOffset = referanceObject.transform.localScale.x / 2;
        yOffset = objectPrefabs[0].transform.position.y;
        zOffset= referanceObject.transform.localScale.z / 2;
        objectsInScene = new List<GameObject>();
        droppedObjectsArray = new DroppedObjects[numberOfColors];

        float minX = referanceObject.transform.position.x - xOffset;
        float maxX = referanceObject.transform.position.x + xOffset;

        float minZ = referanceObject.transform.position.z - zOffset;
        float maxZ = referanceObject.transform.position.z + zOffset;

        for (int i=0; i<numberOfColors; i++)
        {
            randomIndex = Random.Range(0, objectPrefabs.Count);
            objectToInstantiate = objectPrefabs[randomIndex];
           
            droppedObjectsArray[i] = new DroppedObjects();
            droppedObjectsArray[i].droppedObjectsList = new List<GameObject>();

            droppedObjectsArray[i].objectColor = objectToInstantiate.tag;

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
        }
    }

    public void AddDroppedObject(GameObject droppedObject, string objectTag)
    {
        foreach(DroppedObjects element in droppedObjectsArray)
        {
            if (element.objectColor == objectTag)
            {
                element.droppedObjectsList.Add(droppedObject);
            }
        }
    }

    public bool IsDroppedListEmpty(string objectTag)
    {
        foreach(DroppedObjects element in droppedObjectsArray)
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
        foreach (DroppedObjects element in droppedObjectsArray)
        {
            if (element.objectColor == objectTag)
            {
                return element.droppedObjectsList[element.droppedObjectsList.Count - 1];
            }
        }

        return null;
    }

}
