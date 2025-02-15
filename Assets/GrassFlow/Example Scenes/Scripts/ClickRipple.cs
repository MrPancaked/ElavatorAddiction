using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for SceneManager

namespace GrassFlow.Examples
{
    public class ClickRipple : MonoBehaviour
    {

        [Header("References")]
        public Gun gun;

        [Header("Settings")]

        public float contactOffset = 3f;
        public float ripStrength = 1.2f;
        public float ripDecayRate = 1f;
        public float ripSpeed = 20f;
        public float ripRadius = 0.1f;

        Ray ray;
        RaycastHit hit;
        private float timer = 0;
        private Collider grassCollider;
        private string grassTag = "Grass"; // Tag for the grass object.
        private bool canApplyRipples = false; // Flag to control Update logic

        private void OnEnable() 
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() 
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject grassTerrain = GameObject.FindGameObjectWithTag(grassTag);

            if (grassTerrain != null)
            {
                grassCollider = grassTerrain.GetComponent<Collider>(); // Get the Collider component
                canApplyRipples = true;
            }
            else
            {
                canApplyRipples = false;
            }
        }

        private void Update()
        {
            if (!canApplyRipples) return;
            if (Input.GetMouseButton(0) && timer > gun.gunSettings.fireRate)
            {
                timer = 0;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (grassCollider.Raycast(ray, out hit, 9999f))
                {
                    GrassFlowRenderer.AddRipple(hit.point + hit.normal * contactOffset, ripStrength, ripDecayRate, ripSpeed, ripRadius, 0);
                }
            }
            timer += Time.deltaTime;
        }
    }
}