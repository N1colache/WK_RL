using System;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
   private Canvas canvas;

   private void Start()
   {
      canvas = GameObject.Find("ShopCanvas").GetComponent<Canvas>();
      canvas.gameObject.SetActive(false);
   }

   private void OnTriggerEnter(Collider other)
   {
      canvas.gameObject.SetActive(true);
   }

   private void OnTriggerExit(Collider other)
   {
      canvas.gameObject.SetActive(false);
   }
}
