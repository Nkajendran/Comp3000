using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace DEBA.Models
{
    [Table("UserCredentials", Schema = "dbo")]

    public class UserCredentialsController
    {
        public  int Id { get; set; }

        public  int UserNumber { get; set; }

        public  string PinNumber { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string ExpirationDate { get; set; }

        public string BankBalance { get; set; }

    }
}