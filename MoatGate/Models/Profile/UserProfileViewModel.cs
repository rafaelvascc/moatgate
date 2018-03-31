using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        [JwtClaimName("given_name")]
        public string FirstName { set; get; }
        [JwtClaimName("middle_name")]
        public string MiddleName { set; get; }
        [JwtClaimName("family_name")]
        public string LastName { set; get; }
        [JwtClaimName("nickname")]
        public string Nickname { set; get; }
        [JwtClaimName("preferred_username")]
        public string PreferedUserName { set; get; }
        [JwtClaimName("profile")]
        [DataType(DataType.Url)]
        public string ProfileUrl { set; get; }
        [JwtClaimName("picture")]
        [DataType(DataType.ImageUrl)]
        public string ProfilePictureUrl { set; get; }
        [JwtClaimName("website")]
        [DataType(DataType.Url)]
        public string Website { set; get; }
        [JwtClaimName("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }
        [JwtClaimName("email_verified")]
        public bool EmailVerified { set; get; }
        [JwtClaimName("gender")]
        public string Gender { set; get; }
        [JwtClaimName("birthdate")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { set; get; }
        [JwtClaimName("zoneinfo")]
        public string TimeZone { set; get; }
        [JwtClaimName("locale")]
        public string Locale { set; get; }
        [JwtClaimName("phone_number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { set; get; }
        [JwtClaimName("phone_number_verified")]
        public bool PhoneNumberVerified { set; get; }
        [JwtClaimName("address")]
        public string Address { set; get; }
        [JwtClaimName("updated_at")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { set; get; }

        public UserProfileViewModel()
        {
        }
        
        public UserProfileViewModel(IList<Claim> claims)
        {
            var propertiesTypes = typeof(UserProfileViewModel).GetProperties();
            foreach (var c in claims)
            {
                var prop = propertiesTypes.Where(p => ((JwtClaimNameAttribute)p.GetCustomAttributes(true).Where(a => a as JwtClaimNameAttribute != null).Single()).ClaimName == c.Type).SingleOrDefault();
                if (prop != null && !String.IsNullOrEmpty(c.Value))
                {
                    if (prop.GetType().IsAssignableFrom(typeof(DateTime?)))
                    {
                        prop.SetValue(this, DateTime.Parse(c.Value));
                    }
                    else if (prop.GetType().IsAssignableFrom(typeof(bool)))
                    {
                        prop.SetValue(this, bool.Parse(c.Value));
                    }
                    else
                    {
                        prop.SetValue(this, c.Value);
                    }
                }
            }
        }


        public IList<Claim> ToClaims()
        {
            var result = new List<Claim>();
            var propertiesTypes = typeof(UserProfileViewModel).GetProperties();
            foreach (var p in propertiesTypes)
            {
                var value = p.GetValue(this);
                if (value != null)
                {
                    var claimType = ((JwtClaimNameAttribute)p.GetCustomAttributes(true).Where(a => a as JwtClaimNameAttribute != null).Single()).ClaimName;
                    result.Add(new Claim(claimType, value.ToString()));
                }
            }

            return result;
        }
    }
}
