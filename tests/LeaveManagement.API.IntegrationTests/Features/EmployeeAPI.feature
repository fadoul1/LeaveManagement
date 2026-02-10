Feature: Employee API
  As a user of the system
  I want to interact with the Employee API
  So that I can manage employees

Background:
	Given the employee API is configured for testing

Scenario: Retrieve the list of employees
	When the user requests the list of employees
	Then the response should contain a list of employees

Scenario: Retrieve an employee by ID
	Given there is an existing employee with ID 1
	When the user requests the employee with ID 1
	Then the response should contain the employee with ID 1

Scenario: Create a new employee
	When the user creates a new employee with the following details:
		| FirstName | LastName | Email                | PhoneNumber |
		| John      | Doe      | john.doe@example.com | 123456789   |
	Then the response should indicate a successful creation

Scenario: Update an existing employee
	Given there is an existing employee with ID 1
	When the user updates the employee with ID 1 with the following details:
		| FirstName | LastName | Email                     | PhoneNumber |
		| Updated   | Employee | updated.employee@test.com | 987654321   |
	Then the response should indicate a successful update

Scenario: Delete an existing employee
	Given there is an existing employee with ID 1
	When the user deletes the employee with ID 1
	Then the response should indicate a successful deletion
