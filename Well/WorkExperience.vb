﻿Imports System.Data.SqlClient

Public Class WorkExperience

    Dim connectionString As String = "Data Source=124.121.233.223\SQLEXPRESS,1433;Initial Catalog=Project ;User Id=admin;Password=adminadminadmin"
    Dim sqlConnection As New SqlConnection(connectionString)
    Private Sub WorkExperience_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' สร้าง query
        Dim query1 As String = "SELECT * FROM Work_Experience"

        ' สร้าง command
        Dim command1 As New SqlCommand(query1, sqlConnection)

        ' สร้าง DataAdapter เพื่อดึงข้อมูล
        Dim adapter As New SqlDataAdapter(command1)

        ' สร้าง DataSet เพื่อเก็บข้อมูล
        Dim dataSet As New DataSet()

        ' เตรียมข้อมูลใน DataSet
        adapter.Fill(dataSet)

        ' แสดงข้อมูลใน DataGridView
        DataGridView1.DataSource = dataSet.Tables(0)

        ' ปิดการเชื่อมต่อ
        sqlConnection.Close()

        exid.ReadOnly = True

    End Sub

    Private Sub btnsearchmed_Click(sender As Object, e As EventArgs) Handles btnsearchmed.Click
        Try
            ' Retrieve the search criteria entered by the user
            Dim searchText As String = txtBoxSearch.Text.Trim()

            ' Create a SQL SELECT statement to retrieve patient information
            Dim selectQuery As String

            If String.IsNullOrWhiteSpace(searchText) Then
                ' If the search box is empty, retrieve all patient records
                selectQuery = "SELECT * FROM Work_Experience"
            Else
                ' If a search term is entered, filter by FirstName, LastName, or Tel
                selectQuery = "SELECT * FROM Work_Experience WHERE ExperienceID LIKE @SearchText OR StaffID LIKE @SearchText OR OrganizationName LIKE @SearchText OR Position LIKE @SearchText OR StartDate LIKE @SearchText OR FinishDate LIKE @SearchText"
            End If

            Using connection As New SqlConnection(connectionString)
                Using command As New SqlCommand(selectQuery, connection)
                    If Not String.IsNullOrWhiteSpace(searchText) Then
                        ' Add a parameter for the search term
                        command.Parameters.AddWithValue("@SearchText", "%" & searchText & "%")
                    End If

                    connection.Open()

                    ' Create a DataTable to store the retrieved data
                    Dim dataTable As New DataTable()

                    ' Execute the query and fill the DataTable with the patient information
                    Dim dataAdapter As New SqlDataAdapter(command)
                    dataAdapter.Fill(dataTable)

                    ' Bind the DataTable to the DataGridView
                    DataGridView1.DataSource = dataTable

                    connection.Close()
                End Using
            End Using
        Catch ex As Exception
            ' Handle any exceptions and display an error message
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetNextID() As String
        ' อย่าลืมเปลี่ยนชื่อ Table นะจ้ะ
        Dim query As String = "SELECT MAX(ExperienceID) FROM Work_Experience"
        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                connection.Open()
                Dim maxPatientID As Object = command.ExecuteScalar()

                If maxPatientID Is DBNull.Value Then
                    ' ถ้าไม่มีข้อมูลจะเริ่มที่ P0001
                    Return "W01"
                Else
                    Dim currentID As Integer = Integer.Parse(maxPatientID.ToString().Substring(1))
                    Dim nextID As Integer = currentID + 1
                    Return "W" & nextID.ToString("D2")
                End If
            End Using
        End Using
    End Function

    Private Sub ButtonAdd_Click(sender As Object, e As EventArgs) Handles ButtonAdd.Click

        'Dim StartDateT As DateTime = Convert.ToDateTime(startDate.Text)
        'Dim EndDateT As DateTime = Convert.ToDateTime(EndDate.Text)

        Try
            ' Get the next available patient ID
            Dim nextID As String = GetNextID()

            ' Create a SQL INSERT statement with the formatted patient ID
            Dim sqlin As String = "INSERT INTO Work_Experience (ExperienceID, StaffID, OrganizationName, Position, StartDate, FinishDate) " &
                                  "VALUES('" & nextID & "','" & staffid.Text & "', '" & organi.Text & "', '" & pos.Text & "', '" & startDate.Value.ToString("yyyy-MM-dd HH:mm:ss") & "', '" & EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") & "')"

            Dim sqlCmd = sqlConnection.CreateCommand()
            sqlCmd.CommandText = sqlin

            sqlConnection.Open()
            sqlCmd.ExecuteNonQuery()

            MessageBox.Show("Saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            WorkExperience_Load(Nothing, Nothing)
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            sqlConnection.Close()
        End Try
    End Sub

    Private Sub ButtonEdit_Click(sender As Object, e As EventArgs) Handles ButtonEdit.Click
        Try
            ' Check if a row is selected in the DataGridView
            If DataGridView1.SelectedRows.Count > 0 Then
                ' Check if only one row is selected
                If DataGridView1.SelectedRows.Count = 1 Then
                    ' Get the selected row
                    Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

                    ' Retrieve the unique identifier (e.g., a primary key) from the selected row
                    Dim primaryKeyValue As String = selectedRow.Cells("ExperienceID").Value.ToString()

                    ' Create a SQL UPDATE statement
                    Dim updateQuery As String = "UPDATE Work_Experience " &
                                                "SET StaffID = @StaffID, OrganizationName = @OrganizationName, " &
                                                "Position = @Position, StartDate = @StartDate, FinishDate = @FinishDate " &
                                                "WHERE ExperienceID = @PrimaryKey"

                    Using connection As New SqlConnection(connectionString)
                        Using command As New SqlCommand(updateQuery, connection)
                            ' Add parameters for the primary key and TextBox values
                            command.Parameters.AddWithValue("@PrimaryKey", primaryKeyValue)
                            command.Parameters.AddWithValue("@StaffID", staffid.Text)
                            command.Parameters.AddWithValue("@OrganizationName", organi.Text)
                            command.Parameters.AddWithValue("@Position", pos.Text)
                            command.Parameters.AddWithValue("@StartDate", startDate.Value)
                            command.Parameters.AddWithValue("@FinishDate", EndDate.Value)

                            ' Open the database connection
                            connection.Open()

                            ' Execute the UPDATE query
                            command.ExecuteNonQuery()

                            ' Close the database connection
                            connection.Close()
                        End Using
                    End Using

                    ' Inform the user that changes have been saved
                    MessageBox.Show("Edited successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Reload the data to reflect the changes
                    WorkExperience_Load(Nothing, Nothing)
                Else
                    ' Inform the user to select only one row for editing
                    MessageBox.Show("Please select only one row to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ' Inform the user that no row is selected
                MessageBox.Show("Please select a row to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            ' Handle any exceptions and show an error message
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub buttonDelete_Click(sender As Object, e As EventArgs) Handles buttonDelete.Click
        Try
            ' Check if any rows are selected in the DataGridView
            If DataGridView1.SelectedRows.Count > 0 Then
                ' Ask the user to confirm the deletion
                Dim result As DialogResult = MessageBox.Show("Do you want to delete the selected row(s)?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    ' Iterate through all selected rows and delete them
                    For Each selectedRow As DataGridViewRow In DataGridView1.SelectedRows
                        ' Retrieve the unique identifier (e.g., a primary key) from the selected row
                        Dim primaryKeyValue As String = selectedRow.Cells("ExperienceID").Value.ToString()

                        ' Create a SQL DELETE statement
                        Dim deleteQuery As String = "DELETE FROM Work_Experience WHERE ExperienceID = @PrimaryKey"

                        ' Create a SqlConnection and SqlCommand
                        Using connection As New SqlConnection(connectionString)
                            Using command As New SqlCommand(deleteQuery, connection)
                                ' Add a parameter for the primary key value
                                command.Parameters.AddWithValue("@PrimaryKey", primaryKeyValue)

                                ' Open the database connection
                                connection.Open()

                                ' Execute the DELETE query
                                command.ExecuteNonQuery()

                                ' Close the database connection
                                connection.Close()
                            End Using
                        End Using

                        ' Remove the selected row from the DataGridView
                        DataGridView1.Rows.Remove(selectedRow)
                    Next

                    MessageBox.Show("Deleted Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    WorkExperience_Load(Nothing, Nothing)
                End If
            Else
                MessageBox.Show("Please select rows to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            ' Handle any exceptions and show an error message
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Check if the clicked cell is not a header cell and a row is selected
        If e.RowIndex >= 0 AndAlso DataGridView1.SelectedRows.Count > 0 Then
            ' Get data from the selected row
            Dim selectedRow = DataGridView1.SelectedRows(0)
            Dim cell1Value As String = selectedRow.Cells(0).Value.ToString() ' Assuming column index 0
            Dim cell2Value As String = selectedRow.Cells(1).Value.ToString()
            Dim cell3Value As String = selectedRow.Cells(2).Value.ToString()
            Dim cell4Value As String = selectedRow.Cells(3).Value.ToString()
            Dim cell5Value As String = selectedRow.Cells(4).Value.ToString()
            Dim cell6Value As String = selectedRow.Cells(5).Value.ToString()
            ' Populate data into text boxes
            exid.Text = cell1Value
            staffid.Text = cell2Value
            organi.Text = cell3Value
            pos.Text = cell4Value

            ' Convert cell5Value to DateTime and set it to the StartDateT DateTimePicker
            If DateTime.TryParse(cell5Value, Nothing) Then
                startDate.Value = DateTime.Parse(cell5Value)
            End If

            ' Convert cell6Value to DateTime and set it to the EndDateT DateTimePicker
            If DateTime.TryParse(cell6Value, Nothing) Then
                EndDate.Value = DateTime.Parse(cell6Value)
            End If
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Dim newFormSChoStaff As ChooseStaffName ' สร้างตัวแปรสำหรับฟอร์มใหม่
        newFormSChoStaff = New ChooseStaffName()
        newFormSChoStaff.ShowDialog()
        staffid.Text = newFormSChoStaff.txtStaffID
    End Sub
End Class