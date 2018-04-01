using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoatGate.Models.Profile
{
    public class EdiProfileViewModel
    {
        [DisplayName("First Name")]
        [Required]
        [JwtClaimName("given_name")]
        public string FirstName { set; get; }

        [DisplayName("Middle Name")]
        [JwtClaimName("middle_name")]
        public string MiddleName { set; get; }

        [DisplayName("Last Name")]
        [Required]
        [JwtClaimName("family_name")]
        public string LastName { set; get; }

        [DisplayName("Nickname")]
        [JwtClaimName("nickname")]
        public string Nickname { set; get; }

        [DisplayName("Prefered User Name")]
        [JwtClaimName("preferred_username")]
        public string PreferedUserName { set; get; }

        [JwtClaimName("picture")]
        [DataType(DataType.ImageUrl)]
        public string ProfilePictureUrl { set; get; }

        [DisplayName("Website")]
        [JwtClaimName("website")]
        [DataType(DataType.Url)]
        public string Website { set; get; }

        [Required]
        [DisplayName("Email")]
        [JwtClaimName("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [DisplayName("Gender")]
        [JwtClaimName("gender")]
        public string Gender { set; get; }

        [DisplayName("Birth Date")]
        [JwtClaimName("birthdate")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { set; get; }

        [DisplayName("Timezone")]
        [JwtClaimName("zoneinfo")]
        public string TimeZone { set; get; }

        [DisplayName("Language")]
        [JwtClaimName("locale")]
        public string Locale { set; get; }

        [DisplayName("Phone")]
        [JwtClaimName("phone_number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { set; get; }

        [DisplayName("Address")]
        [JwtClaimName("address")]
        public string Address { set; get; }

        [BindNever]
        [DisplayName("Last Updated")]
        [JwtClaimName("updated_at")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { set; get; }

        public EdiProfileViewModel()
        {
        }

        public EdiProfileViewModel(IEnumerable<Claim> claims)
        {
            var propertiesTypes = typeof(EdiProfileViewModel).GetProperties();
            foreach (var c in claims)
            {
                var prop = propertiesTypes.Where(p => ((JwtClaimNameAttribute)p.GetCustomAttributes(true).Where(a => a as JwtClaimNameAttribute != null).Single()).ClaimName == c.Type).SingleOrDefault();
                if (prop != null && !String.IsNullOrEmpty(c.Value))
                {
                    if (prop.PropertyType == typeof(DateTime?))
                    {
                        prop.SetValue(this, DateTime.Parse(c.Value));
                    }
                    else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(Boolean))
                    {
                        prop.SetValue(this, bool.Parse(c.Value));
                    }
                    else
                    {
                        if (prop.SetMethod != null)
                            prop.SetValue(this, c.Value);
                    }
                }
            }
        }

        public IEnumerable<Claim> ToClaims()
        {
            var result = new List<Claim>();
            var propertiesTypes = typeof(EdiProfileViewModel).GetProperties();
            foreach (var p in propertiesTypes)
            {
                var value = p.GetValue(this);
                if (value != null && value.ToString() != string.Empty)
                {
                    var claimType = ((JwtClaimNameAttribute)p.GetCustomAttributes(true).Where(a => a as JwtClaimNameAttribute != null).Single()).ClaimName;
                    result.Add(new Claim(claimType, value.ToString()));
                }
            }

            return result;
        }
    }
}
