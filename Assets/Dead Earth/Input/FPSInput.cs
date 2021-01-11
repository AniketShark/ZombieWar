// GENERATED AUTOMATICALLY FROM 'Assets/Dead Earth/Input/FPSInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @FPSInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @FPSInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""FPSInput"",
    ""maps"": [
        {
            ""name"": ""Kyeboard/Mouse"",
            ""id"": ""f2c24292-906a-47f8-baeb-f8e4ae15989f"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0c6f03f9-3f1f-4683-8f28-5cc1a871967c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""46cff049-cf28-4e75-9ad6-ae747d76a4ea"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b34b0394-22fe-4f04-a720-276fbbc56363"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""275c8afd-566f-4932-92a2-548988c17e21"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3e8539a0-e825-4779-90a6-f6e434849052"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b7692942-d9e6-4a8c-b35b-1c0899873c3e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""7710e093-a6a1-408d-9cf0-789541d05d38"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a2533cc8-e66c-4ca8-8f87-cb41583e854a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2285b8f7-7b64-4e7e-8f4b-1f27cb7e6cd6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""55d5b91b-4d35-4623-9709-acb83bc0252d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ac793d74-d2ae-49ac-afc2-4ee7a5175169"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4db283be-debe-49d6-b72f-89434335120f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Kyeboard/Mouse
        m_KyeboardMouse = asset.FindActionMap("Kyeboard/Mouse", throwIfNotFound: true);
        m_KyeboardMouse_Look = m_KyeboardMouse.FindAction("Look", throwIfNotFound: true);
        m_KyeboardMouse_Sprint = m_KyeboardMouse.FindAction("Sprint", throwIfNotFound: true);
        m_KyeboardMouse_Move = m_KyeboardMouse.FindAction("Move", throwIfNotFound: true);
        m_KyeboardMouse_Jump = m_KyeboardMouse.FindAction("Jump", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Kyeboard/Mouse
    private readonly InputActionMap m_KyeboardMouse;
    private IKyeboardMouseActions m_KyeboardMouseActionsCallbackInterface;
    private readonly InputAction m_KyeboardMouse_Look;
    private readonly InputAction m_KyeboardMouse_Sprint;
    private readonly InputAction m_KyeboardMouse_Move;
    private readonly InputAction m_KyeboardMouse_Jump;
    public struct KyeboardMouseActions
    {
        private @FPSInput m_Wrapper;
        public KyeboardMouseActions(@FPSInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Look => m_Wrapper.m_KyeboardMouse_Look;
        public InputAction @Sprint => m_Wrapper.m_KyeboardMouse_Sprint;
        public InputAction @Move => m_Wrapper.m_KyeboardMouse_Move;
        public InputAction @Jump => m_Wrapper.m_KyeboardMouse_Jump;
        public InputActionMap Get() { return m_Wrapper.m_KyeboardMouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KyeboardMouseActions set) { return set.Get(); }
        public void SetCallbacks(IKyeboardMouseActions instance)
        {
            if (m_Wrapper.m_KyeboardMouseActionsCallbackInterface != null)
            {
                @Look.started -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnLook;
                @Sprint.started -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnSprint;
                @Move.started -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_KyeboardMouseActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_KyeboardMouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public KyeboardMouseActions @KyeboardMouse => new KyeboardMouseActions(this);
    public interface IKyeboardMouseActions
    {
        void OnLook(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}
