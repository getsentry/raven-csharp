namespace SharpRaven.Data
{
    /// <summary>
    /// This defines the level of the event. If not provided it defaults to info which is the middle level. In the order of priority from highest to lowest the levels are critical, error, warning, info and debug. Levels are used in the UI to emphasize and deemphasize the crumb.
    /// </summary>
    public enum BreadcrumbsLevel
    {
        Critical, Error, Warning, Info, Debug
    }
}