using System;
using UnityEngine;

public class CameraFitter : MonoBehaviour
{

    [SerializeField] private float verticalOffset = 0.5f;
    
    [SerializeField] private float zoomOutFactor = 1.25f;


    [SerializeField] private float minCameraSize = 6.25f;



    private void Start()
    {
        GameServiceLocator.Get<ILevelService>().OnLoadLevelData += StartFit;
    }

    private void StartFit(LevelData data)
    {
       FitCameraToGrid(data.width, data.height, 1.35f);
    }

    public void FitCameraToGrid(int width, int height, float spacing)
    {
        Camera cam = GetComponent<Camera>();

        transform.position = new Vector3(0, verticalOffset, -10f);

        float totalWidth = (width - 1) * spacing;
        float totalHeight = (height - 1) * spacing;

        float aspectRatio = (float)Screen.width / Screen.height;

        float sizeByHeight = (totalHeight / 2f) * zoomOutFactor;
        float sizeByWidth = ((totalWidth / 2f) * zoomOutFactor) / aspectRatio;

        float finalSize = Mathf.Max(sizeByHeight, sizeByWidth);

        cam.orthographicSize = Mathf.Max(finalSize, minCameraSize);
    }


    private void OnDestroy()
    {
        GameServiceLocator.Get<ILevelService>().OnLoadLevelData -= StartFit;
    }
}