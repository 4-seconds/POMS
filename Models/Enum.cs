namespace PurchaseOrderManagementSystem.Models
{
    public enum Gender
    {
        Male,
        Female,
    }
    public enum SupplierStatus
    {
        Pending,
        Active,
        Inactive,
        Suspended
    }
    public enum PurchaseRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        AuctionCreated,
        PendingBudgetReview,
        Denied
    }
    /// <summary>
    /// Defines the possible statuses for a user account.
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// The account is active and can be used.
        /// </summary>
        Active,
        /// <summary>
        /// The account is inactive and cannot be used.
        /// </summary>
        Inactive,
        /// <summary>
        /// The account is pending approval or activation.
        /// </summary>
        Pending
    }
    public enum BidStatus
    {
        Open,
        Closed,
        Won,
        Lost
    }

    public enum AuctionStatus
    {
        Open,
        Closed,
        Cancelled
    }
}
