using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera cam;
    private Vector3 startPoint;
    private float startPointYOffset;
    private bool isPlacedRight = false;
    private bool isDragging;
    
    private bool isStartPoint = true;

    private GameObject tutorialCursor;
    private GameObject tutorialParticle;
    private GameObject tutorialTargetBox;

    private levelTypes selectedType;
    private Transform hitColliderObj;
    private BoxCollider hitColliderBox;
    private BoxCollider objectBox;
    private static List<DragAndDrop> draggableObjects;
    private static List<GameObject> targets;
    public static bool isStarted = false;
    public bool hasSearched = false;
    public static int currentObjectIndex = 0;
    public static bool isMyTurn = false;
    private bool isDraggingActive = false;

    private float distance;
    private float radius;

    private ObjectsController draggableObjectsController;
    private AudioSource correctDropAudio, wrongDropAudio;
    private Animator mascotAnimator;

    private void Awake()
    {
        cam = Camera.main;
        draggableObjectsController = GameObject.Find("ObjectsController").GetComponent<ObjectsController>();
        selectedType = GameObject.Find("LevelTypesController").GetComponent<LevelTypes>().GetSelectedLevelType();
        correctDropAudio = GameObject.Find("Audio").transform.Find("CorrectDrop").GetComponent<AudioSource>();
        wrongDropAudio = GameObject.Find("Audio").transform.Find("WrongDrop").GetComponent<AudioSource>();
        mascotAnimator = GameObject.FindGameObjectWithTag("Mascot").transform.GetChild(0).GetComponent<Animator>();
        objectBox = GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        startPointYOffset = startPoint.y;

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
        else if (selectedType == levelTypes.Field)
        {
            radius = 2.1f;
        }
        else if (selectedType == levelTypes.Tree)
        {
            radius = 2.1f;
            Transform box = GameObject.FindGameObjectWithTag(transform.tag + "Box").transform;
            startPointYOffset = box.position.y + objectBox.size.y/2;
        }
        else if (selectedType == levelTypes.Train)
        {
            radius = 1.5f;
        }
        else if (selectedType == levelTypes.Car)
        {
            radius = 5.3f;
        }
        else if (selectedType == levelTypes.Tutorial)
        {
            radius = 0.6f;
        }
    }

    private void Update()
    {

        if(selectedType == levelTypes.Tutorial && isStarted && !hasSearched && isMyTurn)
        {
            hasSearched = true;
            isMyTurn = false;

            draggableObjects = new List<DragAndDrop>();
            targets = new List<GameObject>();

            foreach(DragAndDrop obj in FindObjectsOfType<DragAndDrop>())
            {
                if (obj != this)
                {
                    draggableObjects.Add(obj);
                    obj.hasSearched = true;
                }
            }

            tutorialCursor = transform.Find("Cursor").gameObject;
            tutorialParticle = transform.Find("Particle").gameObject;
            tutorialCursor.SetActive(true);
            tutorialParticle.SetActive(true);
            targets = draggableObjectsController.GetBoxes();

            foreach(DragAndDrop obj in draggableObjects)
                obj.gameObject.GetComponent<BoxCollider>().enabled = false;
           
            foreach(GameObject box in targets)
            {
                if (box.tag == tutorialCursor.tag + "Box")
                    tutorialTargetBox = box;
            }

            isDraggingActive = true;
        }

        if (isDraggingActive)
        {
            if (isStartPoint)
            {
                tutorialCursor.transform.position = Vector3.MoveTowards(tutorialCursor.transform.position, tutorialTargetBox.transform.position+new Vector3(0f,0f, -1f), 0.6f * Time.deltaTime);
                if(tutorialCursor.transform.position == tutorialTargetBox.transform.position + new Vector3(0f, 0f, -1f))
                {
                    isStartPoint = false;
                }
            }
            else
            {
                tutorialCursor.transform.position = Vector3.MoveTowards(tutorialCursor.transform.position, transform.position + new Vector3(-0.2f, 0.1f, -0.2f), 0.6f * Time.deltaTime);
                if (tutorialCursor.transform.position == transform.position + new Vector3(-0.2f, 0.1f, -0.2f))
                {
                    isStartPoint = true;
                }
            }
        }

    }


    private void OnMouseDrag()
    {
        if (isDragging && !isPlacedRight)
        {
            if (tutorialCursor)
                tutorialCursor.SetActive(false);
                
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);

            if (rayPoint.y < startPointYOffset)
                transform.position = new Vector3(rayPoint.x, startPointYOffset, rayPoint.z);
            else
                transform.position = rayPoint;
        }
    }


    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && isStarted)
        {
            distance = Vector3.Distance(transform.position, cam.transform.position);
            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        if (isStarted)
        {
            isDragging = false;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var hitCollider in hitColliders)
            {
                //Debug.Log("hitCollider TAG "+ hitCollider.gameObject.tag);
                if (hitCollider.gameObject.tag == gameObject.tag + "Box")
                {
                    if (selectedType == levelTypes.Tutorial && isDraggingActive)
                    {
                        isDraggingActive = false;
                        tutorialCursor.SetActive(false);
                        tutorialParticle.SetActive(false);
                    }

                    hitColliderObj = hitCollider.gameObject.transform;
                    hitColliderBox = hitColliderObj.GetComponent<BoxCollider>();

                    if (selectedType == levelTypes.Torus)
                    {
                        DropTorus();
                    }
                    else if (selectedType == levelTypes.Pencil)
                    {
                        DropPencil();
                    }
                    else if (selectedType == levelTypes.Book)
                    {
                        DropBook();
                    }
                    else if (selectedType == levelTypes.Field || selectedType == levelTypes.Tree)
                    {
                        DropField();
                    }
                    else if (selectedType == levelTypes.Train)
                    {
                        DropTrain();
                    }
                    else if (selectedType == levelTypes.Car)
                    {
                        DropCar();
                    }
                    else if (selectedType == levelTypes.Tutorial)
                    {
                        DropDemoObject();
                    }

                    objectBox.enabled = false;
                    draggableObjectsController.AddDroppedObject(gameObject, gameObject.tag);
                    correctDropAudio.Play();
                    mascotAnimator.SetTrigger("Jump");
                    isPlacedRight = true;

                    if (selectedType == levelTypes.Tutorial && currentObjectIndex < draggableObjects.Count)
                    {
                        DragAndDrop currentDraggableObj = draggableObjects[currentObjectIndex];
                        currentDraggableObj.tutorialCursor = currentDraggableObj.transform.Find("Cursor").gameObject;
                        currentDraggableObj.tutorialParticle = currentDraggableObj.transform.Find("Particle").gameObject;
                        currentDraggableObj.tutorialCursor.SetActive(true);
                        currentDraggableObj.tutorialParticle.SetActive(true);
                        currentDraggableObj.gameObject.GetComponent<BoxCollider>().enabled = true;

                        foreach (GameObject box in targets)
                        {
                            if (box.tag == currentDraggableObj.tutorialCursor.tag + "Box")
                            {
                                currentDraggableObj.tutorialTargetBox = box;
                            }
                        }
                        currentDraggableObj.isDraggingActive = true;
                        currentObjectIndex++;
                    }
                    return;
                }
            }
            wrongDropAudio.Play();
            mascotAnimator.SetTrigger("Hurt");
            transform.position = startPoint;

            if (tutorialCursor)
                tutorialCursor.SetActive(true);
        }
    }

    private void DropTorus()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderObj.position.x,
            hitColliderBox.bounds.min.y + objectBox.size.y*2, hitColliderObj.position.z);
        }
        else
        {
            transform.position = new Vector3(hitColliderObj.position.x,
                draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.y + objectBox.size.y,
                hitColliderObj.position.z);
        }
    }
    private void DropPencil()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + objectBox.size.x * 3,
                hitColliderBox.bounds.min.y + 0.1f,
                hitColliderBox.bounds.max.z - objectBox.size.z * 4);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                objectBox.size.x * 1.5f, hitColliderBox.bounds.min.y + 0.1f,
                hitColliderBox.bounds.max.z - objectBox.size.z * 4);
            transform.rotation = Quaternion.identity;
        }
    }
    private void DropBook()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + objectBox.size.z,
                hitColliderBox.bounds.min.y, hitColliderBox.bounds.max.z - objectBox.size.y);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                objectBox.size.z, hitColliderBox.bounds.min.y, 
                hitColliderBox.bounds.max.z - objectBox.size.y);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    private void DropField()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.25f,
                //hitColliderBox.bounds.min.y + objectBox.size.y + 0.05f, 
                hitColliderBox.bounds.min.y + 0.45f,
                hitColliderBox.bounds.max.z - 0.5f);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                objectBox.size.x * (3/2),
                //hitColliderBox.bounds.min.y + objectBox.size.y + 0.05f, 
                hitColliderBox.bounds.min.y + 0.45f,
                hitColliderBox.bounds.max.z - 0.5f);
        }
    }
    private void DropTrain()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + objectBox.size.x / 2,
                hitColliderBox.bounds.min.y + 0.05f, hitColliderBox.bounds.max.z - objectBox.size.z);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                objectBox.size.x + 0.05f, hitColliderBox.bounds.min.y + 0.05f, hitColliderBox.bounds.max.z - objectBox.size.z);
        }
    }
    private void DropCar()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.12f, 
                hitColliderBox.bounds.min.y + objectBox.size.y / 6, 
                hitColliderBox.bounds.max.z - objectBox.size.z / 4);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x + 0.25f, 
                hitColliderBox.bounds.min.y + objectBox.size.y / 6, 
                hitColliderBox.bounds.max.z - objectBox.size.z / 4);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    private void DropDemoObject()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.13f, hitColliderBox.bounds.min.y + objectBox.size.x + 0.01f,
                hitColliderBox.size.z + objectBox.size.z / 3);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x + objectBox.size.y/2,
                hitColliderBox.bounds.min.y + objectBox.size.x + 0.01f, hitColliderBox.size.z + objectBox.size.z / 3);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

}
