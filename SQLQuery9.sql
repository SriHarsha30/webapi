CREATE PROCEDURE CheckLeaseStatus
    @Tenant_Id nvarchar(25),
    @PropertyId INT,
    @IsLeaseConfirmed BIT OUTPUT
AS
BEGIN
    -- Check if the lease is confirmed by the owner
    SELECT @IsLeaseConfirmed = CASE WHEN L.Owner_Signature = 'true'  THEN 1 ELSE 0 END
    FROM Leases1 AS L
    WHERE L.Property_Id = @PropertyId AND L.ID = @Tenant_Id;
END;


