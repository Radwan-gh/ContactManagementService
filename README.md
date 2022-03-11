# Contact Management System

**Brief**

Web API application to manage Extendable Customers data (simple contact management). 

**Requirements**

•	Store data for two entities Companies and Contacts. Every contact can be in multiple companies 
•	Company 
•	ID: auto number 
•	Name: unique text 
•	Contact 
•	ID: auto number
•	Name: unique text
•	Company: The related company (allow multiple)
•	Users can extend the company and contact entities to add as many attributes as they want to customize the entities from types (Text, Number, and Date). For example, Birthdate for Contact
•	Provide APIs for 
•	Add a new attribute to the entities company or contact 
•	Read, Create, Update, Delete, Company Contact. Note, the extended fields should be updated, read, etc.  
•	Filters on any existing field or user extended attribute 
•	 The APIs should be working with at least a million of records without any performance issues. 

API : You can find the documentation on the home page /

**Sulotion Structure** based on [clean architecture](https://github.com/ardalis/CleanArchitecture/) by Ardalis

**Technologies**
Mongodb
.Net Core 6

**Completed Tasks:**
1. Search by contact and company fields/custom attribues
2. Company CRUD
3. Contact CRUD
4. Custom Attributes CRUD
6. Generate random contact related to fixed companies to test Search performance (1 milion record) [link](https://github.com/Radwan-gh/ContactManagementService/blob/main/Generate%20Contacts%20with%20random%20data.js)
7. Enable Swagger

**Not Completed Tasks:**
1. Exception handling
2. API authentication
3. Validating Custom Attribute Types on search

