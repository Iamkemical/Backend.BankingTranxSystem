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
  docker run redis
  docker run --name myredis -d -p 6379:6379 redis
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
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "userId": "4bc1809c-c72f-4b9e-9197-c5a03c5e54b5",
        "requestId": "$2a$11$XkzxX0ZgiJc25d6uqOOLL.pchYrK5u"
    },
    "links": []
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
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "requestId": "$2a$11$XkzxX0ZgiJc25d6uqOOLL.pchYrK5u"
    },
    "links": []
  }
  ```

2. Authenticated Endpoints
   
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
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "transactionReference": "092a4762-918f-453f-9739-da3222d5dc42",
        "amount": 2500,
        "walletReference": "7074153110"
    },
    "links": []
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
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "transactionReference": "092a4762-918f-453f-9739-da3222d5dc42",
        "amount": 2500,
        "walletReference": "7074153110"
    },
    "links": []
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
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "sourceReference": "7074153110",
        "destinationReference": "3326580521"
    },
    "links": []
  }
  ```
- Wallet Transaction History Endpoint: ````{{baseUrl}}/api/v1/wallet/wallet-transaction-history?pageSize=20&pageNumber=1```` (GET)
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "balance": 2600.0000,
        "reference": "7074153110",
        "transactionHistory": [
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 0,
                "narration": "Deposit",
                "amount": 2500.0000,
                "createdAt": "2024-09-10T17:38:17.0568129"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "3326580521",
                "transactionType": 2,
                "narration": "Transfer to Gabriel",
                "amount": 100.0000,
                "createdAt": "2024-09-10T17:32:53.8548507"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "3326580521",
                "transactionType": 2,
                "narration": "Transfer to Gabriel",
                "amount": 300.0000,
                "createdAt": "2024-09-10T17:29:44.8160942"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "3326580521",
                "transactionType": 2,
                "narration": "Transfer to Gabriel",
                "amount": 300.0000,
                "createdAt": "2024-09-10T17:28:01.009095"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 1,
                "narration": "Withdrawal",
                "amount": 500.0000,
                "createdAt": "2024-09-10T17:11:08.3880503"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 1,
                "narration": "Withdrawal",
                "amount": 2000.0000,
                "createdAt": "2024-09-10T17:07:06.8819159"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 0,
                "narration": "Deposit",
                "amount": 1000.0000,
                "createdAt": "2024-09-10T17:06:40.838957"
            }
        ],
        "totalCount": 7,
        "totalPages": 1,
        "pageNumber": 1,
        "pageSize": 20,
        "paginationData": "{\"totalCount\":7,\"pageSize\":20,\"currentPage\":1,\"totalPages\":1}"
    },
    "links": []
  }
  ```

- Wallet Monthly Statement Generation Endpoint: ````{{baseUrl}}/api/v1/wallet/wallet-transaction-history?pageSize=20&pageNumber=1&isMonthlyStatement=1&month=9```` (GET)
  Response
  ```
  {
    "status": "000",
    "message": "Successful",
    "data": {
        "balance": 2600.0000,
        "reference": "7074153110",
        "transactionHistory": [
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 0,
                "narration": "Deposit",
                "amount": 2500.0000,
                "createdAt": "2024-09-10T17:38:17.0568129"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "3326580521",
                "transactionType": 2,
                "narration": "Transfer to Gabriel",
                "amount": 100.0000,
                "createdAt": "2024-09-10T17:32:53.8548507"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "3326580521",
                "transactionType": 2,
                "narration": "Transfer to Gabriel",
                "amount": 300.0000,
                "createdAt": "2024-09-10T17:29:44.8160942"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "3326580521",
                "transactionType": 2,
                "narration": "Transfer to Gabriel",
                "amount": 300.0000,
                "createdAt": "2024-09-10T17:28:01.009095"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 1,
                "narration": "Withdrawal",
                "amount": 500.0000,
                "createdAt": "2024-09-10T17:11:08.3880503"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 1,
                "narration": "Withdrawal",
                "amount": 2000.0000,
                "createdAt": "2024-09-10T17:07:06.8819159"
            },
            {
                "sourceReference": "7074153110",
                "destinationReference": "0467256446",
                "transactionType": 0,
                "narration": "Deposit",
                "amount": 1000.0000,
                "createdAt": "2024-09-10T17:06:40.838957"
            }
        ],
        "totalCount": 7,
        "totalPages": 1,
        "pageNumber": 1,
        "pageSize": 20,
        "paginationData": "{\"totalCount\":7,\"pageSize\":20,\"currentPage\":1,\"totalPages\":1}"
    },
    "links": []
  }
  ```
