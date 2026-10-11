using System;
using TG.Control.Touch.UI.Theme;
using TG.Control.UnityContracts;
using UnityEngine;
using UnityEngine.UI;

namespace TG.Control.Touch.UI.Components
{
    /// <summary>Touch-safe playback controls that only emit operator intent.</summary>
    public sealed class PlaybackControlBar
    {
        private readonly TouchTheme theme;
        private readonly Image frame;
        private readonly Image surface;
        private readonly GameObject normalControls;
        private readonly GameObject stopConfirmation;
        private readonly Button primaryButton;
        private readonly Button retryButton;
        private readonly Button skipButton;
        private readonly Button volumeDownButton;
        private readonly Button volumeUpButton;
        private readonly Text volumeValueLabel;
        private readonly Image volumeFill;
        private readonly Button stopButton;
        private readonly Button cancelStopButton;
        private readonly Button confirmStopButton;
        private bool paused;
        private double videoVolume = 0.25;
        private double? pendingVideoVolume;
        private float pendingVolumeRequestedAt;

        public RectTransform Root => frame.rectTransform;
        public event Action PauseRequested;
        public event Action ResumeRequested;
        public event Action RetryRequested;
        public event Action SkipRequested;
        public event Action<double> VideoVolumeRequested;
        public event Action StopRequested;
        public event Action StopCancelled;

        public PlaybackControlBar(TouchUiFactory factory, TouchTheme theme, Transform parent)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            this.theme = theme ?? throw new ArgumentNullException(nameof(theme));

            frame = factory.RoundedImage("Playback Control Frame", parent, theme.PlaybackBorder);
            surface = factory.RoundedImage("Playback Control Surface", frame.transform,
                theme.PlaybackControlSurface);
            TouchUiFactory.Stretch(surface.rectTransform, 1, 1, -1, -1);
            surface.raycastTarget = false;

            normalControls = factory.Rect("Playback Normal Controls", surface.transform).gameObject;
            TouchUiFactory.Stretch(normalControls.GetComponent<RectTransform>());

            primaryButton = factory.TouchButton(normalControls.transform, "暂停讲解", true, OnPrimary);
            TouchUiFactory.Anchor(primaryButton.GetComponent<RectTransform>(), 0, .5f, 0, .5f,
                24, -52, 304, 52);
            ConfigureButton(primaryButton, theme.Primary, Color.white, true);

            retryButton = factory.TouchButton(normalControls.transform, "重试当前", false,
                () => RetryRequested?.Invoke());
            TouchUiFactory.Anchor(retryButton.GetComponent<RectTransform>(), 0, .5f, 0, .5f,
                320, -45, 524, 45);
            ConfigureButton(retryButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);

            skipButton = factory.TouchButton(normalControls.transform, "跳过当前", false,
                () => SkipRequested?.Invoke());
            TouchUiFactory.Anchor(skipButton.GetComponent<RectTransform>(), 0, .5f, 0, .5f,
                540, -45, 744, 45);
            ConfigureButton(skipButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);

            var volumeGroupBorder = factory.RoundedImage("Playback Volume Border", normalControls.transform,
                theme.HomeDarkBorderSoft);
            TouchUiFactory.Anchor(volumeGroupBorder.rectTransform, 0, .5f, 1, .5f,
                760, -52, -394, 52);
            volumeGroupBorder.raycastTarget = false;
            var volumeGroup = factory.RoundedImage("Playback Volume Surface", volumeGroupBorder.transform,
                theme.PlaybackPanelSurface);
            TouchUiFactory.Stretch(volumeGroup.rectTransform, 1, 1, -1, -1);
            volumeGroup.raycastTarget = false;

