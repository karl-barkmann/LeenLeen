namespace Leen.Media
{
    /// <summary>
    /// Contains constants for specifiying the type of seek for the <see cref="ISeekableMediaPlayer"/>.
    /// </summary>
    public enum MediaSeekingFlags
    {
        /// <summary>
        /// The specified position is ignored.
        /// </summary>
        NoPositioning = 0,

        /// <summary>
        /// The specified position is absolute.
        /// </summary>
        AbsolutePositioning = 1,

        /// <summary>
        /// The specified position is relative to the current position.
        /// </summary>
        RelativePositioning = 2,

        /// <summary>
        /// The specified position is relative to the previous value. 
        /// </summary>
        IncrementalPositioning = 3,

        /// <summary>
        /// Seek to the nearest key frame. 
        /// </summary>
        SeekToKeyFrame = 4,
    }
}
