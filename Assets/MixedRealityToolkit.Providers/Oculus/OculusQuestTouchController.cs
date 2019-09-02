﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusTouch,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/OculusControllersTouch")]
    public class OculusQuestTouchController : GenericJoystickController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusQuestTouchController(TrackingState trackingState, Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            nodeType = controllerHandedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand;
        }

        private XRNode nodeType;

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastXrNodeStateReading { get; protected set; }

        /// <summary>
        /// Tracking states returned from the InputTracking state tracking manager
        /// </summary>
        private readonly List<XRNodeState> nodeStates = new List<XRNodeState>();

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(2, "Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(3, "Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_13),
            new MixedRealityInteractionMapping(4, "Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(5, "Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            new MixedRealityInteractionMapping(6, "Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2),
            new MixedRealityInteractionMapping(7, "Button.PrimaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(8, "Button.PrimaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_15),
            new MixedRealityInteractionMapping(9, "Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(10, "Button.Three Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(11, "Button.Four Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(12, "Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(13, "Button.Three Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton12),
            new MixedRealityInteractionMapping(14, "Button.Four Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton13),
            new MixedRealityInteractionMapping(15, "Touch.PrimaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, KeyCode.JoystickButton18),
            new MixedRealityInteractionMapping(16, "Touch.PrimaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_17)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Axis1D.SecondaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(2, "Axis1D.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(3, "Axis1D.SecondaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, ControllerMappingLibrary.AXIS_14),
            new MixedRealityInteractionMapping(4, "Axis1D.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(5, "Axis1D.SecondaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
            new MixedRealityInteractionMapping(6, "Axis2D.SecondaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5),
            new MixedRealityInteractionMapping(7, "Button.SecondaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(8, "Button.SecondaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_16),
            new MixedRealityInteractionMapping(9, "Button.SecondaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(10, "Button.One Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(11, "Button.Two Press", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(12, "Button.One Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton10),
            new MixedRealityInteractionMapping(13, "Button.Two Touch", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton11),
            new MixedRealityInteractionMapping(14, "Touch.SecondaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, KeyCode.JoystickButton19),
            new MixedRealityInteractionMapping(15, "Touch.SecondaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, ControllerMappingLibrary.AXIS_18)
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }

        /// <inheritdoc />
        public override void UpdateController()
        {
            if (!Enabled) { return; }

            InputTracking.GetNodeStates(nodeStates);

            for (int i = 0; i < nodeStates.Count; i++)
            {
                if (nodeStates[i].nodeType == nodeType)
                {
                    var xrNodeState = nodeStates[i];
                    UpdateControllerData(xrNodeState);
                    LastXrNodeStateReading = xrNodeState;
                    break;
                }
            }

            base.UpdateController();
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="state"></param>
        protected void UpdateControllerData(XRNodeState state)
        {
            var lastState = TrackingState;

            LastControllerPose = CurrentControllerPose;

            if (nodeType == XRNode.LeftHand || nodeType == XRNode.RightHand)
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = state.TryGetPosition(out CurrentControllerPosition);
                IsPositionApproximate = false;

                IsRotationAvailable = state.TryGetRotation(out CurrentControllerRotation);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                CurrentControllerPosition = MixedRealityPlayspace.TransformPoint(CurrentControllerPosition);
                CurrentControllerRotation = MixedRealityPlayspace.Rotation * CurrentControllerRotation;
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            CurrentControllerPose.Position = CurrentControllerPosition;
            CurrentControllerPose.Rotation = CurrentControllerRotation;

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && LastControllerPose != CurrentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePoseChanged(InputSource, this, CurrentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePositionChanged(InputSource, this, CurrentControllerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourceRotationChanged(InputSource, this, CurrentControllerRotation);
                }
            }
        }
    }
}