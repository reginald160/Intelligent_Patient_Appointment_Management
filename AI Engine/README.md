# Django Python Web API Project

This project is the IPAM AI Engine, a Web API built with python and Django Rest Framework.

### 1. Clone the Repository

Start by cloning the project repository:


$ git clone https://github.com/reginald160/Intelligent_Patient_Appointment_Management-
$ cd your-repo

## Local Setup

### Prerequisites

- Python 3.8 or higher
- pip (Python package manager)

### Creating a Virtual Environment

1. Open a terminal in your project directory.
2. Create a virtual environment:
   ```
   python -m venv venv
   ```
3. Activate the virtual environment:
   - On Windows:
     ```
     venv\Scripts\activate
     ```
   - On macOS and Linux:
     ```
     source venv/bin/activate
     ```

### Installing Dependencies

1. Ensure your virtual environment is activated.
2. Install the required packages:
   ```
   pip install -r requirements.txt
   ```

### Running the Project Locally

1. Apply database migrations:
   ```
   python manage.py migrate
   ```
2. Create a superuser (optional):
   ```
   python manage.py createsuperuser
   ```
3. Start the development server:
   ```
   python manage.py runserver
   ```
4. Access the API at `http://127.0.0.1:8000/`

## Azure Deployment

To deploy this Django project to Azure App Service, follow these steps:

1. Create an Azure App Service:
   - Log in to the [Azure Portal](https://portal.azure.com/).
   - Create a new Web App resource.
   - Choose Python as the runtime stack.

2. Configure App Settings:
   - In the Azure Portal, go to your App Service.
   - Navigate to Configuration > App settings.
   - Add the following settings:
     - `SCM_DO_BUILD_DURING_DEPLOYMENT`: Set to `true`
     - `PYTHON_VERSION`: Set to your project's Python version (e.g., `3.8`)

3. Deploy Your Code:
   - Install the Azure CLI and log in:
     ```
     az login
     ```
   - Deploy your code:
     ```
     az webapp up --name <your-app-name> --runtime "PYTHON|3.8" --sku B1
     ```

4. Configure Database (if applicable):
   - If using a database, set up an Azure Database for PostgreSQL.
   - Update your `settings.py` to use the Azure database connection string.

5. Apply Migrations:
   - In the Azure Portal, open the Console for your App Service.
   - Run:
     ```
     python manage.py migrate
     ```

6. Create a Superuser (optional):
   - In the Azure Console, run:
     ```
     python manage.py createsuperuser
     ```

The Django API should now be accessible at `https://<your-app-name>.azurewebsites.net/`.

## API Documentation

(Include details about your API endpoints, request/response formats, and any authentication requirements)

## Running Tests

To run the test suite:

```
python manage.py test
```

## Contributing

(Include guidelines for contributing to your project)

## License

This project is licensed under York St. John University London
