Feature: Account operations
	Tests account functionalities for Bank API

Scenario: Check account balance
	Given an account 'ACC0123' exists with a balance of 1000
	When I check the account balance
	Then the balance should be 1000

Scenario: Deposit money
	Given an account 'ACC0123' exists with a balance of 1000
	And I've made a deposit of 100
	When I check the account balance
	Then the balance should be 1100

Scenario: Withdraw money
	Given an account 'ACC0123' exists with a balance of 1000
	And I've made a withdrawal of 100
	When I check the account balance
	Then the balance should be 900

Scenario: Several transaction
	Given an account 'ACC0123' exists with a balance of 1000
	And I've made the following transactions
	| Type       | Amount |
	| Deposit    | 100    |
	| Withdrawal | 200    |
	| Deposit    | 300    |
	| Deposit    | 100    |
	| Withdrawal | 500    |
	| Withdrawal | 200    |
	When I check the account balance
	Then the balance should be 600

Scenario: Get account statement
	Given an account 'ACC0123' exists with a balance of 1000
	And I've made the following transactions
	| Type       | Amount |
	| Deposit    | 100    |
	| Withdrawal | 200    |
	| Deposit    | 300    |
	| Deposit    | 100    |
	| Withdrawal | 500    |
	| Withdrawal | 200    |
	When I get the account statement
	Then I get the following transactions back
	| Type       | Amount | Balance |
	| Deposit    | 100    | 1100    |
	| Withdrawal | 200    | 900     |
	| Deposit    | 300    | 1200    |
	| Deposit    | 100    | 1300    |
	| Withdrawal | 500    | 800     |
	| Withdrawal | 200    | 600     |