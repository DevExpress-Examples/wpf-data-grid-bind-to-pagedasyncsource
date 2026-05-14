Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Xpf.Data
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks

Namespace PagedAsyncSourceMVVMSample

    Public Class IssueServiceViewModel
        Inherits ViewModelBase

        <Command>
        Public Sub FetchIssues(ByVal args As FetchPageAsyncArgs)
            args.Result = GetIssuesAsync(args)
        End Sub

        Private Async Function GetIssuesAsync(ByVal args As FetchPageAsyncArgs) As Task(Of FetchRowsResult)
            Dim issues = Await IssuesService.GetIssuesAsync(page:=args.Skip \ args.Take, pageSize:=args.Take, sortOrder:=GetIssueSortOrder(args.SortOrder), filter:=CType(args.Filter, IssueFilter))
            Return New FetchRowsResult(issues, hasMoreRows:=issues.Length = args.Take)
        End Function

        Private Shared Function GetIssueSortOrder(ByVal sortOrder As SortDefinition()) As IssueSortOrder
            If sortOrder.Length > 0 Then
                Dim sort = sortOrder.[Single]()
                If Equals(sort.PropertyName, "Created") Then
                    If sort.Direction <> ListSortDirection.Descending Then Throw New InvalidOperationException()
                    Return IssueSortOrder.CreatedDescending
                End If

                If Equals(sort.PropertyName, "Votes") Then
                    Return If(sort.Direction = ListSortDirection.Ascending, IssueSortOrder.VotesAscending, IssueSortOrder.VotesDescending)
                End If
            End If

            Return IssueSortOrder.Default
        End Function

        <Command>
        Public Sub GetTotalSummaries(ByVal args As GetSummariesAsyncArgs)
            args.Result = GetTotalSummariesAsync(args)
        End Sub

        Private Shared Async Function GetTotalSummariesAsync(ByVal e As GetSummariesAsyncArgs) As Task(Of Object())
            Dim summaryValues = Await GetSummariesAsync(CType(e.Filter, IssueFilter))
            Return e.Summaries.[Select](Function(x)
                If x.SummaryType = SummaryType.Count Then Return CObj(summaryValues.Count)
                If x.SummaryType = SummaryType.Max AndAlso Equals(x.PropertyName, "Created") Then Return summaryValues.LastCreated
                Throw New InvalidOperationException()
            End Function).ToArray()
        End Function

        <Command>
        Public Sub GetUniqueValues(ByVal args As GetUniqueValuesAsyncArgs)
            If Equals(args.PropertyName, "Priority") Then
                Dim values = [Enum].GetValues(GetType(Priority)).Cast(Of Object)().ToArray()
                args.Result = Task.FromResult(values)
            Else
                Throw New InvalidOperationException()
            End If
        End Sub
    End Class
End Namespace
