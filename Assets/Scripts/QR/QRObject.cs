
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

namespace QRTracking
{
    [RequireComponent(typeof(QRTracking.SpatialGraphNodeTracker))]
    public class QRObject : MonoBehaviour
    {
        public Microsoft.MixedReality.QR.QRCode qrCode;
        private GameObject _qrCodeBase;

        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }

        private long lastTimeStamp = 0;

        // Use this for initialization
        void Start()
        {
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;

            _qrCodeBase = gameObject.transform.Find("Base").gameObject;
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                PhysicalSize = qrCode.PhysicalSideLength;

                _qrCodeBase.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                _qrCodeBase.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
        }

        public void OnInputClicked()
        {
        }
    }
}