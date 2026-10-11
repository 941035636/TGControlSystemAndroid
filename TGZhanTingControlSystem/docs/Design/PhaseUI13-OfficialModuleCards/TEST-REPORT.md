# Phase UI-13 — 甲方正式 12 模块卡片素材接入验收记录

## 基线与范围

- 验收日期：2026-10-10
- 分支：`codex/product-upgrade`
- 基线提交：`ba057dd98b861658efa2fb9af3687612aeafc156`
- Unity：2020.3.35f1c2
- 范围：仅 TouchClient 首页 12 张模块卡片的正式封面接入与显示验证。
- 未修改：Server、LedPlayer、MeloTTS、待机页、欢迎词流程、模块业务名称与 ID、路线、选择逻辑、播放 Session。

## 素材核验

已阅读素材包中的 `README_使用说明.txt` 与 `manifest.json`。12 张 `Cards_1024x544` PNG 均为 1024×544，按 manifest 逐张计算 SHA-256，12/12 与清单一致。

| 顺序 | 业务模块 | Unity 运行时资源 |
| --- | --- | --- |
| 01 | 一路向新 | `Module_01_Official.png` |
| 02 | 历史沿革 | `Module_02_Official.png` |
| 03 | 不曾忘却的记忆 | `Module_03_Official.png` |
| 04 | 子分公司 | `Module_04_Official.png` |
| 05 | 领导关怀 | `Module_05_Official.png` |
| 06 | 企业荣誉 | `Module_06_Official.png` |
| 07 | 科技创新 | `Module_07_Official.png` |
| 08 | 产品体系 | `Module_08_Official.png` |
| 09 | 数智领航 | `Module_09_Official.png` |
| 10 | 绿色低碳 | `Module_10_Official.png` |
| 11 | 敢当石特 | `Module_11_Official.png` |
| 12 | 党建赋能 | `Module_12_Official.png` |

## 实施结果

- 12 张图片以 `Sprite (2D and UI)` 导入，单图最大尺寸 1024，关闭 Mipmap，使用无损压缩设置。
- 卡片封面使用固定 1024:544 宽高比与覆盖式适配，不拉伸变形。
- 封面图片关闭 Raycast，不拦截外层卡片按钮的触摸事件。
- 正式图片显示时，旧标题、旧副标题/遮罩及箭头装饰隐藏，避免与原图内中文标题重复。
- 模块编号、蓝色选中边框及选择顺序标识仍为独立 Unity UGUI 元素。
- 4×3 布局、深蓝科技外壳、左侧导航和底部操作区保持不变。
- 当正式资源缺失时仍保留原有动态封面的回退路径，避免空白卡片。

## Windows Player 验收

使用实际 Windows Player 连接本地 Server 内容接口运行。验收驱动只存在于临时 QA 工程，截图完成后已移除；正式交付构建不包含该驱动。

| 检查项 | 状态 | 实际结果 / 证据 |
| --- | --- | --- |
| 12 张素材顺序与模块对应 | PASS | Player 日志逐项验证 `Module_01_Official` 至 `Module_12_Official`，12/12 有效。 |
| 图片尺寸 | PASS | Player 运行时逐张读取均为 1024×544。 |
| 1920×1080 首页 | PASS | [01-Official-Cards-1920x1080.png](./01-Official-Cards-1920x1080.png) |
| 1280×720 首页 | PASS | [02-Official-Cards-1280x720.png](./02-Official-Cards-1280x720.png) |
| 三模块选中状态 | PASS | [03-Official-Cards-Three-Selected-1920x1080.png](./03-Official-Cards-Three-Selected-1920x1080.png)；前三项显示蓝色边框及 01/02/03 选择顺序。 |
| 重复标题/旧装饰 | PASS | 旧卡片标题和遮罩已隐藏，画面中只显示甲方图片内标题。 |
| 宽高比与裁切 | PASS | 1920×1080 与 1280×720 下均未发现拉伸；按卡片容器覆盖裁切。 |
| 点击热区与选择逻辑 | PASS | Player 中连续点击前三张卡片，状态显示“已选择 3 个展区”。 |
| TouchClient 脚本编译 | PASS | Unity Player 构建阶段 `TG.Control.Touch.dll` 编译成功。 |
| 正式 Windows Player 构建 | PASS | Unity 批处理返回成功，构建报告为 `Build Successful`，完整构建大小约 91.4 MB。 |
| 正式讲解启动及视频播放 | NOT RUN | 本阶段为首页视觉素材接入，未启动正式讲解 Session，不将其计为通过。 |
| 人工视觉确认 | NOT RUN | 等待甲方/产品人员查看本报告截图后确认。 |

说明：验收电脑物理显示分辨率为 1366×768。1920×1080 证据由真实 Windows Player 使用目标分辨率离屏截图生成；1280×720 为真实 Player 目标窗口截图。两者均不是编辑器预览或设计稿合成图。

## 正式构建产物

- Windows Player：`artifacts/phase-ui13-official-module-cards/Windows/TouchClient.exe`
- 构建日志：`C:/Users/L1532/AppData/Local/Temp/TGPhaseUI13QA-20261010/production-build.log`

## 已知问题

- 无已知代码或显示阻塞项。
- 最终视觉观感仍需人工确认；本轮完成后不继续下一阶段。
