using System;
using TG.Control.Touch.UI.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace TG.Control.Touch.UI.Components
{
    /// <summary>Pure presentation card for a single truthful system-health domain.</summary>
    public sealed class SystemHealthCard
    {
        private readonly TouchTheme theme;
        private readonly Image frame;
        private readonly Image surface;
        private readonly Image semanticAccent;
        private readonly StatusBadge badge;
        private readonly Text status;
        private readonly Text detail;
        private readonly Text primaryFact;
        private readonly Text secondaryFact;
        private StatusTone tone;

        public RectTransform Root => frame.rectTransform;

        public SystemHealthCard(TouchUiFactory factory, TouchTheme theme, Transform parent,
            string title, string number, string iconName)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            this.theme = theme ?? throw new ArgumentNullException(nameof(theme));

            frame = factory.RoundedImage("System Health - " + title, parent, theme.HomeDarkBorderSoft);
            surface = factory.RoundedImage("Card Surface", frame.transform, theme.HomeDarkSurfaceElevated);
            TouchUiFactory.Stretch(surface.rectTransform, 1, 1, -1, -1);
            surface.raycastTarget = false;

            semanticAccent = factory.Image("Semantic Accent", surface.transform, theme.HomeDarkAccent);
            TouchUiFactory.Anchor(semanticAccent.rectTransform, 0, 0, 0, 1, 0, 14, 4, -14);
            semanticAccent.raycastTarget = false;

            var iconPlate = factory.RoundedImage("Card Icon Plate", surface.transform, theme.HomeDarkGlass);
            TouchUiFactory.Anchor(iconPlate.rectTransform, 0, 1, 0, 1, 22, -88, 92, -18);
            iconPlate.raycastTarget = false;
            var icon = factory.Image("Card Icon", iconPlate.transform, theme.HomeDarkAccent);
            TouchUiFactory.Stretch(icon.rectTransform, 15, 15, -15, -15);
            icon.sprite = HomeDarkVisualAssets.NavigationIcon(iconName, true);
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var index = factory.Label("Card Index", surface.transform, number, theme.Caption,
                FontStyle.Bold, theme.HomeDarkAccent, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(index.rectTransform, 0, 1, 0, 1, 112, -50, 148, -18);
            var heading = factory.Label("Card Title", surface.transform, title, theme.CardTitle,
                FontStyle.Bold, theme.HomeDarkTextPrimary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(heading.rectTransform, 0, 1, .62f, 1, 152, -56, 0, -14);

            badge = new StatusBadge(factory, theme, surface.transform, title + " Badge");
            badge.SetDarkMode(true);
            TouchUiFactory.Anchor(badge.Root, 1, 1, 1, 1, -188, -62, -20, -18);

            status = factory.Label("Operator Status", surface.transform, "状态检查中", theme.H2,
                FontStyle.Bold, theme.HomeDarkTextPrimary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(status.rectTransform, 0, 1, 1, 1, 112, -106, -20, -62);
            status.resizeTextForBestFit = true;
            status.resizeTextMinSize = theme.CardTitle;
            status.resizeTextMaxSize = theme.H2;

            detail = factory.Label("Operator Guidance", surface.transform, string.Empty, theme.Secondary,
                FontStyle.Normal, theme.HomeDarkTextSecondary, TextAnchor.UpperLeft);
            TouchUiFactory.Anchor(detail.rectTransform, 0, 1, 1, 1, 112, -158, -20, -110);
            detail.resizeTextForBestFit = true;
            detail.resizeTextMinSize = theme.Caption;
            detail.resizeTextMaxSize = theme.Secondary;

            var divider = factory.Image("Fact Divider", surface.transform, theme.HomeDarkBorderSoft);
            TouchUiFactory.Anchor(divider.rectTransform, 0, 0, 1, 0, 22, 60, -22, 62);
            divider.raycastTarget = false;
            primaryFact = factory.Label("Primary Fact", surface.transform, string.Empty, theme.Caption,
                FontStyle.Bold, theme.HomeDarkTextPrimary, TextAnchor.MiddleLeft);
            TouchUiFactory.Anchor(primaryFact.rectTransform, 0, 0, .55f, 0, 22, 14, 0, 54);
            primaryFact.resizeTextForBestFit = true;
            primaryFact.resizeTextMinSize = 12;
            primaryFact.resizeTextMaxSize = theme.Caption;
            secondaryFact = factory.Label("Secondary Fact", surface.transform, string.Empty, theme.Caption,
                FontStyle.Normal, theme.HomeDarkTextSecondary, TextAnchor.MiddleRight);
            TouchUiFactory.Anchor(secondaryFact.rectTransform, .45f, 0, 1, 0, 0, 14, -22, 54);
            secondaryFact.resizeTextForBestFit = true;
            secondaryFact.resizeTextMinSize = 12;
            secondaryFact.resizeTextMaxSize = theme.Caption;
        }

        public void Render(string statusText, string detailText, StatusTone value,
            string primaryFactText, string secondaryFactText)
        {
            tone = value;
            status.text = statusText ?? string.Empty;
            detail.text = detailText ?? string.Empty;
            primaryFact.text = primaryFactText ?? string.Empty;
            secondaryFact.text = secondaryFactText ?? string.Empty;
            badge.Set(ToneLabel(value), value);
            semanticAccent.color = ToneColor(value);
            frame.color = Color.Lerp(theme.HomeDarkBorderSoft, ToneColor(value), .18f);
        }

        public void RefreshTheme()
        {
            frame.color = Color.Lerp(theme.HomeDarkBorderSoft, ToneColor(tone), .18f);
            surface.color = theme.HomeDarkSurfaceElevated;
            badge.SetDarkMode(true);
            badge.RefreshTheme();
            semanticAccent.color = ToneColor(tone);
        }

        private static string ToneLabel(StatusTone value)
        {
            switch (value)
            {
                case StatusTone.Success: return "正常";
                case StatusTone.Warning: return "请留意";
                case StatusTone.Error: return "需处理";
                case StatusTone.Info: return "运行中";
                default: return "待确认";
            }
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
