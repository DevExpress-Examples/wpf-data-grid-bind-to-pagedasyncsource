Imports System

Namespace PagedAsyncSourceSample
	Public Class IssuesSummaries
		Public Sub New(ByVal count As Integer, ByVal lastCreated? As DateTime)
			Me.Count = count
			Me.LastCreated = lastCreated
		End Sub

		Private privateCount As Integer
		Public Property Count() As Integer
			Get
				Return privateCount
			End Get
			Private Set(ByVal value As Integer)
				privateCount = value
			End Set
		End Property
		Private privateLastCreated? As DateTime
		Public Property LastCreated() As DateTime?
			Get
				Return privateLastCreated
			End Get
			Private Set(ByVal value? As DateTime)
				privateLastCreated = value
			End Set
		End Property
	End Class
End Namespace
