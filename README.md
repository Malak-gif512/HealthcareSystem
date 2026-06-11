# Healthcare System Core: Enterprise Clinical Backend

## 🚀 Overview
I built this system to solve the fragmentation in clinical data management. By implementing a **Clean Architecture** approach, I successfully decoupled the business logic from infrastructure concerns, ensuring the system remains scalable, secure, and maintainable.

## 🏗 System Structure
<img width="1920" height="1020" alt="System_Architecture" src="https://github.com/user-attachments/assets/19d006ad-dd03-4205-b78b-86b53e6de930" />
*The solution is organized into four distinct layers (Domain, Application, Infrastructure, API) to ensure strict adherence to SOLID principles.*

## 🎥 Live Demo & Testing
Watch a quick demonstration of the system's core capabilities, including automated audit logging and secure clinical data handling:
[**Watch System Demo**](https://drive.google.com/file/d/1LspOlLYUSsmnMJqh6Y14UYnH63fCKnSn/view?usp=drive_link)

Test the live API environment here:
[**API Reference (Scalar)**](https://healthcare-backend-mh-e0bxeqf3hpb6a9ft.uaenorth-01.azurewebsites.net/scalar/v1)

<img width="1920" height="865" alt="API_Interface" src="https://github.com/user-attachments/assets/4eb22bd7-9454-4e1b-85a7-dd6ed19c7679" />
*A clear view of the API documentation interface, providing developers with an interactive environment to test endpoints, manage authentication tokens, and inspect response schemas in real-time.*

## 💡 Key Technical Challenges & Solutions
* **The "Transient Failure" Hurdle**: During cloud deployment on Azure, I encountered transient database connectivity issues. I resolved this by implementing **Entity Framework Core's `EnableRetryOnFailure` policy**, which adds resilience to cloud-hosted databases.
* **Audit Transparency**: I needed a way to track data changes without polluting business services. I solved this by implementing **EF Core Interceptors**, creating an automated, immutable audit trail for every entity change.
* **Deployment Constraints**: Azure’s regional policies initially blocked deployment. By analyzing policy assignments, I successfully migrated the infrastructure to `UAE North`, ensuring optimal latency and compliance.

## 🛠 Tech Stack
* **Core**: .NET 9.0
* **Persistence**: Azure SQL Database (Cloud-native)
* **Security**: JWT-based Authentication with RBAC
* **Observability**: Global Audit Logging & Exception Handling Middlewares

## 📊 Database Insight
Our data is managed with strict integrity. The following capture shows the real-time `AuditLogs` table tracking user operations directly from our production Azure instance:
<img width="1920" height="868" alt="Azure_Database" src="https://github.com/user-attachments/assets/6bd05588-d67f-4994-bc7c-fd018290e514" />
*A real-time snapshot of the AuditLogs table in our Azure SQL instance, demonstrating the immutable tracking of user actions, including timestamps and operations, ensuring full data accountability and integrity.*

## 📝 How to Test
1. **Register**: Use the `/api/Auth/register` endpoint to create an identity.
2. **Authenticate**: Log in to receive your secure `Bearer Token`.
3. **Authorize**: Click the "Authorize" button in the Scalar UI and paste your token.
4. **Explore**: Perform operations on `Patients` or `Appointments` and watch the `AuditLogs` update in real-time.

---
*Developed with a focus on security, scalability, and code cleanliness. Open for feedback and professional discussion.*
