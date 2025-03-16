﻿CREATE PROCEDURE CheckPropertyAndFetchOwner
    @PropertyId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Properties WHERE Property_Id = @PropertyId)
    BEGIN
        SELECT Owner_Id 
        FROM Properties
        WHERE Property_Id = @PropertyId;
    END
    ELSE
    BEGIN
        SELECT NULL AS Owner_Id;
    END
END;


------------------------
CREATE PROCEDURE CheckTenantAndProperty
    @Tenant_Id VARCHAR(MAX),
    @Property_Id INT
AS
BEGIN
    SET NOCOUNT ON;
 
    -- Check if the Tenant_Id exists in the Registration table
    IF NOT EXISTS (SELECT 1 FROM Registrationss WHERE ID = @Tenant_Id)
    BEGIN
        RAISERROR('Tenant does not exist.', 16, 1);
        RETURN;
    END

 
    -- Check if the Property_Id exists in the Props table
    IF NOT EXISTS (SELECT 1 FROM Properties WHERE Property_Id = @Property_Id)
    BEGIN
        RAISERROR('Property does not exist.', 16, 1);
        RETURN;
    END
 
    -- If both checks pass, return success
    SELECT 'Both Tenant and Property exist.' AS Message;
END
------------------------------------------

CREATE PROCEDURE GenerateNewSignature
 
    @Id NVARCHAR(50),
 
    @PhoneNumber NVARCHAR(50),
 
    @newSignature NVARCHAR(50) OUTPUT
 
AS
 
BEGIN
 
    DECLARE @randomString NVARCHAR(6)
 
    DECLARE @tempSignature NVARCHAR(50)
 
    DECLARE @chars NVARCHAR(62) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
 
    DECLARE @idx INT
 
    -- Generate a random string
 
    SET @randomString = ''
 
    WHILE LEN(@randomString) < 6
 
    BEGIN
 
        -- Generate a random integer using CHECKSUM and NEWID
 
        SET @idx = ABS(CHECKSUM(NEWID())) % 62 + 1
 
        SET @randomString = @randomString + SUBSTRING(@chars, @idx, 1)
 
    END
 
    -- Generate the signature
 
    SET @tempSignature = CONCAT(SUBSTRING(@Id, 1, 1), SUBSTRING(@PhoneNumber, 1, 3), @randomString)
 
    -- Return the new signature
 
    SET @newSignature = @tempSignature
 
END
-----------------------------------------------------------------------
CREATE PROCEDURE GetOwnerDetailsByOwnerId
    @Owner_Id NVARCHAR(50),
    @Owner_Name NVARCHAR(100) OUTPUT,
    @Owner_PhoneNumber NVARCHAR(15) OUTPUT
AS
BEGIN
    SELECT 
        @Owner_Name = r.Name,
        @Owner_PhoneNumber = r.PhoneNumber
    FROM 
        Registrationss r
    WHERE 
        r.ID = @Owner_Id AND r.RoleofUser = 'o';
END
------------------------------------------------------------------------------

CREATE PROCEDURE GetPaymentsByOwnerId
    @Owner_Id VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
 
    SELECT p.Tenant_Id, p.Amount, p.PaymentDate, p.Status, p.Ownerstatus, pr.Owner_Id,p.PaymentID,p.PropertyId
    FROM Payments AS p
    INNER JOIN Properties AS pr ON p.PropertyId = pr.Property_Id
    WHERE pr.Owner_Id = @Owner_Id;
END
----------------------------------------------------------------------------------

CREATE PROCEDURE GetPaymentsByTenantId
    @Tenant_Id VARCHAR(MAX)
AS
BEGIN

 
    SELECT PaymentID, Tenant_Id, PropertyId, Amount,PaymentDate,Status, Ownerstatus
    FROM Payments
    WHERE Tenant_Id = @Tenant_Id;
END
-------------------------------------------------------------------------------------
CREATE PROCEDURE GetPropertiesByOwner
    @OwnerId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
 
    -- Fetch properties and their related registration data for the given OwnerId
    SELECT 
        p.Property_Id,
        p.Owner_Id,
		p.Address,
		p.Description,
		p.AvailableStatus,
        r.ID,
        r.Signature
    FROM Properties p
    INNER JOIN Registrationss r ON p.Owner_Id = r.ID
    WHERE p.Owner_Id = @OwnerId;
END

------------------------------------------------------------------------------------------

CREATE proc InsertIntoNotificcation1
	@sendersId varchar(20),
	@receiversId varchar(20),
	@notification_Descpirtion varchar(250)
as
begin
	insert into notifications1 (sendersId,receiversId, CreatedDate, notification_Descpirtion) values(@sendersId, @receiversId, GETDATE(), @notification_Descpirtion);
	print 'inserted message to notification';
end
--------------------------------------------------------------------------------------------
CREATE PROCEDURE sp_GetMaintainancesByOwnerId

@OwnerId VARCHAR(50)

AS

BEGIN

    SELECT m.RequestId,m.PropertyId, m.TenantId, m.Description, m.Status,m.ImagePath

    FROM Maintainances m

    INNER JOIN Properties p ON m.PropertyId = p.Property_Id

    WHERE p.Owner_Id = @OwnerId AND m.Status COLLATE Latin1_General_CI_AS = 'pending';

END
-----------------------------------------------------------------------------------------------
CREATE PROCEDURE UpdateID
    @ID NVARCHAR(50),
    @RoleofUser NVARCHAR(50)
AS
BEGIN
    IF @RoleofUser = 'o'
    BEGIN
        UPDATE Registrationss
        SET ID = 'O_' + @ID
        WHERE ID = @ID;
    END
    ELSE IF @RoleofUser = 't'
    BEGIN
        UPDATE Registrationss
        SET ID = 'T_' + @ID
        WHERE ID = @ID;
    END
END
-------------------------------------------------------------------------------------------------
CREATE PROCEDURE UpdatePaymentStatus
    @PaymentID INT,
    @OwnerStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
 
    -- Update the Ownerstatus and Status based on the OwnerStatus value
    UPDATE Payments
    SET Ownerstatus = @OwnerStatus,
        Status = CASE
                    WHEN @OwnerStatus = 'true' THEN 'true'
                    WHEN @OwnerStatus = 'false' THEN 'false'
                    ELSE Status
                 END
    WHERE PaymentID = @PaymentID;
 
    -- Check if the update was successful
    IF @@ROWCOUNT = 0
    BEGIN
        -- If no rows were affected, return an error message
        RAISERROR('Payment not found or no changes made.', 16, 1);
    END
END
-----------------------------------------------------------------------------
CREATE TRIGGER trgAfterInsertRegistration
ON dbo.Registrationss
AFTER INSERT
AS
BEGIN
    DECLARE @Id NVARCHAR(50)
    DECLARE @PhoneNumber NVARCHAR(50)
    DECLARE @newSignature NVARCHAR(50)
    -- Get the inserted values
    SELECT @Id = ID, @PhoneNumber = PhoneNumber FROM INSERTED
    -- Call the stored procedure to generate the new signature
    EXEC GenerateNewSignature @Id, @PhoneNumber, @newSignature OUTPUT
    -- Update the inserted row with the new signature
    UPDATE dbo.Registrationss
    SET Signature = @newSignature
    WHERE ID = @Id
END
----------------------------------------------------------------------------