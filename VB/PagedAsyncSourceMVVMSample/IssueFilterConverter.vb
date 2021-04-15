Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports System
Imports System.Globalization
Imports System.Linq
Imports System.Windows.Data
Imports System.Windows.Markup

Namespace PagedAsyncSourceMVVMSample
	Public Class IssueFilterConverter
		Inherits MarkupExtension
		Implements IValueConverter

		Private Function IValueConverter_Convert(ByVal filter As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
			Return DirectCast(filter, CriteriaOperator).Match(binary:= Function(propertyName, value, type)
				If propertyName = "Votes" AndAlso type = BinaryOperatorType.GreaterOrEqual Then
					Return New IssueFilter(minVotes:= CInt(Math.Truncate(value)))
				End If
				If propertyName = "Priority" AndAlso type = BinaryOperatorType.Equal Then
					Return New IssueFilter(priority:= CType(value, Priority))
				End If
				If propertyName = "Created" Then
					If type = BinaryOperatorType.GreaterOrEqual Then
						Return New IssueFilter(createdFrom:= CDate(value))
					End If
					If type = BinaryOperatorType.Less Then
						Return New IssueFilter(createdTo:= CDate(value))
					End If
				End If
				Throw New InvalidOperationException()
			End Function, [and]:= Function(filters)
				Return New IssueFilter(createdFrom:= filters.Select(Function(x) x.CreatedFrom).SingleOrDefault(Function(x) x IsNot Nothing), createdTo:= filters.Select(Function(x) x.CreatedTo).SingleOrDefault(Function(x) x IsNot Nothing), minVotes:= filters.Select(Function(x) x.MinVotes).SingleOrDefault(Function(x) x IsNot Nothing), priority:= filters.Select(Function(x) x.Priority).SingleOrDefault(Function(x) x IsNot Nothing))
			End Function, null:= CType(Nothing, IssueFilter))
		End Function
		Private Function IValueConverter_ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
			Throw New NotImplementedException()
		End Function
		Public Overrides Function ProvideValue(ByVal serviceProvider As IServiceProvider) As Object
			Return Me
		End Function
	End Class
End Namespace
