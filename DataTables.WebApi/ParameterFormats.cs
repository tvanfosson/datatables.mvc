using System.Net.Http;
using DataTables.Core;

namespace DataTables.WebApi
{
    public abstract class ParameterFormats
    {
        public static ParameterFormats GetFormats(HttpMethod method)
        {
            if (method == HttpMethod.Get)
            {
                return new QueryParameterFormats();
            }

            return new FormParameterFormats();
        }

        public abstract string ColumnDataFormat { get; }
        public abstract string ColumnNameFormat { get; }
        public abstract string ColumnSearchableFormat { get; }
        public abstract string ColumnOrderableFormat { get; }
        public abstract string ColumnSearchValueFormat { get; }
        public abstract string ColumnSearchRegexFormat { get; }
        public abstract string OrderColumnFormat { get; }
        public abstract string OrderDirectionFormat { get; }
        public abstract string SearchValue { get; }
        public abstract string SearchRegex { get; }
    }

    public class QueryParameterFormats : ParameterFormats
    {

        public override string ColumnDataFormat { get { return QueryParameterConstants.COLUMN_DATA_FORMATTING; } }

        public override string ColumnNameFormat { get { return QueryParameterConstants.COLUMN_NAME_FORMATTING; } }

        public override string ColumnSearchableFormat { get { return QueryParameterConstants.COLUMN_SEARCHABLE_FORMATTING; } }

        public override string ColumnOrderableFormat { get { return QueryParameterConstants.COLUMN_ORDERABLE_FORMATTING; } }

        public override string ColumnSearchValueFormat { get { return QueryParameterConstants.COLUMN_SEARCH_VALUE_FORMATTING; } }

        public override string ColumnSearchRegexFormat { get { return QueryParameterConstants.COLUMN_SEARCH_REGEX_FORMATTING; } }

        public override string OrderColumnFormat { get { return QueryParameterConstants.ORDER_COLUMN_FORMATTING; } }

        public override string OrderDirectionFormat { get { return QueryParameterConstants.ORDER_DIRECTION_FORMATTING; } }

        public override string SearchValue { get { return QueryParameterConstants.SEARCH_VALUE; } }

        public override string SearchRegex { get { return QueryParameterConstants.SEARCH_REGEX; } }
    }

    public class FormParameterFormats : ParameterFormats
    {

        public override string ColumnDataFormat { get { return ParameterConstants.COLUMN_DATA_FORMATTING; } }

        public override string ColumnNameFormat { get { return ParameterConstants.COLUMN_NAME_FORMATTING; } }

        public override string ColumnSearchableFormat { get { return ParameterConstants.COLUMN_SEARCHABLE_FORMATTING; } }

        public override string ColumnOrderableFormat { get { return ParameterConstants.COLUMN_ORDERABLE_FORMATTING; } }

        public override string ColumnSearchValueFormat { get { return ParameterConstants.COLUMN_SEARCH_VALUE_FORMATTING; } }

        public override string ColumnSearchRegexFormat { get { return ParameterConstants.COLUMN_SEARCH_REGEX_FORMATTING; } }

        public override string OrderColumnFormat { get { return ParameterConstants.ORDER_COLUMN_FORMATTING; } }

        public override string OrderDirectionFormat { get { return ParameterConstants.ORDER_DIRECTION_FORMATTING; } }

        public override string SearchValue { get { return ParameterConstants.SEARCH_VALUE; } }

        public override string SearchRegex { get { return ParameterConstants.SEARCH_REGEX; } }
    }

}
