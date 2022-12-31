# VTube-IFacial-Link

[简体中文](./README-zh_CN.md)

A *VTube Studio* plugin that bridging facial tracking from *iFacialMocap* (IOS), enabling full apple ARKit facial tracking features.

*This is a version implemented in C#, there is also a [version implemented in Python](https://github.com/xuan25/VTube-IFacial-Link)*

![](imgs/screenshot.png)

## Quick Start Guide

*Note: The program is developed on Visual Studio 2022*

1. Clone and Compile the repository.
2. Make sure both your iPhone and PC are on the same network.
3. Launch both Vtube Studio on your PC and iFacialMocap on your iPhone.
4. Make sure the `VTube Studio API` is enabled.
5. Launch the Bridging Plugin and type in the IP address of your capture device (shown in the iFacialMocap) and the API address of your VTube Studio.
6. You should see all of the captured parameters in the App and the plugin should be detected by Vtube Studio. 

## Supported Parameters

### VTube Studio Default

- FacePositionX
- FacePositionY
- FacePositionZ
- FaceAngleX
- FaceAngleY
- FaceAngleZ
- MouthSmile
- MouthOpen
- Brows
- TongueOut
- EyeOpenLeft
- EyeOpenRight
- EyeLeftX
- EyeLeftY
- EyeRightX
- EyeRightY
- CheekPuff
- FaceAngry
- BrowLeftY
- BrowRightY
- MouthX

### Custom Parameters (ARKit)

- EyeBlinkLeft
- EyeLookDownLeft
- EyeLookInLeft
- EyeLookOutLeft
- EyeLookUpLeft
- EyeSquintLeft
- EyeWideLeft
- EyeBlinkRight
- EyeLookDownRight
- EyeLookInRight
- EyeLookOutRight
- EyeLookUpRight
- EyeSquintRight
- EyeWideRight
- JawForward
- JawLeft
- JawRight
- JawOpen
- MouthClose
- MouthFunnel
- MouthPucker
- MouthLeft
- MouthRight
- MouthSmileLeft
- MouthSmileRight
- MouthFrownLeft
- MouthFrownRight
- MouthDimpleLeft
- MouthDimpleRight
- MouthStretchLeft
- MouthStretchRight
- MouthRollLower
- MouthRollUpper
- MouthShrugLower
- MouthShrugUpper
- MouthPressLeft
- MouthPressRight
- MouthLowerDownLeft
- MouthLowerDownRight
- MouthUpperUpLeft
- MouthUpperUpRight
- BrowDownLeft
- BrowDownRight
- BrowInnerUp
- BrowOuterUpLeft
- BrowOuterUpRight
- CheekPuff
- CheekSquintLeft
- CheekSquintRight
- NoseSneerLeft
- NoseSneerRight
- TongueOut

## Acknowledgment

The model in the screenshot is created by [Yuri幽里_official](https://www.bilibili.com/video/BV1S8411H7zf/)
