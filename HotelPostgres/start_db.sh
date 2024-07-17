#!/bin/bash

# Set the database connection string
DB_CONNECTION_STRING="jdbc:postgresql://localhost:5432/postgres"

# Run the SQL scripts to create your database and tables
psql $DB_CONNECTION_STRING -f ./dbo/Tables/RoomTypes.sql
psql $DB_CONNECTION_STRING -f ./dbo/Tables/Rooms.sql
psql $DB_CONNECTION_STRING -f ./dbo/Tables/Guests.sql
psql $DB_CONNECTION_STRING -f ./dbo/Tables/Bookings.sql

echo "Database initialized successfully."