
CREATE TABLE OutboxPostmarks 
(
	ChangeId bigint identity(1,1) primary key,
	ActualLSN binary(10),
	EventType varchar(500),
	EventTopic varchar(500),
	EventSentUTC datetimeoffset 
)

