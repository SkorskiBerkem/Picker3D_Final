using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controllers.Pool;
using Controllers.UI;
using DG.Tweening;
using Managers;
using Signals;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        public TextMeshPro gemtext;
        public List<GameObject> RewardInTrigger;
        public int gemamount;

        #endregion

        #region Serialized Variables

        [SerializeField] private PlayerManager manager;
        [SerializeField] private new Collider collider;
        [SerializeField] private new Rigidbody rigidbody;

        #endregion

        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("StageArea"))
            {
                manager.ForceCommand.Execute();
                CoreGameSignals.Instance.onStageAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();
                DOVirtual.DelayedCall(3, () =>
                {
                    var result = other.transform.parent.GetComponentInChildren<PoolController>()
                        .TakeStageResult(manager.StageValue);
                    if (result)
                    {
                        CoreGameSignals.Instance.onStageAreaSuccessful?.Invoke(manager.StageValue);
                        InputSignals.Instance.onEnableInput?.Invoke();
                    }
                    else CoreGameSignals.Instance.onLevelFailed?.Invoke();
                });
                return;
            }

            if (other.CompareTag("Finish"))
            {
                CoreGameSignals.Instance.onFinishAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();
                CoreGameSignals.Instance.onLevelSuccessful?.Invoke();
                return;
            }

            if (other.CompareTag("MiniGame"))
            {
                CoreGameSignals.Instance.onMinigameAreaEntered?.Invoke();
            }

            if (other.CompareTag("Gem"))
            {
                RewardInTrigger.Add(other.gameObject);
                gemamount += int.Parse(other.gameObject.name);
                Debug.Log("gems:" + gemamount);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag == "Gem")
            {
                RewardInTrigger.Remove(other.gameObject);
                gemamount -= int.Parse(other.gameObject.name);


            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var transform1 = manager.transform;
            var position = transform1.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - 1.2f, position.z + 1f), 1.65f);
        }

        internal void OnReset()
        {
        }

        internal void UpdateGem()
        {
            UIManager.Instance.RewardText.text = gemamount.ToString();
        }

    }

}