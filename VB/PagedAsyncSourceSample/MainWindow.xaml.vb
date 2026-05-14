Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows

Namespace PagedAsyncSourceSample

    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
            Dim source = New PagedAsyncSource() With {.ElementType = GetType(IssueData), .PageNavigationMode = PageNavigationMode.ArbitraryWithTotalPageCount}
            AddHandler Unloaded, Sub(o, e) source.Dispose()
            AddHandler source.FetchPage, Sub(o, e) e.Result = FetchRowsAsync(e)
            AddHandler source.GetUniqueValues, Sub(o, e)
                If Equals(e.PropertyName, "Priority") Then
                    Dim values = [Enum].GetValues(GetType(Priority)).Cast(Of Object)().ToArray()
                    e.Result = Task.FromResult(values)
                Else
                    Throw New InvalidOperationException()
                End If
            End Sub
            AddHandler source.GetTotalSummaries, Sub(o, e) e.Result = GetTotalSummariesAsync(e)
            Me.grid.ItemsSource = source
        End Sub

        Private Shared Async Function FetchRowsAsync(ByVal e As FetchPageAsyncEventArgs) As Task(Of FetchRowsResult)
            Dim sortOrder As IssueSortOrder = GetIssueSortOrder(e)
            Dim filter As IssueFilter = MakeIssueFilter(e.Filter)
            Dim issues = Await GetIssuesAsync(page:=e.Skip \ e.Take, pageSize:=e.Take, sortOrder:=sortOrder, filter:=filter)
            Return New FetchRowsResult(issues, hasMoreRows:=issues.Length = e.Take)
        End Function

        Private Shared Async Function GetTotalSummariesAsync(ByVal e As GetSummariesAsyncEventArgs) As Task(Of Object())
            Dim filter As IssueFilter = MakeIssueFilter(e.Filter)
            Dim summaryValues = Await GetSummariesAsync(filter)
            Return e.Summaries.[Select](Function(x)
                If x.SummaryType = SummaryType.Count Then Return CObj(summaryValues.Count)
                If x.SummaryType = SummaryType.Max AndAlso Equals(x.PropertyName, "Created") Then Return summaryValues.LastCreated
                Throw New InvalidOperationException()
            End Function).ToArray()
        End Function

        Private Shared Function GetIssueSortOrder(ByVal e As FetchPageAsyncEventArgs) As IssueSortOrder
            If e.SortOrder.Length > 0 Then
                Dim sort = e.SortOrder.[Single]()
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

        Private Shared Function MakeIssueFilter(ByVal filter As CriteriaOperator) As IssueFilter
            Return filter.Match(binary:=Function(propertyName, value, type)
                If Equals(propertyName, "Votes") AndAlso type = BinaryOperatorType.GreaterOrEqual Then Return New IssueFilter(minVotes:=CInt(value))
                If Equals(propertyName, "Priority") AndAlso type = BinaryOperatorType.Equal Then Return New IssueFilter(priority:=CType(value, Priority))
                If Equals(propertyName, "Created") Then
                    If type = BinaryOperatorType.GreaterOrEqual Then Return New IssueFilter(createdFrom:=CDate(value))
                    If type = BinaryOperatorType.Less Then Return New IssueFilter(createdTo:=CDate(value))
                End If

                Throw New InvalidOperationException()
            End Function, [and]:=Function(filters) New IssueFilter(createdFrom:=filters.[Select](Function(x) x.CreatedFrom).SingleOrDefault(Function(x) x IsNot Nothing), createdTo:=filters.[Select](Function(x) x.CreatedTo).SingleOrDefault(Function(x) x IsNot Nothing), minVotes:=filters.[Select](Function(x) x.MinVotes).SingleOrDefault(Function(x) x IsNot Nothing), priority:=filters.[Select](Function(x) x.Priority).SingleOrDefault(Function(x) x IsNot Nothing)), null:=Nothing)
        End Function
    End Class
End Namespace
