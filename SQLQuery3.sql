DECLARE @IsLeaseConfirmed BIT;
EXEC CheckLeaseStatus @Tenant_Id = 'T_102', @PropertyId = 1, @IsLeaseConfirmed = @IsLeaseConfirmed OUTPUT;
SELECT @IsLeaseConfirmed AS IsLeaseConfirmed;