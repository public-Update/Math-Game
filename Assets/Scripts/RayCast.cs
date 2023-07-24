using UnityEngine;

public class RayCast : MonoBehaviour
{
    private RaycastHit hit;
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    public void Take()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,6f))
        {
           if(hit.transform.GetComponent<Table>())
            {
              hit.transform.GetComponent<Table>().CheckOut();
            }   
        }
    }
}
