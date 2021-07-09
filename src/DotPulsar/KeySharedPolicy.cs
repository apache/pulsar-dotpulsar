namespace DotPulsar
{
    /// <summary>
    /// only KeySharedMode == AutoSplit is supported at the moment.
    /// </summary>
    public class KeySharedPolicy
    {
        public bool AllowOutOfOrderDelivery { get; set; } = false;
    }
}
