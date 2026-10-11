# Phase UI-15 — 首页视觉精修验收记录

## 范围

- 日期：2026-10-10
- 分支：`codex/product-upgrade`
- 基线：延续本地未提交的 Phase UI-13、UI-14 成果。
- 本轮仅调整首页主题透明度、单层线框、局部留白和卡片选中反馈。
- 未替换 4K 背景，未修改 12 张正式图片、4×3 布局、业务事件、Session、Server、LedPlayer、MeloTTS 或其他页面主题。

## 精修内容

- 顶部栏与左侧导航改为安静的半透明深蓝表面，移除重复双层霓虹外框。
- 内容区、标题区、底部操作区统一为单层低对比蓝色边框。
- 适度提高首页背景遮罩，保留科技场景，同时降低城市、地面线条对卡片的干扰。
- 标题区与卡片区之间、卡片区与底部操作区之间统一为接近 12px 的视觉留白。
- 未选卡片使用低对比边框；选中卡片使用高亮蓝边、柔和外光和独立 01/02/03 顺序角标。
- 导航选中底色与按钮描边降低饱和度和亮度，避免与卡片争夺视觉焦点。

## Windows Player 验收

| 检查项 | 状态 | 结果 / 证据 |
| --- | --- | --- |
| 1920×1080 首页 | PASS | [01-Home-Polished-1920x1080.png](./01-Home-Polished-1920x1080.png) |
| 1280×720 首页 | PASS | [02-Home-Polished-1280x720.png](./02-Home-Polished-1280x720.png) |
| 三模块选中状态 | PASS | [03-Home-Polished-Three-Selected-1920x1080.png](./03-Home-Polished-Three-Selected-1920x1080.png) |
| 12 张正式卡片 | PASS | Player 检测到 12 个有效卡片按钮，正式图片与编号保持不变。 |
| 选中顺序 | PASS | 前三张卡片显示独立 01、02、03 顺序标识，日志检测到 3 个选中角标。 |
| 页面布局 | PASS | 1920×1080 与 1280×720 均保持 4×3，顶部、导航、内容和底部操作区无重叠。 |
| 编译与构建 | PASS | Unity 2020.3.35f1c2 Windows Player 构建成功。 |
| 业务端到端播放 | NOT RUN | 本轮没有修改播放业务，不将其标记为本轮通过。 |
| 人工视觉确认 | NOT RUN | 等待甲方/产品人员确认。 |

截图来自真实 Unity Windows Player。临时截图驱动已从临时工程删除，不属于源码改动。

## 构建产物

- QA Player：`artifacts/phase-ui15-visual-polish/Windows/TouchClient-UI15-QA.exe`
- 构建日志：`C:/Users/L1532/AppData/Local/Temp/TGPhaseUI13QA-20261010/ui15-build.log`
- Player 日志：`C:/Users/L1532/AppData/Local/Temp/TGPhaseUI13QA-20261010/ui15-player-final.log`

## 结论

精修目标已实现，等待人工视觉确认；不自动提交或推送。
