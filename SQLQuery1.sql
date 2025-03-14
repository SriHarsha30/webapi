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