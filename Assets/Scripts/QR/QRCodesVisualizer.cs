
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QRTracking
{
    public class QRCodesVisualizer : MonoBehaviour
    {
        public GameObject qrCodePrefab;
        public TextMeshPro QRInfo;

        public GameObject QR120mm;
        public GameObject QR60mm;

        public GameObject QRFootObj;
        public GameObject QRArmObj;

        private SortedDictionary<System.Guid, GameObject> qrCodesObjectsList;
        private Queue<ActionData> pendingActions = new Queue<ActionData>();
        private bool clearExisting = false;

        // Use this for initialization
        void Start()
        {
            QRInfo.text = "";
            qrCodesObjectsList = new SortedDictionary<System.Guid, GameObject>();

            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
            if (qrCodePrefab == null)
            {
                throw new System.Exception("Prefab not assigned");
            }
        }
        private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                clearExisting = true;
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        private void HandleEvents()
        {
            lock (pendingActions)
            {
                while (pendingActions.Count > 0)
                {
                    var action = pendingActions.Dequeue();

                    if (action.type == ActionData.Type.Added)
                    {
                        switch(action.qrCode.Data)
                        {
                            case "333333":
                                GameObject qrCode120 = Instantiate(QR120mm, new Vector3(0, 0, 0), Quaternion.identity);
                                qrCode120.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                                qrCode120.GetComponent<QRCode>().qrCode = action.qrCode;
                                qrCodesObjectsList.Add(action.qrCode.Id, qrCode120);
                                QRInfo.text += $"Added={action.qrCode.Data}\r\n";
                                break;
                            case "666666":
                                GameObject qrCode60 = Instantiate(QR60mm, new Vector3(0, 0, 0), Quaternion.identity);
                                qrCode60.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                                qrCode60.GetComponent<QRCode>().qrCode = action.qrCode;
                                qrCodesObjectsList.Add(action.qrCode.Id, qrCode60);
                                QRInfo.text += $"Added={action.qrCode.Data}\r\n";
                                break;
                            case "101001":
                                GameObject qrFoot = Instantiate(QRFootObj, new Vector3(0, 0, 0), Quaternion.identity);
                                qrFoot.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                                qrFoot.GetComponent<QRObject>().qrCode = action.qrCode;
                                qrCodesObjectsList.Add(action.qrCode.Id, qrFoot);
                                QRInfo.text += $"Added={action.qrCode.Data}\r\n";
                                break;
                            case "101002":
                                GameObject qrArm = Instantiate(QRArmObj, new Vector3(0, 0, 0), Quaternion.identity);
                                qrArm.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                                qrArm.GetComponent<QRObject>().qrCode = action.qrCode;
                                qrCodesObjectsList.Add(action.qrCode.Id, qrArm);
                                QRInfo.text += $"Added={action.qrCode.Data}\r\n";
                                break;
                            default:
                                QRInfo.text += $"Found={action.qrCode.Data}\r\n";
                                break;
                        }
                    }
                    else if (action.type == ActionData.Type.Updated)
                    {
                        if (!qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            //GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            //qrCodeObject.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                            //qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                            //qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);

                            //QRInfo.text += $"Updated={action.qrCode.Data}\r\n";
                        }
                    }
                    else if (action.type == ActionData.Type.Removed)
                    {
                        if (qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            QRInfo.text += $"Removed={action.qrCode.Data}\r\n";
                            Destroy(qrCodesObjectsList[action.qrCode.Id]);
                            qrCodesObjectsList.Remove(action.qrCode.Id);
                        }
                    }
                }
            }
            if (clearExisting)
            {
                clearExisting = false;
                foreach (var obj in qrCodesObjectsList)
                {
                    Destroy(obj.Value);
                }
                qrCodesObjectsList.Clear();

            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleEvents();
        }
    }
}