using System;
using System.Collections.Generic;
using System.Linq;
using TG.Control.Touch.UI.Services;
using TG.Control.Touch.UI.Theme;
using TG.Control.UnityContracts;
using UnityEngine;
using UnityEngine.UI;

namespace TG.Control.Touch.UI.Components
{
    /// <summary>
    /// Pure presentation of the server-owned playback snapshot and published content. It does not
    /// infer media time, coordinate playback, or maintain a second session state machine.
    /// </summary>
    public sealed class PlaybackProgressPanel
    {
        private readonly TouchTheme theme;
        private readonly TouchImageLoader imageLoader;
        private readonly Image frame;
        private readonly Image surface;
        private readonly Image stateAccent;
        private readonly StatusBadge stateBadge;
        private readonly Text routeContext;
        private readonly Text moduleSequence;
        private readonly Text moduleName;
        private readonly Text nodeName;
        private readonly Text description;
        private readonly Image coverImage;
        private readonly Image coverDownloadProxy;
        private readonly GameObject coverPlaceholder;
        private readonly Text progressLabel;
        private readonly Text progressPercent;
        private readonly Image progressFill;
        private readonly Text preparationLabel;
        private readonly AspectRatioFitter coverAspect;
        private string coverSignature;

        public RectTransform Root => frame.rectTransform;

