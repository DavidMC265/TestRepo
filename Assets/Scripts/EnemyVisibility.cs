using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Numerics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Scripting.APIUpdating;

public class EnemyVisibility : MonoBehaviour
{
    public Transform target =null;
    public float maxDistance = 10f;
    [Range(0f, 360f)] public float angle = 45f;
    [SerializeField] bool visualize = true;
    public bool targetIsVisible {get; private set; }

    public float Speed = 5.0f;

    void Update(){

        transform.Translate(UnityEngine.Vector3.forward * Speed * Time.deltaTime);

        targetIsVisible = CheckVisibility();

        if(visualize) {
            var color = targetIsVisible ? Color.yellow : Color.white;
            GetComponent<Renderer>().material.color = color;
        }
    }

    public bool CheckVisibilityToPoint(UnityEngine.Vector3 worldPoint){
        var DirectionToTarget = worldPoint - transform.position;
        var degreesToTarget = UnityEngine.Vector3.Angle(transform.forward, DirectionToTarget);
        var withinArc = degreesToTarget < (angle /2);

        if(withinArc == false){
            return false;
        }

        var distanceToTarget = DirectionToTarget.magnitude;
        var RayDistance = Mathf.Min(maxDistance, distanceToTarget);
        var ray = new Ray(transform.position, DirectionToTarget);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, RayDistance)){
            if(hit.collider.transform == target) {
                return true;
            }
            return false;
        }else {
            return true;
        }
    }


    public bool CheckVisibility(){
        var DirectionToTarget = target.position - transform.position;
        var degreesToTarget = UnityEngine.Vector3.Angle(transform.forward, DirectionToTarget);
        var withinArc = degreesToTarget < (angle /2);

        if(withinArc == false){
            return false;
        }
        var DistanceToTarget = DirectionToTarget.magnitude;
        var RayDistance = Mathf.Min(maxDistance, DistanceToTarget);
        var ray = new Ray(transform.position, DirectionToTarget);

        RaycastHit hit;

        var canSee = false;

        if(Physics.Raycast(ray, out hit, RayDistance)){
            if(hit.collider.transform == target){
                canSee = true;
            }
            Debug.DrawLine(transform.position, hit.point);
        }
        else{
            Debug.DrawRay(transform.position, DirectionToTarget.normalized * RayDistance);
        }
        return canSee;
    }
}


[CustomEditor(typeof(EnemyVisibility))]
public class EnemyVisibilityEditor: Editor {
    
    private void OnSceneGUI(){
        var visibility = target as EnemyVisibility;
        Handles.color = new Color(1,1,1,0.1f);
        var forwardPointMinusHalfAngle = UnityEngine.Quaternion.Euler(0, -visibility.angle / 2,0) * visibility.transform.forward;

        UnityEngine.Vector3 arcStart= forwardPointMinusHalfAngle *visibility.maxDistance;
        Handles.DrawSolidArc(
            visibility.transform.position,
            UnityEngine.Vector3.up,
            arcStart,
            visibility.angle,
            visibility.maxDistance
        );

        Handles.color = Color.white;
        UnityEngine.Vector3 handlePosition = visibility.transform.forward * visibility.maxDistance;

        visibility.maxDistance = Handles.ScaleValueHandle(
            visibility.maxDistance,
            handlePosition,
            visibility.transform.rotation,
            1,
            Handles.ConeHandleCap,
            0.25f);
        
    }
}
