namespace QueryPack.Criterias.ImMemory
{
    /// <summary>
    /// The basic interface which contains the definition for text search
    /// </summary>
    public interface ITextSearch
    {
        /// <summary>
        /// The text search parameter
        /// </summary>
        string TextSearch { get; set; }
    }
}