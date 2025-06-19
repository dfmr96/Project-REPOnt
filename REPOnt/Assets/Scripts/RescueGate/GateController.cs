using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace RescueGate
{
    public class GateController : MonoBehaviourPun
    {
        [SerializeField] private Transform gateModel;
        [SerializeField] private float openHeight = 3f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float autoCloseDelay = 5f;

        private Vector3 _closedPosition;
        private Vector3 _openPosition;

        private Coroutine _moveCoroutine;

        public bool IsBusy { get; private set; }

        public event Action OnGateOpened;
        public event Action OnGateClosed;

        private void Awake()
        {
            _closedPosition = gateModel.localPosition;
            _openPosition = _closedPosition + Vector3.up * openHeight;
        }

        public void OpenGate()
        {
            photonView.RPC(nameof(OpenGateRPC), RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void OpenGateRPC()
        {
            if (IsBusy) return;

            Debug.Log("[GateController] RPC received. Gate opening!");
            StartGateMovement(_openPosition, autoCloseDelay, () =>
            {
                if (photonView.IsMine)
                    photonView.RPC(nameof(CloseGateRPC), RpcTarget.AllBuffered);
            });
        }

        [PunRPC]
        public void CloseGateRPC()
        {
            Debug.Log("[GateController] RPC received. Gate closing!");
            StartGateMovement(_closedPosition, 0f, () => OnGateClosed?.Invoke());
        }

        private void StartGateMovement(Vector3 target, float delayAfter, Action onComplete)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(MoveGateRoutine(target, delayAfter, onComplete));
        }

        private IEnumerator MoveGateRoutine(Vector3 targetPosition, float delayAfter, Action onComplete)
        {
            IsBusy = true;

            while (Vector3.Distance(gateModel.localPosition, targetPosition) > 0.01f)
            {
                gateModel.localPosition = Vector3.MoveTowards(gateModel.localPosition, targetPosition, Time.deltaTime * moveSpeed);
                yield return null;
            }

            gateModel.localPosition = targetPosition;

            if (targetPosition == _openPosition)
                OnGateOpened?.Invoke();
            else if (targetPosition == _closedPosition)
                OnGateClosed?.Invoke();

            yield return new WaitForSeconds(delayAfter);

            IsBusy = false;

            onComplete?.Invoke();
        }
    }
}