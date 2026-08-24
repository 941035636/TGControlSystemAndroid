# CenterSystem.apk 恢复报告

## 恢复结论

本次已从 `CenterSystem.apk` 恢复出可供后续重建的 Unity 工程、C# 业务代码、场景、Prefab、UI 图片、字体、插件程序集和现场控制配置。

APK 使用 **Unity 2020.3.35f1 + Mono**，不是 IL2CPP，因此 `Assembly-CSharp.dll` 中的业务逻辑可高质量反编译。恢复结果不是原作者工程的逐字节备份：原注释、源码排版、部分原始资源文件名及编辑器组织信息无法从 APK 完整还原，但核心业务代码和运行资源仍然存在。

## APK 基本信息

- 文件：`Recovery/Input/CenterSystem.apk`
- SHA-256：`9200E60B9DB53752B3508E2721980E3282B572DF805E0B8ADED09FFD0E3D91D5`
- 大小：128,729,710 字节
- Android 包名：`com.dysk.center`
- 版本：`1.4`（versionCode 1）
- Unity：`2020.3.35f1`
- 脚本后端：Mono
- CPU：`armeabi-v7a`
- 主 Activity：`com.unity3d.player.UnityPlayerActivity`
- 主要权限：网络、网络状态、蓝牙、相机、录音、音频设置

## 已恢复内容

### Unity 工程

工程入口：`Recovery/RecoveredProject/CenterSystem/ExportedProject`

- Unity 场景：1 个，`Assets/Scenes/level0.unity`
- 场景 GameObject：约 500 个
- 场景 MonoBehaviour：约 581 个
- 场景中显式 `m_Script fileID=0`：0 个
- 场景和 Prefab 引用的脚本 GUID：全部能映射到已恢复的 C# 或 DLL
- Prefab：48 个
- PNG 图片：457 张
- 字体：19 个（TTF 10、OTF 9）
- C# 源文件：228 个
- 插件/托管 DLL：154 个
- 已恢复的原始 StreamingAssets 配置：23 个
- `ProjectSettings/ProjectVersion.txt` 已指定 `2020.3.35f1`

### 单独反编译源码

源码目录：`Recovery/RecoveredSource/Assembly-CSharp`

核心类包括：

- TCP：`TcpController`、`TcpControl80/81/82/85/86/90/91`
- UDP：`UDPClient`、`UDPClient2`、`UDPClienthongwai`、`UDPClientServer`
- 串口：`PortControl`、`SerialPortControl`
- 继电器和设备：`RelayManager`、`RelayModelControl`、`AddrelayPanel`、`AddEquiprelayPanel`
- 红外：`RedOrder`、`UDPClienthongwai`
- 播放联动：`VideoPlayControl`、`ControlVideoPanel`、`SoftWareManager`
- 网络发现/远程控制：`FMClient`、`FMServer`、`FMNetworkManager`、`NetworkDiscovery`
- 配置与数据：`ExcelTool`、`SqlManager`、`FileUtil`、`GameResources`
- 唤醒：`WakeUpComputer`、`wakeup`

## 恢复出的控制协议与地址

### 固定设备地址

| 类型 | 地址 | 端口 | 用途线索 |
|---|---:|---:|---|
| TCP 继电器 | 192.168.86.80 | 10000 | 序厅、历史沿革、绿色低碳等 |
| TCP 继电器 | 192.168.86.81 | 10000 | 门柱、环幕、地幕、党建赋能等 |
| TCP 继电器 | 192.168.86.82 | 10000 | 互动飞屏一体机 |
| TCP 继电器 | 192.168.86.85 | 10000 | 代码中保留的控制器 |
| TCP 继电器 | 192.168.86.86 | 10000 | 代码中保留的控制器 |
| 红外/设备控制 | 192.168.86.90 | 502 | 历史沿革、数智领航、留言拍照等 |
| 红外/设备控制 | 192.168.86.91 | 502 | 互动飞屏电视 |
| UDP 红外桥接 | 192.168.20.201 | 1001 | `openPinjieping` 等 ASCII 命令 |
| 展项客户端 | 192.168.86.41—58 等 | 7002/7013 | 视频、互动软件控制 |
| FM 网络组件 | 动态 | 3333/3334 | 服务端/客户端通信 |
| 网络发现 | 动态 | 2222 | UDP 发现 |

### 串口默认值

- 端口：`COM3`
- 波特率：9600
- 数据位：8
- 校验：None（枚举默认值）
- 停止位：1
- 支持字符串发送和十六进制字节发送

### 指令资产

`Assets/StreamingAssets/item.xlsx` 实际为 Excel 97-2003 二进制文件（BIFF/OLE），只是沿用 `.xlsx` 扩展名。该文件可由 APK 自带的 `Excel.dll` 正常读取，包含：

- 1 个八路网口设备控制器
- 1 个八路灯光控制器
- 10 个两路控制器（101—110）
- 1 个八路红外控制器
- 每路开/关、全开/全关的完整十六进制指令
- 拍照服务器默认地址 `127.0.0.1`

同时恢复：

- `设备控制器/Relay.json`
- `设备控制器/Red.json`
- 继电器设备模板文本
- 红外模块的投影开关和三组电视指令
- `softController/Equip.json`
- `softController/SoftWare.json`

## 播放与设备联动线索

原系统已包含“媒体播放过程中按时间点触发设备动作”的机制。例如 `VideoPlayControl` 根据视频名称及计时器，在播放到指定秒数时向 `192.168.86.90:502` 发送红外指令，再延时发送关闭指令。这套机制可作为新自动讲解系统中“语音、画面、灯光、设备动作同步时间轴”的迁移参考，但建议重构为数据驱动的讲解节点和动作时间轴，不继续写死 IP、端口和时间。

## 当前验证状态

已完成：

- APK ZIP 结构与 Android Manifest 解析
- Unity 版本、脚本后端、CPU 架构识别
- `Assembly-CSharp.dll` 反编译
- AssetRipper 工程导出
- 场景、Prefab、图片、脚本、插件数量检查
- 场景显式丢失脚本引用扫描
- 修复 Windows 文件名大小写冲突造成的 `AddPanelSoftWare` 脚本 GUID 冲突
- StreamingAssets 补回工程
- Excel 控制指令表读取验证

尚未完成：

- 使用 Unity 2020.3.35f1 实际打开并完成一次全量导入/编译
- 处理 Unity Console 中可能出现的 API、插件和平台兼容错误
- 在 Android 或 Windows 目标平台重新构建并进行现场协议联调
- 原 APK 是 Android 中控；若迁移到 Windows 触控终端，需要调整 Android 专用路径、权限、串口和网络实现

已知导出提示：AssetRipper 读取 `globalgamemanagers` 的 `BuildSettings`（asset type 129）时报告一处结构计数错误，因此构建场景列表和部分平台构建设置应以首次 Unity 导入后的实际结果为准；主场景、业务资源和代码仍已成功导出。

## 下一步建议

1. 安装 Unity 2020.3.35f1，并用恢复工程执行首次导入。
2. 根据 Unity Console 错误修复插件引用和少量反编译兼容问题。
3. 先制作“旧中控功能基线测试”：设备开关、红外、视频播放、双端控制、网络发现。
4. 将硬编码 IP、端口、十六进制命令迁入后台可配置的设备与协议表。
5. 在旧协议适配层之上开发新的自动讲解编排、任意模块组合、语音/画面/设备同步功能。
