using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Data;
using DevExpress.Mvvm.Xpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using PagedAsyncSourceMVVMSample;

namespace PagedAsyncSourceMVVMSample {
    public class IssueServiceViewModel : ViewModelBase {
        [Command]
        public void FetchIssues(FetchPageAsyncArgs args) {
            args.Result = GetIssuesAsync(args);
        }
        async Task<FetchRowsResult> GetIssuesAsync(FetchPageAsyncArgs args) {
            var issues = await IssuesService.GetIssuesAsync(
                page: args.Skip/args.Take,
                pageSize: args.Take,
                sortOrder: GetIssueSortOrder(args.SortOrder),
                filter: (IssueFilter)args.Filter);

            return new FetchRowsResult(issues, hasMoreRows: issues.Length == args.Take);
        }
        static IssueSortOrder GetIssueSortOrder(SortDefinition[] sortOrder) {
            if(sortOrder.Length > 0) {
                var sort = sortOrder.Single();
                if(sort.PropertyName == "Created") {
                    if(sort.Direction != ListSortDirection.Descending)
                        throw new InvalidOperationException();
                    return IssueSortOrder.CreatedDescending;
                }
                if(sort.PropertyName == "Votes") {
                    return sort.Direction == ListSortDirection.Ascending
                        ? IssueSortOrder.VotesAscending
                        : IssueSortOrder.VotesDescending;
                }
            }
            return IssueSortOrder.Default;
        }

        [Command]
        public void GetTotalSummaries(GetSummariesAsyncArgs args) {
            args.Result = GetTotalSummariesAsync(args);
        }
        static async Task<object[]> GetTotalSummariesAsync(GetSummariesAsyncArgs e) {
            var summaryValues = await IssuesService.GetSummariesAsync((IssueFilter)e.Filter);
            return e.Summaries.Select(x => {
                if(x.SummaryType == SummaryType.Count)
                    return (object)summaryValues.Count;
                if(x.SummaryType == SummaryType.Max && x.PropertyName == "Created")
                    return summaryValues.LastCreated;
                throw new InvalidOperationException();
            }).ToArray();
        }

        [Command]
        public void GetUniqueValues(GetUniqueValuesAsyncArgs args) {
            if(args.PropertyName == "Priority") {
                var values = Enum.GetValues(typeof(Priority)).Cast<object>().ToArray();
                args.Result = Task.FromResult(values);
            } else {
                throw new InvalidOperationException();
            }
        }
    }
}
