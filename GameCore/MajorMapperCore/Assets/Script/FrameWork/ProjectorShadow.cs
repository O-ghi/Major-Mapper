/* 
 * -----------------------------------------------
 * Copyright (c) zhou All rights reserved.
 * -----------------------------------------------
 * 
 * Coder：Zhou XiQuan
 * Time ：2017.06.12
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectorShadow : MonoBehaviour 
{
    public Vector2 shadowSize = new Vector2(1024, 1024);
    public Camera shadowCamera;
    public Projector projector;

    private RenderTexture shadowTexture;
    private Matrix4x4 projectorMatrix;
    private Matrix4x4 matVP;

	void Start ()
    {
        shadowTexture = RenderTexture.GetTemporary((int)shadowSize.x, (int)shadowSize.y, 0, RenderTextureFormat.ARGB32);
        shadowTexture.name = "shadowmap_texture";
        shadowTexture.isPowerOfTwo = true;
        shadowTexture.hideFlags = HideFlags.DontSave;

        shadowCamera.enabled = false;
        shadowCamera.targetTexture = shadowTexture;

        projector.material.SetTexture("_ShadowTex", shadowTexture);
	}

    void OnDestroy()
    {
        RenderTexture.ReleaseTemporary(shadowTexture);
    }

    void Update ()
    {
        //createCameraProjecterMatrix();
        matVP = GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix, true) * shadowCamera.worldToCameraMatrix;
        projector.material.SetMatrix("ShadowMatrix", matVP);
        shadowCamera.Render();
	}


    /*private List<Transform> charactorList = new List<Transform>();
    void createCameraProjecterMatrix()
    {
        Vector3 v3MaxPosition = -Vector3.one * 500000.0f;
        Vector3 v3MinPosition = Vector3.one * 500000.0f;
        for(int vertId = 0, len = charactorList.Count; vertId < len; ++vertId)
        {
            if(charactorList[vertId] == null || !charactorList[vertId].gameObject.activeSelf)
                continue;

            // Light view space
            Vector3 v3Position = shadowCamera.worldToCameraMatrix.MultiplyPoint3x4(charactorList[vertId].position);
            if(v3Position.x > v3MaxPosition.x)
            {
                v3MaxPosition.x = v3Position.x;
            }
            if(v3Position.y > v3MaxPosition.y)
            {
                v3MaxPosition.y = v3Position.y;
            }
            if(v3Position.z > v3MaxPosition.z)
            {
                v3MaxPosition.z = v3Position.z;
            }
            if(v3Position.x < v3MinPosition.x)
            {
                v3MinPosition.x = v3Position.x;
            }
            if(v3Position.y < v3MinPosition.y)
            {
                v3MinPosition.y = v3Position.y;
            }
            if(v3Position.z < v3MinPosition.z)
            {
                v3MinPosition.z = v3Position.z;
            }
        }
        Vector3 off = v3MaxPosition - v3MinPosition;
        Vector3 sizeOff = off;
        sizeOff.z = 0;
        float dis = sizeOff.magnitude;
        //CreateOrthogonalProjectMatrix (ref m_projMatrix, v3MaxPosition, v3MinPosition);
        //Debug.Log (v3MaxPosition.ToString() + v3MinPosition.ToString());
        //Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
        //shadowCamera.projectionMatrix = m * m_projMatrix;
        shadowCamera.orthographicSize = dis / 1.8f;
        shadowCamera.farClipPlane = off.z + 50;
        matVP = GL.GetGPUProjectionMatrix(shadowCamera.projectionMatrix, true) * shadowCamera.worldToCameraMatrix;
    }*/
}