        public PlaybackProgressPanel(TouchUiFactory factory, TouchTheme theme, TouchImageLoader imageLoader,
            Transform parent)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            this.theme = theme ?? throw new ArgumentNullException(nameof(theme));
            this.imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));

            frame = factory.RoundedImage("Playback Content Frame", parent, theme.PlaybackBorder);
            surface = factory.RoundedImage("Playback Content Surface", frame.transform,
                theme.PlaybackPanelElevated);
            TouchUiFactory.Stretch(surface.rectTransform, 1, 1, -1, -1);
            surface.raycastTarget = false;

            stateAccent = factory.Image("Playback Top Accent", surface.transform, theme.HomeDarkAccent);
            TouchUiFactory.Anchor(stateAccent.rectTransform, 0, 1, 1, 1, 26, -3, -26, 0);
            stateAccent.raycastTarget = false;

            var eyebrow = factory.Label("Playback Eyebrow", surface.transform, "当前讲解  /  PLAYBACK",
                theme.Caption, FontStyle.Bold, theme.HomeDarkAccent, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(eyebrow.rectTransform, 0, 1, .50f, 1, 30, -42, 0, -14);

            var pageTitle = factory.Label("Playback Page Title", surface.transform, "当前讲解",
                theme.Display, FontStyle.Bold, theme.HomeDarkTextPrimary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(pageTitle.rectTransform, 0, 1, .52f, 1, 30, -93, 0, -44);

            var subtitle = factory.Label("Playback Page Subtitle", surface.transform,
                "当前页面显示服务器返回的讲解状态，可在下方控制播放",
                theme.Body, FontStyle.Normal, theme.HomeDarkTextSecondary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(subtitle.rectTransform, 0, 1, .66f, 1, 30, -126, 0, -96);

            stateBadge = new StatusBadge(factory, theme, surface.transform, "Playback State Badge");
            stateBadge.SetDarkMode(true);
            TouchUiFactory.Anchor(stateBadge.Root, 1, 1, 1, 1, -262, -66, -30, -20);

            routeContext = factory.Label("Playback Route Context", surface.transform, "当前讲解任务",
                theme.Secondary, FontStyle.Bold, theme.HomeDarkTextSecondary, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(routeContext.rectTransform, .58f, 1, 1, 1, 0, -112, -30, -76);
            routeContext.resizeTextForBestFit = true;
            routeContext.resizeTextMinSize = theme.Caption;
            routeContext.resizeTextMaxSize = theme.Secondary;

            var headerDivider = factory.Image("Playback Header Divider", surface.transform,
                theme.HomeDarkBorderSoft);
            TouchUiFactory.Anchor(headerDivider.rectTransform, 0, 1, 1, 1, 28, -142, -28, -140);
            headerDivider.raycastTarget = false;

            var coverBorder = factory.RoundedImage("Playback Current Cover Border", surface.transform,
                theme.PlaybackBorder);
            TouchUiFactory.Anchor(coverBorder.rectTransform, 0, 0, .44f, 1, 28, 128, -10, -166);
            coverBorder.raycastTarget = false;
            var coverViewport = factory.RoundedImage("Playback Current Cover Viewport", coverBorder.transform,
                new Color(.01f, .05f, .13f, 1f));
            TouchUiFactory.Stretch(coverViewport.rectTransform, 2, 2, -2, -2);
            coverViewport.gameObject.AddComponent<Mask>().showMaskGraphic = true;
            coverViewport.raycastTarget = false;

            coverImage = factory.Image("Playback Current Cover", coverViewport.transform, Color.clear);
            TouchUiFactory.Stretch(coverImage.rectTransform);
            coverImage.raycastTarget = false;
            coverAspect = coverImage.gameObject.AddComponent<AspectRatioFitter>();
            coverAspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            coverAspect.aspectRatio = 1024f / 544f;

            coverPlaceholder = factory.Rect("Playback Cover Empty", coverViewport.transform).gameObject;
            TouchUiFactory.Stretch(coverPlaceholder.GetComponent<RectTransform>());
            var placeholderGlow = factory.RoundedImage("Playback Cover Empty Glow", coverPlaceholder.transform,
                new Color(theme.HomeDarkAccent.r, theme.HomeDarkAccent.g, theme.HomeDarkAccent.b, .12f));
            TouchUiFactory.Anchor(placeholderGlow.rectTransform, .5f, .5f, .5f, .5f, -58, -58, 58, 58);
            placeholderGlow.raycastTarget = false;
            var placeholderMark = factory.Label("Playback Cover Empty Mark", placeholderGlow.transform, "TG",
                theme.PageTitle, FontStyle.Bold, theme.HomeDarkAccent, TextAnchor.MiddleCenter);
            TouchUiFactory.Stretch(placeholderMark.rectTransform);
            var placeholderText = factory.Label("Playback Cover Empty Text", coverPlaceholder.transform,
                "当前主题暂无封面", theme.Secondary, FontStyle.Normal, theme.HomeDarkTextMuted,
                TextAnchor.MiddleCenter);
            TouchUiFactory.Anchor(placeholderText.rectTransform, .12f, .5f, .88f, .5f, 0, -112, 0, -70);

            coverDownloadProxy = factory.Image("Playback Cover Download Proxy", coverViewport.transform,
                Color.clear);
            TouchUiFactory.Anchor(coverDownloadProxy.rectTransform, .5f, .5f, .5f, .5f, 0, 0, 0, 0);
            coverDownloadProxy.raycastTarget = false;

            var detailSurface = factory.RoundedImage("Playback Detail Surface", surface.transform,
                theme.PlaybackPanelSurface);
            TouchUiFactory.Anchor(detailSurface.rectTransform, .44f, 0, 1, 1, 10, 128, -28, -166);
            detailSurface.raycastTarget = false;

            var moduleKey = factory.Label("Playback Module Key", detailSurface.transform, "当前主题",
                theme.Body, FontStyle.Bold, theme.HomeDarkTextSecondary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(moduleKey.rectTransform, 0, 1, .55f, 1, 30, -48, 0, -16);
            moduleSequence = factory.Label("Playback Module Sequence", detailSurface.transform, string.Empty,
                theme.Body, FontStyle.Bold, theme.HomeDarkAccent, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(moduleSequence.rectTransform, .45f, 1, 1, 1, 0, -48, -30, -16);

            moduleName = factory.Label("Playback Module Name", detailSurface.transform,
                "正在恢复当前主题", theme.Display, FontStyle.Bold, theme.HomeDarkTextPrimary,
                TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(moduleName.rectTransform, 0, 1, 1, 1, 30, -116, -30, -52);
            moduleName.resizeTextForBestFit = true;
            moduleName.resizeTextMinSize = theme.PageTitle;
            moduleName.resizeTextMaxSize = theme.Display;

            var detailDivider = factory.Image("Playback Detail Divider", detailSurface.transform,
                theme.HomeDarkBorderSoft);
            TouchUiFactory.Anchor(detailDivider.rectTransform, 0, 1, 1, 1, 30, -138, -30, -136);
            detailDivider.raycastTarget = false;

            var nodeKey = factory.Label("Playback Node Key", detailSurface.transform, "当前节点",
                theme.Body, FontStyle.Bold, theme.HomeDarkTextSecondary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(nodeKey.rectTransform, 0, 1, 1, 1, 30, -178, -30, -146);
            nodeName = factory.Label("Playback Node Name", detailSurface.transform,
                "正在读取当前节点", theme.PageTitle, FontStyle.Bold, theme.HomeDarkTextPrimary,
                TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(nodeName.rectTransform, 0, 1, 1, 1, 30, -230, -30, -182);
            nodeName.resizeTextForBestFit = true;
            nodeName.resizeTextMinSize = theme.Body;
            nodeName.resizeTextMaxSize = theme.PageTitle;

            var descriptionKey = factory.Label("Playback Description Key", detailSurface.transform,
                "讲解说明", theme.Secondary, FontStyle.Bold, theme.HomeDarkTextSecondary,
                TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(descriptionKey.rectTransform, 0, 1, 1, 1, 30, -274, -30, -242);
            description = factory.Label("Playback Description", detailSurface.transform,
                "当前内容未配置讲解说明", theme.CardTitle, FontStyle.Normal, theme.HomeDarkTextSecondary,
                TextAnchor.UpperLeft);
            TouchUiFactory.Anchor(description.rectTransform, 0, 0, 1, 1, 30, 24, -30, -282);
            description.resizeTextForBestFit = true;
            description.resizeTextMinSize = theme.Secondary;
            description.resizeTextMaxSize = theme.CardTitle;

            var progressArea = factory.RoundedImage("Playback Overall Progress", surface.transform,
                theme.PlaybackGlass);
            TouchUiFactory.Anchor(progressArea.rectTransform, 0, 0, 1, 0, 28, 20, -28, 108);
            progressArea.raycastTarget = false;
            progressLabel = factory.Label("Playback Progress Label", progressArea.transform,
                "正在读取讲解进度", theme.Body, FontStyle.Bold, theme.HomeDarkTextPrimary,
                TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(progressLabel.rectTransform, 0, 1, .68f, 1, 18, -36, 0, -8);
            progressPercent = factory.Label("Playback Progress Percent", progressArea.transform, "",
                theme.Secondary, FontStyle.Bold, theme.HomeDarkAccent, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(progressPercent.rectTransform, .68f, 1, 1, 1, 0, -36, -18, -8);
            preparationLabel = factory.Label("Playback Preparation Label", progressArea.transform, string.Empty,
                theme.Caption, FontStyle.Bold, theme.Warning, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(preparationLabel.rectTransform, .55f, 0, 1, 0, 0, 10, -18, 34);

            var progressTrack = factory.RoundedImage("Playback Progress Track", progressArea.transform,
                new Color(.08f, .32f, .62f, .72f));
            TouchUiFactory.Anchor(progressTrack.rectTransform, 0, 0, 1, 0, 18, 16, -18, 28);
            progressTrack.raycastTarget = false;
            progressFill = factory.RoundedImage("Playback Progress Fill", progressTrack.transform,
                theme.HomeDarkAccent);
            TouchUiFactory.Anchor(progressFill.rectTransform, 0, 0, 0, 1, 0, 0, 0, 0);
            progressFill.raycastTarget = false;
        }

        public void Render(TouchUiState state, string verifiedRouteName,
            IReadOnlyList<string> verifiedModuleIds, Func<string, string> resolveAssetUrl)
        {
            if (state == null) return;
            var session = state.Session;
            var presentation = PlaybackPresentation.From(state, theme);
            stateBadge.Set(presentation.StateLabel, presentation.Tone);
            stateAccent.color = presentation.Color;
            frame.color = Color.Lerp(theme.PlaybackBorder, presentation.Color, .20f);

            var route = string.IsNullOrWhiteSpace(verifiedRouteName) ? "当前讲解任务" : verifiedRouteName;
            routeContext.text = "路线 · " + route;
            moduleName.text = string.IsNullOrWhiteSpace(session?.moduleName)
                ? presentation.EmptyModuleLabel : session.moduleName;
            nodeName.text = string.IsNullOrWhiteSpace(session?.nodeName)
                ? presentation.EmptyNodeLabel : session.nodeName;

            var module = FindModule(state.Content, session?.moduleId);
            var node = FindNode(module, session?.nodeId);
            var detail = !string.IsNullOrWhiteSpace(module?.description)
                ? module.description : node?.narrationText;
            description.text = string.IsNullOrWhiteSpace(detail)
                ? "当前内容未配置讲解说明" : detail;

            RenderVerifiedModuleContext(session, verifiedModuleIds);
            RenderCover(state.Content, module, resolveAssetUrl);
            RenderProgress(session);
        }

        public void RefreshTheme()
        {
            stateBadge.SetDarkMode(true);
            stateBadge.RefreshTheme();
            surface.color = theme.PlaybackPanelElevated;
            progressFill.color = theme.HomeDarkAccent;
        }

        private void RenderVerifiedModuleContext(PlaybackSessionStatus session,
            IReadOnlyList<string> verifiedModuleIds)
        {
            moduleSequence.text = string.Empty;
            if (session == null || verifiedModuleIds == null || verifiedModuleIds.Count == 0) return;

            var currentIndex = IndexOf(verifiedModuleIds, session.moduleId);
            if (currentIndex < 0) return;
            moduleSequence.text = "主题进度  " + (currentIndex + 1) + " / " + verifiedModuleIds.Count;
        }

        private void RenderProgress(PlaybackSessionStatus session)
        {
            var progress = session == null || session.totalNodes <= 0
                ? 0 : Mathf.Clamp01((float)session.currentNodeNumber / session.totalNodes);
            progressFill.rectTransform.anchorMax = new Vector2(progress, 1);
            progressFill.rectTransform.offsetMin = Vector2.zero;
            progressFill.rectTransform.offsetMax = Vector2.zero;
            progressLabel.text = session == null || session.totalNodes <= 0
                ? "正在读取讲解进度"
                : "整条路线节点进度  " + session.currentNodeNumber + " / " + session.totalNodes;
            progressPercent.text = session == null || session.totalNodes <= 0
                ? string.Empty : Mathf.RoundToInt(progress * 100) + "%";
            preparationLabel.text = session != null && !session.playPublished && session.preparationProgress > 0
                ? "素材准备 " + Mathf.Clamp01((float)session.preparationProgress).ToString("P0")
                : string.Empty;
        }

        private void RenderCover(PublishedContent content, ExhibitionModule module,
            Func<string, string> resolveAssetUrl)
        {
            var signature = (content?.version ?? 0) + "|" + (module?.id ?? string.Empty) + "|" +
                            (module?.coverUrl ?? string.Empty);
            if (string.Equals(coverSignature, signature, StringComparison.Ordinal)) return;
            coverSignature = signature;

            coverImage.sprite = null;
            coverImage.color = Color.clear;
            coverPlaceholder.SetActive(true);

            var fallback = LoadOfficialArtwork(content, module);
            if (fallback != null) ApplyCover(fallback);

            if (module == null || string.IsNullOrWhiteSpace(module.coverUrl)) return;
            var requestSignature = signature;
            var url = resolveAssetUrl?.Invoke(module.coverUrl) ?? module.coverUrl;
            imageLoader.Load(coverDownloadProxy, url, success =>
            {
                if (!success || !string.Equals(coverSignature, requestSignature, StringComparison.Ordinal) ||
                    coverDownloadProxy == null || coverDownloadProxy.sprite == null) return;
                ApplyCover(coverDownloadProxy.sprite);
            });
        }

        private void ApplyCover(Sprite sprite)
        {
            if (sprite == null) return;
            coverImage.sprite = sprite;
            coverImage.color = Color.white;
            coverImage.type = Image.Type.Simple;
            coverAspect.aspectRatio = sprite.rect.width / Mathf.Max(1, sprite.rect.height);
            coverPlaceholder.SetActive(false);
        }

        private static Sprite LoadOfficialArtwork(PublishedContent content, ExhibitionModule module)
        {
            if (content?.modules == null || module == null) return null;
            var ordered = content.modules.Where(item => item != null && item.enabled)
                .OrderBy(item => item.order).ToArray();
            var index = Array.FindIndex(ordered, item =>
                string.Equals(item.id, module.id, StringComparison.OrdinalIgnoreCase));
            if (index < 0 || index >= 12) return null;
            return Resources.Load<Sprite>("TGExhibitionUI/OfficialModuleCards/Module_" +
                                          (index + 1).ToString("00") + "_Official");
        }

        private static ExhibitionModule FindModule(PublishedContent content, string id)
        {
            if (content?.modules == null || string.IsNullOrWhiteSpace(id)) return null;
            return content.modules.FirstOrDefault(item => item != null &&
                string.Equals(item.id, id, StringComparison.OrdinalIgnoreCase));
        }

        private static NarrationNode FindNode(ExhibitionModule module, string id)
        {
            if (module?.nodes == null || string.IsNullOrWhiteSpace(id)) return null;
            return module.nodes.FirstOrDefault(item => item != null &&
                string.Equals(item.id, id, StringComparison.OrdinalIgnoreCase));
        }

        private static int IndexOf(IReadOnlyList<string> values, string id)
        {
            for (var index = 0; index < values.Count; index++)
                if (string.Equals(values[index], id, StringComparison.OrdinalIgnoreCase)) return index;
            return -1;
        }

        private sealed class PlaybackPresentation
        {
            public string StateLabel;
            public string EmptyModuleLabel;
            public string EmptyNodeLabel;
            public StatusTone Tone;
            public Color Color;

            public static PlaybackPresentation From(TouchUiState state, TouchTheme theme)
            {
                if (!state.Connected)
                    return Create("服务连接异常", "正在恢复讲解状态", "连接恢复后将自动更新当前节点",
                        StatusTone.Error, theme.Error);
                if (!state.HasActiveSession)
                    return Create("讲解已结束", "本次讲解已结束", "可以返回讲解首页开始新的讲解",
                        StatusTone.Neutral, theme.HomeDarkTextSecondary);
                if (state.Session == null)
                    return Create("正在恢复讲解", "正在恢复当前主题", "正在向服务器读取真实讲解状态",
                        StatusTone.Warning, theme.Warning);
                if (state.Session.paused)
                    return Create("讲解已暂停", "当前主题", "当前节点",
                        StatusTone.Warning, theme.Warning);
                if (!state.Session.playPublished)
                    return Create("正在准备播放", "正在准备当前主题", "正在同步大屏与语音，请稍候…",
                        StatusTone.Info, theme.HomeDarkAccent);
                return Create("正在讲解", "当前主题", "当前节点",
                    StatusTone.Success, theme.Success);
            }

            private static PlaybackPresentation Create(string label, string emptyModule, string emptyNode,
                StatusTone tone, Color color) => new PlaybackPresentation
            {
                StateLabel = label,
                EmptyModuleLabel = emptyModule,
                EmptyNodeLabel = emptyNode,
                Tone = tone,
                Color = color
            };
        }
    }
}
