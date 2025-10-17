using System.Collections.Generic;
using Aarthificial.Reanimation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerRenderer : MonoBehaviour
    {
        private static class Drivers
        {
            public const string DashCompletion = "dashCompletion";
            public const string HitDirection = "hitDirection";
            public const string AttackCompletion = "attackCompletion";
            public const string IsGrounded = "isGrounded";
            public const string IsMoving = "isMoving";
            public const string State = "state";
            public const string StepEvent = "stepEvent";
        }

        private Reanimator _reanimator;
        private PlayerController _controller;

        private void Awake()
        {
            _reanimator = GetComponent<Reanimator>();
            _controller = GetComponent<PlayerController>();
        }
        private void FixedUpdate()
        {
            var velocity = _controller.Velocity;
            bool isMoving = Mathf.Abs(_controller.DesiredDirection.x) > 0 && Mathf.Abs(velocity.x) > 0.01f
                || Mathf.Abs(_controller.DesiredDirection.y) > 0 && Mathf.Abs(velocity.y) > 0.01f;
            

            _reanimator.Flip = _controller.FacingDirection > 0;
            _reanimator.Set(Drivers.IsMoving, isMoving);
        }

        private void Update()
        {
            _reanimator.Set(Drivers.DashCompletion, _controller.DashCompletion);
            _reanimator.Set(Drivers.AttackCompletion, _controller.AttackCompletion);
            _reanimator.Set(Drivers.State, (int)_controller.State);
            bool didDashInThisFrame = _reanimator.WillChange(Drivers.State, (int)PlayerState.Dash);
            bool didLandInThisFrame = _reanimator.WillChange(Drivers.IsGrounded, true);
            if (didLandInThisFrame || didDashInThisFrame)
            {
                _reanimator.ForceRerender();
            }
        }


       
    }
}