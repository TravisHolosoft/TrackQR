using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QRTracking
{
    public struct ActionData
    {
        public enum Type
        {
            Added,
            Updated,
            Removed
        };
        public Type type;
        public Microsoft.MixedReality.QR.QRCode qrCode;

        public ActionData(Type type, Microsoft.MixedReality.QR.QRCode qRCode) : this()
        {
            this.type = type;
            qrCode = qRCode;
        }
    }

    public class QRInstrumentVisualizer : MonoBehaviour
    {
        public GameObject QRInstrument120mm;
        //public GameObject QRInstrument60mm;

        private SortedDictionary<System.Guid, GameObject> _qrCodesObjectsList;
        private Queue<ActionData> _pendingActions = new Queue<ActionData>();
        private bool _clearExisting = false;


        // Use this for initialization
        void Start()
        {
            _qrCodesObjectsList = new SortedDictionary<System.Guid, GameObject>();

            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
        }

        private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                _clearExisting = true;
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        private void HandleEvents()
        {
            lock (_pendingActions)
            {
                while (_pendingActions.Count > 0)
                {
                    var action = _pendingActions.Dequeue();

                    if (action.type == ActionData.Type.Added)
                    {
                        switch (action.qrCode.Data)
                        {
                            case "333333":
                                GameObject qrCode120 = Instantiate(QRInstrument120mm, new Vector3(0, 0, 0), Quaternion.identity);
                                qrCode120.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                                qrCode120.GetComponent<QRInstrument>().QRCodeObj = action.qrCode;
                                _qrCodesObjectsList.Add(action.qrCode.Id, qrCode120);
                                //QRInfo.text += $"Added={action.qrCode.Data}\r\n";
                                break;
                            case "666666":
                                //GameObject qrCode60 = Instantiate(QR60mm, new Vector3(0, 0, 0), Quaternion.identity);
                                //qrCode60.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                                //qrCode60.GetComponent<QRInstrument>().qrCode = action.qrCode;
                                //_qrCodesObjectsList.Add(action.qrCode.Id, qrCode60);
                                break;
                        }
                    }
                    else if (action.type == ActionData.Type.Updated)
                    {
                        if (!_qrCodesObjectsList.ContainsKey(action.qrCode.Id))
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
                        if (_qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            //QRInfo.text += $"Removed={action.qrCode.Data}\r\n";
                            Destroy(_qrCodesObjectsList[action.qrCode.Id]);
                            _qrCodesObjectsList.Remove(action.qrCode.Id);
                        }
                    }
                }
            }
            if (_clearExisting)
            {
                _clearExisting = false;
                foreach (var obj in _qrCodesObjectsList)
                {
                    Destroy(obj.Value);
                }
                _qrCodesObjectsList.Clear();
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleEvents();
        }
    }
} // end of ns