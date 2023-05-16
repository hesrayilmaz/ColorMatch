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

    private bool isGround = false;
    private levelTypes selectedType;

    private float distance;
    //public static float numOfDropPoints;

    private ObjectsController draggableObjectsController;
    private GameObject instructionClone;
    private AudioSource dropAudio, wrongDropAudio;

    // Start is called before the first frame update
    void Start()
    {
        draggableObjectsController = GameObject.Find("ObjectsController").GetComponent<ObjectsController>();
        selectedType = GameObject.Find("LevelTypesController").GetComponent<LevelTypes>().GetSelectedLevelType();
        Debug.Log("selectedType " + selectedType);
        startPoint = transform.position;
        mouseZCoord = Camera.main.ScreenToWorldPoint(transform.position).z;
        //numOfDropPoints = GameObject.Find("InstructionDropPoints").transform.childCount;
        //dropAudio = GameObject.Find("Audio").transform.GetChild(2).GetComponent<AudioSource>();
        //wrongDropAudio = GameObject.Find("Audio").transform.GetChild(3).GetComponent<AudioSource>();
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

                //hitCollider.gameObject.transform.parent = null;
                //instructionClone.transform.localScale = new Vector2(0.35f, 0.35f);


                if (selectedType == levelTypes.Torus)
                {
                    if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
                    {
                        transform.position = new Vector3(hitCollider.transform.position.x,
                        hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.1f, hitCollider.transform.position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(hitCollider.transform.position.x,
                            draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.y + gameObject.GetComponent<BoxCollider>().size.y,
                            hitCollider.transform.position.z);
                    }
                }
                
                else if (selectedType == levelTypes.Pencil)
                {
                    
                    if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
                    {
                        transform.position = new Vector3(hitCollider.transform.GetComponent<BoxCollider>().bounds.min.x + 2*transform.GetComponent<BoxCollider>().size.x,
                        hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.05f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z- 3 * transform.GetComponent<BoxCollider>().size.z);
                        transform.rotation = Quaternion.identity;
                    }
                    else
                    {
                        transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                            1.5f * transform.GetComponent<BoxCollider>().size.x, hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.05f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - 3 * transform.GetComponent<BoxCollider>().size.z);
                        transform.rotation = Quaternion.identity;
                    }
                   
                }
                else if(selectedType == levelTypes.Book)
                {
                    if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
                    {
                        transform.position = new Vector3(hitCollider.transform.GetComponent<BoxCollider>().bounds.min.x + transform.GetComponent<BoxCollider>().size.z,
                        hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z);
                        transform.rotation = Quaternion.Euler(0,90,0);
                    }
                    else
                    {
                        transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                            transform.GetComponent<BoxCollider>().size.z+0.01f, hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z);
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
                else if (selectedType == levelTypes.Fruit) 
                {
                    if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
                    {
                        transform.position = new Vector3(hitCollider.transform.GetComponent<BoxCollider>().bounds.min.x + transform.GetComponent<BoxCollider>().size.x,
                        hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.1f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
                    }
                    else
                    {
                        transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                            transform.GetComponent<BoxCollider>().size.x, hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.1f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
                    }
                }
                else if (selectedType == levelTypes.Train)
                {
                    if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
                    {
                        transform.position = new Vector3(hitCollider.transform.GetComponent<BoxCollider>().bounds.min.x + transform.GetComponent<BoxCollider>().size.x/2,
                        hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.05f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
                    }
                    else
                    {
                        transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                            transform.GetComponent<BoxCollider>().size.x, hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.05f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - transform.GetComponent<BoxCollider>().size.z);
                    }
                }
                else if (selectedType == levelTypes.Car)
                {
                    if (draggableObjectsController.IsDroppedListEmpty(gameObject.tag))
                    {
                        transform.position = new Vector3(hitCollider.transform.GetComponent<BoxCollider>().bounds.min.x + 0.12f,
                        hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.1f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - transform.GetComponent<BoxCollider>().size.z/8);
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        transform.position = new Vector3(draggableObjectsController.GetLastDroppedObject(gameObject.tag).transform.position.x +
                            0.25f, hitCollider.transform.GetComponent<BoxCollider>().bounds.min.y + 0.1f, hitCollider.transform.GetComponent<BoxCollider>().bounds.max.z - transform.GetComponent<BoxCollider>().size.z/8);
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                }


                transform.GetComponent<BoxCollider>().enabled = false;
                draggableObjectsController.AddDroppedObject(gameObject, gameObject.tag);
                //dropAudio.Play();
                isPlacedRight = true;
                //numOfDropPoints--;

                //if (numOfDropPoints == 0)
                  //  carController.StartMove();

                //isPlacedRight = false;
                return;
            }
        }
        //wrongDropAudio.Play();
        transform.position = startPoint;
    }


    private Vector3 GetMouseWorldPos()
    {
        mousePos = Input.mousePosition;
        mousePos.z = mouseZCoord;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

}
