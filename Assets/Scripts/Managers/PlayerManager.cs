using System.Security.Cryptography;
using Commands.Player;
using Controllers;
using Controllers.Player;
using Controllers.UI;
using Data.UnityObjects;
using Data.ValueObjects;
using DG.Tweening;
using Keys;
using Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Update = Unity.VisualScripting.Update;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        public byte StageValue = 0;

        internal ForceBallsToPoolCommand ForceCommand;

        #endregion

        #region Serialized Variables

        [SerializeField] private PlayerMovementController movementController;
        [SerializeField] private PlayerPhysicsController physicsController;
        [SerializeField] private PlayerMeshController meshController;

        #endregion

        #region Private Variables

        [ShowInInspector] private PlayerData _data;

        #endregion

        #endregion

        private void Awake()
        {
            _data = GetPlayerData();
            SendDataToControllers();
            Init();
        }

        private void Init()
        {
            ForceCommand = new ForceBallsToPoolCommand(this, _data.MovementData);
        }

        private PlayerData GetPlayerData()
        {
            return Resources.Load<CD_Player>("Data/CD_Player").Data;
        }

        private void SendDataToControllers()
        {
            movementController.GetMovementData(_data.MovementData);
            meshController.GetMeshData(_data.ScaleData);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            InputSignals.Instance.onInputTaken += OnInputTaken;
            InputSignals.Instance.onInputReleased += OnInputReleased;
            InputSignals.Instance.onInputDragged += OnInputDragged;
            CoreGameSignals.Instance.onPlay += OnPlay;
            CoreGameSignals.Instance.onLevelSuccessful += OnLevelSuccessful;
            CoreGameSignals.Instance.onLevelFailed += OnLevelFailed;
            CoreGameSignals.Instance.onStageAreaEntered += OnStageAreaEntered;
            CoreGameSignals.Instance.onFinishAreaEntered += OnFinishAreaEntered;
            CoreGameSignals.Instance.onStageAreaSuccessful += OnStageAreaSuccessful;
            CoreGameSignals.Instance.onReset += OnReset;
            CoreGameSignals.Instance.onMinigameAreaEntered += OnMinigameAreaEntered;
            CoreGameSignals.Instance.updateGems += UpdateGems;
        }

        private void UnSubscribeEvents()
        {
            InputSignals.Instance.onInputTaken -= OnInputTaken;
            InputSignals.Instance.onInputReleased -= OnInputReleased;
            InputSignals.Instance.onInputDragged -= OnInputDragged;
            CoreGameSignals.Instance.onPlay -= OnPlay;
            CoreGameSignals.Instance.onLevelSuccessful -= OnLevelSuccessful;
            CoreGameSignals.Instance.onLevelFailed -= OnLevelFailed;
            CoreGameSignals.Instance.onStageAreaEntered -= OnStageAreaEntered;
            CoreGameSignals.Instance.onFinishAreaEntered -= OnFinishAreaEntered;
            CoreGameSignals.Instance.onStageAreaSuccessful -= OnStageAreaSuccessful;
            CoreGameSignals.Instance.onReset -= OnReset;
            CoreGameSignals.Instance.onMinigameAreaEntered -= OnMinigameAreaEntered;
            CoreGameSignals.Instance.updateGems -= UpdateGems;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void OnPlay()
        {
            movementController.IsReadyToPlay(true);
        }

        private void OnInputTaken()
        {
            movementController.IsReadyToMove(true);
        }

        private void OnInputDragged(HorizontalInputParams inputParams)
        {
            movementController.UpdateInputParams(inputParams);
        }

        private void OnInputReleased()
        {
            movementController.IsReadyToMove(false);
        }

        private void OnLevelSuccessful()
        {
            movementController.IsReadyToPlay(false);

        }

        private void OnLevelFailed()
        {
            movementController.IsReadyToPlay(false);
        }

        private void OnStageAreaEntered()
        {
            movementController.IsReadyToPlay(false);
        }

        private void OnStageAreaSuccessful(int value)
        {
            StageValue = (byte)++value;
            movementController.IsReadyToPlay(true);
            meshController.ScaleUpPlayer();
            meshController.ShowUpText();
            meshController.PlayConfettiParticle();
        }
        private void OnMinigameAreaEntered()
        {
            meshController.PlayBoostParticle();
            movementController.SpeedBoost();
        }

        private void OnFinishAreaEntered()
        {
            movementController.IsReadyToPlay(false);
        }

        private void UpdateGems()
        {
            movementController.IsReadyToPlay(false);
            physicsController.UpdateGem();
        }

        private void OnReset()
        {
            StageValue = 0;
            movementController.OnReset();
            meshController.OnReset();
            physicsController.OnReset();
        }
    }
}