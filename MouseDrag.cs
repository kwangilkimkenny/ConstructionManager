using UnityEngine;
using System.Collections;

public class MouseDrag : MonoBehaviour
{

	[SerializeField] private LayerMask layerMask;

	private Vector3 EndMousePos;
    private Vector3 StartMousePos;



    void OnMouseDown()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask);
        //Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);
        StartMousePos = raycastHit.point;
    }

    void OnMouseDrag()
    {
        // Get the click location.

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask);
        //Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask);

        EndMousePos = raycastHit.point;

        //Debug.Log("EndMousePos :" + EndMousePos);

        // Adjust the location by adding an offset.
        Vector3 newPosition = new Vector3(EndMousePos.x, EndMousePos.y, EndMousePos.z);
         
        // Assign new position.
        transform.position = newPosition;
    }
}
