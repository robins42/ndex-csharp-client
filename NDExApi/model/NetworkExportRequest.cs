using System;
using System.Collections.Generic;

namespace NDExApi.model
{
    /// <summary>
    /// <para>Used to specify export parameters.</para>
    /// Hardcoded constant exportFormat values are defined in this class to use. 
    /// </summary>
    public class NetworkExportRequest
    {
        /// <summary>
        /// Hardcoded value for exportFormat to export the network to GraphML format
        /// </summary>
        public const string ExportFormatGraphML = "GraphML";
        
        /// <summary>
        /// Hardcoded value for exportFormat to export the network to GSEA Gene Set format
        /// </summary>
        public const string ExportFormatGSEA = "GSEA Gene Set";
        
        /// <summary>
        /// The format which to export to
        /// </summary>
        public string exportFormat;
        
        /// <summary>
        /// Network IDs to export for. No more than 1000 are allowed.
        /// </summary>
        public HashSet<Guid> networkIds;
    }
}