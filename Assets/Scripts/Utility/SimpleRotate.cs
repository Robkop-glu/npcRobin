using System;
using UnityEditor.Rendering;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public enum RotationType
    {
        Continuous,
        PingPong
    }

    [SerializeField] private RotationType rotationType = RotationType.Continuous;

    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotateSpeedMin = 1f;
    [SerializeField] private float rotateSpeedMax = 2f;

    [SerializeField] private float rotateAngleRadians = (float)Math.PI * 0.2f;

    private float timer = 0f;
    private float relativeTime = 0f;
    private float maxTime = 0f;
    Quaternion initialRotation;

    private void Awake()
    {
        maxTime = 1f / UnityEngine.Random.Range(rotateSpeedMin, rotateSpeedMax);
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        if (rotationType == RotationType.Continuous)
        {
            float rotation = Time.deltaTime * rotateSpeedMax * rotateAngleRadians;
            transform.rotation = transform.rotation * Quaternion.Euler(rotationAxis * rotation);
        }
        else
        {
            timer += Time.deltaTime;
            float rotation = Mathf.Sin(2f * (float)Math.PI * timer * rotateSpeedMax) * rotateAngleRadians;
            transform.rotation = initialRotation * Quaternion.Euler(rotationAxis * rotation * Mathf.Rad2Deg);
        }
        
    }
}
