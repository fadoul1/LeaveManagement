Feature: Leave API
  As a user of the system
  I want to interact with the Leave API
  So that I can manage leaves

Background:
	Given the employee API is configured for testing

Scenario: Retrieve the list of leaves
	Given there are existing leaves
	When the user requests the list of leaves
	Then the response should contain a list of leaves

Scenario: Retrieve leaves for an employee by ID
	Given there is an existing employee with ID 1
	When the user requests the leaves for employee with ID 1
	Then the response should contain leaves for employee with ID 1

Scenario: Create a new leave
	Given there is an existing employee with ID 1
	When the user creates a new leave with the following details:
		| Type        | Status      | StartDate   | EndDate     | Reason   | EmployeeId |
		| AnnualLeave | InProgress  | 2025-12-20  | 2025-12-24 | Vacation | 1          |
	Then the response should indicate a successful leave creation
