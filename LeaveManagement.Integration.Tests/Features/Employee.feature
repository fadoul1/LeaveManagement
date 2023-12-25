Feature: Employee

Employee Persistence operations

Background:
	Given there are existing employees in the database

Scenario: Retrieve list of employees
	When the user queries for the list of employees
	Then the system should return a list of employees from the database

Scenario: Retrieve employee by ID
	Given there is an employee with ID 1 in the database
	When the user queries for the employee with ID 1
	Then the system should return the employee with ID 1 from the database

Scenario: Create a new employee
	When the user creates a new employee with details:
		| FirstName | LastName | Email                |
		| John      | Doe      | john.doe@example.com |
	Then the system should successfully create the employee

Scenario: Update an existing employee
	Given there is an existing employee with ID 1 in the database
	When the user updates the employee with ID 1 with details:
		| FirstName | LastName | Email                        |
		| Updated   | Employee | updated.employee@example.com |
	Then the system should successfully update the employee with ID 1

Scenario: Delete an existing employee
	Given there is an employee with ID 4 in the database we want to delete
	When the user deletes the employee with ID 4
	Then the system should successfully delete the employee with ID 4