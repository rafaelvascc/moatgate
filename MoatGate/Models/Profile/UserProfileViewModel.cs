using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace MoatGate.Models.Profile
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class JwtClaimNameAttribute : Attribute
    {
        public string ClaimName { get; }

        public JwtClaimNameAttribute(string claimName)
        {
            ClaimName = claimName;
        }
    }

    public class UserProfileViewModel
    {
        [DisplayName("Full Name")]
        [JwtClaimName("name")]
        public string FullName => $"{FirstName}{(string.IsNullOrEmpty(MiddleName) ? string.Empty : " " + MiddleName)} {LastName}";

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

        [DisplayName("Profile URL")]
        [JwtClaimName("profile")]
        [DataType(DataType.Url)]
        public string ProfileUrl { set; get; }

        [JwtClaimName("picture")]
        public string ProfilePicture { set; get; }

        [DisplayName("Website")]
        [JwtClaimName("website")]
        [DataType(DataType.Url)]
        public string Website { set; get; }

        [DisplayName("Email")]
        [JwtClaimName("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [DisplayName("Email verified")]
        [JwtClaimName("email_verified")]
        public bool EmailVerified { set; get; }

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

        [DisplayName("Phone Number verified")]
        [JwtClaimName("phone_number_verified")]
        public bool PhoneNumberVerified { set; get; }

        [DisplayName("Address")]
        [JwtClaimName("address")]
        public string Address { set; get; }

        [DisplayName("Last Updated")]
        [JwtClaimName("updated_at")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { set; get; }

        public UserProfileViewModel()
        {
        }

        public UserProfileViewModel(IEnumerable<Claim> claims)
        {
            var propertiesTypes = typeof(UserProfileViewModel).GetProperties();
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
    }
}