            var volumeHeading = factory.Label("Playback Volume Heading", volumeGroup.transform,
                "大屏原声音量", theme.Caption, FontStyle.Bold, theme.HomeDarkTextSecondary,
                TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(volumeHeading.rectTransform, 0, 1, .42f, 1, 18, -32, 0, -8);

            volumeDownButton = factory.TouchButton(volumeGroup.transform, "−", false,
                () => AdjustVideoVolume(-0.1));
            TouchUiFactory.Anchor(volumeDownButton.GetComponent<RectTransform>(), 0, 0, 0, 0,
                16, 12, 72, 62);
            ConfigureButton(volumeDownButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);

            var volumeTrack = factory.RoundedImage("Playback Volume Track", volumeGroup.transform,
                new Color(.13f, .35f, .62f, .56f));
            TouchUiFactory.Anchor(volumeTrack.rectTransform, 0, 0, 1, 0, 88, 31, -126, 43);
            volumeTrack.raycastTarget = false;
            volumeFill = factory.RoundedImage("Playback Volume Fill", volumeTrack.transform,
                theme.HomeDarkAccent);
            TouchUiFactory.Anchor(volumeFill.rectTransform, 0, 0, 0, 1, 0, 0, 0, 0);
            volumeFill.raycastTarget = false;

            volumeValueLabel = factory.Label("Playback Volume Value", volumeGroup.transform, "25%",
                theme.Secondary, FontStyle.Bold, theme.HomeDarkTextPrimary, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(volumeValueLabel.rectTransform, 1, 0, 1, 0, -118, 13, -68, 61);

            volumeUpButton = factory.TouchButton(volumeGroup.transform, "+", false,
                () => AdjustVideoVolume(0.1));
            TouchUiFactory.Anchor(volumeUpButton.GetComponent<RectTransform>(), 1, 0, 1, 0,
                -62, 12, -8, 62);
            ConfigureButton(volumeUpButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);

            var dangerDivider = factory.Image("Playback Danger Divider", normalControls.transform,
                theme.HomeDarkBorderSoft);
            TouchUiFactory.Anchor(dangerDivider.rectTransform, 1, 0, 1, 1, -376, 20, -374, -20);
            dangerDivider.raycastTarget = false;

            stopButton = factory.TouchButton(normalControls.transform, "终止讲解", false,
                () => StopRequested?.Invoke());
            TouchUiFactory.Anchor(stopButton.GetComponent<RectTransform>(), 1, .5f, 1, .5f,
                -352, -45, -24, 45);
            StyleDangerButton(stopButton, false);

            stopConfirmation = factory.RoundedImage("Stop Confirmation", surface.transform,
                new Color(.16f, .035f, .075f, .96f)).gameObject;
            TouchUiFactory.Stretch(stopConfirmation.GetComponent<RectTransform>(), 10, 10, -10, -10);
            var confirmTitle = factory.Label("Stop Confirmation Title", stopConfirmation.transform,
                "确认终止本次讲解？", theme.H2, FontStyle.Bold, theme.HomeDarkTextPrimary,
                TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(confirmTitle.rectTransform, 0, 1, .58f, 1, 26, -58, 0, -16);
            var confirmDetail = factory.Label("Stop Confirmation Detail", stopConfirmation.transform,
                "终止后当前讲解任务结束，如需继续必须重新发起。", theme.Body, FontStyle.Normal,
                theme.HomeDarkTextSecondary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(confirmDetail.rectTransform, 0, 0, .62f, 1, 26, 16, 0, -58);
            cancelStopButton = factory.TouchButton(stopConfirmation.transform, "取消", false,
                () => StopCancelled?.Invoke());
            TouchUiFactory.Anchor(cancelStopButton.GetComponent<RectTransform>(), 1, .5f, 1, .5f,
                -494, -34, -274, 34);
            ConfigureButton(cancelStopButton, theme.HomeDarkGlass, theme.HomeDarkTextPrimary, false);
            confirmStopButton = factory.TouchButton(stopConfirmation.transform, "确认终止", false,
                () => StopRequested?.Invoke());
            TouchUiFactory.Anchor(confirmStopButton.GetComponent<RectTransform>(), 1, .5f, 1, .5f,
                -258, -34, -26, 34);
            StyleDangerButton(confirmStopButton, true);
            stopConfirmation.SetActive(false);
        }

        public void Render(bool connected, bool hasActiveSession, PlaybackSessionStatus session,
            bool stopConfirmationPending)
        {
            paused = session?.paused == true;
            var canControl = connected && hasActiveSession;
            var hasSnapshot = session != null;
            if (hasSnapshot)
            {
                var serverVolume = Math.Max(0, Math.Min(1, session.videoVolume));
                if (pendingVideoVolume.HasValue &&
                    (Math.Abs(pendingVideoVolume.Value - serverVolume) < 0.005 ||
                     Time.realtimeSinceStartup - pendingVolumeRequestedAt > 2f))
                    pendingVideoVolume = null;
                videoVolume = pendingVideoVolume ?? serverVolume;
            }
            UpdateVolumePresentation();

            primaryButton.GetComponentInChildren<Text>().text = !hasSnapshot
                ? "正在恢复状态" : paused ? "继续讲解" : "暂停讲解";
            primaryButton.interactable = canControl && hasSnapshot;
            retryButton.interactable = canControl;
            skipButton.interactable = canControl;
            volumeDownButton.interactable = canControl && hasSnapshot && videoVolume > 0.005;
            volumeUpButton.interactable = canControl && hasSnapshot && videoVolume < 0.995;
            stopButton.interactable = canControl;
            cancelStopButton.interactable = true;
            confirmStopButton.interactable = canControl;
            normalControls.SetActive(!stopConfirmationPending);
            stopConfirmation.SetActive(stopConfirmationPending);
        }

        public void RefreshTheme()
        {
            frame.color = theme.PlaybackBorder;
            surface.color = theme.PlaybackControlSurface;
            ConfigureButton(primaryButton, theme.Primary, Color.white, true);
            ConfigureButton(retryButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);
            ConfigureButton(skipButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);
            ConfigureButton(volumeDownButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);
            ConfigureButton(volumeUpButton, theme.PlaybackGlass, theme.HomeDarkTextPrimary, false);
            ConfigureButton(cancelStopButton, theme.HomeDarkGlass, theme.HomeDarkTextPrimary, false);
            StyleDangerButton(stopButton, false);
            StyleDangerButton(confirmStopButton, true);
            volumeFill.color = theme.HomeDarkAccent;
        }

        private void OnPrimary()
        {
            if (paused) ResumeRequested?.Invoke();
            else PauseRequested?.Invoke();
        }

        private void AdjustVideoVolume(double delta)
        {
            var next = Math.Max(0, Math.Min(1, Math.Round((videoVolume + delta) * 10) / 10));
            if (Math.Abs(next - videoVolume) < 0.005) return;
            videoVolume = next;
            pendingVideoVolume = next;
            pendingVolumeRequestedAt = Time.realtimeSinceStartup;
            UpdateVolumePresentation();
            VideoVolumeRequested?.Invoke(next);
        }

        private void UpdateVolumePresentation()
        {
            volumeValueLabel.text = Math.Round(videoVolume * 100).ToString("0") + "%";
            volumeFill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01((float)videoVolume), 1);
            volumeFill.rectTransform.offsetMin = Vector2.zero;
            volumeFill.rectTransform.offsetMax = Vector2.zero;
        }

        private void ConfigureButton(Button button, Color background, Color foreground, bool emphasize)
        {
            var image = button.GetComponent<Image>();
            image.color = background;
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.color = foreground;
                label.fontSize = emphasize ? theme.SectionTitle : theme.Body;
            }
            var outline = button.GetComponent<Outline>();
            if (outline == null) outline = button.gameObject.AddComponent<Outline>();
            outline.effectColor = emphasize
                ? new Color(theme.HomeDarkAccent.r, theme.HomeDarkAccent.g, theme.HomeDarkAccent.b, .52f)
                : theme.HomeDarkBorderSoft;
            outline.effectDistance = emphasize ? new Vector2(2, -2) : new Vector2(1, -1);
            outline.useGraphicAlpha = true;
        }

        private void StyleDangerButton(Button button, bool solid)
        {
            var background = solid
                ? theme.Error : new Color(theme.Error.r, theme.Error.g, theme.Error.b, .22f);
            ConfigureButton(button, background, solid ? Color.white : new Color(1f, .60f, .64f, 1f), false);
            var outline = button.GetComponent<Outline>();
            outline.effectColor = new Color(theme.Error.r, theme.Error.g, theme.Error.b, .55f);
        }
    }
}
