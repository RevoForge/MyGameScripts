using UnityEngine;
using Valve.VR;

namespace Wacki
{
    public class SteamVRUILaserPointer : IUILaserPointer
    {
        public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
        private SteamVR_Action_Boolean button;
        private SteamVR_Behaviour_Pose _trackedObject1;
        private SteamVR_Behaviour_Skeleton _trackedObject2;
        private bool _connected = false;

        protected override void Initialize()
        {
            base.Initialize();
            button = SteamVR_Actions.default_InteractUI;
            _trackedObject1 = GetComponentInParent<SteamVR_Behaviour_Pose>();

            if (_trackedObject1 != null)
            {
                _connected = true;
            }
            _trackedObject2 = GetComponentInParent<SteamVR_Behaviour_Skeleton>();

            if (_trackedObject2 != null)
            {
                _connected = true;
            }
        }

        public override bool ButtonDown()
        {
            if (!_connected)
                return false;

            return button.GetStateDown(inputSource);
        }

        public override bool ButtonUp()
        {
            if (!_connected)
                return false;

            return button.GetStateUp(inputSource);
        }

        public override void OnEnterControl(GameObject control)
        {
            if (!_connected)
                return;
            //     SteamVR_Actions.default_Haptic.Execute(0.5f, 0.1f, 1.0f, 1.0f, inputSource);
        }

        public override void OnExitControl(GameObject control)
        {
            if (!_connected)
                return;
            //     SteamVR_Actions.default_Haptic.Execute(0.3f, 0.1f, 1.0f, 1.0f, inputSource);
        }

        int controllerIndex
        {
            get
            {
                if (!_connected)
                    return 0;

                if (_trackedObject1 != null)
                    return (int)_trackedObject1.inputSource;

                if (_trackedObject2 != null)
                    return (int)_trackedObject2.inputSource;

                return 0;
            }
        }
    }
}
