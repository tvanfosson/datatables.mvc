
namespace DataTables.WebApi
{
    public class QueryParameterConstants
    {
        /// <summary>
        /// Formatting to retrieve data for each column.
        /// </summary>
        public const string COLUMN_DATA_FORMATTING = "columns[{0}].data";
        /// <summary>
        /// Formatting to retrieve name for each column.
        /// </summary>
        public const string COLUMN_NAME_FORMATTING = "columns[{0}].name";
        /// <summary>
        /// Formatting to retrieve searchable indicator for each column.
        /// </summary>
        public const string COLUMN_SEARCHABLE_FORMATTING = "columns[{0}].searchable";
        /// <summary>
        /// Formatting to retrieve orderable indicator for each column.
        /// </summary>
        public const string COLUMN_ORDERABLE_FORMATTING = "columns[{0}].orderable";
        /// <summary>
        /// Formatting to retrieve search value for each column.
        /// </summary>
        public const string COLUMN_SEARCH_VALUE_FORMATTING = "columns[{0}].search.value";
        /// <summary>
        /// Formatting to retrieve search regex indicator for each column.
        /// </summary>
        public const string COLUMN_SEARCH_REGEX_FORMATTING = "columns[{0}].search.regex";
        /// <summary>
        /// Formatting to retrieve ordered columns.
        /// </summary>
        public const string ORDER_COLUMN_FORMATTING = "order[{0}].column";
        /// <summary>
        /// Formatting to retrieve columns order direction.
        /// </summary>
        public const string ORDER_DIRECTION_FORMATTING = "order[{0}].dir";

        /// <summary>
        /// Formatting to retrieve search value.
        /// </summary>
        public const string SEARCH_VALUE = "search.value";

        /// <summary>
        /// Formatting to retrieve search regex indicator.
        /// </summary>
        public const string SEARCH_REGEX = "search.regex";
    }
}
