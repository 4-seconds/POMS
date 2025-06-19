using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PurchaseOrderManagementSystem.Models
{
    /// <summary>
    /// Represents a user account in the application, extending the default IdentityUser provided by ASP.NET Core Identity.
    /// This model includes additional properties specific to the application's user management,
    /// such as first name, last name, account status, and an optional association with a supplier.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the first name of the user.
        /// This field is required and has a maximum length of 100 characters.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// This field is required and has a maximum length of 100 characters.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        /// <summary>
        /// Gets or sets the status of the user's account.
        /// This uses the <see cref="AccountStatus"/> enum to define possible states (e.g., Active, Inactive, Pending).
        /// </summary>
        public AccountStatus AccountStatus { get; set; }

        /// <summary>
        /// Gets or sets the ID of the branch associated with the user.
        /// This is a foreign key to the Branch model.
        /// </summary>
        [ForeignKey("Branch")]
        public Guid? BranchId { get; set; }

        public Boolean PasswordResetRequired { get; set; } = true;

        /// <summary>
        /// Gets or sets the navigation property to the Branch associated with the user.
        /// </summary>
        public Branch? Branch { get; set; }
    }

}
