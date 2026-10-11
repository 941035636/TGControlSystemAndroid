# Phase UI-14 — TouchClient 首页 4K 科技背景验收记录

## 范围

- 日期：2026-10-10
- 分支：`codex/product-upgrade`
- 基线：`ba057dd98b861658efa2fb9af3687612aeafc156`
- Unity：2020.3.35f1c2
- 本轮仅替换 12 模块首页底层背景及其专用纹理导入设置。
- 未修改 Server、LedPlayer、MeloTTS、Canvas Scaler、待机/欢迎业务、Session、12 模块绑定与选择逻辑。

## 实施结果

- 输入素材 `HomeTechBackground_4K.png` 已确认实际尺寸为 3840×2160、24 位 RGB、16:9。
- 运行时资源位于：
  `Assets/Resources/TGExhibitionUI/HomeDarkV2/Backgrounds/HomeTechBackground_4K.png`
- 首页继续通过现有 `TouchAppShell -> HomeDarkVisualAssets` 链路加载，处于顶部栏、导航、标题、卡片和操作栏的下方。
- 背景 Image 保持 `Raycast Target=false` 与 `Preserve Aspect=true`。
- 待机页继续使用自己的 `Resources/Touch/standby-clean-background`，未改动。

## 纹理导入

| 设置 | 实际值 |
| --- | --- |
| Texture Type | Default（现有加载器运行时创建 Sprite） |
| 原图尺寸 | 3840×2160 |
| Max Size | 4096 |
| Non Power of 2 | None |
| Compression | None |
| Filter Mode | Bilinear |
| Mip Maps | 关闭 |
| Windows/Standalone 覆盖 | 开启，4096、RGB24、不压缩 |

运行时日志确认加载到的实际纹理仍为 `3840x2160`，没有被缩小为 1024 或 2048。

## Windows Player 验收

| 检查项 | 状态 | 证据 / 结果 |
| --- | --- | --- |
| 1920×1080 首页 | PASS | [01-Home-4K-1920x1080.png](./01-Home-4K-1920x1080.png) |
| 1280×720 首页 | PASS | [02-Home-4K-1280x720.png](./02-Home-4K-1280x720.png) |
| 三模块选中状态 | PASS | [03-Home-4K-Three-Selected-1920x1080.png](./03-Home-4K-Three-Selected-1920x1080.png) |
| 待机页面未受影响 | PASS | [04-Standby-Unchanged-1920x1080.png](./04-Standby-Unchanged-1920x1080.png) |
| 2560×1440 首页 | PASS | [05-Home-4K-2560x1440.png](./05-Home-4K-2560x1440.png) |
| 背景层级 | PASS | 背景位于所有业务 UI 下方，顶部栏、导航、标题、卡片和底部栏正常显示。 |
| 16:9 显示 | PASS | 三种目标分辨率均未发现拉伸、压扁、露白、灰带、重复边框或错位。 |
| 背景触摸穿透 | PASS | Player 日志确认 `Raycast Target=False`。 |
| 正式卡片保护 | PASS | 12 张甲方卡片仍正常显示，前三张选择顺序为 01/02/03。 |
| Unity 编译与 Windows Player 构建 | PASS | 构建报告为 `Build Successful`，完整构建约 115.2 MB。 |
| 人工视觉确认 | NOT RUN | 等待甲方/产品人员确认截图。 |

说明：验收截图来自真实 Unity Windows Player。验收机物理屏幕为 1366×768，通过调整 Player 客户区取得真实的 1920×1080 与 2560×1440渲染输出，并另外直接验证 1280×720。

首次临时验收驱动未关闭欢迎页，产生的截图已作废。修正临时驱动后进行了必要的增量重建；该驱动仅存在于临时 QA 工程，已删除，不属于源码改动。

## 构建产物

- QA 验证 Player：`artifacts/phase-ui14-home-4k-background/Windows/TouchClient-UI14-QA.exe`
- 构建日志：`C:/Users/L1532/AppData/Local/Temp/TGPhaseUI13QA-20261010/ui14-build-capture-fix.log`
- Player 验收日志：`C:/Users/L1532/AppData/Local/Temp/TGPhaseUI13QA-20261010/ui14-player-final.log`

## 结论

代码、导入质量和目标分辨率运行检查均通过；本阶段停止，等待人工视觉确认。不提交、不推送。
