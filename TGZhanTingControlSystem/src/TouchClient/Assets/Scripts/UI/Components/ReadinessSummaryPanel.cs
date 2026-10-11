using System;
using TG.Control.Touch.UI.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace TG.Control.Touch.UI.Components
{
    /// <summary>Maps the existing readiness result to one clear operator-facing conclusion.</summary>
    public sealed class ReadinessSummaryPanel
    {
        private readonly TouchTheme theme;
        private readonly Image frame;
        private readonly Image surface;
        private readonly Image semanticAccent;
        private readonly StatusBadge badge;
        private readonly Text title;
        private readonly Text detail;
        private readonly Text facts;
        private StatusTone tone;

        public RectTransform Root => frame.rectTransform;

        public ReadinessSummaryPanel(TouchUiFactory factory, TouchTheme theme, Transform parent)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            this.theme = theme ?? throw new ArgumentNullException(nameof(theme));

            frame = factory.RoundedImage("Readiness Summary", parent, theme.HomeDarkBorder);
            var element = frame.gameObject.AddComponent<LayoutElement>();
            element.minHeight = theme.SystemStatusSummaryHeight;
            element.preferredHeight = theme.SystemStatusSummaryHeight;
            element.flexibleHeight = 0;
            surface = factory.RoundedImage("Readiness Surface", frame.transform,
                theme.HomeDarkSurfaceElevated);
            TouchUiFactory.Stretch(surface.rectTransform, 1, 1, -1, -1);
            surface.raycastTarget = false;

            semanticAccent = factory.Image("Readiness Semantic Accent", surface.transform,
                theme.HomeDarkTextMuted);
            TouchUiFactory.Anchor(semanticAccent.rectTransform, 0, 0, 0, 1, 0, 16, 5, -16);
            semanticAccent.raycastTarget = false;

            var eyebrow = factory.Label("Readiness Eyebrow", surface.transform,
                "系统状态  /  SYSTEM", theme.Caption, FontStyle.Bold,
                theme.HomeDarkAccent, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(eyebrow.rectTransform, 0, 1, .5f, 1, 28, -42, 0, -14);

            title = factory.Label("Readiness Title", surface.transform, "状态检查中", theme.H1,
                FontStyle.Bold, theme.HomeDarkTextPrimary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(title.rectTransform, 0, 1, .65f, 1, 28, -99, 0, -44);
            detail = factory.Label("Readiness Guidance", surface.transform,
                "正在读取展厅服务与LED播放端状态。", theme.Body, FontStyle.Normal,
                theme.HomeDarkTextSecondary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(detail.rectTransform, 0, 0, .70f, 1, 28, 24, 0, -104);
            detail.resizeTextForBestFit = true;
            detail.resizeTextMinSize = theme.Secondary;
            detail.resizeTextMaxSize = theme.Body;

            badge = new StatusBadge(factory, theme, surface.transform, "Readiness Badge");
            badge.SetDarkMode(true);
            TouchUiFactory.Anchor(badge.Root, 1, 1, 1, 1, -254, -72, -28, -24);
            facts = factory.Label("Readiness Facts", surface.transform, string.Empty, theme.Secondary,
                FontStyle.Normal, theme.HomeDarkTextSecondary, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(facts.rectTransform, .64f, 0, 1, 1, 0, 22, -28, -82);
            facts.resizeTextForBestFit = true;
            facts.resizeTextMinSize = theme.Caption;
            facts.resizeTextMaxSize = theme.Secondary;
        }

        public void Render(TouchUiState state)
        {
            if (state?.Connected != true)
            {
                Set("连接异常", "展厅服务连接已中断，系统正在自动重连；恢复连接后状态会自动更新。",
                    StatusTone.Error, "Server 离线 · LED与内容状态暂不可用");
                return;
            }
            if (state.HasActiveSession)
            {
                var paused = state.Session?.paused == true;
                Set(paused ? "讲解任务已暂停" : "讲解任务运行中",
                    paused ? "当前任务处于暂停状态，可进入当前讲解页面继续。" :
                    "系统正在执行讲解任务，结束或终止后才能开始新的任务。",
                    paused ? StatusTone.Warning : StatusTone.Info, BuildFacts(state));
                return;
            }
            if (state.Readiness == null)
            {
                Set("状态检查中", "正在核验LED播放端和正式内容，请等待状态检查完成。",
                    StatusTone.Warning, "Server 在线 · 就绪条件检查中");
                return;
            }
            if (state.Readiness.canStart && state.Readiness.ledReady)
            {
                Set("系统可接待", "服务、LED播放端和当前正式内容均已就绪，可以开始新的自动讲解。",
                    StatusTone.Success, BuildFacts(state));
                return;
            }
            if (state.Readiness.canStart)
            {
                Set("受限可用", "系统允许开始讲解，但部分素材或LED状态尚未完全就绪；请留意下方说明。",
                    StatusTone.Warning, BuildFacts(state));
                return;
            }
            Set("暂不可接待", FriendlyReadinessMessage(state.Readiness.message),
                StatusTone.Warning, BuildFacts(state));
        }

        public void RefreshTheme()
        {
            frame.color = Color.Lerp(theme.HomeDarkBorder, ToneColor(tone), .20f);
            surface.color = theme.HomeDarkSurfaceElevated;
            badge.SetDarkMode(true);
            badge.RefreshTheme();
            semanticAccent.color = ToneColor(tone);
        }

        private void Set(string heading, string guidance, StatusTone value, string factText)
        {
            tone = value;
            title.text = heading;
            detail.text = guidance;
            facts.text = factText;
            badge.Set(heading, value);
            semanticAccent.color = ToneColor(value);
            frame.color = Color.Lerp(theme.HomeDarkBorder, ToneColor(value), .20f);
        }

        private static string BuildFacts(TouchUiState state)
        {
            var readiness = state.Readiness;
            var version = readiness?.contentVersion ?? state.Content?.version ?? 0;
            var led = readiness == null ? "LED 状态检查中" : readiness.ledOnline
                ? (readiness.ledReady ? "LED 已就绪" : "LED 未就绪")
                : "LED 离线";
            return "Server 在线 · " + led +
                   (version > 0 ? " · 内容 V" + version : " · 暂无正式内容");
        }

        private static string FriendlyReadinessMessage(string serverMessage)
        {
            if (string.IsNullOrWhiteSpace(serverMessage))
                return "系统尚未满足开始讲解条件，请根据下方状态卡完成检查。";
            var lower = serverMessage.ToLowerInvariant();
            if (lower.Contains("led"))
                return "LED播放端尚未满足开始条件，请检查播放主机和素材准备状态。";
            if (lower.Contains("content") || lower.Contains("publish") || lower.Contains("内容"))
                return "当前没有可用的正式内容，请先在管理端完成内容发布。";
            return "系统尚未满足开始讲解条件，请根据下方状态卡完成检查。";
        }

        private Color ToneColor(StatusTone value)
        {
            switch (value)
            {
                case StatusTone.Success: return theme.Success;
                case StatusTone.Warning: return theme.Warning;
                case StatusTone.Error: return theme.Error;
                case StatusTone.Info: return theme.HomeDarkAccent;
                default: return theme.HomeDarkTextMuted;
            }
        }
    }
}
