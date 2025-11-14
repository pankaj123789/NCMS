NCMS Profile Soft-Delete Feature
================================

1\. Overview
------------

This feature introduces a non-destructive "soft-delete" mechanism for user profiles. Instead of permanently deleting a user from the database, this feature marks a user as IsDeleted.

This approach cleans up the application UI by hiding inactive or test profiles while preserving all historical data for auditing, compliance, and potential future restoration.

2\. Key Features
----------------

*   **New API Endpoint:** A new endpoint (e.g., api/person/softdelete) is available to trigger a soft-delete.
    
*   **Global Invisibility:** Once a profile is deleted, it is **automatically** hidden from all application-wide searches, lists, and filters.
    
*   **Data Preservation:** No data is ever permanently lost. The IsDeleted flag can be reversed in the database to restore a profile.
    

3\. Technical Documentation
---------------------------

For a detailed, layer-by-layer breakdown of the technical implementation, including file-by-file changes and architectural decisions, please see the full documentation:

➡️ [**View the Full Technical Implementation Document**](./SOFT_DELETE_IMPLEMENTATION.md)