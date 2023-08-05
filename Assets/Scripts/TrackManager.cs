using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    private GameObject _origin;
    private float _baseLineWidth = 0.001f;
    private float _baseOriginLen = 0.1f;
    private float _baseGradLen = 0.0025f;

    // Start is called before the first frame update
    void Start()
    {
        CoreServices.DiagnosticsSystem.ShowProfiler = false;
        Material whiteMat = new Material(Shader.Find("Standard"));
        whiteMat.SetColor("_Color", Color.white);
        _origin = Helpers.CreateAxis(whiteMat, _baseLineWidth, _baseOriginLen, _baseGradLen);
        _origin.transform.position = new Vector3(0, 0, 0);
        _origin.transform.rotation = Quaternion.identity;
    }

}
