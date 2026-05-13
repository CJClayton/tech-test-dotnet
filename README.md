### ClearBank Developer Test


#### Overview


This refactor focused on improving the readability, testability, and maintainability of the existing payment processing logic while preserving the original behaviour of the system.

I treated this as a refactoring exercise rather than a redesign exercise, so my priority was to improve the structure of the code without introducing unnecessary behavioural changes.

The `MakePayment` method signature was intentionally left unchanged as required by the exercise.



#### Approach


I approached the refactor incrementally:

1. Introduced an abstraction around account persistence to improve separation of concerns and make the payment logic testable.
2. Added characterisation tests to validate my understanding of the existing behaviour before making structural changes.
3. Refactored the payment validation logic into smaller, intention-revealing methods.
4. Preserved existing behaviour throughout the refactor and validated changes using unit tests.

Given the financial nature of the domain, I prioritised safe, behaviour-preserving refactoring over larger architectural redesigns.



#### Changes Made


##### Introduced `IAccountRepository`


`PaymentService` originally instantiated concrete datastore implementations directly, which tightly coupled business logic to persistence concerns and made unit testing difficult.

To improve testability and separation of concerns I introduced:

* `IAccountRepository`
* `AccountRepository`

This allowed the payment logic to be tested independently from the underlying datastore implementations.



##### Simplified Payment Validation Logic


The original implementation contained repeated conditional logic inside a large `switch` statement.

This was refactored into smaller validation methods:

* `CanProcessBacs`
* `CanProcessFasterPayments`
* `CanProcessChaps`

This made the payment rules easier to read and reason about while preserving the original behaviour.



##### Added Unit Tests


I added unit tests covering:

* Successful payment scenarios
* Failure scenarios
* Balance validation
* Account status validation
* Persistence behaviour
* Existing edge-case behaviour

The tests were designed primarily as characterisation tests to protect the refactor and verify that behaviour remained unchanged.



#### Existing Edge-Case Behaviour


During the refactor I identified several behaviours in the original implementation that may warrant further discussion in a production system:

* Unsupported `PaymentScheme` values currently succeed
* Negative payment amounts currently increase the account balance
* `MakePayment` dereferences the request before validation and will throw if the request is null

I intentionally preserved these behaviours because the exercise requested refactoring without changing the underlying logic.

In a real-world environment I would clarify the intended behaviour with product and domain stakeholders before introducing semantic changes.



#### Trade-offs / Further Improvements


If I had more time I would consider:

* Reducing duplication within `AccountRepository`
* Introducing explicit request validation
* Reviewing edge-case handling around invalid payment amounts and unsupported payment schemes
* Potentially extracting payment validation into separate strategy classes if the number of payment schemes or business rules continued to grow

I considered introducing additional abstraction during the refactor, but chose to keep the solution intentionally lightweight to avoid adding unnecessary complexity for the current size of the codebase.



#### Running the Tests


```bash

dotnet test

