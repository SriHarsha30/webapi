CREATE TRIGGER UpdatePropertyAvailabilityOnLeaseStatus
ON Leases1
AFTER UPDATE
AS
BEGIN
    -- Check if the Lease_status is updated to TRUE
    IF EXISTS (SELECT 1 FROM inserted WHERE Lease_status = 'true')
    BEGIN
        -- Update the Property table's AvailableStatus to FALSE
        UPDATE Properties
        SET AvailableStatus = 'false'
        WHERE Property_Id IN (
            SELECT Property_Id
            FROM inserted
            WHERE Lease_status = 'true'
        );
    END
END;
-----------------------------------------------------------------------------------------------------