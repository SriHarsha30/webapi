CREATE TRIGGER InsertHistoryOnLeaseUpdate
ON Leases1
AFTER UPDATE
AS
BEGIN
    INSERT INTO History (Tenant_id, Tenant_name, Tenant_Phonenumber, leased_property_id, startTime, endTime)
    SELECT 
        inserted.ID,
        r.Name,
        r.PhoneNumber,
        inserted.Property_Id,
        inserted.StartDate,
        inserted.EndDate
    FROM 
        inserted
    INNER JOIN 
        Registrationss r ON inserted.ID = r.ID
    WHERE 
        inserted.Lease_status = 1;
END