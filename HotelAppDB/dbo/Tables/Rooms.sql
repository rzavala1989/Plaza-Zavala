CREATE TABLE Rooms (
    Id INT NOT NULL PRIMARY KEY ,
    RoomNumber VARCHAR(6) NOT NULL,
    RoomTypeId INT NOT NULL
                  CONSTRAINT fk_Rooms_RoomTypes_Id
                   REFERENCES RoomTypes(Id)
);