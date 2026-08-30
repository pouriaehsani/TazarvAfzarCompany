using System.Collections.Generic;
using Company.Domain.Entities;

namespace Company.web.Models
{
    /// <summary>
    /// View model for the BaseInfo screen. It bundles everything the page needs
    /// to render into a single object, so the controller can pass the whole
    /// screen in one call instead of juggling separate lists.
    ///
    /// It sits in the Presentation layer and only shapes data for the view —
    /// it carries no business rules.
    /// </summary>
    public class BaseInfoViewModel
    {
        /// <summary>All categories to display in the Categories panel.</summary>
        public List<Category> Categories { get; set; } = new();

        /// <summary>All tags to display in the Tags panel.</summary>
        public List<Tag> Tags { get; set; } = new();
    }
}
