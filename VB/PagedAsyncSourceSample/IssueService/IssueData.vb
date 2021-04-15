﻿Imports System

Namespace PagedAsyncSourceSample
	Public Class IssueData
		Public Sub New(ByVal subject As String, ByVal user As String, ByVal created As DateTime, ByVal votes As Integer, ByVal priority As Priority)
			Me.Subject = subject
			Me.User = user
			Me.Created = created
			Me.Votes = votes
			Me.Priority = priority
		End Sub
		Private privateSubject As String
		Public Property Subject() As String
			Get
				Return privateSubject
			End Get
			Private Set(ByVal value As String)
				privateSubject = value
			End Set
		End Property
		Private privateUser As String
		Public Property User() As String
			Get
				Return privateUser
			End Get
			Private Set(ByVal value As String)
				privateUser = value
			End Set
		End Property
		Private privateCreated As DateTime
		Public Property Created() As DateTime
			Get
				Return privateCreated
			End Get
			Private Set(ByVal value As DateTime)
				privateCreated = value
			End Set
		End Property
		Private privateVotes As Integer
		Public Property Votes() As Integer
			Get
				Return privateVotes
			End Get
			Private Set(ByVal value As Integer)
				privateVotes = value
			End Set
		End Property
		Private privatePriority As Priority
		Public Property Priority() As Priority
			Get
				Return privatePriority
			End Get
			Private Set(ByVal value As Priority)
				privatePriority = value
			End Set
		End Property
	End Class
	Public Enum Priority
		Low
		BelowNormal
		Normal
		AboveNormal
		High
	End Enum
End Namespace
