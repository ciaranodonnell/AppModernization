namespace CDC.Common
{
    /// <summary>
    /// An enum to represent the meaning of the __$operation field in the SQL Server CDC tables
    /// </summary>
    public enum CDCOperation
    {
        Delete = 1,
        Insert =2,
        UpdateBeforeChange=3,
        UpdateAfterChange=4,
        /// <summary>
        /// This is used with 'all with merge' row filter to indicate an Insert or Update happened to the row
        /// </summary>
        Upsert = 5
        
    }
}