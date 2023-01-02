namespace IFacial
{
    public class CapturedData
    {
        public class TrackingData
        {
            public class RotationData
            {
                public float X { get; set; }
                public float Y { get; set; }
                public float Z { get; set; }
            }

            public class PositionData
            {
                public float X { get; set; }
                public float Y { get; set; }
                public float Z { get; set; }
            }

            public class HeadData
            {
                public RotationData Rotation { get; set; } = new RotationData();
                public PositionData Position { get; set; } = new PositionData();
            }

            public class EyeData
            {
                public RotationData Rotation { get; set; } = new RotationData();
            }

            public HeadData Head { get; set; } = new HeadData();
            public EyeData RightEye { get; set; } = new EyeData();
            public EyeData LeftEye { get; set; } = new EyeData();
        }
        public class BlendShapesData
        {
            // Left Eye
            public float EyeBlinkLeft { get; set; }
            public float EyeLookDownLeft { get; set; }
            public float EyeLookInLeft { get; set; }
            public float EyeLookOutLeft { get; set; }
            public float EyeLookUpLeft { get; set; }
            public float EyeSquintLeft { get; set; }
            public float EyeWideLeft { get; set; }

            // Right Eye
            public float EyeBlinkRight { get; set; }
            public float EyeLookDownRight { get; set; }
            public float EyeLookInRight { get; set; }
            public float EyeLookOutRight { get; set; }
            public float EyeLookUpRight { get; set; }
            public float EyeSquintRight { get; set; }
            public float EyeWideRight { get; set; }

            // Mouth and Jaw
            public float JawForward { get; set; }
            public float JawLeft { get; set; }
            public float JawRight { get; set; }
            public float JawOpen { get; set; }
            public float MouthClose { get; set; }
            public float MouthFunnel { get; set; }
            public float MouthPucker { get; set; }
            public float MouthLeft { get; set; }
            public float MouthRight { get; set; }
            public float MouthSmileLeft { get; set; }
            public float MouthSmileRight { get; set; }
            public float MouthFrownLeft { get; set; }
            public float MouthFrownRight { get; set; }
            public float MouthDimpleLeft { get; set; }
            public float MouthDimpleRight { get; set; }
            public float MouthStretchLeft { get; set; }
            public float MouthStretchRight { get; set; }
            public float MouthRollLower { get; set; }
            public float MouthRollUpper { get; set; }
            public float MouthShrugLower { get; set; }
            public float MouthShrugUpper { get; set; }
            public float MouthPressLeft { get; set; }
            public float MouthPressRight { get; set; }
            public float MouthLowerDownLeft { get; set; }
            public float MouthLowerDownRight { get; set; }
            public float MouthUpperUpLeft { get; set; }
            public float MouthUpperUpRight { get; set; }

            // Eyebrows, Cheeks, and Nose
            public float BrowDownLeft { get; set; }
            public float BrowDownRight { get; set; }
            public float BrowInnerUp { get; set; }
            public float BrowOuterUpLeft { get; set; }
            public float BrowOuterUpRight { get; set; }
            public float CheekPuff { get; set; }
            public float CheekSquintLeft { get; set; }
            public float CheekSquintRight { get; set; }
            public float NoseSneerLeft { get; set; }
            public float NoseSneerRight { get; set; }

            // Tongue
            public float TongueOut { get; set; }
        }

        public TrackingData Tracking { get; private set; } = new TrackingData();
        public BlendShapesData BlendShapes { get; private set; } = new BlendShapesData();
    }
}
