using System;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
   private Canvas canvas;
   public Transform PlayerTransform;
   private void Start()
   {
      canvas = GameObject.Find("ShopCanvas").GetComponent<Canvas>();
      canvas.gameObject.SetActive(false);
   }

   private void OnTriggerEnter(Collider other)
   {
      canvas.gameObject.SetActive(true);
      if(other.gameObject.CompareTag("Player"))
      {
          other.transform.position = PlayerTransform.position;
      }
      else
      {

         PlayerTransform = null;

      }
     
   }

   private void OnTriggerExit(Collider other)
   {
      canvas.gameObject.SetActive(false);
   }
}
