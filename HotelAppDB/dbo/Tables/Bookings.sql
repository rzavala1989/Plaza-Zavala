CREATE TABLE Bookings(
    Id INT NOT NULL PRIMARY KEY,
    Room INT NOT NULL
         CONSTRAINT fk_Bookings_Rooms
          REFERENCES Rooms(Id),
    Guest INT NOT NULL
            CONSTRAINT fk_Bookings_Guests
             REFERENCES Guests(Id),
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsCheckedIn BOOLEAN NOT NULL,
    TotalCost MONEY NOT NULL
)