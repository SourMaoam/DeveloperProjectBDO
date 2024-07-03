# Exchange Rate Application

This project is a console and web API application to fetch, store, and display exchange rates using the Fixer API.

## Features

- **Console Application**:
  - Fetch the latest exchange rates and display them.
  - Calculate and display cross rates between specified currencies.
  - View stored exchange rates from the SQLite database.
  - Help menu for available options.

- **Web API**:
  - Endpoint to fetch the latest exchange rates from the Fixer API and update the SQLite database.

## Setup

### Prerequisites

- .NET SDK 8.0 or later
- SQLite

### Configuration

1. **API Key**:
   - Obtain an API key from [Fixer.io](https://fixer.io).
   - Set the environment variable `FIXER_API_KEY` with your API key.

#### For Console Application

- Set the environment variable `FIXER_API_KEY` in your system environment variables.

#### For Web API

- Set the environment variable `FIXER_API_KEY` in the `launchSettings.json` file located in the `Properties` folder of your API project. Example:

  ```json
  {
    "$schema": "http://json.schemastore.org/launchsettings.json",
    "iisSettings": {
      "windowsAuthentication": false,
      "anonymousAuthentication": true,
      "iisExpress": {
        "applicationUrl": "http://localhost:26877",
        "sslPort": 44393
      }
    },
    "profiles": {
      "http": {
        "commandName": "Project",
        "dotnetRunMessages": true,
        "launchBrowser": true,
        "applicationUrl": "http://localhost:5283",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development",
          "FIXER_API_KEY": "your_api_key_here"
        }
      },
      "https": {
        "commandName": "Project",
        "dotnetRunMessages": true,
        "launchBrowser": true,
        "applicationUrl": "https://localhost:7274;http://localhost:5283",
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        }
      },
      "IIS Express": {
        "commandName": "IISExpress",
        "launchBrowser": true,
        "environmentVariables": {
          "ASPNETCORE_ENVIRONMENT": "Development"
        }
      }
    }
  }


## Usage

### Console Application

- **Get Latest Exchange Rates**:
  - Select option `1` to fetch and display the latest exchange rates.

- **Get Cross Rate**:
  - Select option `2` and follow the prompts to calculate the cross rate between two specified currencies.

- **View Database Contents**:
  - Select option `3` to display the stored exchange rates from the SQLite database.

- **Help**:
  - Select option `4` to view the available options.

- **Exit**:
  - Select option `5` to exit the application.

### Web API

- **Update Exchange Rates**:
  - Send a POST request to `http://localhost:5283/api/exchangerate/update` to fetch the latest exchange rates from the Fixer API and update the SQLite database.
