using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Api.DTOs
{
    public record CreateTaskRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; init; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; init; } = string.Empty;
    }
}