# ASP.NET Core MVC Project

This project is an ASP.NET Core MVC application configured to run locally on `https://localhost:44334/`. The system settings are managed via `appsettings.json`.

## Features

- ASP.NET Core MVC framework
- HTTPS enabled on `https://localhost:44334/`
- Configurable system settings via `appsettings.json`
- Separate deployment instructions for IIS and Azure

---

## Prerequisites

Before running or deploying this project, ensure you have the following tools installed:

- **.NET Core SDK**: [Download .NET Core SDK](https://dotnet.microsoft.com/download)
- **Visual Studio**: [Download Visual Studio](https://visualstudio.microsoft.com/vs/)
- **IIS (for deployment)**: Install IIS on Windows Server if deploying to IIS.
- **Azure Account**: Required for Azure deployment.

---

## Getting Started

### 1. Clone the Repository

Start by cloning the project repository:


$ git clone https://github.com/reginald160/Intelligent_Patient_Appointment_Management
$ cd your-repo

# Run Locally
1. **Open the project in Visual Studio**.
2. **Build the solution by pressing Ctrl + Shift + B.**
3. **Run the project by pressing F5 or clicking Run.**
4. **The project will now be running at https://localhost:44334**.


# Deployment Instructions

## 1. Deploying to IIS (Internet Information Services)

To deploy the project to IIS on a Windows Server:

### Install IIS and .NET Core Hosting Bundle

1. **Install IIS**:
   * Open **Server Manager** on your Windows Server.
   * Add the **Web Server (IIS)** role.

2. **Install the .NET Core Hosting Bundle**:
   * Download the .NET Core Hosting Bundle from [here](https://dotnet.microsoft.com/download/dotnet-core).
   * Install the bundle on your Windows Server.

### Publish the Project

1. In Visual Studio, right-click on the project and select **Publish**.
2. Choose **Folder** as the target location for publishing.
3. Once the publishing process is complete, copy the contents of the output folder to your server.

### Configure IIS

1. Open **IIS Manager**.
2. Right-click on **Sites** and select **Add Website**.
3. Set the physical path to the folder where you published your project.
4. Bind the website to port `44334` (or another available port if necessary) for HTTPS.
5. Ensure the **application pool** is set to:
   * **.NET CLR version**: No Managed Code
   * **Managed Pipeline Mode**: Integrated
6. Restart IIS and browse to your site at `https://your-server:44334/`.

## 2. Deploying to Azure

To deploy the project to **Azure App Service**, follow these steps:

### Create an App Service on Azure

1. Log in to the **Azure Portal** at [portal.azure.com](https://portal.azure.com).
2. Navigate to **App Services** and click on **Create**.
3. Choose **.NET Core** as the runtime stack.
4. Configure the required resource group, app name, and region.

### Publish to Azure from Visual Studio

1. In Visual Studio, right-click on the project and select **Publish**.
2. Choose **Azure App Service** and select **Create New** if you haven't already created an app service.
3. Select the **App Service** you created on Azure, then click **Publish**.

Once the publish process is completed, your application will be live on the Azure-provided URL.

## Troubleshooting

* **Connection Strings**: Ensure that the connection strings in `appsettings.json` are correct.
* **IIS Issues**: If you encounter issues while deploying on IIS, make sure the `.NET Core Hosting Bundle` is installed.
* **Azure Errors**: Ensure that the App Service has the correct .NET Core runtime version.

## License

This project is licensed under York St. John University London
