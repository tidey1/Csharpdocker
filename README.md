# Csharpdocker

currently there is no way to configure this app from docker. the source must be downloaded and changed.

build docker image:
docker build -t tidey1/csharpdocker:1.1 . 

run docker container:
docker run -p 8080:8080 -t tidey1/csharpdocker:1.1

docker run -e MY_VARIABLE=helloworld -p 8080:8080 -t tidey1/csharpdocker:1.3

change appsettings.json for own configuration

"DefaultConnection": "Server=192.168.178.21;Database=dockertest;User=docker;Password=docker;"

1. GET /getAllTables:

    Description: Fetch all tables in the connected MySQL database.

    Method: GET

    Example Response:

     ["table1", "table2", "table3"]

2. POST /createTable/{tableName}:

    Description: Create a new table in the database. The table will have an auto-incrementing primary key and a "Value" column.

    Method: POST

    Example Request: No body (the table name is passed in the URL).

    URL Example: http://localhost:5000/createTable/myNewTable

    Example Response: 

    {
        "message": "Table 'myNewTable' created successfully."
    }

3. POST /insertValue/{tableName}/{value}:

    Description: Insert a value into the specified table.

    Method: POST

    Example Request: No body (value passed in the URL).

    URL Example: http://localhost:5000/insertValue/myNewTable/42

    Example Response:
    {
        "message": "Inserted value 42 into table 'myNewTable'."
    }

4. GET /getValues/{tableName}:

    Description: Retrieve all values from the specified table.

    Method: GET

    URL Example: http://localhost:5000/getValues/myNewTable

    Example Response:

    [42, 10, 17]
