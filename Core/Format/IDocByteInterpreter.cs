namespace dCForm.Core.Format
{
    public interface IDocByteInterpreter : IDocDataInterpreter
    {
        /// <summary>
        ///     Synonymous with the verbs parse & deserialize
        /// </summary>
        /// <param name="DocData"></param>
        /// <param name="DocRevStrict"></param>
        /// <returns></returns>
        BaseDoc Read(byte[] DocData, bool DocRevStrict = false);

        /// <summary>
        ///     Desterilize only properties associated with this solution's internal DocProcessing.
        /// </summary>
        /// <param name="DocData"></param>
        /// <returns>filled DocProcessingInstructions or null if they can't be extract</returns>
        DocProcessingInstructions ReadDocPI(byte[] srcDocData);

        /// <summary>
        ///     Name of document type that correlates with the folder name in the ~/form/doctypename of this app
        /// </summary>
        /// <param name="DocData"></param>
        /// <returns>actual name or null if it can't be extract</returns>
        string ReadDocTypeName(byte[] DocData);

        /// <summary>
        ///     Name of document type that correlates with the folder name in the ~/form/doctypename of this app
        /// </summary>
        /// <param name="DocData"></param>
        /// <returns></returns>
        string ReadRevision(byte[] DocData);

        void Validate(byte[] DocData);
        byte[] Write<T>(T source, bool includeProcessingInformation = true) where T : DocProcessingInstructions;

        byte[] WritePI(byte[] srcDocData, DocProcessingInstructions pi);
    }
}