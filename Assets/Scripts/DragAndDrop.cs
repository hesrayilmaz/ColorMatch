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
    private levelTypes selectedType;
    private Transform hitColliderObj;
    private BoxCollider hitColliderBox;

    private float distance;
    //public static float numOfDropPoints;

    private ObjectsController draggableObjectsController;
    private GameObject instructionClone;
    private AudioSource correctDropAudio, wrongDropAudio;

    // Start is called before the first frame update
    void Start()
    {
        draggableObjectsController = GameObject.Find("ObjectsController").GetComponent<ObjectsController>();
        selectedType = GameObject.Find("LevelTypesController").GetComponent<LevelTypes>().GetSelectedLevelType();
        Debug.Log("selectedType " + selectedType);
        startPoint = transform.position;
        mouseZCoord = Camera.main.ScreenToWorldPoint(transform.position).z;
        //numOfDropPoints = GameObject.Find("InstructionDropPoints").transform.childCount;
        correctDropAudio = GameObject.Find("Audio").transform.Find("CorrectDrop").GetComponent<AudioSource>();
        wrongDropAudio = GameObject.Find("Audio").transform.Find("WrongDrop").GetComponent<AudioSource>();
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
   
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log("hitCollider TAG "+ hitCollider.gameObject.tag);
            if (hitCollider.gameObject.tag == gameObject.tag + "Box")
            {
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
                else if (selectedType == levelTypes.Pillow)
                {
                    DropPillow();
                }


                transform.GetComponent<BoxCollider>().enabled = false;
                draggableObjectsController.AddDroppedObject(gameObject, gameObject.tag);
                correctDropAudio.Play();
                isPlacedRight = true;
                //numOfDropPoints--;

                //if (numOfDropPoints == 0)
                  //  carController.StartMove();

                //isPlacedRight = false;
                return;
            }
        }
        wrongDropAudio.Play();
        transform.position = startPoint;
    }

    private void DropTorus()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderObj.position.x,
            hitColliderBox.bounds.min.y + 0.1f, hitColliderObj.position.z);
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
                hitColliderBox.bounds.min.y, hitColliderBox.bounds.max.z);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                transform.GetComponent<BoxCollider>().size.z + 0.01f, hitColliderBox.bounds.min.y, hitColliderBox.bounds.max.z);
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
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.12f, hitColliderBox.bounds.min.y + 0.1f, 
                hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z / 8);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x + 0.25f, 
                hitColliderBox.bounds.min.y + 0.1f, hitColliderBox.bounds.max.z - transform.GetComponent<BoxCollider>().size.z / 8);
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    private void DropPillow()
    {
        if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
        {
            transform.position = new Vector3(hitColliderBox.bounds.min.x + 0.13f, hitColliderBox.bounds.min.y + transform.localScale.y / 2,
                hitColliderBox.size.z);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x + 0.1f,
                hitColliderBox.bounds.min.y + transform.localScale.y / 2, hitColliderBox.size.z);
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
