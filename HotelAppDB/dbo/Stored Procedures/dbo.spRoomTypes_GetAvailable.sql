CREATE OR REPLACE FUNCTION spRoomTypes_GetAvailable(
    vStartDate DATE,
    vEndDate DATE
)
    RETURNS TABLE (
                      Id INT,
                      Name VARCHAR(50),
                      Description VARCHAR(100),
                      Price MONEY
                  ) AS $$
BEGIN
    RETURN QUERY
        SELECT t.Id, t.name, t.description, t.price
        FROM rooms r
                 INNER JOIN roomtypes t ON t.id = r.roomtypeid
        WHERE r.id NOT IN (
            SELECT b.room
            FROM bookings b
            WHERE (vStartDate < b.startdate AND vEndDate > b.enddate) OR
                (b.startdate <= vEndDate AND vEndDate < b.enddate) OR
                (b.startdate <= vStartDate AND vStartDate < b.enddate)
        )
        GROUP BY t.id, t.name, t.description, t.price;
END;
$$ LANGUAGE plpgsql;