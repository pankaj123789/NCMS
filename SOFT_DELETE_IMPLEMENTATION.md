## Technical Implementation: NCMS Profile Soft-Delete (DevTask17)
This document outlines the implementation of the Profile Soft-Delete feature (DevTask17) for the NCMS backend.

### 1. 🎯 Overview & Objectives

**The Problem:** The NCMS application lacked a method for deleting user profiles. This resulted in data clutter from test accounts, duplicate entries, and inactive users, which impacted data accuracy and operational efficiency.

**The Solution:** I implemented a **soft-delete** mechanism. Instead of permanently destroying a record (a "hard delete"), this approach marks a profile as "deleted" by setting a flag in the database.

**My Core Objectives were:**
* **Preserve Data:** Ensure no data is ever permanently lost, maintaining historical integrity.
* **Ensure Consistency:** Make "deleted" profiles globally invisible. They must *never* appear in search results, dropdown lists, or reports.
* **Establish Auditing:** Lay the groundwork for auditing by tracking *who* deleted a profile and *when*.

---

### 2. 🏛️ Architectural Approach: The "Why"

I chose a specific, layered approach to guarantee consistency and maintainability.

* **Why Soft-Delete (vs. Hard-Delete)?**
    A hard delete (`DELETE FROM tblPerson...`) is destructive and irreversible. Soft-delete is superior because it's non-destructive. We retain the profile's history, which is essential for compliance, auditing, and potential future "undo" or restoration functionality.

* **Why a New `BaseEntity`?**
    I created `BaseEntity.cs` because the concept of being "deletable" or "auditable" is not unique to a `Person`. By creating a base class with properties like `IsDeleted` and `DeletedOn`, I've made it simple to apply this same logic to *other* entities in the future (e.g., `Applications`, `Credentials`) by just inheriting from it. This follows the **DRY (Don't Repeat Yourself)** principle.

* **Why a Global NHibernate Filter?**
    To ensure deleted users *never* appear, I had two options:
    1.  **Manual:** Add `.Where(p => !p.IsDeleted)` to every single database query (e.g., in `SearchPeople`, `GetPersonById`, `GetPersonList`).
    2.  **Automatic:** Tell the database "rulebook" (the ORM) to *always* apply this filter.

    The manual approach is a "bug-in-waiting"—I would inevitably miss one. I chose the **automatic** approach by adding `mapping.Where("IsDeleted = 0")` to the `PersonMap.cs`. This is the most crucial part of the implementation, as it enforces the business rule at the lowest level, making it impossible for the application to fetch a deleted record accidentally.

---

### 3. ⚙️ Implementation Details: The "What" & "How"

I implemented this feature by modifying each layer of the application.

