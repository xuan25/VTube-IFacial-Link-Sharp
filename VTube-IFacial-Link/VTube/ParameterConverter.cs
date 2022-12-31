﻿using IFacial;
using VTube.DataModel;

namespace VTube
{
    static class ParameterConverter
    {
        const double FACE_POSITION_X_RATIO = 100;
        const double FACE_POSITION_Y_RATIO = 100;
        const double FACE_POSITION_Z_RATIO = 20;

        const double FACE_ANGLE_X_RATIO = 1;
        const double FACE_ANGLE_Y_RATIO = 1;
        const double FACE_ANGLE_Z_RATIO = 1;

        const double MOUTH_SMILE_RATIO = 2;
        const double MOUTH_OPEN_RATIO = 1.2;

        const double BROWS_RATIO = 2;

        const double TONGUE_OUT_RATIO = 0.4;

        const double EYE_OPEN_RATIO = 1.25;
        const double EYE_ROTATION_RATIO = 1.5;

        const double CHEEK_PUFF_RATIO = 2;
        const double FACE_ANGRY_RATIO = 0.3;

        const double BROW_LEFT_Y_RATIO = 2;
        const double BROW_RIGHT_Y_RATIO = 2;

        const double MOUTH_X_RATIO = 2;

        public static List<InjectParameterDataRequest.DataSection.ParameterValue> Convert(CapturedData capturedData)
        {
            List<InjectParameterDataRequest.DataSection.ParameterValue> parameterValues = new List<InjectParameterDataRequest.DataSection.ParameterValue>()
            {
                // VTubeStudio Default
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FacePositionX, 1, capturedData.Tracking.Head.Position.X * FACE_POSITION_X_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FacePositionY, 1, capturedData.Tracking.Head.Position.Y * FACE_POSITION_Y_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FacePositionZ, 1, -capturedData.Tracking.Head.Position.Z * FACE_POSITION_Z_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FaceAngleX, 1, capturedData.Tracking.Head.Rotation.Y * FACE_ANGLE_X_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FaceAngleY, 1, -capturedData.Tracking.Head.Rotation.X * FACE_ANGLE_Y_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FaceAngleZ, 1, -capturedData.Tracking.Head.Rotation.Z * FACE_ANGLE_Z_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthSmile, 1,
                    (
                        (
                            Math.Max(capturedData.BlendShapes.MouthSmileLeft + capturedData.BlendShapes.MouthSmileRight - 0.2, 0)                            // mouth smile (pos*2)
                            - Math.Pow(Math.Max(capturedData.BlendShapes.MouthShrugLower - 0.4, 0), 1) * 1                                                  // mouth shrug (neg*1) (threshold: 0.4)
                            - Math.Pow(Math.Max((capturedData.BlendShapes.BrowDownLeft + capturedData.BlendShapes.BrowDownRight) / 2 - 0.3 + (0.08 + ((capturedData.BlendShapes.JawOpen - capturedData.BlendShapes.MouthClose) * 0.15)), 0), 0.4) * 1.5    // brow low (neg*1.5) (threshold: 0.08 + mouth_open_factor)
                        ) * MOUTH_SMILE_RATIO                                                                       // ratio
                    ) / 2 + 0.5                                                                                     // range re-mapping
                ),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthOpen, 1, (capturedData.BlendShapes.JawOpen - capturedData.BlendShapes.MouthClose) * MOUTH_OPEN_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.Brows, 1, capturedData.BlendShapes.BrowInnerUp * BROWS_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.TongueOut, 1, capturedData.BlendShapes.TongueOut < TONGUE_OUT_RATIO ? 0 : 1),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeOpenLeft, 1, (1 - capturedData.BlendShapes.EyeBlinkLeft) * EYE_OPEN_RATIO - (EYE_OPEN_RATIO - 1)),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeOpenRight, 1, (1 - capturedData.BlendShapes.EyeBlinkRight) * EYE_OPEN_RATIO - (EYE_OPEN_RATIO - 1)),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeOpenRight, 1, (1 - capturedData.BlendShapes.EyeBlinkRight) * EYE_OPEN_RATIO - (EYE_OPEN_RATIO - 1)),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLeftX, 1, (capturedData.BlendShapes.EyeLookInLeft - capturedData.BlendShapes.EyeLookOutLeft) * EYE_ROTATION_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLeftY, 1, (capturedData.BlendShapes.EyeLookUpLeft - capturedData.BlendShapes.EyeLookDownLeft) * EYE_ROTATION_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeRightX, 1, (capturedData.BlendShapes.EyeLookOutRight - capturedData.BlendShapes.EyeLookInRight) * EYE_ROTATION_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeRightY, 1, (capturedData.BlendShapes.EyeLookUpRight - capturedData.BlendShapes.EyeLookDownRight) * EYE_ROTATION_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.CheekPuff, 1, capturedData.BlendShapes.CheekPuff * CHEEK_PUFF_RATIO),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.FaceAngry, 1, (capturedData.BlendShapes.MouthRollLower * capturedData.BlendShapes.MouthShrugLower) < FACE_ANGRY_RATIO? 0 : 1),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowLeftY, 1, ((capturedData.BlendShapes.BrowOuterUpLeft - capturedData.BlendShapes.BrowDownLeft) * BROW_LEFT_Y_RATIO + 1) / 2),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowRightY, 1, ((capturedData.BlendShapes.BrowOuterUpRight - capturedData.BlendShapes.BrowDownRight) * BROW_RIGHT_Y_RATIO + 1) / 2),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthX, 1, (capturedData.BlendShapes.MouthLeft - capturedData.BlendShapes.MouthRight) * MOUTH_X_RATIO),

                // ARKit
                // Left Eye
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeBlinkLeft, 1, capturedData.BlendShapes.EyeBlinkLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookDownLeft, 1, capturedData.BlendShapes.EyeLookDownLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookInLeft, 1, capturedData.BlendShapes.EyeLookInLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookOutLeft, 1, capturedData.BlendShapes.EyeLookOutLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookUpLeft, 1, capturedData.BlendShapes.EyeLookUpLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeSquintLeft, 1, capturedData.BlendShapes.EyeSquintLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeWideLeft, 1, capturedData.BlendShapes.EyeWideLeft),

                // Right Eye
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeBlinkRight, 1, capturedData.BlendShapes.EyeBlinkRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookDownRight, 1, capturedData.BlendShapes.EyeLookDownRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookInRight, 1, capturedData.BlendShapes.EyeLookInRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookOutRight, 1, capturedData.BlendShapes.EyeLookOutRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeLookUpRight, 1, capturedData.BlendShapes.EyeLookUpRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeSquintRight, 1, capturedData.BlendShapes.EyeSquintRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.EyeWideRight, 1, capturedData.BlendShapes.EyeWideRight),

                // Mouth and Jaw
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.JawForward, 1, capturedData.BlendShapes.JawForward),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.JawLeft, 1, capturedData.BlendShapes.JawLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.JawRight, 1, capturedData.BlendShapes.JawRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.JawOpen, 1, capturedData.BlendShapes.JawOpen),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthClose, 1, capturedData.BlendShapes.MouthClose),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthFunnel, 1, capturedData.BlendShapes.MouthFunnel),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthPucker, 1, capturedData.BlendShapes.MouthPucker),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthLeft, 1, capturedData.BlendShapes.MouthLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthRight, 1, capturedData.BlendShapes.MouthRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthSmileLeft, 1, capturedData.BlendShapes.MouthSmileLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthSmileRight, 1, capturedData.BlendShapes.MouthSmileRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthFrownLeft, 1, capturedData.BlendShapes.MouthFrownLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthFrownRight, 1, capturedData.BlendShapes.MouthFrownRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthDimpleLeft, 1, capturedData.BlendShapes.MouthDimpleLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthDimpleRight, 1, capturedData.BlendShapes.MouthDimpleRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthStretchLeft, 1, capturedData.BlendShapes.MouthStretchLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthStretchRight, 1, capturedData.BlendShapes.MouthStretchRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthRollLower, 1, capturedData.BlendShapes.MouthRollLower),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthRollUpper, 1, capturedData.BlendShapes.MouthRollUpper),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthShrugLower, 1, capturedData.BlendShapes.MouthShrugLower),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthShrugUpper, 1, capturedData.BlendShapes.MouthShrugUpper),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthPressLeft, 1, capturedData.BlendShapes.MouthPressLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthPressRight, 1, capturedData.BlendShapes.MouthPressRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthLowerDownLeft, 1, capturedData.BlendShapes.MouthLowerDownLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthLowerDownRight, 1, capturedData.BlendShapes.MouthLowerDownRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthUpperUpLeft, 1, capturedData.BlendShapes.MouthUpperUpLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.MouthUpperUpRight, 1, capturedData.BlendShapes.MouthUpperUpRight),

                // Eyebrows, Cheeks and Nose
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowDownLeft, 1, capturedData.BlendShapes.BrowDownLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowDownRight, 1, capturedData.BlendShapes.BrowDownRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowInnerUp, 1, capturedData.BlendShapes.BrowInnerUp),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowOuterUpLeft, 1, capturedData.BlendShapes.BrowOuterUpLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.BrowOuterUpRight, 1, capturedData.BlendShapes.BrowOuterUpRight),
                // new InjectParameterDataRequest.Data.ParameterValue(Params.CheekPuff, 1, capturedData.BlendShapes.CheekPuff),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.CheekSquintLeft, 1, capturedData.BlendShapes.CheekSquintLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.CheekSquintRight, 1, capturedData.BlendShapes.CheekSquintRight),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.NoseSneerLeft, 1, capturedData.BlendShapes.NoseSneerLeft),
                new InjectParameterDataRequest.DataSection.ParameterValue(Params.NoseSneerRight, 1, capturedData.BlendShapes.NoseSneerRight),

                // Tongue
                // new InjectParameterDataRequest.Data.ParameterValue(Params.TongueOut, 1, capturedData.BlendShapes.TongueOut),
            };
            return parameterValues;
        }
    }
}
