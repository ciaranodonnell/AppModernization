
CREATE PROCEDURE cdc.Customer_CNC
AS

DECLARE @BatchId INT

BEGIN TRAN

INSERT INTO OutbookEnvelope 
-- record stats, get batchid 
Values ('hello')

	SELECT @BatchId = @@IDENTITY


	INSERT INTO OutboxItems
		Select @BatchId, c.*, CASE WHEN o.CustomerId IS NULL THEN 'I' ELSE 'U' END as UpdateType
		 from Customer c
			LEFT OUTER JOIN CNC_Customer o on c.CustomerId = o.CustomerID
		WHERE 
			(c.CustomerName != o.customername OR o.CustomerName IS NULL)
	UNION 
		Select @BatchId, o.*, 'D' as UpdateType
		 from Customer c
			RIGHT OUTER JOIN CNC_Customer o on c.CustomerId = o.CustomerId
		WHERE 
			c.CustomerId IS NULL

	--Copy over the deletes
	Delete  CNC_Customer FROM CNC_Customer 
	JOIN OutboxItems o ON CustomerId = o.CustomerId 
	where o.UpdateType = 'D'
	
	--Copy of the Updates
	Update CNC_Customer  
	SET CNC_Customer.CustomerName = o.CustomerName
	FROM CNC_Customer  
	JOIN OutboxItems o ON CNC_Customer.CustomerId = o.CustomerId 
	where o.UpdateType = 'U'
	
	--Copy over the inserts
	INSERT CNC_Customer 
	Select (CustomerName)
	FROM OutboxItems o where UpdateType = 'I'




	Select * from OutboxEnvelope where BatchId = @BatchId
	Select * from OutboxItems where @BatchId = @BatchId


	COMMIT TRAN

