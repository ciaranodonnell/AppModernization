CREATE PROCEDURE [dbo].[spGetOutboxItems]
	@batchSize INT NULL
AS
	SET @batchSize = COALESCE(@batchSize, 10)

	SELECT TOP (@batchSize) * 
	FROM CommonOutbox	
		WITH (READCOMMITTED)
	WHERE SentDate IS NULL


