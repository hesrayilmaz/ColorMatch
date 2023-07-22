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
        public bool isSoundPlayed = false;
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
        public Transform soundsParent;
        public List<AudioSource> objectSounds;
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
    [SerializeField] private List<AudioSource> soundsToBePlayed;
    private List<GameObject> objectsInScene;
    private GameObject draggableObject;
    private GameObject objectToInstantiate;

    private int randomIndex;
    private Vector3 spawnPoint;
    private float xOffset, yOffset, zOffset;
    private float radius;
    private levelTypes selectedType;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        selectedType = GameObject.Find("LevelTypesController").GetComponent<LevelTypes>().GetSelectedLevelType();
    }

    // Start is called before the first frame update
    void Start()
    {   
        objectsInScene = new List<GameObject>();

        foreach (ObjectCreator obj in objectCreatorArray)
        {
            totalNumberOfColors += obj.numberOfColors;
            totalNumberOfObjects += (obj.numberOfColors * obj.numberOfEachColor);

            for(int i = 0; i < obj.soundsParent.childCount; i++)
            {
                obj.objectSounds.Add(obj.soundsParent.GetChild(i).GetComponent<AudioSource>());
            }
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
            BoxCollider refCollider = obj.referanceObject.GetComponent<BoxCollider>();

            for (int i = 0; i < obj.numberOfColors; i++)
            {
                randomIndex = Random.Range(0, obj.objectPrefabs.Count);

                bool sameColorFound = true;
                while (sameColorFound)
                {
                    sameColorFound = false; // Assume no same color found
                    foreach (string color in selectedColors)
                    {
                        if (color == obj.objectPrefabs[randomIndex].tag)
                        {
                            obj.objectPrefabs.RemoveAt(randomIndex);
                            obj.objectSounds.RemoveAt(randomIndex);
                            randomIndex = Random.Range(0, obj.objectPrefabs.Count);
                            sameColorFound = true; // Set flag to continue checking
                            break;
                        }
                    }
                }

                objectToInstantiate = obj.objectPrefabs[randomIndex];
                selectedColors.Add(objectToInstantiate.tag);

                BoxCollider currentObjCollider = objectToInstantiate.GetComponent<BoxCollider>();

                float minX = refCollider.bounds.min.x;
                float maxX = refCollider.bounds.max.x;

                float minY = refCollider.bounds.min.y;
                float maxY = refCollider.bounds.max.y;

                float minZ = refCollider.bounds.min.z;
                float maxZ = refCollider.bounds.max.z;
                
               
                if (selectedType == levelTypes.Torus)
                {
                    yOffset = refCollider.transform.position.y + currentObjCollider.size.y / 2;
                    xOffset = currentObjCollider.size.x;
                    zOffset = currentObjCollider.size.z * 2;
                    radius = currentObjCollider.size.x;
                }
                else if (selectedType == levelTypes.Pencil)
                {
                    yOffset = refCollider.transform.position.y + currentObjCollider.size.z / 2;
                    xOffset = currentObjCollider.size.y;
                    zOffset = currentObjCollider.size.x;
                    //radius = obj.objectPrefabs[0].GetComponent<BoxCollider>().size.y/2;
                    radius = 0.16f;
                }
                else if (selectedType == levelTypes.Book)
                {
                    yOffset = refCollider.transform.position.y + currentObjCollider.size.z / 2;
                    xOffset = currentObjCollider.size.y;
                    zOffset = currentObjCollider.size.x;
                    radius = currentObjCollider.size.y;
                }
                else if (selectedType == levelTypes.Field)
                {
                    yOffset = refCollider.transform.position.y + currentObjCollider.size.y / 2;
                    xOffset = currentObjCollider.size.x;
                    zOffset = currentObjCollider.size.z / 2;
                    radius = currentObjCollider.size.x / 2;
                }
                else if (selectedType == levelTypes.Tree)
                {
                    yOffset = currentObjCollider.size.y;
                    xOffset = currentObjCollider.size.x;
                    zOffset = refCollider.bounds.min.z - currentObjCollider.size.z * 3;
                    radius = currentObjCollider.size.x / 2;
                } 
                else if (selectedType == levelTypes.Train)
                {
                    yOffset = refCollider.transform.position.y + currentObjCollider.size.y / 2;
                    xOffset = currentObjCollider.size.x * (2.5f);
                    zOffset = obj.referanceObject.transform.position.z;
                    radius = currentObjCollider.size.x + 0.02f;
                }
                else if (selectedType == levelTypes.Car)
                {
                    yOffset = refCollider.transform.position.y;
                    xOffset = objectToInstantiate.transform.localScale.x * 8;
                    zOffset = objectToInstantiate.transform.localScale.z * 2;
                    radius = obj.objectPrefabs[0].transform.localScale.x * 2.5f;
                }
                else if (selectedType == levelTypes.Tutorial)
                {
                    yOffset = refCollider.transform.position.y;
                    xOffset = currentObjCollider.size.x;
                    zOffset = currentObjCollider.size.z - 0.2f;
                    radius = objectToInstantiate.transform.localScale.x/2;
                }


                for (int j = 0; j < obj.numberOfEachColor; j++)
                {
                    if (selectedType == levelTypes.Train)
                    {
                        spawnPoint = new Vector3(refCollider.bounds.min.x+xOffset, yOffset, zOffset);
                        xOffset += currentObjCollider.size.z/2;
                    }
                    else if(selectedType == levelTypes.Tree)
                    {
                        spawnPoint = new Vector3(Random.Range(minX + xOffset, maxX - xOffset), Random.Range(minY + yOffset, maxY - yOffset), zOffset);
                    }
                    else
                        spawnPoint = new Vector3(Random.Range(minX + xOffset, maxX - xOffset), yOffset, Random.Range(minZ + zOffset, maxZ - zOffset));

    
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
                soundsToBePlayed.Add(obj.objectSounds[randomIndex]);
                obj.objectSounds.RemoveAt(randomIndex);
            }
        }


        for (int i = 0; i < selectedColors.Count; i++)
        {
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
                
                if (!obj.isSoundPlayed)
                {
                    foreach(AudioSource sound in soundsToBePlayed)
                    {
                        if (sound.tag == objectTag)
                        {
                            sound.Play();
                            soundsToBePlayed.Remove(sound);
                            obj.isSoundPlayed = true;
                            break;
                        }
                    }
                }

                numberOfDroppedObjects++;
                if (numberOfDroppedObjects == totalNumberOfObjects)
                    gameManager.ShowLevelEndPanel();
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
