using System;
using UnityEngine;
using VNyanInterface;
using LZLayer;
using System.Collections.Generic;

namespace LZArmSwayy_Test1
{
    // Test case, this just instantiates/registers the pose layer, and then does basic sway calculation based on pendulum chain to check if 
    // it's what we expect.
    public class LZArmSway_Test1 : MonoBehaviour
    {
        // Set up pose layer according to our class
        IPoseLayer LZSwayLayer = new LZLayer.LZLayer();

        public Dictionary<int, VNyanQuaternion> swayBoneRotations { get; set; }


        // Set private variables
        private float pendulumSwayMult = 0f;
        private float ShoulderSway = 0f;
        private float UpperSway = 0f;
        private float LowerSway = 0f;
        private float HandSway = 0f;


        // Set public Pose Parameters
        public float LeftShoulderZ = 0f;
        public float LeftUpperArmZ = 0f;
        public float LeftLowerArmZ = 0f;
        public float LeftHandZ = 0f;

        public void Start()
        {
            // Register Pose Layer for VNyan to listen to
            VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(LZSwayLayer);
        }

        public void Update()
        {
            // Get Pendulum sway value
            // TODO: replace with internal pendulum in code
            pendulumSwayMult = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat("SwayMultiplier_L");
            ShoulderSway = 2 * pendulumSwayMult;
            UpperSway = 1 * pendulumSwayMult;
            LowerSway = 1 * pendulumSwayMult;
            HandSway = 10 * pendulumSwayMult;

            // Calculate Rotation values
            LeftShoulderZ = ShoulderSway;
            LeftUpperArmZ = UpperSway + 5;
            LeftLowerArmZ = LowerSway;
            LeftHandZ = HandSway - 15;

            // // Check values in VNyan
            // VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("LeftShoulderZ2", LeftShoulderZ);
            // VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("LeftUpperArmZ2", LeftUpperArmZ);
            // VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("LeftLowerArmZ2", LeftLowerArmZ);
            // VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("LeftHandZ2", LeftHandZ);
        }
    }
}