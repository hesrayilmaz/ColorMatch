using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 startPos;
    private Vector3 mousePos;
    private Vector3 mouseOffset;
    private float mouseZCoord;
    private bool isPlacedRight = false;
    private bool isDragging;
    private float xOffset, yOffset, zOffset;

    private bool isGround = false;
    private static bool isStarted = false;

    private bool isStartPoint = true;

    private static GameObject demoCursor;
    private static GameObject demoTarget;
    private static int demoIndex = 0;
    private bool isDemoActive = false;

    private levelTypes selectedType;
    private Transform hitColliderObj;
    private BoxCollider hitColliderBox;
    private static List<DragAndDrop> draggableObjects;
    private static List<GameObject> targets;
    private static bool hasSearched = false;

    private float distance;
    private float radius;
    //public static float numOfDropPoints;

    private ObjectsController draggableObjectsController;
    private GameObject instructionClone;
    private AudioSource correctDropAudio, wrongDropAudio;

    // Start is called before the first frame update
    void Start()
    {
        if (selectedType == levelTypes.Torus)
        {
            radius = 0.6f;
        }
        else if (selectedType == levelTypes.Pencil)
        {
            radius = 0.6f;
        }
        else if (selectedType == levelTypes.Book)
        {
            radius = 5f;
        }
        else if (selectedType == levelTypes.Fruit)
        {
            radius = 2.1f;
        }
        else if (selectedType == levelTypes.Train)
        {
            radius = 1.5f;
        }
        else if (selectedType == levelTypes.Car)
        {
            radius = 5.3f;
        }
        else if (selectedType == levelTypes.Demo)
        {
            radius = 0.6f;
        }

        draggableObjectsController = GameObject.Find("ObjectsController").GetComponent<ObjectsController>();
        selectedType = GameObject.Find("LevelTypesController").GetComponent<LevelTypes>().GetSelectedLevelType();
        
        startPoint = transform.position;
        mouseZCoord = Camera.main.ScreenToWorldPoint(transform.position).z;
        //numOfDropPoints = GameObject.Find("InstructionDropPoints").transform.childCount;
        correctDropAudio = GameObject.Find("Audio").transform.Find("CorrectDrop").GetComponent<AudioSource>();
        wrongDropAudio = GameObject.Find("Audio").transform.Find("WrongDrop").GetComponent<AudioSource>();

    }

    private void Update()
    {
        if(selectedType==levelTypes.Demo && !hasSearched && !isStarted)
        {
            hasSearched = true;
            isStarted = true;

            draggableObjects = new List<DragAndDrop>();
            targets = new List<GameObject>();
            Debug.Log("11111111111111111");

            foreach(DragAndDrop obj in FindObjectsOfType<DragAndDrop>())
            {
                if (obj != this)
                {
                    draggableObjects.Add(obj);
                }
            }
            Debug.Log("draggableObjects.count: "+ draggableObjects.Count);
            demoCursor = gameObject.transform.Find("Cursor").gameObject;
            demoCursor.SetActive(true);
            targets = draggableObjectsController.GetBoxes();
            
            foreach(DragAndDrop obj in draggableObjects)
                obj.gameObject.GetComponent<BoxCollider>().enabled = false;

            foreach(GameObject box in targets)
            {
                if (box.tag == demoCursor.tag + "Box")
                    demoTarget = box;
            }

            isDemoActive = true;

        }

        if (isDemoActive)
        {
            if (isStartPoint)
            {
                demoCursor.transform.position = Vector3.MoveTowards(demoCursor.transform.position, demoTarget.transform.position+new Vector3(0f,0f, -1f), 0.6f * Time.deltaTime);
                if(demoCursor.transform.position == demoTarget.transform.position + new Vector3(0f, 0f, -1f))
                {
                    isStartPoint = false;
                }
            }
            else
            {
                demoCursor.transform.position = Vector3.MoveTowards(demoCursor.transform.position, transform.position + new Vector3(-0.2f, 0.1f, -0.2f), 0.6f * Time.deltaTime);
                if (demoCursor.transform.position == transform.position + new Vector3(-0.2f, 0.1f, -0.2f))
                {
                    isStartPoint = true;
                }
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Debug.Log("Groundddddddddd");
        }    
    }

    private void OnMouseDrag()
    {
        if (isDragging && !isPlacedRight)
        {
            /*mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            transform.position = new Vector3(mousePos.x - startPos.x, mousePos.y - startPos.y, mousePos.z - startPos.z);
        */
            //transform.position = GetMouseWorldPos() + mouseOffset;

            if (demoCursor)
                demoCursor.SetActive(false);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            //  transform.position = rayPoint;

            //rayPoint.y < startPoint.y
            if (rayPoint.y < startPoint.y)
            {
                transform.position = new Vector3(rayPoint.x, startPoint.y, rayPoint.z);
            }
            else
            {
                transform.position = rayPoint;
            }
                
        }
    }


    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /*if (gameObject.tag == "Light")
                instructionClone = gameObject;
            else
                instructionClone = Instantiate(gameObject, transform.parent);
            */

            //mousePos = Input.mousePosition;
            //mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            //mouseOffset = transform.position - GetMouseWorldPos();


            distance = Vector3.Distance(transform.position, Camera.main.transform.position);


            //startPos = mousePos - transform.position;

            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
   
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log("hitCollider TAG "+ hitCollider.gameObject.tag);
            if (hitCollider.gameObject.tag == gameObject.tag + "Box")
            {
                if (selectedType == levelTypes.Demo && isDemoActive)
                {
                    isDemoActive = false;
                    demoCursor.SetActive(false);
                }
                

                hitColliderObj = hitCollider.gameObject.transform;
                hitColliderBox = hitColliderObj.GetComponent<BoxCollider>();
                //hitCollider.gameObject.transform.parent = null;
                //instructionClone.transform.localScale = new Vector2(0.35f, 0.35f);


                if (selectedType == levelTypes.Torus)
                {
                    DropTorus();
                }
                else if (selectedType == levelTypes.Pencil)
                {
                    DropPencil();
                }
                else if(selectedType == levelTypes.Book)
                {
                    DropBook();
                }
                else if (selectedType == levelTypes.Fruit) 
                {
                    DropFruit();
                }
                else if (selectedType == levelTypes.Train)
                {
                    DropTrain();
                }
                else if (selectedType == levelTypes.Car)
                {
                    DropCar();
                }
                else if (selectedType == levelTypes.Demo)
                {
                    DropDemoObject();
                }


                transform.GetComponent<BoxCollider>().enabled = false;
                draggableObjectsController.AddDroppedObject(gameObject, gameObject.tag);
                correctDropAudio.Play();
                isPlacedRight = true;

                if (selectedType == levelTypes.Demo && demoIndex<draggableObjects.Count)
                {
                    Debug.Log("second demoIndex: " + demoIndex);
                    demoCursor = draggableObjects[demoIndex].gameObject.transform.Find("Cursor").gameObject;
                    demoCursor.SetActive(true);
                    draggableObjects[demoIndex].gameObject.GetComponent<BoxCollider>().enabled = true;

                    foreach (GameObject box in targets)
                    {
                        if (box.tag == demoCursor.tag + "Box")
                        {
                            demoTarget = box;
                        }
                    }
                    draggableObjects[demoIndex].isDemoActive = true;
                    demoIndex++;
                }
                

                //numOfDropPoints--;

                //if (numOfDropPoints == 0)
                //  carController.StartMove();

                //isPlacedRight = false;
                return;
            }
        }
        wrongDropAudio.Play();
        transform.position = startPoint;

        if (demoCursor)
            demoCursor.SetActive(true);
    }

    private void DropTorus()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderObj.position.x,
            hitColliderBox.bounds.min.y + transform.GetComponent<BoxCollider>().size.y*2, hitColliderObj.position.z);
        }
        else
        {
            transform.position = new Vector3(hitColliderObj.position.x,
                draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.y + transform.GetComponent<BoxCollider>().size.y,
                hitColliderObj.position.z);
        }
    }
    private void DropPencil()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + transform.GetComponent<BoxCollider>().size.x * 3,
                hitColliderBox.bounds.min.y + 0.1f,
                hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z * 4);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                transform.GetComponent<BoxCollider>().size.x * 1.5f, hitColliderBox.bounds.min.y + 0.1f,
                hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z * 4);
            transform.rotation = Quaternion.identity;
        }
    }
    private void DropBook()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + transform.GetComponent<BoxCollider>().size.z,
                hitColliderBox.bounds.min.y, hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.y);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                transform.GetComponent<BoxCollider>().size.z, hitColliderBox.bounds.min.y, 
                hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.y);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    private void DropFruit()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + transform.GetComponent<BoxCollider>().size.x,
                hitColliderBox.bounds.min.y + 0.1f, hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                transform.GetComponent<BoxCollider>().size.x, hitColliderBox.bounds.min.y + 0.1f, hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
        }
    }
    private void DropTrain()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + transform.GetComponent<BoxCollider>().size.x / 2,
                hitColliderBox.bounds.min.y + 0.05f, hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                transform.GetComponent<BoxCollider>().size.x + 0.05f, hitColliderBox.bounds.min.y + 0.05f, hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
        }
    }
    private void DropCar()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.12f, 
                hitColliderBox.bounds.min.y + transform.GetComponent<BoxCollider>().size.y / 6, 
                hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z / 4);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x + 0.25f, 
                hitColliderBox.bounds.min.y + transform.GetComponent<BoxCollider>().size.y / 6, 
                hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z / 4);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    private void DropDemoObject()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.13f, hitColliderBox.bounds.min.y + transform.GetComponent<BoxCollider>().size.x + 0.01f,
                hitColliderBox.size.z + transform.GetComponent<BoxCollider>().size.z / 3);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x + transform.GetComponent<BoxCollider>().size.y/2,
                hitColliderBox.bounds.min.y + transform.GetComponent<BoxCollider>().size.x + 0.01f, hitColliderBox.size.z + transform.GetComponent<BoxCollider>().size.z / 3);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        mousePos = Input.mousePosition;
        mousePos.z = mouseZCoord;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

}
