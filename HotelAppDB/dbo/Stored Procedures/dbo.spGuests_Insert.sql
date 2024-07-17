CREATE OR REPLACE FUNCTION spRoomTypes_GuestInsert(
    vFirstName VARCHAR(20),
    vLastName VARCHAR(30),
    vEmail VARCHAR(50),
    vPhone VARCHAR(18
    )
        RETURNS TABLE (
                      Id INT,
                      FirstName VARCHAR(20),
                      LastName VARCHAR(30),
                      Email VARCHAR(50),
                      Phone VARCHAR(18)
                  ) AS $$
BEGIN
    INSERT INTO Guests (FirstName, LastName, Email, Phone)
    VALUES (vFirstName, vLastName, vEmail, vPhone)
    RETURNING Id, FirstName, LastName, Email, Phone;
END;
$$ LANGUAGE plpgsql;
       

   
       
)
--     RETURNS TABLE (
--                       Id INT,
--                       Name VARCHAR(50),
--                       Description VARCHAR(100),
--                       Price MONEY
--                   ) AS $$
-- BEGIN
-- RETURN QUERY
-- SELECT t.Id, t.name, t.description, t.price
-- FROM rooms r
--          INNER JOIN roomtypes t ON t.id = r.roomtypeid
-- WHERE r.id NOT IN (
--     SELECT b.room
--     FROM bookings b
--     WHERE (vStartDate < b.startdate AND vEndDate > b.enddate) OR
--         (b.startdate <= vEndDate AND vEndDate < b.enddate) OR
--         (b.startdate <= vStartDate AND vStartDate < b.enddate)
-- )
-- GROUP BY t.id, t.name, t.description, t.price;
-- END;
-- $$ LANGUAGE plpgsql;