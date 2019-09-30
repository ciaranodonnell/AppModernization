CREATE TYPE cdc.Outbox AS TABLE
(
	ChangeId bigint NOT NULL,
    [EventSentUTC] DATETIMEOFFSET (7) NULL
);