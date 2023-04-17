using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableObjectsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToInstantiate;
    private List<GameObject> objectsInScene;
    [SerializeField] private GameObject referanceObject;
    private float xOffset, yOffset, zOffset;
    [SerializeField] private int numberOfColors;
    [SerializeField] private int numberOfEachColor;
    private int randomIndex;
    private GameObject draggableObject;

    // Start is called before the first frame update
    void Start()
    {
        xOffset = referanceObject.transform.localScale.x / 2;
        yOffset = objectsToInstantiate[0].transform.position.y;
        zOffset= referanceObject.transform.localScale.z / 2;
        objectsInScene = new List<GameObject>();

        Debug.Log("xOffset " + xOffset);
        Debug.Log("zOffset " + zOffset);

        for (int i=0; i<numberOfColors; i++)
        {
            randomIndex = Random.Range(0, objectsToInstantiate.Count);

            for(int j = 0; j < numberOfEachColor; j++)
            {
                draggableObject = Instantiate(objectsToInstantiate[randomIndex], new Vector3(Random.Range(referanceObject.transform.position.x-xOffset, referanceObject.transform.position.x+xOffset), yOffset, Random.Range(referanceObject.transform.position.z - zOffset, referanceObject.transform.position.z+zOffset)), Quaternion.identity);
                objectsInScene.Add(draggableObject);
            }

            objectsToInstantiate.RemoveAt(randomIndex);
        }
    }

}
