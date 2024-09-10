# Banking Transaction System

### Description
Banking Transaction System Backend Solution

### Built with
- .NET 8
- C#
- Redis
- Seq
- MSSQL

### Requirements
- Install [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- Setup Seq on a docker container by pulling the image from Docker Hub
  ```
  docker run datalust/seq
  docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
  ```
- Setup Redis on a docker container by pulling the image from the Docker Hub
  ```
  docker run redis:alpine3.20
  docker run --name some-redis -d redis
  ```
- Update/Apply Migration after building the project on Visual Studio
  ```
  Update-Database
  ```

### Documentation
The project has 5 endpoints namely

1. UnAuthenticated Endpoints
- Create User Endpoint: ````{{baseUrl}}/api/v1/user```` (POST)
  
  Request
  ```
  {
     "firstName": "Chidinma",
     "lastName": "Solomon",
     "otherNames": "Sandra",
     "emailAddress": "sandrasolomon625@gmail.com",
     "password": "SandraSolomon1@",
     "dateOfBirth": "1998-05-15",
     "permanentAddress": "Eboh's Lodge, Ikot-Udoro Street, Off Ikot-Ekepene Road, Uyo",
     "telephoneNumber": "2349040167727",
     "bvn": "2320328034",
     "country": "Nigeria",
     "state": "Akwa-Ibom",
     "gender": 2,
     "accountType": 1
  }
  ```
- Login User Endpoint: ````{{baseUrl}}/api/v1/user/login```` (POST)

  Request
  ```
  {
     "emailAddress": "sandrasolomon625@gmail.com",
     "password": "SandraSolomon1@"
  }
  ```

2. Authorized Endpoints
   
   Add 'X-API-KEY' and 'X-REQ-ID' to the header of all requests here

   The X-API-KEY is on the appSettings.json config file
   
   The X-REQ-ID can be gotten from the response of ````{{baseUrl}}/api/v1/user/login````

- Wallet Deposit Endpoint: ````{{baseUrl}}/api/v1/wallet/process-wallet-transaction```` (POST)

  Request
  ```
  {
     "amount": 2500,
     "narration": "Deposit",
     "transactionType": 0
  }
  ```
- Wallet Withdrawal Endpoint: ````{{baseUrl}}/api/v1/wallet/process-wallet-transaction```` (POST)

  Request
  ```
  {
     "amount": 2500,
     "narration": "Withdrawal",
     "transactionType": 1
  }
  ```
- Wallet To Wallet Transfer Endpoint: ````{{baseUrl}}/api/v1/wallet/wallet-to-wallet-transfer```` (POST)

  Request
  ```
  {
     "destinationReference": "3326580521",
     "amount": 100,
     "narration": "Transfer to Gabriel"
  }
  ```
- Wallet Transaction History Endpoint: ````{{baseUrl}}/api/v1/wallet/wallet-transaction-history?pageSize=20&pageNumber=1```` (GET)

- Wallet Monthly Statement Generation Endpoint: ````{{baseUrl}}/api/v1/wallet/wallet-transaction-history?pageSize=20&pageNumber=1&isMonthlyStatement=1&month=9```` (GET)
