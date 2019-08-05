
/*
CREATE TABLE CDCOutBoxData
(
	OutBoxEnvelopeId BIGINT IDENTITY(1,1) PRIMARY KEY,
	CreatedDate DATETIME NOT NULL,
	AuditTableName VARCHAR(200) NOT NULL,
	AuditTableSchema VARCHAR(200) NOT NULL,
	FirstProcessedRecord bigint  NOT NULL,
	LastProcessedRecord bigint NOT NULL,
	HasBeenCompleted bit NOT NULL
)

GO

*/

IF EXISTS (Select * from INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_NAME = 'GetAuditOutboxData')
	DROP PROCEDURE GetAuditOutboxData

GO
CREATE PROCEDURE GetAuditOutboxData
( 
	@AuditTableName VARCHAR(200),
	@AuditSchema VARCHAR(200) = 'dbo',
	@LastProcessedRecord bigint = NULL,
	@MaxBatchSize INT = 500 -- This is to prevent us trying to create an unreadable batch after a pause reading. we should get data in chunks. It also makes it better to distribute between senders
)
AS
BEGIN

	DECLARE @ERRORTEXT VARCHAR(500)

	IF  NOT EXISTS (Select * from INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_NAME = @AuditTableName AND t.TABLE_SCHEMA = @AuditSchema)
	BEGIN
		SELECT @ERRORTEXT = 'The audit table ' + @AuditTableName + 'doesnt exist in schema ' + @AuditSchema
			
		RAISERROR (@ERRORTEXT,16,1)
	END


	DECLARE @LowestAuditId BIGINT
	DECLARE @MaxAuditId BIGINT


	DECLARE @EnvelopeId INT
	DECLARE @ParmDefinition nvarchar(500); 
	DECLARE @SQLString nvarchar(max); 

	IF @LastProcessedRecord IS NULL
	BEGIN



		-- This is a new process that doesnt know where it was so we should assume it failed to send the last batch and give them that one.
		 SELECT @EnvelopeId = OutBoxEnvelopeId,@LowestAuditId =[FirstProcessedRecord],@MaxAuditId=[LastProcessedRecord]    
		 FROM  CDCOutBoxData 
		 WHERE  AuditTableName = @AuditTableName AND AuditTableSchema = @AuditSchema


		IF @EnvelopeId IS NULL 
		BEGIN
			-- THIS IS the first one for this table
			

			SET @ParmDefinition = N'@MaxAuditIdOUT BIGINT OUTPUT, @MinAuditIdOUT BIGINT OUTPUT'
			SET @SQLString = N'SELECT @MaxAuditIdOUT = MAX(Audit_UpdateNumber), @MinAuditIdOUT = MIN(Audit_UpdateNumber) FROM (SELECT TOP ' + CAST(@MaxBatchSize AS VARCHAR(20)) + ' * FROM ' + @AuditSchema + '.' + @AuditTableName + ') t'
			
			
			EXEC sp_executesql @SQLString, @ParmDefinition, @MaxAuditIdOUT=@MaxAuditId OUTPUT, @MinAuditIdOUT = @LowestAuditId OUTPUT;

			IF @MaxAuditId IS NULL 
			BEGIN 
				-- THERE ARE NO AUDIT RECORDS
				RETURN 0
			END

			


			INSERT INTO CDCOutBoxData
			(
				CreatedDate, AuditTableName, AuditTableSchema, FirstProcessedRecord, LastProcessedRecord, HasBeenCompleted
			)
			VALUES
			( GETUTCDATE(), @AuditTableName, @AuditSchema, @LowestAuditId, @MaxAuditId, 0)
					
		END
		
	END
	ELSE
	BEGIN
		-- They told us where they go to, so we need to do another.


		SET @ParmDefinition = N'@MaxAuditIdOUT BIGINT OUTPUT, @MinAuditIdOUT BIGINT OUTPUT'
		SET @SQLString = N'SELECT @MaxAuditIdOUT = MAX(Audit_UpdateNumber), @MinAuditIdOUT = MIN(Audit_UpdateNumber) FROM (SELECT TOP ' + CAST(@MaxBatchSize AS VARCHAR(20)) + 
			' * FROM ' + @AuditSchema + '.' + @AuditTableName + ' WHERE [Audit_UpdateNumber] > ' + CAST(@LastProcessedRecord AS VARCHAR(20)) + ' ) t'
			
			
		EXEC sp_executesql @SQLString, @ParmDefinition, @MaxAuditIdOUT=@MaxAuditId OUTPUT, @MinAuditIdOUT = @LowestAuditId OUTPUT;


		INSERT INTO CDCOutBoxData
		(
			CreatedDate, AuditTableName, AuditTableSchema, FirstProcessedRecord, LastProcessedRecord, HasBeenCompleted
		)
		VALUES
		( GETUTCDATE(), @AuditTableName, @AuditSchema, @LowestAuditId, @MaxAuditId, 0)
					

		--remember to mark the last one complete

	END
	
	
	SET @SQLString = 'SELECT * FROM ' + @AuditSchema + '.' + @AuditTableName + ' WHERE Audit_UpdateNumber >= ' + CAST(@LowestAuditId AS VARCHAR(20)) + ' AND Audit_UpdateNumber <= ' + CAST(@MaxAuditId as VARCHAR(20))
	EXECUTE sp_executesql @SQLString

END



GO

EXEC GetAuditOutboxData 'Customer_Audit', 'dbo', 2, 2