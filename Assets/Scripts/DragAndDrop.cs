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


    private float distance;
    //public static float numOfDropPoints;

    private GameObject instructionClone;
    private AudioSource dropAudio, wrongDropAudio;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position;
        mouseZCoord = Camera.main.ScreenToWorldPoint(transform.position).z;
        //numOfDropPoints = GameObject.Find("InstructionDropPoints").transform.childCount;
        //dropAudio = GameObject.Find("Audio").transform.GetChild(2).GetComponent<AudioSource>();
        //wrongDropAudio = GameObject.Find("Audio").transform.GetChild(3).GetComponent<AudioSource>();
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

            if (rayPoint.y < startPoint.y)
                transform.position = new Vector3(rayPoint.x,startPoint.y,rayPoint.z);
            else
                transform.position = rayPoint;
          
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
   
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.3f);
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log("hitCollider TAG "+ hitCollider.gameObject.tag);
            if (hitCollider.gameObject.tag == gameObject.tag + "Box")
            {
              
                //hitCollider.gameObject.transform.parent = null;
                //instructionClone.transform.localScale = new Vector2(0.35f, 0.35f);
                transform.position = new Vector3(hitCollider.transform.position.x,
                    hitCollider.transform.position.y+0.1f, hitCollider.transform.position.z);
                transform.GetComponent<BoxCollider>().enabled = false;
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
