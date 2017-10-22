namespace Songhay.HelloWorlds.Client.Models
{
    /// <summary>
    /// Defines an Activity in a shell environment.
    /// </summary>
    public interface IActivity
    {
        /// <summary>
        /// Starts with the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        void Start(string[] args);
    }
}
