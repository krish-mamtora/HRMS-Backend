using HRMS_Backend.Entities;
using HRMS_Backend.Entities.TravelandExpense;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Backend.Model.TravelandExpense
{
    public class TravelAssignmentCreateUpdateDto
    {

        public int EmpId { get; set; } 
        [Required]
        public int PId { get; set; }
        [Required]
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}

