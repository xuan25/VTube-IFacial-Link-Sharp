using System;

namespace IFacial
{
    public class DataParser
    {
        public CapturedData Data { get; private set; }

        public DataParser(CapturedData captured)
        {
            Data = captured;
        }

        public void Parse(string payload)
        {
            string[] sections = payload.Trim('|').Split('|');
            foreach (string section in sections)
            {
                ParseSection(section);
            }
        }

        public void ParseSection(string section)
        {
            if (!section.Contains('#'))
            {
                string[] pair = section.Split('&');
                string key = pair[0].Trim();
                float val = float.Parse(pair[1].Trim()) / 100;
                ParseBlendShapes(key, val);
            }
            else
            {
                string[] pair = section.Split('#');
                string key = pair[0].Trim();
                string[] valsStr = pair[1].Trim().Split(',');
                float[] vals = new float[valsStr.Length];
                for(int i = 0; i < valsStr.Length; i++)
                {
                    vals[i] = float.Parse(valsStr[i].Trim());
                }
                ParseTracking(key, vals);
            }
        }

        public void ParseBlendShapes(string key, float value)
        {
            switch (key)
            {
                // Tracking
                case "trackingStatus":
                    Data.BlendShapes.TrackingStatus = value;
                    break;

                // Left Eye
                case "eyeBlink_L":
                    Data.BlendShapes.EyeBlinkLeft = value;
                    break;
                case "eyeLookDown_L":
                    Data.BlendShapes.EyeLookDownLeft = value;
                    break;
                case "eyeLookIn_L":
                    Data.BlendShapes.EyeLookInLeft = value;
                    break;
                case "eyeLookOut_L":
                    Data.BlendShapes.EyeLookOutLeft = value;
                    break;
                case "eyeLookUp_L":
                    Data.BlendShapes.EyeLookUpLeft = value;
                    break;
                case "eyeSquint_L":
                    Data.BlendShapes.EyeSquintLeft = value;
                    break;
                case "eyeWide_L":
                    Data.BlendShapes.EyeWideLeft = value;
                    break;

                // Right Eye
                case "eyeBlink_R":
                    Data.BlendShapes.EyeBlinkRight = value;
                    break;
                case "eyeLookDown_R":
                    Data.BlendShapes.EyeLookDownRight = value;
                    break;
                case "eyeLookIn_R":
                    Data.BlendShapes.EyeLookInRight = value;
                    break;
                case "eyeLookOut_R":
                    Data.BlendShapes.EyeLookOutRight = value;
                    break;
                case "eyeLookUp_R":
                    Data.BlendShapes.EyeLookUpRight = value;
                    break;
                case "eyeSquint_R":
                    Data.BlendShapes.EyeSquintRight = value;
                    break;
                case "eyeWide_R":
                    Data.BlendShapes.EyeWideLeft = value;
                    break;

                // Mouth and Jaw
                case "jawForward":
                    Data.BlendShapes.JawForward = value;
                    break;
                case "jawLeft":
                    Data.BlendShapes.JawLeft = value;
                    break;
                case "jawRight":
                    Data.BlendShapes.JawRight = value;
                    break;
                case "jawOpen":
                    Data.BlendShapes.JawOpen = value;
                    break;
                case "mouthClose":
                    Data.BlendShapes.MouthClose = value;
                    break;
                case "mouthFunnel":
                    Data.BlendShapes.MouthFunnel = value;
                    break;
                case "mouthPucker":
                    Data.BlendShapes.MouthPucker = value;
                    break;
                case "mouthLeft":
                    Data.BlendShapes.MouthLeft = value;
                    break;
                case "mouthRight":
                    Data.BlendShapes.MouthRight = value;
                    break;
                case "mouthSmile_L":
                    Data.BlendShapes.MouthSmileLeft = value;
                    break;
                case "mouthSmile_R":
                    Data.BlendShapes.MouthSmileRight = value;
                    break;
                case "mouthFrown_L":
                    Data.BlendShapes.MouthFrownLeft = value;
                    break;
                case "mouthFrown_R":
                    Data.BlendShapes.MouthFrownRight = value;
                    break;
                case "mouthDimple_L":
                    Data.BlendShapes.MouthDimpleLeft = value;
                    break;
                case "mouthDimple_R":
                    Data.BlendShapes.MouthDimpleRight = value;
                    break;
                case "mouthStretch_L":
                    Data.BlendShapes.MouthStretchLeft = value;
                    break;
                case "mouthStretch_R":
                    Data.BlendShapes.MouthStretchRight = value;
                    break;
                case "mouthRollLower":
                    Data.BlendShapes.MouthRollLower = value;
                    break;
                case "mouthRollUpper":
                    Data.BlendShapes.MouthRollUpper = value;
                    break;
                case "mouthShrugLower":
                    Data.BlendShapes.MouthShrugLower = value;
                    break;
                case "mouthShrugUpper":
                    Data.BlendShapes.MouthShrugUpper = value;
                    break;
                case "mouthPress_L":
                    Data.BlendShapes.MouthPressLeft = value;
                    break;
                case "mouthPress_R":
                    Data.BlendShapes.MouthPressRight = value;
                    break;
                case "mouthLowerDown_L":
                    Data.BlendShapes.MouthLowerDownLeft = value;
                    break;
                case "mouthLowerDown_R":
                    Data.BlendShapes.MouthLowerDownRight = value;
                    break;
                case "mouthUpperUp_L":
                    Data.BlendShapes.MouthUpperUpLeft = value;
                    break;
                case "mouthUpperUp_R":
                    Data.BlendShapes.MouthUpperUpRight = value;
                    break;

                // Eyebrows, Cheeks, and Nose
                case "browDown_L":
                    Data.BlendShapes.BrowDownLeft = value;
                    break;
                case "browDown_R":
                    Data.BlendShapes.BrowDownRight = value;
                    break;
                case "browInnerUp":
                    Data.BlendShapes.BrowInnerUp = value;
                    break;
                case "browOuterUp_L":
                    Data.BlendShapes.BrowOuterUpLeft = value;
                    break;
                case "browOuterUp_R":
                    Data.BlendShapes.BrowOuterUpRight = value;
                    break;
                case "cheekPuff":
                    Data.BlendShapes.CheekPuff = value;
                    break;
                case "cheekSquint_L":
                    Data.BlendShapes.CheekSquintLeft = value;
                    break;
                case "cheekSquint_R":
                    Data.BlendShapes.CheekSquintRight = value;
                    break;
                case "noseSneer_L":
                    Data.BlendShapes.NoseSneerLeft = value;
                    break;
                case "noseSneer_R":
                    Data.BlendShapes.NoseSneerRight = value;
                    break;

                // Tongue
                case "tongueOut":
                    Data.BlendShapes.TongueOut = value;
                    break;

                // Undefined
                case "hapihapi":
                    //System.Diagnostics.Debug.WriteLine($"Undefined blendshape hapihapi: {value}");
                    break;

                default:
                    throw new Exception($"Unknown blendshape key {key}");
            }
        }

        public void ParseTracking(string key, float[] values)
        {
            switch (key)
            {
                case "=head":
                    Data.Tracking.Head.Rotation.X = values[0];
                    Data.Tracking.Head.Rotation.Y = values[1];
                    Data.Tracking.Head.Rotation.Z = values[2];
                    Data.Tracking.Head.Position.X = values[3];
                    Data.Tracking.Head.Position.Y = values[4];
                    Data.Tracking.Head.Position.Z = values[5];
                    break;
                case "leftEye":
                    Data.Tracking.LeftEye.Rotation.X = values[0];
                    Data.Tracking.LeftEye.Rotation.Y = values[1];
                    Data.Tracking.LeftEye.Rotation.Z = values[2];
                    break;
                case "rightEye":
                    Data.Tracking.RightEye.Rotation.X = values[0];
                    Data.Tracking.RightEye.Rotation.Y = values[1];
                    Data.Tracking.RightEye.Rotation.Z = values[2];
                    break;

                default:
                    throw new Exception($"Unknown tracking key {key}");
            }
        }
    }
}
