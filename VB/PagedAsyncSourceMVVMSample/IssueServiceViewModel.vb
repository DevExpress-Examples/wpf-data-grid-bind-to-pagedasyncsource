Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Xpf.Data
Imports DevExpress.Mvvm.Xpf
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks
Imports PagedAsyncSourceMVVMSample

Namespace PagedAsyncSourceMVVMSample
	Public Class IssueServiceViewModel
		Inherits ViewModelBase

		<Command>
		Public Sub FetchIssues(ByVal args As FetchPageAsyncArgs)
			args.Result = GetIssuesAsync(args)
		End Sub
		Private Async Function GetIssuesAsync(ByVal args As FetchPageAsyncArgs) As Task(Of FetchRowsResult)
'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim issues = Await IssuesService.GetIssuesAsync(page:= args.Skip/args.Take, pageSize:= args.Take, sortOrder:= GetIssueSortOrder(args.SortOrder), filter:= CType(args.Filter, IssueFilter))

			Return New FetchRowsResult(issues, hasMoreRows:= issues.Length = args.Take)
		End Function
		Private Shared Function GetIssueSortOrder(ByVal sortOrder() As SortDefinition) As IssueSortOrder
			If sortOrder.Length > 0 Then
				Dim sort = sortOrder.Single()
				If sort.PropertyName = "Created" Then
					If sort.Direction <> ListSortDirection.Descending Then
						Throw New InvalidOperationException()
					End If
					Return IssueSortOrder.CreatedDescending
				End If
				If sort.PropertyName = "Votes" Then
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
			Dim summaryValues = Await IssuesService.GetSummariesAsync(CType(e.Filter, IssueFilter))
			Return e.Summaries.Select(Function(x)
				If x.SummaryType = SummaryType.Count Then
					Return DirectCast(summaryValues.Count, Object)
				End If
				If x.SummaryType = SummaryType.Max AndAlso x.PropertyName = "Created" Then
					Return summaryValues.LastCreated
				End If
				Throw New InvalidOperationException()
			End Function).ToArray()
		End Function

		<Command>
		Public Sub GetUniqueValues(ByVal args As GetUniqueValuesAsyncArgs)
			If args.PropertyName = "Priority" Then
				Dim values = System.Enum.GetValues(GetType(Priority)).Cast(Of Object)().ToArray()
				args.Result = Task.FromResult(values)
			Else
				Throw New InvalidOperationException()
			End If
		End Sub
	End Class
End Namespace
