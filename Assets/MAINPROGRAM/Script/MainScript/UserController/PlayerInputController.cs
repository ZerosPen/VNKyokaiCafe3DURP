using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DIALOGUE
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;

        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();

        // Start is called before the first frame update
        private void Awake()
        {
            input = GetComponent<PlayerInput>();

            InitializeAction();
        }

        private void InitializeAction()
        {
            actions.Add((input.actions["Next"], OnNext));
        }

        private void OnEnable()
        {
            foreach (var inputAction in actions)
                inputAction.action.performed += inputAction.command;
        }

        private void OnDisable()
        {
            foreach (var inputAction in actions)
                inputAction.action.performed -= inputAction.command;
        }

        public void OnNext(InputAction.CallbackContext c)
        {
            DialogController.Instance.OnUserPromt_Next();
        }
    }
}
