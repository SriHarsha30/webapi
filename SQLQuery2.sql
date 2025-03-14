CREATE PROCEDURE InsertIntoNotification12
    @RequestId VARCHAR(20),
    @notification_Description VARCHAR(250)
AS
BEGIN
    DECLARE @sendersId VARCHAR(20);
    DECLARE @receiversId VARCHAR(20);

    -- Fetch the receiversId based on the provided conditions
    SELECT @receiversId = m.TenantId
    FROM Maintainances m
    WHERE m.request_id = @RequestId;

    -- Fetch the sendersId based on the provided conditions
    SELECT @sendersId = p.owner_id
    FROM props p
    INNER JOIN Maintainances m2 ON m2.PropertyId = p.Property_Id
    WHERE m2.Tenantid = @receiversId;

    -- Insert into notifications1 table
    INSERT INTO notifications1 (sendersId, receiversId, CreatedDate, notification_Description)
    VALUES (@sendersId, @receiversId, GETDATE(), @notification_Description);

    PRINT 'Inserted message to notification';
END;