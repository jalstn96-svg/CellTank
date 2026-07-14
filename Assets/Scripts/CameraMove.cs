using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform target;

    Vector3 targetPos;

    
    void Start()
    {
        
    }

    
    void LateUpdate()
    {
        targetPos=new Vector3(target.position.x,target.position.y,-10f);

        transform.position = targetPos;
    }
}
