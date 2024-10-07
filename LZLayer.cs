using System.Collections.Generic;
using VNyanInterface;
using UnityEngine;
using LZArmSway;


namespace LZLayer
{
    public class LZLayer : IPoseLayer
    {
        // Creates class based on "IPoseLayer" instance, so we have to implement everything.

        // Set up our frame, I think this is needed because doUpdate() is looking for a PoseLayerFrame argument.
        public PoseLayerFrame LZLayerFrame = new PoseLayerFrame();
        // I just set up holding arguments here that we'll load in the tracked data each frame from doUpdate(). Follows the format found in the "PoseLayerFrame" Class
        public Dictionary<int, VNyanQuaternion> BoneRotations;
        public Dictionary<int, VNyanVector3> BonePositions;
        public Dictionary<int, VNyanVector3> BoneScales;
        public VNyanVector3 RootPos;
        public VNyanQuaternion RootRot;

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

        // Bone Transform values
        // You can do your bone transformations in these get functions by adjusting the values in the dictionaries and Root values
        // This is what VNyan pulls from to "get" our pose from our pose layer. VNyan handles the actual bone rotation/position transforms, we just need to adjust the values.
        // Probably, we could build "set" functions for ourselves on top of this that would let us do the transformations that go inside these "get" functions.
        VNyanVector3 IPoseLayer.getBonePosition(int i)
        {
            return BonePositions[i];
        }
        VNyanQuaternion IPoseLayer.getBoneRotation(int i)
        {
            return BoneRotations[i];
        }
        VNyanVector3 IPoseLayer.getBoneScaleMultiplier(int i)
        {
            return BoneScales[i];
        }
        VNyanVector3 IPoseLayer.getRootPosition()
        {
            return RootPos;
        }
        VNyanQuaternion IPoseLayer.getRootRotation()
        {
            return RootRot;
        }

        // This is probably to be able to turn the pose off. Likely if we build this to be able to be changed, then you'd have a quick way to toggle thi pose layer
        bool IPoseLayer.isActive()
        {
            return true;
        }

        // doUpdate is how we get all current bone values.
        // From Suvi: basically the doUpdate()-function gives you a tracked frame. This basically all bone values tracked until that point. Then you're supposed to do your calculations on values and return new ones with the getBonePosition() and getBoneRotation(). So you can completely overwrite the pose or alternatively you can blend it in.
        public void doUpdate(in PoseLayerFrame LZLayerFrame)
        {
            // Get all current Bone and Root values up to this point, and load them in our holdover values.
            BoneRotations = LZLayerFrame.BoneRotation;
            BonePositions = LZLayerFrame.BonePosition;
            BoneScales = LZLayerFrame.BoneScaleMultiplier;
            RootPos = LZLayerFrame.RootPosition;
            RootRot = LZLayerFrame.RootRotation;

            // NOTE:
            // I think this is where we will need to do actual calculations? "doUpdate" is everything that VNyan will do for our pose layer
            // (calculations wise) at this moment in frame calculation. So anything we do after we Get" the current BoneRotations will be what gets 
            // set in the frame.

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

            // Now we should do the rotations here
            // bone # references: 12, 14, 16, 18
            BoneRotations[12].Z = BoneRotations[12].Z - LeftShoulderZ;
            BoneRotations[14].Z = BoneRotations[14].Z - LeftUpperArmZ;
            BoneRotations[16].Z = BoneRotations[16].Z - LeftLowerArmZ;
            BoneRotations[18].Z = BoneRotations[18].Z - LeftHandZ;

            // This actuall does *something*, but the rotations are wrong. Likely we need to do proper rotations in quartenion space, rather than just directly
            // adding values. Even in unity the components note "do not modify this directly". https://docs.unity3d.com/ScriptReference/Quaternion.html
            // in unity we could do like Quarternion A * Quartenion.Euler(90, 0, 0) to rotate about X axis in 90 degrees.
            // but VNyanQuartenion's don't seem to inherit this multiplier method?

            // I wonder if we could create our own rotation method. It seems to be a matrix dot product. so probably trivial.


            // Maybe we can do something like this instead?
            // 1. convert VNyanQuarternion into Unity Quarternion (possibly just create new Quarternion with the VNyan's x/y/z/w values copied directly
            // 2. do the rotation as we want
            // 3. convert back to unity quarternion

            // That seems like a lot of extra work to happen every frame for like 100 bones though.


            // This is what Jayo did with her VMC plugin (when receiving from VMC)
            //  Quaternion rot = new Quaternion((float)message.values[4], (float)message.values[5], (float)message.values[6], (float)message.values[7]);
            // so, yes to just inserting directly, but she wasn't mixing anything.

            // could ignore mixing for now and just replace.
            // So we do Quaternion.Euler(X,Y,Z)
            // and then transplant the quarternion into the VNyanQuarternion
            Quaternion LeftHandNewRotation = Quaternion.Euler(0, 0, -LeftHandZ);
            BoneRotations[18].X = LeftHandNewRotation.x;
            BoneRotations[18].Y = LeftHandNewRotation.y;
            BoneRotations[18].Z = LeftHandNewRotation.z;
            BoneRotations[18].W = LeftHandNewRotation.w;


        }
    }
}
