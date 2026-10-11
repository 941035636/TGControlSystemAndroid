using System;
using System.Collections.Generic;
using TG.Control.Touch.UI.Components;
using TG.Control.Touch.UI.Services;
using TG.Control.Touch.UI.Theme;
using UnityEngine;

namespace TG.Control.Touch.UI.Pages
{
    /// <summary>
    /// Current-narration presentation. It renders the existing TouchUiState and emits operator
    /// intent only; session recovery and playback commands remain in the existing facade path.
    /// </summary>
    public sealed class PlaybackPage
    {
        private readonly TouchTheme theme;
        private readonly RectTransform root;
        private readonly PlaybackProgressPanel progressPanel;
        private readonly PlaybackControlBar controlBar;
        private readonly ErrorBanner errorBanner;

        public RectTransform Root => root;
        public event Action PauseRequested;
        public event Action ResumeRequested;
        public event Action RetryRequested;
        public event Action SkipRequested;
        public event Action<double> VideoVolumeRequested;
        public event Action StopRequested;
        public event Action StopCancelled;

        public PlaybackPage(TouchUiFactory factory, TouchTheme theme, TouchImageLoader imageLoader,
            Transform parent)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            this.theme = theme ?? throw new ArgumentNullException(nameof(theme));
            if (imageLoader == null) throw new ArgumentNullException(nameof(imageLoader));

            root = factory.Rect("Playback Page", parent);

            progressPanel = new PlaybackProgressPanel(factory, theme, imageLoader, root);
            TouchUiFactory.Anchor(progressPanel.Root, 0, 0, 1, 1, 0, 182, 0, 0);

            controlBar = new PlaybackControlBar(factory, theme, root);
            TouchUiFactory.Anchor(controlBar.Root, 0, 0, 1, 0, 0, 0, 0, 166);
            controlBar.PauseRequested += () => PauseRequested?.Invoke();
            controlBar.ResumeRequested += () => ResumeRequested?.Invoke();
            controlBar.RetryRequested += () => RetryRequested?.Invoke();
            controlBar.SkipRequested += () => SkipRequested?.Invoke();
            controlBar.VideoVolumeRequested += value => VideoVolumeRequested?.Invoke(value);
            controlBar.StopRequested += () => StopRequested?.Invoke();
            controlBar.StopCancelled += () => StopCancelled?.Invoke();

            errorBanner = new ErrorBanner(factory, theme, root);
            TouchUiFactory.Anchor(errorBanner.Root, 0, 1, 1, 1, theme.Space16,
                -126, -theme.Space16, -theme.Space16);
        }

        public void Render(TouchUiState state, string verifiedRouteName,
            IReadOnlyList<string> verifiedModuleIds, bool stopConfirmationPending,
            Func<string, string> resolveAssetUrl)
        {
            if (state == null) return;
            progressPanel.Render(state, verifiedRouteName, verifiedModuleIds, resolveAssetUrl);
            controlBar.Render(state.Connected, state.HasActiveSession, state.Session, stopConfirmationPending);
        }

        public void ShowError(string message) => errorBanner.Show(SanitizeOperatorError(message));
        public void ClearError() => errorBanner.Hide();

        public void RefreshTheme()
        {
            progressPanel.RefreshTheme();
            controlBar.RefreshTheme();
            errorBanner.RefreshTheme();
        }

        private static string SanitizeOperatorError(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "讲解操作未完成，请稍后重试。";
            var lower = message.ToLowerInvariant();
            if (lower.Contains("409") || lower.Contains("conflict"))
                return "当前讲解状态已变化，请等待页面刷新后再操作。";
            if (lower.Contains("exception") || lower.Contains("stack") || lower.Contains("http"))
                return "讲解服务暂时无法完成操作，请检查连接后重试。";
            return message.StartsWith("操作失败：", StringComparison.Ordinal) ? message : "操作失败：" + message;
        }
    }
}
