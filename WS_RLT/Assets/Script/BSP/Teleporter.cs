using UnityEngine;

public class Teleporter : MonoBehaviour
{
   private Vector3 _destinaion;
  

   public void SetDestination(Vector3 _dest)
   {
      _destinaion = _dest;
   }
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         other.transform.position = _destinaion;
         
      }
   }
   private void OnDrawGizmos()
   {
      if (_destinaion != null)
      {
         Gizmos.DrawLine(transform.localPosition, _destinaion);
      }
      else
      {
         Gizmos.color = Color.red;
         Gizmos.DrawSphere(transform.position, 1.0f);
      }
   }
}
