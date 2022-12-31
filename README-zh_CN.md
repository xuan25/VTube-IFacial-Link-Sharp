# VTube-IFacial-Link

一个 *VTube Studio* 插件，从 *iFacialMocap*（IOS）桥接面部跟踪，实现完整的苹果 ARKit 面部跟踪功能。

*这是一个使用 C# 实现的版本，另外还有一个[使用 Python 实现的版本](https://github.com/xuan25/VTube-IFacial-Link)*

![](imgs/screenshot.png)

## 快速开始指南

*注意：该程序是在 Visual Studio 2022 上开发的*。

1. 克隆并构建代码库。
2. 确保你的 iPhone 和电脑都在同一个网络上。
3. 在你的电脑上启动 Vtube Studio，在你的 iPhone 上启动 iFacialMocap。
4. 确保 "VTube Studio API" 被启用。
5. 启动桥接插件，并且输入捕捉设备的IP地址（iFacialMocap 中显示）以及 Vtube Studio 的 API 地址。
6. 你应该看到所有捕获参数显示在了窗口中，并且插件会被 Vtube Studio 检测到。

## 支持的参数

### VTube Studio 预设

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

### 自定义参数 (ARKit)

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

## 致谢

截图中的模型是由 [Yuri幽里_official](https://www.bilibili.com/video/BV1S8411H7zf/) 创建的。