#### Layer 1: Database & Entity Foundation
* **What I did:** I defined the new data structure for soft-delete.
* **Files:** [`BaseEntity.cs`](./F1Solutions.Naati.Common.Dal.Domain/BaseEntity.cs) (New), [`Person.cs`](./F1Solutions.Naati.Common.Dal.Domain/Person.cs)
* **How:**
    1.  I created the new `BaseEntity.cs` file to hold the shared soft-delete properties.
    2.  I added three new properties to this base class: `IsDeleted` (the `bool` flag), `DeletedOn` (a `DateTime?` for the timestamp), and `DeletedBy` (a `string` for the admin's username).
    3.  I changed the `Person` class to inherit from this new `BaseEntity`: (`public class Person : BaseEntity`).
* **Effect:** This change meant the `tblPerson` database table needed to be updated (via a migration) to include the three new columns: **IsDeleted, DeletedOn,** and **DeletedBy**.

#### Layer 2: Data Access Mapping (The "Rulebook")
* **What I did:** I taught NHibernate (our ORM) how to handle these new columns and, most importantly, how to filter them.
* **File:** [`PersonMap.cs`](./F1Solutions.Naati.Common.Dal.Nhibernate.Mappings/Mappings/PersonMap.cs)
* **How:**
    1.  **Column Mapping:** I added `mapping.Map(...)` entries for `IsDeleted`, `DeletedOn`, and `DeletedBy` to link the C# properties to their SQL column names and set constraints (like `IsDeleted` cannot be null).
    2.  **Global Filter:** I added one critical line: `mapping.Where("IsDeleted = 0");`.
* **What This Does:** This line instructs NHibernate to **automatically and permanently add `WHERE IsDeleted = 0` to every single SQL query** it generates for the `tblPerson` table. This is the core of the solution for hiding deleted profiles.

#### Layer 3: Data Access Repository (The "Worker")
* **What I did:** I implemented the logic to perform the *actual* database "write".
* **File:** [`PersonRepository.cs`](./F1Solutions.Naati.Common.Dal/Portal/Repositories/PersonRepository.cs)
* **How:** I created a `SoftDelete` method. When called by the `PersonService`, this method:
    1.  Takes the `Person` object and the `deletedBy` user as arguments.
    2.  Sets the entity's properties: `person.IsDeleted = true`, `person.DeletedOn = DateTime.UtcNow`, `person.DeletedBy = deletedBy`.
    3.  Calls `_session.Update(person)` to save these changes to the database.

#### Layer 4: Business Logic (The "Brain")
* **What I did:** I created the central "brain" to orchestrate the delete operation.
* **Files:** [`IPersonService.cs`](./Ncms.Contracts/IPersonService.cs), [`PersonService.cs`](./Ncms.Bl/PersonService.cs)
* **How:**
    1.  I added the `SoftDeletePerson` function to the `IPersonService` interface.
    2.  In `PersonService.cs`, I injected the `ILogger` for developer-facing logs.
    3.  I implemented the `SoftDeletePerson` method. Its job is to:
        * **Log** the attempt.
        * **Validate** the incoming request (e.g., is `PersonId` valid?).
        * **Fetch** the `Person` object using the repository.
        * **Call** the `_personRepository.SoftDelete(...)` method.
        * **Log** the success or failure and return a structured `GenericResponse` to the controller.

#### Layer 5: API & DTOs (The "Front Door")
* **What I did:** I exposed this new functionality as a public API endpoint.
* **Files:** [`PersonController.cs`](./Ncms.Ui/Controllers/Api/PersonController.cs), [`DeletePersonRequestModel.cs`](./Ncms.Contracts/Models/Person/DeletePersonRequestModel.cs) (New)
* **How:**
    1.  I created `DeletePersonRequestModel.cs`. This is a "Data Transfer Object" (DTO) whose only job is to safely carry the `PersonId` and `DeletedBy` username from the frontend to the API.
    2.  In `PersonController.cs`, I added a new endpoint (e.g., `[HttpPost, "softdelete"]`). This endpoint:
        * Accepts the `DeletePersonRequestModel` from the request body.
        * Calls `_personService.SoftDeletePerson(...)`.
        * Returns the result (e.g., `Ok` or `BadRequest`) to the frontend.

#### Layer 6: Data Transfer Models
* **What I did:** I updated our "internal" DTOs to be aware of the new fields.
* **Files:** [`PersonModel.cs`](./Ncms.Contracts/Models/Person/PersonModel.cs), [`PersonEntityDto.cs`](./F1Solutions.Naati.Common.Contracts/Dal/DTO/PersonEntityDto.cs), [`PersonProfile.cs`](./Ncms.Bl/AutoMappingProfiles/PersonProfile.cs)
* **How:** I added the `IsDeleted`, `DeletedOn`, and `DeletedBy` properties to these models and updated the `PersonProfile` (our AutoMapper config) to map them. This ensures that if any *other* part of the application needs to know this information, the data can flow correctly from the entity to the model.

---

### 4. 🏁 Key Outcomes & Next Steps

* **Outcome:** The soft-delete feature is fully implemented in the backend. Any call to the `SoftDeletePerson` endpoint will correctly flag a user, and that user will immediately and automatically be excluded from all application queries.
* **Next Steps:**
    * **UI Implementation:** The frontend team must now create the "Delete" button and call this new endpoint.
    * **Restore Feature:** We have not yet built a feature to "un-delete" a user, but the soft-delete architecture makes this a simple task (just flipping the `IsDeleted` flag back to `false`).

---

**Author:** Pankaj Kushwaha

**Date:** 2025-11-14