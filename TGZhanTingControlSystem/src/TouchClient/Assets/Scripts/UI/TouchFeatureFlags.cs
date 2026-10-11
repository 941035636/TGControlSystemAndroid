namespace TG.Control.Touch.UI
{
    /// <summary>
    /// User-facing feature switches. The underlying route models, API calls and editor code stay available
    /// so the temporarily hidden capabilities can be restored without a data or protocol migration.
    /// </summary>
    public static class TouchFeatureFlags
    {
        public static readonly bool EnableCustomRouteUi = false;
        public static readonly bool EnableThemeCombinationUi = false;

        public static bool EnableRouteBasedHomeUi => EnableCustomRouteUi && EnableThemeCombinationUi;
    }
}
