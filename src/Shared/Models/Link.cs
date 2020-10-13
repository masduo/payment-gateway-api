namespace Shared.Models
{
    /// <summary> Hypermedia information about the resource </summary>
    public class Link
    {
        /// <summary> Gets or sets the universal location for the resource </summary>
        /// <example> /v1/payments/c1f76446-4afc-47e2-899d-53c06fa4918 </example>
        public string Href { get; set; }

        /// <summary> Gets or sets relationship to the resource </summary>
        /// <example> self </example>
        public string Rel { get; set; }

        /// <summary> Gets or sets the HTTP verb to invoke on resource with </summary>
        /// <example> GET </example>
        public string Method { get; set; }
    }
}