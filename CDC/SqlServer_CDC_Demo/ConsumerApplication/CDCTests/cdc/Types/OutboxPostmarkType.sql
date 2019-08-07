CREATE TYPE cdc.OutboxPostmarkType AS TABLE
(
	ChangeId bigint NOT NULL,
    [EventSentUTC] DATETIMEOFFSET (7) NULL
);