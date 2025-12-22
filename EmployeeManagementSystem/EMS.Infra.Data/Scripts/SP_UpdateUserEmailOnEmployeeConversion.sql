-- =============================================
-- Stored Procedure: SP_UpdateUserEmailOnEmployeeConversion
-- Description: Updates the user's email to a professional format when converted to an employee.
--              Email format: firstname.lastname@company.com
--              If duplicate names exist, appends incrementing numbers (firstname.lastname1@company.com, etc.)
-- Author: Prachi Kumari
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[SP_UpdateUserEmailOnEmployeeConversion]
    @UserID INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @CompanyDomain NVARCHAR(100) = 'company.com',
    @NewEmail NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @BaseEmail NVARCHAR(255);
        DECLARE @FinalEmail NVARCHAR(255);
        DECLARE @Counter INT = 0;
        DECLARE @EmailExists BIT = 1;

        -- Normalize first and last name (lowercase, remove spaces and special characters)
        DECLARE @NormalizedFirstName NVARCHAR(100);
        DECLARE @NormalizedLastName NVARCHAR(100);

        SET @NormalizedFirstName = LOWER(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@FirstName)), ' ', ''), '''', ''), '-', ''));
        SET @NormalizedLastName = LOWER(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(@LastName)), ' ', ''), '''', ''), '-', ''));

        -- Create the base email format: firstname.lastname@company.com
        SET @BaseEmail = @NormalizedFirstName + '.' + @NormalizedLastName + '@' + @CompanyDomain;

        -- Check if this base email already exists in the Users table (excluding the current user)
        IF NOT EXISTS (
            SELECT 1 
            FROM [dbo].[Users] 
            WHERE EmailID = @BaseEmail AND UserID != @UserID
        )
        BEGIN
            SET @FinalEmail = @BaseEmail;
            SET @EmailExists = 0;
        END
        ELSE
        BEGIN
            -- Find the next available number suffix
            -- Get the highest number suffix currently in use for this name combination
            SELECT @Counter = ISNULL(MAX(
                CASE 
                    WHEN EmailID = @BaseEmail THEN 0
                    WHEN EmailID LIKE @NormalizedFirstName + '.' + @NormalizedLastName + '[0-9]@' + @CompanyDomain THEN
                        CAST(SUBSTRING(EmailID, LEN(@NormalizedFirstName + '.' + @NormalizedLastName) + 1, 
                             CHARINDEX('@', EmailID) - LEN(@NormalizedFirstName + '.' + @NormalizedLastName) - 1) AS INT)
                    WHEN EmailID LIKE @NormalizedFirstName + '.' + @NormalizedLastName + '[0-9][0-9]@' + @CompanyDomain THEN
                        CAST(SUBSTRING(EmailID, LEN(@NormalizedFirstName + '.' + @NormalizedLastName) + 1, 
                             CHARINDEX('@', EmailID) - LEN(@NormalizedFirstName + '.' + @NormalizedLastName) - 1) AS INT)
                    WHEN EmailID LIKE @NormalizedFirstName + '.' + @NormalizedLastName + '[0-9][0-9][0-9]@' + @CompanyDomain THEN
                        CAST(SUBSTRING(EmailID, LEN(@NormalizedFirstName + '.' + @NormalizedLastName) + 1, 
                             CHARINDEX('@', EmailID) - LEN(@NormalizedFirstName + '.' + @NormalizedLastName) - 1) AS INT)
                    ELSE 0
                END
            ), 0)
            FROM [dbo].[Users]
            WHERE UserID != @UserID
              AND (
                  EmailID = @BaseEmail
                  OR EmailID LIKE @NormalizedFirstName + '.' + @NormalizedLastName + '[0-9]%@' + @CompanyDomain
              );

            -- Increment the counter to get the next available number
            SET @Counter = @Counter + 1;
            SET @FinalEmail = @NormalizedFirstName + '.' + @NormalizedLastName + CAST(@Counter AS NVARCHAR(10)) + '@' + @CompanyDomain;

            -- Double-check to ensure uniqueness (in case of any edge cases)
            WHILE EXISTS (SELECT 1 FROM [dbo].[Users] WHERE EmailID = @FinalEmail AND UserID != @UserID)
            BEGIN
                SET @Counter = @Counter + 1;
                SET @FinalEmail = @NormalizedFirstName + '.' + @NormalizedLastName + CAST(@Counter AS NVARCHAR(10)) + '@' + @CompanyDomain;
            END
        END

        -- Update the user's email
        UPDATE [dbo].[Users]
        SET EmailID = @FinalEmail,
            UpdatedOn = GETUTCDATE()
        WHERE UserID = @UserID;

        -- Set the output parameter
        SET @NewEmail = @FinalEmail;

        COMMIT TRANSACTION;

        -- Return success
        SELECT 
            @UserID AS UserID, 
            @FinalEmail AS NewEmail, 
            'Success' AS Status,
            'Email updated successfully' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Return error information
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        SELECT 
            @UserID AS UserID, 
            NULL AS NewEmail, 
            'Error' AS Status,
            @ErrorMessage AS Message;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

-- =====================================================
-- To grant execute permissions to specific role/user:
-- =====================================================
-- GRANT EXECUTE ON [dbo].[SP_UpdateUserEmailOnEmployeeConversion] TO [Admin];
-- GO



