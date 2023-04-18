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
    private Vector3 spawnPoint;
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

        float minX = referanceObject.transform.position.x - xOffset;
        float maxX = referanceObject.transform.position.x + xOffset;

        float minZ = referanceObject.transform.position.z - zOffset;
        float maxZ = referanceObject.transform.position.z + zOffset;

        for (int i=0; i<numberOfColors; i++)
        {
            randomIndex = Random.Range(0, objectsToInstantiate.Count);

            for(int j = 0; j < numberOfEachColor; j++)
            {
                spawnPoint = new Vector3(Random.Range(minX, maxX), yOffset, Random.Range(minZ, maxZ));
                
                Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, objectsToInstantiate[0].GetComponent<BoxCollider>().size.x/2, LayerMask.GetMask("Draggable"));
                if (hitColliders.Length == 0)
                {
                    draggableObject = Instantiate(objectsToInstantiate[randomIndex], spawnPoint, Quaternion.identity);
                    objectsInScene.Add(draggableObject);
                }
                else
                    j--;
                
            }

            objectsToInstantiate.RemoveAt(randomIndex);
        }
    }

}
