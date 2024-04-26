using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Security.KeyVault.Secrets;

namespace SegundaPracticaAzure.Helpers
{
    public class HelperToken
    {
        private SecretClient secretClient;
        public KeyVaultSecret Issuer { get; set; }
        public KeyVaultSecret Audience { get; set; }
        public KeyVaultSecret SecretKey { get; set; }

        public HelperToken(SecretClient secretClient) 
        {
            this.secretClient = secretClient;
            this.Issuer = 
                this.secretClient.GetSecret("Issuer");
            this.Audience =
                this.secretClient.GetSecret("Audience");
            this.SecretKey =
                this.secretClient.GetSecret("SecretKey");
        }

        public SymmetricSecurityKey GetKeyToken()
        {
            byte[] data =
                Encoding.UTF8.GetBytes(this.SecretKey.Value);
            return new SymmetricSecurityKey(data);
        }

        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options =
                new Action<JwtBearerOptions>(options =>
                {
                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Issuer.Value,
                        ValidAudience = this.Audience.Value,
                        IssuerSigningKey = this.GetKeyToken()
                    };
                });
            return options;
        }

        public Action<AuthenticationOptions>
            GetAuthenticateSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                });
            return options;
        }

    }
}
