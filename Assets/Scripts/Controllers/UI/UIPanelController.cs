using System.Collections.Generic;
using System.Linq;
using Controllers.Player;
using Enums;
using Signals;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace Controllers.UI
{
    public class UIPanelController : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        #endregion

        #region Serialized Variables

        [SerializeField] private List<Transform> layers = new List<Transform>();

        #endregion


        #endregion

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreUISignals.Instance.onOpenPanel += OnOpenPanel;
            CoreUISignals.Instance.onClosePanel += OnClosePanel;
            CoreUISignals.Instance.onCloseAllPanels += OnCloseAllPanels;
        }
        private void UnSubscribeEvents()
        {
            CoreUISignals.Instance.onOpenPanel -= OnOpenPanel;
            CoreUISignals.Instance.onClosePanel -= OnClosePanel;
            CoreUISignals.Instance.onCloseAllPanels -= OnCloseAllPanels;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        [Button("OpenPanel")]
        private void OnOpenPanel(UIPanelTypes type, int layerPos)
        {
            Instantiate(Resources.Load<GameObject>($"Screens/{type}Panel"), layers[layerPos]);
        }
        [Button("ClosePanel")]
        private void OnClosePanel(int layerValue)
        {
            if (layers[layerValue].transform.childCount > 0)
            {
                Destroy(layers[layerValue].GetChild(0).gameObject);
            }

        }
        [Button("OnCloseAllPanel")]
        private void OnCloseAllPanels()
        {
            foreach (var t in layers.Where(t => t.childCount > 0))
            {
                Destroy(t.GetChild(0).gameObject);
            }
        }
    }

}