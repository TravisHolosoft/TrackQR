using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[RequireComponent(typeof(QRTracking.SpatialGraphNodeTracker))]
public class QRInstrument : MonoBehaviour
{
    public Microsoft.MixedReality.QR.QRCode QRCodeObj;
    public float PhysicalSize { get; private set; }
    public string CodeText { get; private set; }
    private long _lastTimeStamp = 0;
    private GameObject _qrCodeCube;

    // Start is called before the first frame update
    void Start()
    {
        PhysicalSize = 0.1f;
        CodeText = "Dummy";
        if (QRCodeObj == null)
        {
            throw new System.Exception("QR Code Empty");
        }

        PhysicalSize = QRCodeObj.PhysicalSideLength;
        CodeText = QRCodeObj.Data;
        _qrCodeCube = gameObject.transform.Find("Instrument").gameObject;
    }

    void UpdatePropertiesDisplay()
    {
        // Update properties that change
        if (QRCodeObj != null && _lastTimeStamp != QRCodeObj.SystemRelativeLastDetectedTime.Ticks)
        {
            PhysicalSize = QRCodeObj.PhysicalSideLength;

            //_qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
            _qrCodeCube.transform.localPosition = new Vector3(PhysicalSize , PhysicalSize , 0.0f);
            //_qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
            _lastTimeStamp = QRCodeObj.SystemRelativeLastDetectedTime.Ticks;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePropertiesDisplay();
    }
} // end of class